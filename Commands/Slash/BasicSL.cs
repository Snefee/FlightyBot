using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using FlightyBot.Models;
using FlightyBot.Services;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace FlightyBot.Commands.Slash
{
    public class BasicSL : ApplicationCommandModule
    {
        public AeroDataBoxService AeroDataBoxService { get; set; }
        public AdsbDbService AdsbDbService { get; set; }
        public BotConfig Config { get; set; }

        [SlashCommand("info", "Get info about a flight, use flight number like LO3923")]
        public async Task InfoCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder
            {
                Title = "Flight Information",
                Description = "This is a test flight information embed.",
                Color = DiscordColor.Azure,
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }

        [SlashCommand("Embed", "Embed Message Test")]
        public async Task EmbedCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var embedMessage = new DiscordEmbedBuilder
            {
                Title = "Test Embed",
                Description = "This is a test embed message. <:plane:1384322759970259028> \n" +
                "<:progressline_left:1384324672165908641><:progressline_middle:1384324674221113486><:progressline_middle:1384324674221113486><:progressline_middle:1384324674221113486><:progressline_middle:1384324674221113486><:progressline_middle:1384324674221113486><:progressline_middle:1384324674221113486><:progressline_plane:1384325325470699691><:progressline_middle:1384324674221113486><:progressline_right:1384324670337318973>",
                Color = DiscordColor.Cyan,
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }


        // ===== Track Command =====
        [SlashCommand("Track", "Track a flight in real time with notifications")]
        public async Task TrackCommand(InteractionContext ctx, [Option("FlightNumber", "Type in the flight number (e.g. LO3923)")] string flightNumber)
        {
            await ctx.DeferAsync();
            var flight = await AeroDataBoxService.GetFlightDataAsync(flightNumber);

            if (flight == null)
            {
                var errorEmbed = new DiscordEmbedBuilder
                {
                    Title = "Error",
                    Description = $"Couldn't find flight `{flightNumber}`. Check the flight number (ex. `LO3923`) and try again.",
                    Color = DiscordColor.Red
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorEmbed));
                return;
            }

            var embedMessage = new DiscordEmbedBuilder
            {
                Title = $"{flight.Airline?.Name} {flight.Number}",
                Color = DiscordColor.Cyan,
            };

            if (flight.Location != null && flight.Arrival?.Airport?.Location != null && !string.IsNullOrEmpty(Config.mapBoxApiKey))
            {
                string mapImageUrl = MapPathGenerator.GenerateMapUrl(flight.Location, flight.Arrival.Airport.Location, Config.mapBoxApiKey);
                embedMessage.WithImageUrl(mapImageUrl);
            } else
            {
                embedMessage.Footer = new DiscordEmbedBuilder.EmbedFooter();
                embedMessage.Footer.Text = "Map generation failed: unable to get aircraft position";
            }

            string departureFlag = GetFlagEmoji(flight.Departure?.Airport?.CountryCode);
            string arrivalFlag = GetFlagEmoji(flight.Arrival?.Airport?.CountryCode);

            embedMessage.AddField("Status", flight.Status ?? "N/A", true);
            embedMessage.AddField("Airline", flight.Airline?.Name ?? "N/A", true);
            embedMessage.AddField("Aircraft", $"{flight.Aircraft?.Model ?? "N/A"} (`{flight.Aircraft?.Registration ?? "N/A"}`)", true);

            embedMessage.AddField("Departure",
                $"**Airport:** {departureFlag} {flight.Departure?.Airport?.Name ?? "N/A"} (`{flight.Departure?.Airport?.Iata ?? "N/A"}`)\n" +
                $"**Scheduled:** {FormatDiscordTimestamp(flight.Departure?.ScheduledTime?.Local)}", false);

            embedMessage.AddField("Arrival",
                $"**Airport:** {arrivalFlag} {flight.Arrival?.Airport?.Name ?? "N/A"} (`{flight.Arrival?.Airport?.Iata ?? "N/A"}`)\n" +
                $"**Scheduled:** {FormatDiscordTimestamp(flight.Arrival?.ScheduledTime?.Local)}\n" +
                $"**Expected:** {FormatDiscordTimestamp(flight.Arrival?.PredictedTime?.Local)}", false);

            var builder = new DiscordWebhookBuilder().AddEmbed(embedMessage);

            if (!string.IsNullOrEmpty(flight.Aircraft?.Registration))
            {
                var planeInfoButton = new DiscordButtonComponent(
                    ButtonStyle.Primary,
                    $"plane_info_{flight.Aircraft.Registration}",
                    "✈️ Plane Info"
                );
                builder.AddComponents(planeInfoButton);
            }

            var response = await ctx.EditResponseAsync(builder);
        }


        //Timestamps
        private string FormatDiscordTimestamp(DateTime? dt)
        {
            if (!dt.HasValue) return "N/A";

            var dateTimeOffset = new DateTimeOffset(dt.Value);
            long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();
            return $"<t:{unixTimestamp}:R>";
        }

        //Flags
        private string GetFlagEmoji(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
            {
                return string.Empty;
            }

            countryCode = countryCode.ToUpper();

            int firstLetter = 0x1F1E6 + (countryCode[0] - 'A');
            int secondLetter = 0x1F1E6 + (countryCode[1] - 'A');

            return $"{char.ConvertFromUtf32(firstLetter)}{char.ConvertFromUtf32(secondLetter)}";
        }
    }
}

