using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.AnalyserClass;

internal static partial class Analyser
{
    private static List<string> Scan(List<string> originalSectionNames, string[] adaptedSectionNames, string fileName)
    {
        var strippedRessource = new List<string>();
        var lines = File.ReadAllLines(fileName).ToList();
        var startIndex = 0;
        var endIndex = 0;
        foreach (var sectionName in originalSectionNames)
        {
            var headerName = "// " + sectionName;
            startIndex = lines.IndexOf(headerName);
            endIndex = lines.IndexOf("/////////////////////////////////////////////////////////////////////////////", startIndex);
            if (endIndex == -1) endIndex = lines.Count - 1;

            var currentSectionName = adaptedSectionNames[originalSectionNames.IndexOf(sectionName)];
            for (var i = startIndex; i < endIndex; i++)
            {
                strippedRessource.Add(currentSectionName + "/" + lines[i].Trim());
            }
        }

        return strippedRessource;
    }

    private static List<string> ScanCompletely(string fileName)
    {
        var strippedRessource = File.ReadAllLines(fileName).ToList();
        return strippedRessource;
    }
}