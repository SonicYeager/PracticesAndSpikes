using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourceCompare
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] fileNames = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (fileNames.Length == 2)
                Application.Run(new FrmRC(fileNames[1], ""));
            else if (fileNames.Length == 3)
                Application.Run(new FrmRC(fileNames[1], fileNames[2]));
            else
                Application.Run(new FrmRC());
        }
    }
}
