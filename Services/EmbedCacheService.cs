using DSharpPlus.Entities;
using System.Collections.Concurrent;

namespace FlightyBot.Services
{
    public class EmbedCacheService
    {
        private readonly ConcurrentDictionary<ulong, DiscordEmbed> _cache = new();
        public void StoreEmbed(ulong messageId, DiscordEmbed embed)
        {
            _cache[messageId] = embed;
        }

        public DiscordEmbed GetEmbed(ulong messageId)
        {
            _cache.TryGetValue(messageId, out var embed);
            return embed;
        }
    }
}