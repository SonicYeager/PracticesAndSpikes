using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    public static partial class Comparator
    {
        public static List<string> DoComparatorStandart(List<string> rcA, List<string> rcB)
        {
            List<string> rc = rcA.Except(rcB).ToList();
            return rc;
        }

        public static List<string> DoCompareInverted(List<string> rcA, List<string> rcB)
        {
            List<string> rc = rcA.Intersect(rcB).ToList();

            return rc;
        }
    }
}
