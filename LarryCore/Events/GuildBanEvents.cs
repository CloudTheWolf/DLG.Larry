using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarryCore.Events
{
    internal class GuildBanEvents
    {
        internal static async Task HandleGuildBanAdded(DiscordClient client, GuildBanAddedEventArgs args)
        {
            return; // No action needed for now, but can be extended in the future.
        }
    }
}
