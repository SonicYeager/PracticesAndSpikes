using System;
using System.Windows.Forms;

namespace ResourceCompare;

file static class Program
{
    [STAThread]
    private static void Main()
    {
        var fileNames = Environment.GetCommandLineArgs();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        switch (fileNames.Length)
        {
            case 2:
                Application.Run(new FrmRC(fileNames[1], ""));
                break;
            case 3:
                Application.Run(new FrmRC(fileNames[1], fileNames[2]));
                break;
            default:
                Application.Run(new FrmRC());
                break;
        }
    }
}