using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;
using FlightyBot.config;
using DSharpPlus.SlashCommands;
using FlightyBot.Commands.Slash;
using Microsoft.Extensions.DependencyInjection;
using FlightyBot.Services;
using FlightyBot.Models;

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
    }
}