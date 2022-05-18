using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    static partial class Composer
    {
        private static bool Checker(List<string> mRC, List<string> list, int i)
        {
            bool bDel;

            if (!(list.Contains(list[i])))
            {
                bDel = true;
                mRC.Add(list[i]);
            }
            else bDel = false;

            return bDel;
        }

        private static void Composing(List<string> identifiers, List<string> sidentifiers, List<string> toErease, List<string> mRC, ref string header, ref bool bDel, List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int i1 = 0; i1 < identifiers.Count; i1 += 2)
                {
                    if (list[i].Contains(identifiers[i1]) || list[i].Contains(identifiers[i1 + 1]))
                    {
                        toErease.Add(list[i]);
                        bDel = Checker(mRC, list, i);
                        if (!bDel)
                            header = list[i];
                    }
                    if (i + 1 < list.Count && header != "" && list[i + 1].Contains(sidentifiers[i1 == 0 ? 0 : 1]) && !(list[i + 1].Contains("dialog/IDD") || list[i + 1].Contains("dialog/CG_IDD") || list[i + 1].Contains("menu/IDR_") || list[i + 1].Contains("menu/IDC_MENU") || list[i + 1].Contains("menu/IDR_MENU") || list[i + 1].Contains("menu/POPUP_")))
                    {
                        if (bDel)
                        {
                            toErease.Add(list[i + 1]);
                            if (header.Contains(sidentifiers[1])) mRC.Add("DELETEME/" + header + list[i + 1].Remove(0, 4));
                            else mRC.Add("DELETEME/" + header + list[i + 1].Remove(0, 6));
                        }
                        else
                        {
                            toErease.Add(list[i + 1]);
                            if (header.Contains(sidentifiers[1])) mRC.Add(header + list[i + 1].Remove(0, 4));
                            else mRC.Add(header + list[i + 1].Remove(0, 6));
                        }
                    }
                }
            }
        }

        private static void FixingComposerLines(List<string> rc, List<string> toErease, List<string> mRC)
        {
            for (int i = 0; i < rc.Count; i++)
            {
                if (rc[i].Contains("stringtable/") && rc[i].Contains('"') && !(rc[i].Contains("AFX_") || rc[i].Contains("IDS_") || rc[i].Contains("IDC_") || rc[i].Contains("ID_") || rc[i].Contains("IDR_")))
                {
                    mRC.Add(rc[i - 1] + rc[i].Remove(0, 12));
                    toErease.Add(rc[i]);
                    toErease.Add(rc[i - 1]);
                }
            }
            foreach (var str in toErease)
            {
                rc.Remove(str);
            }
            for (int i = 0; i < mRC.Count; i++)
            {
                rc.Add(mRC[i]);
            }
        }
    }
}
