using System.Collections.Generic;
using System.Linq;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.ComparatorClass;

public static class Comparator
{
    public static List<string> DoComparatorStandard(List<string> rcA, List<string> rcB)
    {
        var rc = rcA.Except(rcB).ToList();
        return rc;
    }

    public static List<string> DoCompareInverted(List<string> rcA, List<string> rcB)
    {
        var rc = rcA.Intersect(rcB).ToList();

        return rc;
    }
}