﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.AnalyserClass;

internal static partial class Analyser
{
    public static Tuple<List<string>, List<string>, int> AnalyseFiles(string fileNameRCTop, string fileNameRCBottom,
        List<string> originalSectionNames)
    {
        var adaptedSectionNames = new string[3];
        foreach (var str in originalSectionNames)
        {
            adaptedSectionNames[originalSectionNames.IndexOf(str)] = str.ToLower();
            if (adaptedSectionNames[originalSectionNames.IndexOf(str)].Contains(" "))
                adaptedSectionNames[originalSectionNames.IndexOf(str)] = adaptedSectionNames[originalSectionNames.IndexOf(str)]
                    .Remove(adaptedSectionNames[originalSectionNames.IndexOf(str)].IndexOf(" "), 1);
        }

        try
        {
            var rcA = Scan(originalSectionNames, adaptedSectionNames, fileNameRCTop);
            var rcB = Scan(originalSectionNames, adaptedSectionNames, fileNameRCBottom);
            return Tuple.Create(rcA, rcB, 0);
        }
        catch (FileNotFoundException)
        {
            return Tuple.Create(new List<string>(), new List<string>(), 1);
        }
    }

    public static Tuple<List<string>, List<string>, int> AnalyseFilesForCompleteSections(string fileNameRCTop, string fileNameRCBottom)
    {
        try
        {
            var rcA = ScanCompletely(fileNameRCTop);
            var rcB = ScanCompletely(fileNameRCBottom);
            return Tuple.Create(rcA, rcB, 0);
        }
        catch (FileNotFoundException)
        {
            return Tuple.Create(new List<string>(), new List<string>(), 1);
        }
    }
}