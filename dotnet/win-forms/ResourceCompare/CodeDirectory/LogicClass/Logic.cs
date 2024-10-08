using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ResourceCompare.CodeDirectory.SectionClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.ChopperClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.ExtractorClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.SorterClass;

namespace ResourceCompare.CodeDirectory.LogicClass;

public sealed class Logic
{
    public static readonly BackgroundWorker BackgroundWorker = new();


    public static void GetDifference(List<List<string>> rc, string newDestination, string fileNameTop, string fileNameBottom)
    {
        BackgroundWorker.WorkerReportsProgress = true;

        BackgroundWorker.ReportProgress(100 / 5 * 0);

        var tRCs = SectionParserStringTable.ApplyToolsStandard(rc[0], rc[3]);
        rc[0] = tRCs.Item1;
        rc[3] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 1);

        tRCs = SectionParserDialog.ApplyToolsStandard(rc[1], rc[4]);
        rc[1] = tRCs.Item1;
        rc[4] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 2);

        tRCs = SectionParserMenu.ApplyToolsStandard(rc[2], rc[5]);
        rc[2] = tRCs.Item1;
        rc[5] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 3);

        ToolsDirectory.PrinterClass.Printer.PrintStandart(rc, newDestination, fileNameTop, fileNameBottom);
        BackgroundWorker.ReportProgress(100 / 5 * 4);

        Launch(newDestination);
        BackgroundWorker.ReportProgress(100 / 5 * 5);
    }

    public static void GetDifferentFormatSpecifier(List<List<string>> rc, string newDestination, string fileNameTop,
        string fileNameBottom)
    {
        BackgroundWorker.WorkerReportsProgress = true;
        BackgroundWorker.ReportProgress(100 / 7 * 0);

        var tRCs = SectionParserStringTable.ApplyToolsForFormatSpecifier(rc[0], rc[3]);
        rc[0] = tRCs.Item1;
        rc[3] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 7 * 1);

        tRCs = SectionParserDialog.ApplyToolsForFormatSpecifier(rc[1], rc[4]);
        rc[1] = tRCs.Item1;
        rc[4] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 7 * 2);

        tRCs = SectionParserMenu.ApllyToolsForFormatSpecifier(rc[2], rc[5]);
        rc[2] = tRCs.Item1;
        rc[5] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 7 * 3);

        rc = Sorter.Sort(rc);
        BackgroundWorker.ReportProgress(100 / 7 * 4);

        Extractor.DeletingIDsFromString(rc);
        BackgroundWorker.ReportProgress(100 / 7 * 5);

        ToolsDirectory.PrinterClass.Printer.PrintFormatSpecifier(rc, newDestination, fileNameTop, fileNameBottom);
        BackgroundWorker.ReportProgress(100 / 7 * 6);

        Launch(newDestination);
        BackgroundWorker.ReportProgress(100);
    }

    public static void GetNotTranslatedStrings(List<List<string>> rc, string newDestination, string fileNameTop, string fileNameBottom)
    {
        BackgroundWorker.WorkerReportsProgress = true;
        BackgroundWorker.ReportProgress(100 / 5 * 0);

        var tRCs = SectionParserStringTable.ApplyToolsForUntranslatedStrings(rc[0], rc[3]);
        rc[0] = tRCs.Item1;
        rc[3] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 1);

        tRCs = SectionParserDialog.ApplyToolsForUntranslatedStrings(rc[1], rc[4]);
        rc[1] = tRCs.Item1;
        rc[4] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 2);

        tRCs = SectionParserMenu.ApplyToolsForUntranslatedStrings(rc[2], rc[5]);
        rc[2] = tRCs.Item1;
        rc[5] = tRCs.Item2;
        BackgroundWorker.ReportProgress(100 / 5 * 3);

        ToolsDirectory.PrinterClass.Printer.PrintUntranslatedStrings(rc, newDestination, fileNameTop, fileNameBottom);
        BackgroundWorker.ReportProgress(100 / 5 * 4);

        Launch(newDestination);
        BackgroundWorker.ReportProgress(100);
    }

    public static void GetSortedRC(List<string> rcA, List<string> rcB, string newDestination, string fileNameTop, string fileNameBottom,
        List<string> sectionNames)
    {
        BackgroundWorker.WorkerReportsProgress = true;
        BackgroundWorker.ReportProgress(100 / 7 * 0);

        var sectionHeadName = new[]
        {
            "// String Table", "// Dialog", "// Menu",
        };
        var sectionOrder = new List<string>();
        foreach (var line in rcA)
        {
            if (line == sectionHeadName[0]) sectionOrder.Add("// String Table");

            if (line == sectionHeadName[1]) sectionOrder.Add("// Dialog");

            if (line == sectionHeadName[2]) sectionOrder.Add("// Menu");
        }

        var ChoppedRCA = Chopper.Chop(rcA, sectionNames);
        var ChoppedRCB = Chopper.Chop(rcB, sectionNames);
        BackgroundWorker.ReportProgress(100 / 7 * 1);

        var SortedChoppedStringTable = SectionParserStringTable.ApplyToolsForSortedRC(ChoppedRCA[0], ChoppedRCB[0]);
        BackgroundWorker.ReportProgress(100 / 7 * 2);

        var SortedChoppedMenu = SectionParserMenu.ApplyToolsForSortedRC(ChoppedRCA[2], ChoppedRCB[2]);
        BackgroundWorker.ReportProgress(100 / 7 * 3);

        var SortedChoppedDialog = SectionParserDialog.ApplyToolsForSortedRC(ChoppedRCA[1], ChoppedRCB[1]);
        BackgroundWorker.ReportProgress(100 / 7 * 4);

        var newRC = ToolsDirectory.ComposerClass.Composer.BuildNewRC(SortedChoppedStringTable, SortedChoppedDialog, SortedChoppedMenu, rcB,
            sectionOrder);
        BackgroundWorker.ReportProgress(100 / 7 * 5);

        ToolsDirectory.PrinterClass.Printer.PrintNewRC(newRC, newDestination);
        BackgroundWorker.ReportProgress(100 / 7 * 6);

        Launch(newDestination);
        BackgroundWorker.ReportProgress(100);
    }

    public static void Launch(string filePath)
    {
        Process.Start(filePath);
    }
}