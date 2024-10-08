using System.Collections.Generic;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.CleanerClass;

static partial class Cleaner
{
    public static class StandardCleaning
    {
        public static void CleaningStringTable(List<string> list)
        {
            list.RemoveAll(static cline => cline.Contains("stringtable/") && !(cline.Contains("AFX_") || cline.Contains("IDS_") ||
                                                                               cline.Contains("IDC_") || cline.Contains("ID_") ||
                                                                               cline.Contains("IDR_")));
        }

        public static void CleaningDialog(List<string> list)
        {
            list.RemoveAll(static cline => cline.Contains("dialog/") && !(cline.Contains("IDC_") || cline.Contains("IDOK") ||
                                                                          cline.Contains("IDCANCEL") || cline.Contains("IDD_") ||
                                                                          cline.Contains("CG_IDD_")));
        }
        public static void CleaningMenu(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("menu/") && !(cline.Contains("IDR_") || cline.Contains("ID_") || cline.Contains("IDC_") ||
                                                cline.Contains("menu/POPUP_")));
        }
    }

    public static class FormatSpecifierCleaning
    {
        public static void CleaningStringTable(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("stringtable/") &&
                   !((cline.Contains("AFX_") || cline.Contains("IDS_") || cline.Contains("IDC_") || cline.Contains("ID_") ||
                      cline.Contains("IDR_")) && cline.Contains('%')));
        }

        public static void CleaningDialog(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("dialog/") &&
                   !((cline.Contains("IDC_") || cline.Contains("IDOK") || cline.Contains("IDCANCEL") || cline.Contains("IDD_") ||
                      cline.Contains("CG_IDD_")) && cline.Contains('%')));
        }
        public static void CleaningMenu(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("menu/") &&
                   !((cline.Contains("IDR_") || cline.Contains("ID_") || cline.Contains("IDC_") || cline.Contains("menu/POPUP_")) &&
                     cline.Contains('%')));
        }
    }

    public static class StringCleaning
    {
        public static void CleaningStringTable(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("stringtable/") &&
                   !((cline.Contains("AFX_") || cline.Contains("IDS_") || cline.Contains("IDC_") || cline.Contains("ID_") ||
                      cline.Contains("IDR_")) && cline.Contains('"')));
        }

        public static void CleaningDialog(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("dialog/") &&
                   !((cline.Contains("IDC_") || cline.Contains("IDOK") || cline.Contains("IDCANCEL") || cline.Contains("IDD_") ||
                      cline.Contains("CG_IDD_")) && cline.Contains('"')));
        }
        public static void CleaningMenu(List<string> list)
        {
            list.RemoveAll(static cline
                => cline.Contains("menu/") &&
                   !((cline.Contains("IDR_") || cline.Contains("ID_") || cline.Contains("IDC_") || cline.Contains("menu/POPUP_")) &&
                     cline.Contains('"')));
        }
    }
}