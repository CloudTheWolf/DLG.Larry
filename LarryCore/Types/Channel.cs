using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarryCore.Types
{
    public class Channel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }

        public DateTime LastUsed { get; set; }
    }
}
