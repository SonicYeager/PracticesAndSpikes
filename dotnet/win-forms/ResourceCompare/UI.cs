using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ResourceCompare.CodeDirectory.LogicClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.AnalyserClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.SorterClass;

namespace ResourceCompare;

public sealed partial class FrmRC : Form
{
    //private readonly Logic Logic = new Logic();

    private readonly string[] fileNames = new string[2];
    private List<string> rcA = new();

    private List<string> rcB = new();

    //readonly string[] sectionName = new string[3] { "stringtable", "dialog", "menu" };
    private string section;

    public FrmRC() : this("", "")
    {
    }

    public FrmRC(string resA, string resB)
    {
        fileNames[0] = resA;
        fileNames[1] = resB;

        InitializeComponent();
        Logic.BackgroundWorker.ProgressChanged += new(BackgroundWorker_ProgressChanged);
        Logic.BackgroundWorker.WorkerReportsProgress = true;

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
            var sections = new List<string>
            {
                section,
            };
            return sections;
        }
        else
        {
            var sections = new List<string>
            {
                "String Table", "Dialog", "Menu",
            };
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
        var open = new OpenFileDialog();
        var dialogResult = open.ShowDialog();
        if (dialogResult == DialogResult.OK) txtBoxRCTop.Text = open.FileName;
    }

    private void BtnFilepickerBottom_Click(object sender, EventArgs e)
    {
        var open = new OpenFileDialog();
        var dialogResult = open.ShowDialog();
        if (dialogResult == DialogResult.OK) txtBoxRCBottom.Text = open.FileName;
    }

    private void BtnTargetTxtDestination_Click(object sender, EventArgs e)
    {
        var open = new OpenFileDialog();
        var dialogResult = open.ShowDialog();
        if (dialogResult == DialogResult.OK) txtBoxDestinationOfTxt.Text = open.FileName;
    }

    private void TxtBoxRCTop_TextChanged(object sender, EventArgs e)
    {
        if (txtBoxRCTop.Text != "" && txtBoxRCBottom.Text != "")
            grpBoxControlls.Enabled = true;
        else
            grpBoxControlls.Enabled = false;

        if (txtBoxRCTop.Text != "") txtBoxDestinationOfTxt.Text = Path.GetDirectoryName(@txtBoxRCTop.Text) + @"\Diff.txt";
    }

    private void TxtBoxRCBottom_TextChanged(object sender, EventArgs e)
    {
        if (txtBoxRCTop.Text != "" && txtBoxRCBottom.Text != "")
            grpBoxControlls.Enabled = true;
        else grpBoxControlls.Enabled = false;
    }

    private void BtnCompareRC_Click_1(object sender, EventArgs e)
    {
        var ThreadDifference = new ThreadStart(GetDifferenceOfRCs);
        var CompareThread = new Thread(ThreadDifference);
        CompareThread.Start();
    }

    private void GetDifferenceOfRCs()
    {
        var Section = new List<string>();
        MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

        var newDestination = GetDestination();

        var rc = new List<List<string>>();

        Invoke(SetsectionInvoke);
        var tRCs =
            Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

        if (tRCs.Item3 == 1)
        {
            MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
        var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

        txtBoxRCBottom.Text = s[0];
    }

    private void TxtBoxRCTop_DragDrop(object sender, DragEventArgs e)
    {
        var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

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
        var ThreadDifferenceFS = new ThreadStart(GetDifferentFormatSpecifier);
        var CompareThreadFS = new Thread(ThreadDifferenceFS);
        CompareThreadFS.Start();
    }

    private void GetDifferentFormatSpecifier()
    {
        var Section = new List<string>();
        MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

        var newDestination = GetDestination();

        var rc = new List<List<string>>();

        Invoke(SetsectionInvoke);
        var tRCs =
            Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

        if (tRCs.Item3 == 1)
        {
            MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
        var ThreadDifferenceS = new ThreadStart(GetNotTransaltedStrings);
        var CompareThreadS = new Thread(ThreadDifferenceS);
        CompareThreadS.Start();
    }

    private void GetNotTransaltedStrings()
    {
        var Section = new List<string>();
        MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };

        var newDestination = GetDestination();

        var rc = new List<List<string>>();

        Invoke(SetsectionInvoke);
        var tRCs =
            Analyser.AnalyseFiles(@txtBoxRCTop.Text, @txtBoxRCBottom.Text, Section);

        if (tRCs.Item3 == 1)
        {
            MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
        var ThreadSort = new ThreadStart(GetSortetRC);
        var SortThreads = new Thread(ThreadSort);
        SortThreads.Start();
    }

    private void GetSortetRC()
    {
        var Section = new List<string>();
        MethodInvoker SetsectionInvoke = delegate { Section = SetSection(); };
        var newDestination = Path.GetDirectoryName(@txtBoxRCBottom.Text) + "\\" + Path.GetFileName(@txtBoxRCBottom.Text) + ".txt";
        var rc = new List<List<string>>();

        Invoke(SetsectionInvoke);
        var tRCs =
            Analyser.AnalyseFilesForCompleteSections(@txtBoxRCTop.Text,
                @txtBoxRCBottom.Text);


        if (tRCs.Item3 == 1)
        {
            MessageBox.Show("Fehlerhafte Pfadangabe / gesuchte Datei nicht vorhanden!", "Messed it up!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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