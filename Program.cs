using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using FlightyBot.Commands.Slash;
using FlightyBot.config;
using FlightyBot.Models;
using FlightyBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public record BotConfig(string mapBoxApiKey);

namespace FlightyBot
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var services = new ServiceCollection()
            .AddSingleton(new AeroDataBoxService(jsonReader.aeroDataBoxApiKey))
            .AddSingleton<AdsbDbService>()
            .AddSingleton(new BotConfig(jsonReader.mapBoxApiKey))
            .BuildServiceProvider();

            var discordConfig = new DiscordConfiguration
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += Client_Ready;

            Client.ComponentInteractionCreated += async (s, e) =>
            {
                await InteractionHandler.HandleInteractionAsync(e, services);
            };

            // Commands Configuration Setup
            var commandsConfig = new CommandsNextConfiguration()
            {
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,
            };

            // Enable Slash Commands
            Commands = Client.UseCommandsNext(commandsConfig);

            var slashCommandsConfig = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = services
            });

            slashCommandsConfig.RegisterCommands<BasicSL>();


            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }

        // Plane Info Button Interaction Handler
        public static class InteractionHandler
        {
            public static async Task HandleInteractionAsync(ComponentInteractionCreateEventArgs e, IServiceProvider services)
            {
                if (e.Interaction.Type != InteractionType.Component) return;

                var customId = e.Id;

                if (customId.StartsWith("plane_info_"))
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Fetching Aircraft Data...").AsEphemeral(true));

                    string registration = customId.Split('_')[2];
                    var adsbDbService = services.GetService<AdsbDbService>();
                    var aircraftDetails = await adsbDbService.GetAircraftDataByRegistrationAsync(registration);

                    var newEmbed = new DiscordEmbedBuilder();
                    if (aircraftDetails != null)
                    {
                        newEmbed.Title = $"Details about {aircraftDetails.Registration}";
                        newEmbed.Color = DiscordColor.Azure;
                        if (!string.IsNullOrEmpty(aircraftDetails.PhotoUrl)) { newEmbed.WithImageUrl(aircraftDetails.PhotoUrl); }
                        newEmbed.AddField("Registration", aircraftDetails.Registration ?? "N/A", true);
                        newEmbed.AddField("Manufacturer", aircraftDetails.Manufacturer ?? "N/A", true);
                        newEmbed.AddField("Type", aircraftDetails.Type ?? "N/A", true);
                        newEmbed.AddField("Registered Owner", aircraftDetails.RegisteredOwner ?? "N/A", true);
                        newEmbed.AddField("Registered Owner Country", aircraftDetails.OwnerCountryName ?? "N/A", true);
                        newEmbed.WithImageUrl(aircraftDetails.PhotoUrl ?? "https://example.com/default-image.png");
                        newEmbed.WithFooter($"Details from AdsbDB");
                    }
                    else
                    {
                        newEmbed.Title = "Error";
                        newEmbed.Description = "Couldn't get details about this aircraft.";
                        newEmbed.Color = DiscordColor.Red;
                    }

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(newEmbed));
                }
            }
        }
    }
}
