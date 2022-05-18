using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    public static partial class Extractor
    {
        public static class DoExtractorStandart                                    
        {
            public static void ExtractingStringTable(List<string> rcA, List<string> rcB)
            {
                string[] identifiers = new string[] { "ID", "AFX_" };

                for (int i = 0; i < rcA.Count; i++)
                {
                    ExtractorStringTable(identifiers, rcA, i);
                }
                for (int i = 0; i < rcB.Count; i++)
                {
                    ExtractorStringTable(identifiers, rcB, i);
                }
            }

            public static void ExtractingDialog(List<string> rcA, List<string> rcB)
            {
                string[] identifiers = new string[] { "ID", "CG_ID", "POPUP_" };

                for (int i = 0; i < rcA.Count; i++)
                {
                    ExtractorDialog(identifiers, rcA, i);
                }
                for (int i = 0; i < rcB.Count; i++)
                {
                    ExtractorDialog(identifiers, rcB, i);
                }
            }

            public static void ExtractingMenu(List<string> rcA, List<string> rcB)
            {
                string[] identifiers = new string[] { "ID", "CG_ID", "POPUP_" };

                for (int i = 0; i < rcA.Count; i++)
                {
                    ExtractorMenu(identifiers, rcA, i);
                }
                for (int i = 0; i < rcB.Count; i++)
                {
                    ExtractorMenu(identifiers, rcB, i);
                }
            }

        }

        public static List<string> DoExtractorFormatSpecifier(List<string> rc)
        {
            bool bTake = false;
            List<string> fsrc = new List<string>();

            ExtractingFormatSpecifier(bTake, fsrc, rc);
            return fsrc;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static void DeletingIDsFromString(List<List<string>> rc)
        {
            //List<string> RCS = new List<string>();
            //List<string> RCD = new List<string>();
            //List<string> RCM = new List<string>();
            //List<string> RCS2 = new List<string>();
            //List<string> RCD2 = new List<string>();
            //List<string> RCM2 = new List<string>();
            List<List<string>> Chapter = new List<List<string>>() { new List<string>() , new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            List<List<string>> deepCopiedRC = DeepClone(rc);
            //List<List<string>> tempID = new List<List<string>>();

            DoExtractorStandart.ExtractingStringTable(deepCopiedRC[0], deepCopiedRC[3]);
            DoExtractorStandart.ExtractingDialog(deepCopiedRC[1], deepCopiedRC[4]);
            DoExtractorStandart.ExtractingStringTable(deepCopiedRC[2], deepCopiedRC[5]);


            for (int i = 0; i < 3; i++)
            {
                if (rc[i].Count >= rc[i +3].Count)
                {
                    foreach (string ID in deepCopiedRC[i])
                    {
                        if (!rc[i + 3].Exists(x => x.Contains(ID)))
                        {
                            rc[i + 3].Add(" n.a.");
                            int index = rc[i].FindIndex(x => x.Contains(ID));
                            string movable = rc[i][index];
                            rc[i].Remove(movable);
                            rc[i].Add(movable);
                        }
                    }
                }
                else if (rc[i].Count <= rc[i + 3].Count)
                {
                    foreach (string ID in deepCopiedRC[i + 3])
                    {
                        if (!rc[i].Exists(x => x.Contains(ID)))
                        {
                            rc[i].Add(ID + " n.a.");
                            int index = rc[i + 3].FindIndex(x => x.Contains(ID));
                            string movable = rc[i + 3][index];
                            rc[i + 3].Remove(movable);
                            rc[i + 3].Add(movable);
                        }
                    }
                }
                if (rc[i].Count != rc[i + 3].Count)
                {
                    if (rc[i].Count >= rc[i + 3].Count)
                    {
                        foreach (string ID in deepCopiedRC[i])
                        {
                            if (!rc[i + 3].Exists(x => x.Contains(ID)))
                            {
                                rc[i + 3].Add(" n.a.");
                                int index = rc[i].FindIndex(x => x.Contains(ID));
                                string movable = rc[i][index];
                                rc[i].Remove(movable);
                                rc[i].Add(movable);
                            }
                        }
                    }
                    else if (rc[i].Count <= rc[i + 3].Count)
                    {
                        foreach (string ID in deepCopiedRC[i + 3])
                        {
                            if (!rc[i].Exists(x => x.Contains(ID)))
                            {
                                rc[i].Add(ID + " n.a.");
                                int index = rc[i + 3].FindIndex(x => x.Contains(ID));
                                string movable = rc[i + 3][index];
                                rc[i + 3].Remove(movable);
                                rc[i + 3].Add(movable);
                            }
                        }
                    }
                }
            }


            for (int i = 3; i < rc.Count; i++)
            {
                for (int i2 = 0; i2 < rc[i].Count; i2++)
                {
                    Chapter[i].Add(rc[i][i2].Remove(0, rc[i][i2].IndexOf(" ")));
                }
            }
            for (int i = 3; i < rc.Count; i++)
            {
                rc[i].Clear();
            }
            for (int i = 3; i < Chapter.Count; i++)
            {
                for (int i2 = 0; i2 < Chapter[i].Count; i2++)
                {
                    rc[i].Add(Chapter[i][i2]);
                }
            }
        }

        public static List<string> DoExtractingStrings(List<string> rc)
        {
            bool bTake = false;
            List<string> src = new List<string>();


            foreach (string line in rc)
            {
                string actualLine = "";
                foreach (Char chr in line)
                {
                    if (Convert.ToString(chr).Contains('"') && bTake)
                    {

                        bTake = false;
                        actualLine += '"' + " ";
                        break;
                    }
                    else if (Convert.ToString(chr).Contains('"')) bTake = true;

                    if (bTake) actualLine += chr;

                }
                src.Add(actualLine);
            }

            return src;
        }

        public static List<string> ExtractSTIDs (List<string> actualStringtable)
        {
            string[] identifiers = new string[] { "ID", "AFX_" };
            actualStringtable.RemoveAll(cline => (!(cline.Contains("AFX_") || cline.Contains("IDS_") || cline.Contains("IDC_") || cline.Contains("ID_") || cline.Contains("IDR_"))));
            for (int i = 0; i < actualStringtable.Count; i++)
            {
                actualStringtable[i] = actualStringtable[i].Trim();
                while (actualStringtable[i].Contains(',') || actualStringtable[i].Contains(' ') || actualStringtable[i].Contains('"') || actualStringtable[i].Contains('\t'))
                {
                    if (actualStringtable[i].Contains(','))
                    {
                        actualStringtable[i] = Cut(actualStringtable[i].IndexOf(','), actualStringtable[i].Length - actualStringtable[i].IndexOf(','), actualStringtable[i]);
                    }
                    if (actualStringtable[i].Contains(' '))
                    {
                        actualStringtable[i] = Cut(actualStringtable[i].IndexOf(' '), actualStringtable[i].Length - actualStringtable[i].IndexOf(' '), actualStringtable[i]);
                    }
                    if (actualStringtable[i].Contains('"'))
                    {
                        actualStringtable[i] = Cut(actualStringtable[i].IndexOf('"'), actualStringtable[i].Length - actualStringtable[i].IndexOf('"'), actualStringtable[i]);
                    }
                    if (actualStringtable[i].Contains('\t'))
                    {
                        actualStringtable[i] = Cut(actualStringtable[i].IndexOf('\t'), actualStringtable[i].Length - actualStringtable[i].IndexOf('\t'), actualStringtable[i]);
                    }
                }
            }


            return actualStringtable;
        }
    }
}
