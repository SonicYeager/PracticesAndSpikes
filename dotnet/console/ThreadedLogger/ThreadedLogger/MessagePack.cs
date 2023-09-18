using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public struct MessagePack
    {
        public string Hash { get; set; }
        public string CalledCount { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
