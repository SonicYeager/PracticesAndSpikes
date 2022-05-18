using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    public static partial class Cleaner
    {

        public static void DoCleanerSpecialisedForFormatSpecifier(List<string> rcA, List<string> rcB)
        {
            rcA.RemoveAll(cline => !(cline.Contains("%")));
            rcB.RemoveAll(cline => !(cline.Contains("%")));
        }
    }
}
