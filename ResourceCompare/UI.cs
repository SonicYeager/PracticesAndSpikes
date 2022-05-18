using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ResourceCompare
{
    public partial class FrmRC : Form
    {
        //private readonly Logic Logic = new Logic();

        readonly string[] fileNames = new string[2];
        List<string> rcA = new List<string>();
        List<string> rcB = new List<string>();
        //readonly string[] sectionName = new string[3] { "stringtable", "dialog", "menu" };
        string section;

        public FrmRC() : this("", "")
        {
        }

        public FrmRC(string resA, string resB)
        {
            fileNames[0] = resA;
            fileNames[1] = resB;

            InitializeComponent();
            Logic.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
            Logic.backgroundWorker.WorkerReportsProgress = true;

            txtBoxRCTop.Text = resA;
            txtBoxRCBottom.Text = resB;
        }
        public void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MethodInvoker ProgressChanged = delegate { prgsBarProgress.Value = e.ProgressPercentage; };
            Invoke(ProgressChanged);
        }

        private List<string> SetSection()
        {
            if ((string)cmBoxChooseSection.SelectedItem != "All")
            {
                section = cmBoxChooseSection.SelectedItem.ToString();
                List<string> sections = new List<string> { section };
                return sections;
            }
            else
            {
                List<string> sections = new List<string> { "String Table", "Dialog", "Menu" };
                return sections;
            }
        }
        private string GetDestination()
        {
            string newDestination;

            if (txtBoxDestinationOfTxt.Text != "")
            {
                newDestination = Path.GetDirectoryName(@txtBoxDestinationOfTxt.Text) + @"\Diff.txt";
            }
            else
            {
                newDestination = Path.GetDirectoryName(@txtBoxRCTop.Text) + @"\Diff.txt";
                txtBoxDestinationOfTxt.Text = newDestination;
            }

            return newDestination;
        }

        private void BtnFilePickerTop_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog open = new OpenFileDialog();
            DialogResult dialogResult = open.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                txtBoxRCTop.Text = open.FileName;
            }

        }

        private void BtnFilepickerBottom_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog open = new OpenFileDialog();
            DialogResult dialogResult = open.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                txtBoxRCBottom.Text = open.FileName;
            }
        }

        private void BtnTargetTxtDestination_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog open = new OpenFileDialog();
            DialogResult dialogResult = open.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                txtBoxDestinationOfTxt.Text = open.FileName;
            }
        }

        private void TxtBoxRCTop_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxRCTop.Text != "" && txtBoxRCBottom.Text != "")
                grpBoxControlls.Enabled = true;
            else
                grpBoxControlls.Enabled = false;

            if (txtBoxRCTop.Text != "")
            {
                txtBoxDestinationOfTxt.Text = Path.GetDirectoryName(@txtBoxRCTop.Text) + @"\Diff.txt";
            }
        }

        private void TxtBoxRCBottom_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxRCTop.Text != "" && txtBoxRCBottom.Text != "")
                grpBoxControlls.Enabled = true;
            else grpBoxControlls.Enabled = false;
        }
 
        private void BtnCompareRC_Click_1(object sender, EventArgs e)
        {

            
            ThreadStart ThreadDifference = new ThreadStart(GetDifferenceOfRCs);
            Thread CompareThread = new Thread(ThreadDifference);
            CompareThread.Start();

        }

        private void GetDifferenceOfRCs()
        {
            List<string> Section = new List<string>();
            MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

            string newDestination = GetDestination();

            List<List<string>> rc = new List<List<string>>();

            Invoke(SetsectionInvoke);
            Tuple<List<string>, List<string>, int> tRCs = Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

            if (tRCs.Item3 == 1)
            {
                MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                rcA = tRCs.Item1;
                rcB = tRCs.Item2;

                rc = Sorter.Split(rcA, rcB);

                Logic.GetDifference(rc, newDestination, @txtBoxRCTop.Text, @txtBoxRCBottom.Text);
            }
        }

        private void TxtBoxRCBottom_DragDrop(object sender, DragEventArgs e)
        {

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false); 

            txtBoxRCBottom.Text = s[0];
        }

        private void TxtBoxRCTop_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            txtBoxRCTop.Text = s[0];
        }

        private void TxtBoxRCTop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TxtBoxRCBottom_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void BtnCFFSIO_Click(object sender, EventArgs e)
        {
            ThreadStart ThreadDifferenceFS = new ThreadStart(GetDifferentFormatSpecifier);
            Thread CompareThreadFS = new Thread(ThreadDifferenceFS);
            CompareThreadFS.Start();

        }

        private void GetDifferentFormatSpecifier()
        {
            List<string> Section = new List<string>();
            MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

            string newDestination = GetDestination();

            List<List<string>> rc = new List<List<string>>();

            Invoke(SetsectionInvoke);
            Tuple<List<string>, List<string>, int> tRCs = Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

            if (tRCs.Item3 == 1)
            {
                MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                prgsBarProgress.Value = 0;
            }
            else
            {
                rcA = tRCs.Item1;
                rcB = tRCs.Item2;

                rc = Sorter.Split(rcA, rcB);

                Logic.GetDifferentFormatSpecifier(rc, newDestination, @txtBoxRCTop.Text, @txtBoxRCBottom.Text);
            }
        }

        private void BtnCheckfNTS_Click(object sender, EventArgs e)
        {
            ThreadStart ThreadDifferenceS = new ThreadStart(GetNotTransaltedStrings);
            Thread CompareThreadS = new Thread(ThreadDifferenceS);
            CompareThreadS.Start();
        }

        private void GetNotTransaltedStrings()
        {
            List<string> Section = new List<string>();
            MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

            string newDestination = GetDestination();

            List<List<string>> rc = new List<List<string>>();

            Invoke(SetsectionInvoke);
            Tuple<List<string>, List<string>, int> tRCs = Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

            if (tRCs.Item3 == 1)
            {
                MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                prgsBarProgress.Value = 0;
            }
            else
            {
                rcA = tRCs.Item1;
                rcB = tRCs.Item2;

                rc = Sorter.Split(rcA, rcB);

                Logic.GetNotTranslatedStrings(rc, newDestination, @txtBoxRCTop.Text, @txtBoxRCBottom.Text);
            }
        }

        private void BtnSortResources_Click(object sender, EventArgs e)
        {
            cmBoxChooseSection.SelectedItem = "All";
            ThreadStart ThreadSort = new ThreadStart(GetSortetRC);
            Thread SortThreads = new Thread(ThreadSort);
            SortThreads.Start();
        }

        private void GetSortetRC()
        {
            List<string> Section = new List<string>();
            MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };
            string newDestination = Path.GetDirectoryName(@txtBoxRCBottom.Text) + "\\" + Path.GetFileName(@txtBoxRCBottom.Text) + ".txt";
            List<List<string>> rc = new List<List<string>>();

            Invoke(SetsectionInvoke);
            Tuple<List<string>, List<string>, int> tRCs = Analyser.AnalyseFilesForCompleteSections(@txtBoxRCTop.Text, @txtBoxRCBottom.Text);


            if (tRCs.Item3 == 1)
            {
                MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                prgsBarProgress.Value = 0;
            }
            else
            {
                rcA = tRCs.Item1;
                rcB = tRCs.Item2;

                Logic.GetSortedRC(rcA, rcB, newDestination, @txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);
            }
        }
    }
}
