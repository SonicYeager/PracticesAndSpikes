using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeContractsNDataAsNuGet
{
    public interface IErrorNotify
    {
        void Notify(string message);
    }
}
