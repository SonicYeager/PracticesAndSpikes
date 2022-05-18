using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    public static partial class Sorter
    {
        public static List<List<string>> Sort(List<List<string>> chapter)
        {
            for (int i = 0; i < chapter.Count; i++)
            {
                chapter[i].Sort();
            }
            return chapter;
        }

        public static List<List<string>> Split(List<string> rcA, List<string> rcB)
        {
            List<string> RCS = new List<string>();
            List<string> RCD = new List<string>();
            List<string> RCM = new List<string>();
            List<string> RCS2 = new List<string>();
            List<string> RCD2 = new List<string>();
            List<string> RCM2 = new List<string>();
            List<List<string>> Chapter = new List<List<string>>() { RCS, RCD, RCM, RCS2, RCD2, RCM2 };

            foreach (string str in rcA)
            {
                if (str.Contains("stringtable/"))
                {
                    Chapter[0].Add(str);
                }
                if (str.Contains("dialog/"))
                {
                    Chapter[1].Add(str);
                }
                if (str.Contains("menu/"))
                {
                    Chapter[2].Add(str);
                }
            }
            foreach (string str in rcB)
            {
                if (str.Contains("stringtable/"))
                {
                    Chapter[3].Add(str);
                }
                if (str.Contains("dialog/"))
                {
                    Chapter[4].Add(str);
                }
                if (str.Contains("menu/"))
                {
                    Chapter[5].Add(str);
                }
            }
            return Chapter;
        }

        public static List<List<string>> SortSection (List<List<string>> SectionA, List<List<string>> SectionB)
        {
            List<List<string>> ChangedSection = new List<List<string>>();
            string searchThat = null;
            for (int i = 0; i < SectionA.Count ; i++)
            {
                if (SectionA[i].Count != 0)
                {
                    searchThat = SectionA[i][0].Remove(SectionA[i][0].IndexOf(' '), SectionA[i][0].Length - SectionA[i][0].IndexOf(' '));
                    int index = SectionB.FindIndex(x => x[0].Contains(searchThat));
                    if (index != -1)
                    {
                        ChangedSection.Add(SectionB[index]);
                    }
                    else
                    {
                        List<string> placebo = new List<string>() { "DIESES ELEMENT GIBTS HIER NICHT" };
                        ChangedSection.Add(placebo);
                    }
                }
            }

            return ChangedSection;
        }

        public static List<string> SortSectionStringTable(List<List<string>> SectionA, List<string> SectionB)
        {
            string endifCopy = "";
            int startIndex;

            List<List<string>> ChangedSection = new List<List<string>>();
            for (int i = 0; i < SectionA.Count; i++)
            {
                List<string> EmptyStringTable = new List<string>() { "STRINGTABLE DISCARDABLE", "BEGIN", "END" };
                List<string> SearchForThese = Extractor.ExtractSTIDs(SectionA[i]);

                for (int ie = 0; ie < SearchForThese.Count; ie++)
                {
                    if (SectionB.Exists(x => x.Contains(SearchForThese[ie])))
                    {
                        startIndex = SectionB.FindIndex(x => x.Contains(SearchForThese[ie]));
                        if (!(SectionB[startIndex + 1].Contains("ID")) && !(SectionB[startIndex + 1].Contains("END")))
                        {
                            EmptyStringTable.Insert(EmptyStringTable.Count - 1, SectionB.Find(x => x.Contains(SearchForThese[ie])));
                            EmptyStringTable.Insert(EmptyStringTable.Count - 1 , SectionB[startIndex + 1]);
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
            List<string> MergedSortedSection = new List<string>();
            startIndex = 0;
            for (int i = 0; i < ChangedSection.Count; i++)
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
}
