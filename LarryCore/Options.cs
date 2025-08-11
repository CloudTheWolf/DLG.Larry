#nullable enable
using DSharpPlus.Interactivity;
using LarryCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarryCore
{
    internal class Options
    {
        /// <summary>
        /// Log Prefix
        /// </summary>
        public static string? LogPrefix { get; set; }

        /// <summary>
        /// Ignored channels for "Secret" Responses
        /// </summary>
        public static ulong ModNotificationsChannel { get; set; }
        
        /// <summary>
        /// List of channels created by larry
        /// </summary>
        public static List<Channel> Channels { get; set; } = new();

        /// <summary>
        /// Channel ID for the claim channel
        /// </summary>
        public static ulong ClaimChannelId { get; set; }

    }
}
