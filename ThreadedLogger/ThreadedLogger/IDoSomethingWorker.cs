using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public interface IDoSomethingWorker
    {
        void Begin();
        void End();
    }
}
