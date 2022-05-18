using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
namespace ResourceCompare
{    public class Logic
    {
        public static BackgroundWorker backgroundWorker = new BackgroundWorker();


        public static void GetDifference(List<List<string>> rc, string newDestination, string fileNameTop, string fileNameBottom)
        {
            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.ReportProgress(100 / 5 * 0);

            Tuple<List<string>, List<string>> tRCs = SectionParserStringTable.ApplyToolsStandart(rc[0], rc[3]);
            rc[0] = tRCs.Item1;
            rc[3] = tRCs.Item2;
            backgroundWorker.ReportProgress(100/5 * 1);

            tRCs = SectionParserDialog.ApplyToolsStandart(rc[1], rc[4]);
            rc[1] = tRCs.Item1;
            rc[4] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 5 * 2);

            tRCs = SectionParserMenu.ApplyToolsStandart(rc[2], rc[5]);
            rc[2] = tRCs.Item1;
            rc[5] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 5 * 3);

            Printer.PrintStandart(rc, newDestination, fileNameTop, fileNameBottom);
            backgroundWorker.ReportProgress(100 / 5 * 4);

            Launch(newDestination);
            backgroundWorker.ReportProgress(100 / 5 * 5);
        }

        public static void GetDifferentFormatSpecifier(List<List<string>> rc, string newDestination, string fileNameTop, string fileNameBottom)
        {
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ReportProgress(100 / 7 * 0);

            Tuple<List<string>, List<string>> tRCs = SectionParserStringTable.ApplyToolsForFormatSpecifier(rc[0], rc[3]);
            rc[0] = tRCs.Item1;
            rc[3] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 7 * 1);

            tRCs = SectionParserDialog.ApllyToolsForFormatSpecifier(rc[1], rc[4]);
            rc[1] = tRCs.Item1;
            rc[4] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 7 * 2);

            tRCs = SectionParserMenu.ApllyToolsForFormatSpecifier(rc[2], rc[5]);
            rc[2] = tRCs.Item1;
            rc[5] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 7 * 3);

            rc = Sorter.Sort(rc);
            backgroundWorker.ReportProgress(100 / 7 * 4);

            Extractor.DeletingIDsFromString(rc);
            backgroundWorker.ReportProgress(100 / 7 * 5);

            Printer.PrintFormatSpecifier(rc, newDestination, fileNameTop, fileNameBottom);
            backgroundWorker.ReportProgress(100 / 7 * 6);

            Launch(newDestination);
            backgroundWorker.ReportProgress(100);


        }

        public static void GetNotTranslatedStrings(List<List<string>> rc, string newDestination, string fileNameTop, string fileNameBottom)
        {
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ReportProgress(100 / 5 * 0);

            Tuple<List<string>, List<string>> tRCs = SectionParserStringTable.ApplyToolsForUntranslatedStrings(rc[0], rc[3]);
            rc[0] = tRCs.Item1;
            rc[3] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 5 * 1);

            tRCs = SectionParserDialog.ApplyToolsForUntranslatedStrings(rc[1], rc[4]);
            rc[1] = tRCs.Item1;
            rc[4] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 5 * 2);

            tRCs = SectionParserMenu.ApplyToolsForUntranslatedStrings(rc[2], rc[5]);
            rc[2] = tRCs.Item1;
            rc[5] = tRCs.Item2;
            backgroundWorker.ReportProgress(100 / 5 * 3);

            Printer.PrintUntranslatedStrings(rc, newDestination, fileNameTop, fileNameBottom);
            backgroundWorker.ReportProgress(100 / 5 * 4);

            Launch(newDestination);
            backgroundWorker.ReportProgress(100);

        }

        public static void GetSortedRC(List<string> rcA, List<string> rcB, string newDestination, string fileNameTop, string fileNameBottom, List<string> sectionNames)
        {
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ReportProgress(100 / 7 * 0);

            string[] sectionHeadName = new string[] { "// String Table", "// Dialog", "// Menu" };
            List<string> sectionOrder = new List<string>();
            foreach (string line in rcA)
            {
                if (line == sectionHeadName[0])
                {
                    sectionOrder.Add("// String Table");
                }
                if (line == sectionHeadName[1])
                {
                    sectionOrder.Add("// Dialog");
                }
                if (line == sectionHeadName[2])
                {
                    sectionOrder.Add("// Menu");
                }
            }

            List<List<string>> ChoppedRCA = Chopper.Chop(rcA, sectionNames);
            List<List<string>> ChoppedRCB = Chopper.Chop(rcB, sectionNames);
            backgroundWorker.ReportProgress(100 / 7 * 1);

            List<string> SortedChoppedStringTable = SectionParserStringTable.ApplyToolsForSortedRC(ChoppedRCA[0], ChoppedRCB[0]);
            backgroundWorker.ReportProgress(100 / 7 * 2);

            List<string> SortedChoppedMenu = SectionParserMenu.ApplyToolsForSortedRC(ChoppedRCA[2], ChoppedRCB[2]);
            backgroundWorker.ReportProgress(100 / 7 * 3);

            List<string> SortedChoppedDialog = SectionParserDialog.ApplyToolsForSortedRC(ChoppedRCA[1], ChoppedRCB[1]);
            backgroundWorker.ReportProgress(100 / 7 * 4);

            List<string> newRC = Composer.BuildNewRC(SortedChoppedStringTable, SortedChoppedDialog, SortedChoppedMenu, rcB, sectionOrder);
            backgroundWorker.ReportProgress(100 / 7 * 5);

            Printer.PrintNewRC(newRC, newDestination);
            backgroundWorker.ReportProgress(100 / 7 * 6);

            Launch(newDestination);
            backgroundWorker.ReportProgress(100);

        }

        public static void Launch(string filePath)
        {
            Process.Start(filePath);
        }

        ~Logic()
        {

        }
    }
}
