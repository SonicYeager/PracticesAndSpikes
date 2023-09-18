using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public struct DataPack
    {
        public int Hash { get; set; }
        public int CalledCount { get; set; }
        public IEnumerable<double> Messages { get; set; }
    }
}
