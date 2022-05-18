using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResourceCompare
{
    static partial class Extractor
    {
        public static string Cut(int startIndex, int length, string listenElement)
        {

            listenElement = listenElement.Remove(startIndex, length);

            return listenElement;
        }

        static void ExtractorStringTable(string[] identifiers, List<string> list, int i)
        {

            while (!(list[i].Contains("stringtable/ID") || list[i].Contains("stringtable/AFX_")))
            {
                RemoveFrontChars(identifiers, list, i);
            }
            while (list[i].Contains(',') || list[i].Contains(' ') || list[i].Contains('"') || list[i].Contains('\t'))
            {
                RemoveBackChars(list, i);
            }
        }

        static void ExtractorDialog(string[] identifiers, List<string> list, int i)
        {

            while (!(list[i].Contains("dialog/CG_IDD_") || list[i].Contains("dialog/ID") || list[i].Contains("dialog/CG_ID")))
            {
                RemoveFrontChars(identifiers, list, i);
            }
            while (list[i].Contains(',') || list[i].Contains(' ') || list[i].Contains('"') || list[i].Contains('\t'))
            {
                RemoveBackChars(list, i);
            }
        }

        static void ExtractorMenu(string[] identifiers, List<string> list, int i)
        {

            while (!(list[i].Contains("menu/ID") || list[i].Contains("menu/POPUP_")))
            {
                RemoveFrontChars(identifiers, list, i);
            }
            while (list[i].Contains(',') || list[i].Contains(' ') || list[i].Contains('"') || list[i].Contains('\t'))
            {
                RemoveBackChars(list, i);
            }
        }

        private static void RemoveBackChars(List<string> list, int i)
        {

            //return Regex.Replace(list[i],"[, \t\"]", "" );



            if (list[i].Contains(','))
            {
                list[i] = Cut(list[i].IndexOf(','), list[i].Length - list[i].IndexOf(','), list[i]);
            }
            if (list[i].Contains(' '))
            {
                list[i] = Cut(list[i].IndexOf(' '), list[i].Length - list[i].IndexOf(' '), list[i]);
            }
            if (list[i].Contains('"'))
            {
                list[i] = Cut(list[i].IndexOf('"'), list[i].Length - list[i].IndexOf('"'), list[i]);
            }
            if (list[i].Contains('\t'))
            {
                list[i] = Cut(list[i].IndexOf('\t'), list[i].Length - list[i].IndexOf('\t'), list[i]);
            }
        }

        private static void RemoveFrontChars(string[] identifiers, List<string> list, int i)
        {
            foreach (string identifier in identifiers)
            {
                if (list[i].Contains("," + identifier))
                {
                    list[i] = Cut(list[i].IndexOf('/') + 1, list[i].IndexOf("," + identifier) - list[i].IndexOf('/'), list[i]);
                }
                if (list[i].Contains(" " + identifier))
                {
                    list[i] = Cut(list[i].IndexOf('/') + 1, list[i].IndexOf(" " + identifier) - list[i].IndexOf('/'), list[i]);
                }
                if (list[i].Contains('"' + identifier))
                {
                    list[i] = Cut(list[i].IndexOf('/') + 1, list[i].IndexOf('"' + identifier) - list[i].IndexOf('/'), list[i]);
                }
                if (list[i].Contains('\t' + identifier))
                {
                    list[i] = Cut(list[i].IndexOf('/') + 1, list[i].IndexOf('\t' + identifier) - list[i].IndexOf('/'), list[i]);
                }
            }
        }

        private static void ExtractingFormatSpecifier(bool bTake, List<string> fsrc, List<string> list)
        {

            //
            List<string> charsToBe = new List<string>() { "d", "s", "i", "x", "c", "p", "u", "m", "f"};
            //

            foreach (string line in list)
            {
                string actualLine = "";
                foreach (char chr in line)
                {
                    if (Convert.ToString(chr).Contains('%'))
                        bTake = true;
                    if (bTake && (Convert.ToString(chr).Contains('"') || Convert.ToString(chr).Contains('\t') || Convert.ToString(chr).Contains(']') || Convert.ToString(chr).Contains(' ') ||
                        Convert.ToString(chr).Contains('H') || Convert.ToString(chr).Contains('M') || Convert.ToString(chr).Contains('S') || Convert.ToString(chr).Contains('Y') || Convert.ToString(chr).Contains('|') || Convert.ToString(chr).Contains(')') ||
                        Convert.ToString(chr).Contains('-') || Convert.ToString(chr).Contains('\\')))
                    {
                        bTake = false;
                        if (actualLine.LastIndexOf('%') >= 0)
                            actualLine = actualLine.Remove(actualLine.LastIndexOf('%'), 1);
                    }

                    if (bTake)
                        actualLine += chr;

                    //
                    if (bTake && charsToBe.Contains(chr.ToString()))
                    {
                        bTake = false;
                        actualLine += " ";
                    }
                    //

                    //if (bTake && (Convert.ToString(chr).Contains('d') || Convert.ToString(chr).Contains('i') || Convert.ToString(chr).Contains('s') || Convert.ToString(chr).Contains('c') ||
                    //    Convert.ToString(chr).Contains('p') || Convert.ToString(chr).Contains('x') || Convert.ToString(chr).Contains('u') || Convert.ToString(chr).Contains('m') || Convert.ToString(chr).Contains('f')))
                    //{
                    //    bTake = false;
                    //    actualLine += " ";
                    //}
                }
                fsrc.Add(actualLine);
            }
        }
    }
}
