using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarryVoice.Types
{
    public class Channel
    {
        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the channel
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// Time when the channel was last used
        /// </summary>
        public DateTime LastUsed { get; set; }
        /// <summary>
        /// Id of the owner of the channel
        /// </summary>
        public ulong Owner { get; set; }
    }
}
