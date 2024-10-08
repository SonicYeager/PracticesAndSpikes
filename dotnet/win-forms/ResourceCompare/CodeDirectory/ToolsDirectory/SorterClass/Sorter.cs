using System.Collections.Generic;
using ResourceCompare.CodeDirectory.ToolsDirectory.ExtractorClass;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.SorterClass;

public static class Sorter
{
    public static List<List<string>> Sort(List<List<string>> chapter)
    {
        for (var i = 0; i < chapter.Count; i++)
        {
            chapter[i].Sort();
        }

        return chapter;
    }

    public static List<List<string>> Split(List<string> rcA, List<string> rcB)
    {
        var RCS = new List<string>();
        var RCD = new List<string>();
        var RCM = new List<string>();
        var RCS2 = new List<string>();
        var RCD2 = new List<string>();
        var RCM2 = new List<string>();
        var Chapter = new List<List<string>>()
        {
            RCS,
            RCD,
            RCM,
            RCS2,
            RCD2,
            RCM2,
        };

        foreach (var str in rcA)
        {
            if (str.Contains("stringtable/")) Chapter[0].Add(str);

            if (str.Contains("dialog/")) Chapter[1].Add(str);

            if (str.Contains("menu/")) Chapter[2].Add(str);
        }

        foreach (var str in rcB)
        {
            if (str.Contains("stringtable/")) Chapter[3].Add(str);

            if (str.Contains("dialog/")) Chapter[4].Add(str);

            if (str.Contains("menu/")) Chapter[5].Add(str);
        }

        return Chapter;
    }

    public static List<List<string>> SortSection(List<List<string>> SectionA, List<List<string>> SectionB)
    {
        var ChangedSection = new List<List<string>>();
        string searchThat = null;
        for (var i = 0; i < SectionA.Count; i++)
        {
            if (SectionA[i].Count != 0)
            {
                searchThat = SectionA[i][0].Remove(SectionA[i][0].IndexOf(' '), SectionA[i][0].Length - SectionA[i][0].IndexOf(' '));
                var index = SectionB.FindIndex(x => x[0].Contains(searchThat));
                if (index != -1)
                {
                    ChangedSection.Add(SectionB[index]);
                }
                else
                {
                    var placebo = new List<string>()
                    {
                        "DIESES ELEMENT GIBTS HIER NICHT",
                    };
                    ChangedSection.Add(placebo);
                }
            }
        }

        return ChangedSection;
    }

    public static List<string> SortSectionStringTable(List<List<string>> SectionA, List<string> SectionB)
    {
        var endifCopy = "";
        int startIndex;

        var ChangedSection = new List<List<string>>();
        for (var i = 0; i < SectionA.Count; i++)
        {
            var EmptyStringTable = new List<string>()
            {
                "STRINGTABLE DISCARDABLE", "BEGIN", "END",
            };
            var SearchForThese = Extractor.ExtractSTIDs(SectionA[i]);

            for (var ie = 0; ie < SearchForThese.Count; ie++)
            {
                if (SectionB.Exists(x => x.Contains(SearchForThese[ie])))
                {
                    startIndex = SectionB.FindIndex(x => x.Contains(SearchForThese[ie]));
                    if (!SectionB[startIndex + 1].Contains("ID") && !SectionB[startIndex + 1].Contains("END"))
                    {
                        EmptyStringTable.Insert(EmptyStringTable.Count - 1, SectionB.Find(x => x.Contains(SearchForThese[ie])));
                        EmptyStringTable.Insert(EmptyStringTable.Count - 1, SectionB[startIndex + 1]);
                    }
                    else
                    {
                        EmptyStringTable.Insert(EmptyStringTable.Count - 1, SectionB.Find(x => x.Contains(SearchForThese[ie])));
                    }
                }
                else
                {
                    EmptyStringTable.Insert(EmptyStringTable.Count - 1, "DIESES ELEMENT GIBTS HIER NICHT");
                }
            }

            EmptyStringTable.Add("");
            ChangedSection.Add(EmptyStringTable);
        }

        endifCopy = SectionB.Find(x => x.Contains("#endif"));
        var MergedSortedSection = new List<string>();
        startIndex = 0;
        for (var i = 0; i < ChangedSection.Count; i++)
        {
            MergedSortedSection.AddRange(ChangedSection[i]);
        }

        startIndex = SectionB.FindIndex(x => x.Contains("STRINGTABLE"));
        SectionB.RemoveRange(startIndex, SectionB.Count - startIndex);
        SectionB.AddRange(MergedSortedSection);
        SectionB.Add(endifCopy);
        //SectionB.Add("/////////////////////////////////////////////////////////////////////////////");

        return SectionB;
    }
}