#nullable enable
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

        public static List<Channel> Channels { get; set; } = new();
    }
}
