using System.Collections.Generic;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.CleanerClass;

public static partial class Cleaner
{

    public static void DoCleanerSpecialisedForFormatSpecifier(List<string> rcA, List<string> rcB)
    {
        rcA.RemoveAll(static cline => !cline.Contains('%'));
        rcB.RemoveAll(static cline => !cline.Contains('%'));
    }
}