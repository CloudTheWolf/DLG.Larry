#nullable enable
using LarryVoice.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarryVoice { 
    internal class Options
    {
        /// <summary>
        /// Log Prefix
        /// </summary>
        public static string? LogPrefix { get; set; }

        /// <summary>
        /// Channel for Larry Logs
        /// </summary>
        public static ulong ModNotificationsChannel { get; set; }

        /// <summary>
        /// Channels created by Larry
        /// </summary>
        public static List<Channel> Channels { get; set; } = new();
    }
}
