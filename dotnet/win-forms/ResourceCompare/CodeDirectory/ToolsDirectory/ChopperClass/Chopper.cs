using System.Collections.Generic;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.ChopperClass;

internal static class Chopper
{
    public static List<List<string>> Chop(List<string> rc, List<string> sectionNames)
    {
        var startIndex = 0;
        var endIndex = 0;
        var ChoppedList = new List<List<string>>();

        foreach (var sectionName in sectionNames)
        {
            var headerName = "// " + sectionName;
            startIndex = rc.IndexOf(headerName);
            endIndex = rc.IndexOf("/////////////////////////////////////////////////////////////////////////////", startIndex);

            ChoppedList.Add(rc.GetRange(startIndex - 2, endIndex - (startIndex - 3)));
        }

        return ChoppedList;
    }

    public static List<List<string>> MenuChopper(List<string> Section)
    {
        var startIndex = 0;
        var endIndex = 0;
        var ChoppedList = new List<List<string>>();

        for (var i = 0; i < Section.Count; i++)
        {
            if (Section[i].Contains("IDR_") && Section[i].Contains("MENU"))
            {
                startIndex = i;
                endIndex =
                    Section.FindIndex(startIndex + 1, x => (x.Contains("IDR_") || x.Contains("POPUP_")) && x.Contains("MENU"));
                if (endIndex == -1)
                    ChoppedList.Add(Section.GetRange(startIndex, Section.Count - 1 - startIndex));
                else
                    ChoppedList.Add(Section.GetRange(startIndex, endIndex - startIndex));
            }
        }

        return ChoppedList;
    }

    public static List<List<string>> DialogChopper(List<string> Section)
    {
        var startIndex = 0;
        var endIndex = 0;
        var ChoppedList = new List<List<string>>();

        for (var i = 0; i < Section.Count; i++)
        {
            if ((Section[i].Contains("IDD_") || Section[i].Contains("CG_IDD_")) &&
                (Section[i].Contains("DIALOGEX") || Section[i].Contains("DIALOG")))
            {
                startIndex = i;
                endIndex =
                    Section.FindIndex(startIndex + 1,
                        x => (x.Contains("IDD_") || x.Contains("CG_IDD_")) &&
                             (Section[i].Contains("DIALOGEX") || Section[i].Contains("DIALOG")));
                if (endIndex == -1)
                    ChoppedList.Add(Section.GetRange(startIndex, Section.Count - 1 - startIndex));
                else
                    ChoppedList.Add(Section.GetRange(startIndex, endIndex - startIndex));
            }
        }

        return ChoppedList;
    }

    public static List<List<string>> StringTableChopper(List<string> Section)
    {
        var startIndex = 0;
        var endIndex = 0;
        var ChoppedList = new List<List<string>>();

        for (var i = 0; i < Section.Count; i++)
        {
            if (Section[i].Contains("STRINGTABLE"))
            {
                startIndex = i;
                endIndex = Section.FindIndex(startIndex + 1, x => x.Contains("STRINGTABLE"));
                if (endIndex == -1)
                    ChoppedList.Add(Section.GetRange(startIndex, Section.Count - 1 - startIndex));
                else
                    ChoppedList.Add(Section.GetRange(startIndex, endIndex - startIndex));
            }
        }

        return ChoppedList;
    }
}