using System.Collections.Generic;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.ComposerClass;

public static partial class Composer
{
    public static void DoComposerStandard(List<string> rcA, List<string> rcB)
    {
        var identifiers = new List<string>()
        {
            "dialog/IDD", "dialog/CG_IDD", "menu/IDR_", "menu/POPUP_",
        };
        var sidentifiers = new List<string>()
        {
            "dialog/", "menu/",
        };
        var toErease = new List<string>();
        var mRC = new List<string>();

        var header = "";
        var bDel = false;

        Composing(identifiers, sidentifiers, toErease, mRC, ref header, ref bDel, rcA);
        foreach (var str in mRC)
        {
            if (str.Contains("DELETEME/"))
                toErease.Add(str);
            rcA.Add(str);
        }

        foreach (var str in toErease)
        {
            rcA.Remove(str);
        }

        mRC.Clear();
        toErease.Clear();

        Composing(identifiers, sidentifiers, toErease, mRC, ref header, ref bDel, rcB);
        foreach (var str in mRC)
        {
            if (str.Contains("DELETEME/"))
                toErease.Add(str);
            rcB.Add(str);
        }

        foreach (var str in toErease)
        {
            rcB.Remove(str);
        }
    }

    public static void DoFormatSpecifierFixLines(List<string> rcA, List<string> rcB)
    {
        var toErease = new List<string>();
        var mRC = new List<string>();

        for (var i = 0; i < rcA.Count; i++)
        {
            if (rcA[i].Contains("stringtable/") && rcA[i].Contains("%") && !(rcA[i].Contains("AFX_") || rcA[i].Contains("IDS_") ||
                                                                             rcA[i].Contains("IDC_") || rcA[i].Contains("ID_") ||
                                                                             rcA[i].Contains("IDR_")))
            {
                mRC.Add(rcA[i - 1] + rcA[i].Remove(0, 12));
                toErease.Add(rcA[i]);
                toErease.Add(rcA[i - 1]);
            }
        }

        foreach (var str in toErease)
        {
            rcA.Remove(str);
        }

        for (var i = 0; i < mRC.Count; i++)
        {
            rcA.Add(mRC[i]);
        }

        toErease.Clear();
        mRC.Clear();

        for (var i = 0; i < rcB.Count; i++)
        {
            if (rcB[i].Contains("stringtable/") && rcB[i].Contains("%") && !(rcB[i].Contains("AFX_") || rcB[i].Contains("IDS_") ||
                                                                             rcB[i].Contains("IDC_") || rcB[i].Contains("ID_") ||
                                                                             rcB[i].Contains("IDR_")))
            {
                mRC.Add(rcB[i - 1] + rcB[i].Remove(0, 12));
                toErease.Add(rcB[i]);
                toErease.Add(rcB[i - 1]);
            }
        }

        foreach (var str in toErease)
        {
            rcB.Remove(str);
        }

        for (var i = 0; i < mRC.Count; i++)
        {
            rcB.Add(mRC[i]);
        }
    }

    public static void DoClippContentTogether(List<string> rcA, List<string> rcB, List<string> fSRCA, List<string> fSRCB)
    {
        for (var i = 0; i < fSRCA.Count; i++)
        {
            rcA[i] += "| " + fSRCA[i];
        }

        for (var i = 0; i < fSRCB.Count; i++)
        {
            rcB[i] += "| " + fSRCB[i];
        }
    }

    public static void DoComposerFixStringLine(List<string> rcA, List<string> rcB)
    {
        var toErease = new List<string>();
        var mRC = new List<string>();

        toErease.Clear();
        mRC.Clear();
        FixingComposerLines(rcA, toErease, mRC);
        toErease.Clear();
        mRC.Clear();
        FixingComposerLines(rcB, toErease, mRC);
        toErease.Clear();
        mRC.Clear();
    }

    public static List<string> MergeMenusToSection(List<List<string>> SortedSection, List<string> OriginalSection)
    {
        var MergedSortedSection = new List<string>();
        var startIndex = 0;
        for (var i = 0; i < SortedSection.Count; i++)
        {
            MergedSortedSection.AddRange(SortedSection[i]);
        }

        startIndex = OriginalSection.FindIndex(x => (x.Contains("IDR_") || x.Contains("IDR_")) && x.Contains("MENU"));
        OriginalSection.RemoveRange(startIndex, OriginalSection.Count - startIndex);
        OriginalSection.AddRange(MergedSortedSection);
        //OriginalSection.Add("/////////////////////////////////////////////////////////////////////////////");

        return OriginalSection;
    }

    public static List<string> MergeDialogsToSection(List<List<string>> SortedSection, List<string> OriginalSection)
    {
        var MergedSortedSection = new List<string>();
        var startIndex = 0;
        for (var i = 0; i < SortedSection.Count; i++)
        {
            MergedSortedSection.AddRange(SortedSection[i]);
        }

        startIndex = OriginalSection.FindIndex(x
            => (x.Contains("IDD_") || x.Contains("CG_IDD_")) && (x.Contains("DIALOGEX") || x.Contains("DIALOG")));
        OriginalSection.RemoveRange(startIndex, OriginalSection.Count - startIndex);
        OriginalSection.AddRange(MergedSortedSection);
        //OriginalSection.Add("/////////////////////////////////////////////////////////////////////////////");

        return OriginalSection;
    }

    public static List<string> BuildNewRC(List<string> StringTable, List<string> Dialoge, List<string> Menus, List<string> OriginalRC,
        List<string> sectionOrder)
    {
        int startIndex;
        int endIndex;

        startIndex = OriginalRC.FindIndex(x => x == "// String Table");
        endIndex = OriginalRC.FindIndex(startIndex,
            x => x == "/////////////////////////////////////////////////////////////////////////////");
        OriginalRC.RemoveRange(startIndex - 2, endIndex - startIndex + 2);

        startIndex = OriginalRC.FindIndex(x => x == "// Menu");
        endIndex = OriginalRC.FindIndex(startIndex,
            x => x == "/////////////////////////////////////////////////////////////////////////////");
        OriginalRC.RemoveRange(startIndex - 1, endIndex - startIndex);

        startIndex = OriginalRC.FindIndex(x => x == "// Dialog");
        endIndex = OriginalRC.FindIndex(startIndex,
            x => x == "/////////////////////////////////////////////////////////////////////////////");
        OriginalRC.RemoveRange(startIndex - 1, endIndex - startIndex);

        foreach (var header in sectionOrder)
        {
            if (OriginalRC.FindIndex(x => x == "// DESIGNINFO") == -1)
            {
                if (sectionOrder.IndexOf("// Menu") < sectionOrder.IndexOf("// Dialog"))
                {
                    if (header == "// String Table")
                        OriginalRC.InsertRange(OriginalRC.FindIndex(x => x == "// Generated from the TEXTINCLUDE 3 resource.") - 4,
                            StringTable);

                    if (header == "// Menu")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// TEXTINCLUDE"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Menus);

                    if (header == "// Dialog")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// Menu"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Dialoge);
                }
                else
                {
                    if (header == "// String Table")
                        OriginalRC.InsertRange(OriginalRC.FindIndex(x => x == "// Generated from the TEXTINCLUDE 3 resource.") - 4,
                            StringTable);

                    if (header == "// Dialog")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// TEXTINCLUDE"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Dialoge);

                    if (header == "// Menu")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// Dialog"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Menus);
                }
            }
            else
            {
                if (sectionOrder.IndexOf("// Menu") < sectionOrder.IndexOf("// Dialog"))
                {
                    if (header == "// String Table")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// DESIGNINFO"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1,
                            StringTable);

                    if (header == "// Menu")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// TEXTINCLUDE"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Menus);

                    if (header == "// Dialog")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// Menu"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Dialoge);
                }
                else
                {
                    if (header == "// String Table")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// DESIGNINFO"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1,
                            StringTable);

                    if (header == "// Dialog")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// TEXTINCLUDE"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Dialoge);

                    if (header == "// Menu")
                        OriginalRC.InsertRange(
                            OriginalRC.FindIndex(OriginalRC.FindIndex(x => x == "// Dialog"),
                                x => x == "/////////////////////////////////////////////////////////////////////////////") + 1, Menus);
                }
            }
        }

        return OriginalRC;
    }
}