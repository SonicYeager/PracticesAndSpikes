namespace ResourceCompare
{
    partial class FrmRC
    {
        
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRC));
            this.btnCompareRC = new System.Windows.Forms.Button();
            this.lblRCTop = new System.Windows.Forms.Label();
            this.txtBoxRCTop = new System.Windows.Forms.TextBox();
            this.grpRCFilepicker = new System.Windows.Forms.GroupBox();
            this.btnTargetTxtDestination = new System.Windows.Forms.Button();
            this.btnFilepickerBottom = new System.Windows.Forms.Button();
            this.btnFilePickerTop = new System.Windows.Forms.Button();
            this.txtBoxDestinationOfTxt = new System.Windows.Forms.TextBox();
            this.txtBoxRCBottom = new System.Windows.Forms.TextBox();
            this.lblDestinationOfTxt = new System.Windows.Forms.Label();
            this.lblRCBottom = new System.Windows.Forms.Label();
            this.grpBoxControlls = new System.Windows.Forms.GroupBox();
            this.btnSortResources = new System.Windows.Forms.Button();
            this.grpBoxSpecialSettings = new System.Windows.Forms.GroupBox();
            this.cmBoxChooseSection = new System.Windows.Forms.ComboBox();
            this.btnCheckfNTS = new System.Windows.Forms.Button();
            this.btnCFFSIO = new System.Windows.Forms.Button();
            this.grpBoxLoad = new System.Windows.Forms.GroupBox();
            this.prgsBarProgress = new System.Windows.Forms.ProgressBar();
            this.lblVersion = new System.Windows.Forms.Label();
            this.grpRCFilepicker.SuspendLayout();
            this.grpBoxControlls.SuspendLayout();
            this.grpBoxSpecialSettings.SuspendLayout();
            this.grpBoxLoad.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCompareRC
            // 
            this.btnCompareRC.BackColor = System.Drawing.Color.Gainsboro;
            this.btnCompareRC.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCompareRC.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCompareRC.FlatAppearance.BorderSize = 0;
            this.btnCompareRC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnCompareRC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCompareRC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompareRC.Location = new System.Drawing.Point(6, 18);
            this.btnCompareRC.Name = "btnCompareRC";
            this.btnCompareRC.Size = new System.Drawing.Size(184, 23);
            this.btnCompareRC.TabIndex = 0;
            this.btnCompareRC.Text = "Compare";
            this.btnCompareRC.UseVisualStyleBackColor = false;
            this.btnCompareRC.Click += new System.EventHandler(this.BtnCompareRC_Click_1);
            // 
            // lblRCTop
            // 
            this.lblRCTop.AutoSize = true;
            this.lblRCTop.Location = new System.Drawing.Point(6, 22);
            this.lblRCTop.Name = "lblRCTop";
            this.lblRCTop.Size = new System.Drawing.Size(66, 13);
            this.lblRCTop.TabIndex = 1;
            this.lblRCTop.Text = "Resource A:";
            // 
            // txtBoxRCTop
            // 
            this.txtBoxRCTop.AllowDrop = true;
            this.txtBoxRCTop.BackColor = System.Drawing.Color.Gainsboro;
            this.txtBoxRCTop.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBoxRCTop.Location = new System.Drawing.Point(87, 22);
            this.txtBoxRCTop.MinimumSize = new System.Drawing.Size(0, 15);
            this.txtBoxRCTop.Name = "txtBoxRCTop";
            this.txtBoxRCTop.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtBoxRCTop.ShortcutsEnabled = false;
            this.txtBoxRCTop.Size = new System.Drawing.Size(361, 13);
            this.txtBoxRCTop.TabIndex = 2;
            this.txtBoxRCTop.WordWrap = false;
            this.txtBoxRCTop.TextChanged += new System.EventHandler(this.TxtBoxRCTop_TextChanged);
            this.txtBoxRCTop.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxtBoxRCTop_DragDrop);
            this.txtBoxRCTop.DragEnter += new System.Windows.Forms.DragEventHandler(this.TxtBoxRCTop_DragEnter);
            // 
            // grpRCFilepicker
            // 
            this.grpRCFilepicker.AutoSize = true;
            this.grpRCFilepicker.Controls.Add(this.btnTargetTxtDestination);
            this.grpRCFilepicker.Controls.Add(this.btnFilepickerBottom);
            this.grpRCFilepicker.Controls.Add(this.btnFilePickerTop);
            this.grpRCFilepicker.Controls.Add(this.txtBoxDestinationOfTxt);
            this.grpRCFilepicker.Controls.Add(this.txtBoxRCBottom);
            this.grpRCFilepicker.Controls.Add(this.lblDestinationOfTxt);
            this.grpRCFilepicker.Controls.Add(this.txtBoxRCTop);
            this.grpRCFilepicker.Controls.Add(this.lblRCBottom);
            this.grpRCFilepicker.Controls.Add(this.lblRCTop);
            this.grpRCFilepicker.Location = new System.Drawing.Point(12, 28);
            this.grpRCFilepicker.Name = "grpRCFilepicker";
            this.grpRCFilepicker.Size = new System.Drawing.Size(513, 154);
            this.grpRCFilepicker.TabIndex = 3;
            this.grpRCFilepicker.TabStop = false;
            this.grpRCFilepicker.Text = "Resourcendateien wählen";
            // 
            // btnTargetTxtDestination
            // 
            this.btnTargetTxtDestination.BackColor = System.Drawing.Color.Gainsboro;
            this.btnTargetTxtDestination.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnTargetTxtDestination.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnTargetTxtDestination.FlatAppearance.BorderSize = 0;
            this.btnTargetTxtDestination.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnTargetTxtDestination.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnTargetTxtDestination.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTargetTxtDestination.Location = new System.Drawing.Point(454, 115);
            this.btnTargetTxtDestination.Name = "btnTargetTxtDestination";
            this.btnTargetTxtDestination.Size = new System.Drawing.Size(53, 20);
            this.btnTargetTxtDestination.TabIndex = 7;
            this.btnTargetTxtDestination.Text = "Browse";
            this.btnTargetTxtDestination.UseVisualStyleBackColor = false;
            this.btnTargetTxtDestination.Click += new System.EventHandler(this.BtnTargetTxtDestination_Click);
            // 
            // btnFilepickerBottom
            // 
            this.btnFilepickerBottom.BackColor = System.Drawing.Color.Gainsboro;
            this.btnFilepickerBottom.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFilepickerBottom.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnFilepickerBottom.FlatAppearance.BorderSize = 0;
            this.btnFilepickerBottom.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnFilepickerBottom.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnFilepickerBottom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilepickerBottom.Location = new System.Drawing.Point(454, 51);
            this.btnFilepickerBottom.Name = "btnFilepickerBottom";
            this.btnFilepickerBottom.Size = new System.Drawing.Size(53, 20);
            this.btnFilepickerBottom.TabIndex = 6;
            this.btnFilepickerBottom.Text = "Browse";
            this.btnFilepickerBottom.UseVisualStyleBackColor = false;
            this.btnFilepickerBottom.Click += new System.EventHandler(this.BtnFilepickerBottom_Click);
            // 
            // btnFilePickerTop
            // 
            this.btnFilePickerTop.BackColor = System.Drawing.Color.Gainsboro;
            this.btnFilePickerTop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFilePickerTop.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnFilePickerTop.FlatAppearance.BorderSize = 0;
            this.btnFilePickerTop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnFilePickerTop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnFilePickerTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilePickerTop.Location = new System.Drawing.Point(454, 18);
            this.btnFilePickerTop.Name = "btnFilePickerTop";
            this.btnFilePickerTop.Size = new System.Drawing.Size(53, 20);
            this.btnFilePickerTop.TabIndex = 1;
            this.btnFilePickerTop.Text = "Browse";
            this.btnFilePickerTop.UseVisualStyleBackColor = false;
            this.btnFilePickerTop.Click += new System.EventHandler(this.BtnFilePickerTop_Click);
            // 
            // txtBoxDestinationOfTxt
            // 
            this.txtBoxDestinationOfTxt.BackColor = System.Drawing.Color.Gainsboro;
            this.txtBoxDestinationOfTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBoxDestinationOfTxt.Location = new System.Drawing.Point(87, 119);
            this.txtBoxDestinationOfTxt.Name = "txtBoxDestinationOfTxt";
            this.txtBoxDestinationOfTxt.ReadOnly = true;
            this.txtBoxDestinationOfTxt.Size = new System.Drawing.Size(361, 13);
            this.txtBoxDestinationOfTxt.TabIndex = 5;
            // 
            // txtBoxRCBottom
            // 
            this.txtBoxRCBottom.AllowDrop = true;
            this.txtBoxRCBottom.BackColor = System.Drawing.Color.Gainsboro;
            this.txtBoxRCBottom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBoxRCBottom.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBoxRCBottom.Location = new System.Drawing.Point(87, 55);
            this.txtBoxRCBottom.Name = "txtBoxRCBottom";
            this.txtBoxRCBottom.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtBoxRCBottom.Size = new System.Drawing.Size(361, 13);
            this.txtBoxRCBottom.TabIndex = 4;
            this.txtBoxRCBottom.WordWrap = false;
            this.txtBoxRCBottom.TextChanged += new System.EventHandler(this.TxtBoxRCBottom_TextChanged);
            this.txtBoxRCBottom.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxtBoxRCBottom_DragDrop);
            this.txtBoxRCBottom.DragEnter += new System.Windows.Forms.DragEventHandler(this.TxtBoxRCBottom_DragEnter);
            // 
            // lblDestinationOfTxt
            // 
            this.lblDestinationOfTxt.AutoSize = true;
            this.lblDestinationOfTxt.Location = new System.Drawing.Point(6, 119);
            this.lblDestinationOfTxt.Name = "lblDestinationOfTxt";
            this.lblDestinationOfTxt.Size = new System.Drawing.Size(64, 13);
            this.lblDestinationOfTxt.TabIndex = 3;
            this.lblDestinationOfTxt.Text = "Speicherort:";
            // 
            // lblRCBottom
            // 
            this.lblRCBottom.AutoSize = true;
            this.lblRCBottom.Location = new System.Drawing.Point(6, 55);
            this.lblRCBottom.Name = "lblRCBottom";
            this.lblRCBottom.Size = new System.Drawing.Size(66, 13);
            this.lblRCBottom.TabIndex = 2;
            this.lblRCBottom.Text = "Resource B:";
            // 
            // grpBoxControlls
            // 
            this.grpBoxControlls.AutoSize = true;
            this.grpBoxControlls.Controls.Add(this.btnSortResources);
            this.grpBoxControlls.Controls.Add(this.grpBoxSpecialSettings);
            this.grpBoxControlls.Controls.Add(this.btnCheckfNTS);
            this.grpBoxControlls.Controls.Add(this.btnCFFSIO);
            this.grpBoxControlls.Controls.Add(this.grpBoxLoad);
            this.grpBoxControlls.Controls.Add(this.btnCompareRC);
            this.grpBoxControlls.Enabled = false;
            this.grpBoxControlls.Location = new System.Drawing.Point(12, 187);
            this.grpBoxControlls.Margin = new System.Windows.Forms.Padding(0);
            this.grpBoxControlls.Name = "grpBoxControlls";
            this.grpBoxControlls.Padding = new System.Windows.Forms.Padding(0);
            this.grpBoxControlls.Size = new System.Drawing.Size(513, 187);
            this.grpBoxControlls.TabIndex = 6;
            this.grpBoxControlls.TabStop = false;
            this.grpBoxControlls.Text = "Operations";
            // 
            // btnSortResources
            // 
            this.btnSortResources.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSortResources.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSortResources.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSortResources.FlatAppearance.BorderSize = 0;
            this.btnSortResources.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnSortResources.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnSortResources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSortResources.Location = new System.Drawing.Point(6, 99);
            this.btnSortResources.Name = "btnSortResources";
            this.btnSortResources.Size = new System.Drawing.Size(184, 23);
            this.btnSortResources.TabIndex = 11;
            this.btnSortResources.Text = "Sort Resource B like A";
            this.btnSortResources.UseVisualStyleBackColor = false;
            this.btnSortResources.Click += new System.EventHandler(this.BtnSortResources_Click);
            // 
            // grpBoxSpecialSettings
            // 
            this.grpBoxSpecialSettings.Controls.Add(this.cmBoxChooseSection);
            this.grpBoxSpecialSettings.Location = new System.Drawing.Point(247, 13);
            this.grpBoxSpecialSettings.Margin = new System.Windows.Forms.Padding(0);
            this.grpBoxSpecialSettings.Name = "grpBoxSpecialSettings";
            this.grpBoxSpecialSettings.Size = new System.Drawing.Size(260, 109);
            this.grpBoxSpecialSettings.TabIndex = 10;
            this.grpBoxSpecialSettings.TabStop = false;
            this.grpBoxSpecialSettings.Text = "Settings";
            // 
            // cmBoxChooseSection
            // 
            this.cmBoxChooseSection.FormattingEnabled = true;
            this.cmBoxChooseSection.Items.AddRange(new object[] {
            "String Table",
            "Dialog",
            "Menu",
            "All"});
            this.cmBoxChooseSection.Location = new System.Drawing.Point(6, 19);
            this.cmBoxChooseSection.Name = "cmBoxChooseSection";
            this.cmBoxChooseSection.Size = new System.Drawing.Size(121, 21);
            this.cmBoxChooseSection.TabIndex = 0;
            this.cmBoxChooseSection.Text = "All";
            // 
            // btnCheckfNTS
            // 
            this.btnCheckfNTS.BackColor = System.Drawing.Color.Gainsboro;
            this.btnCheckfNTS.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCheckfNTS.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCheckfNTS.FlatAppearance.BorderSize = 0;
            this.btnCheckfNTS.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnCheckfNTS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCheckfNTS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckfNTS.Location = new System.Drawing.Point(6, 72);
            this.btnCheckfNTS.Name = "btnCheckfNTS";
            this.btnCheckfNTS.Size = new System.Drawing.Size(184, 23);
            this.btnCheckfNTS.TabIndex = 9;
            this.btnCheckfNTS.Text = "Check for Non-Translated Strings";
            this.btnCheckfNTS.UseVisualStyleBackColor = false;
            this.btnCheckfNTS.Click += new System.EventHandler(this.BtnCheckfNTS_Click);
            // 
            // btnCFFSIO
            // 
            this.btnCFFSIO.BackColor = System.Drawing.Color.Gainsboro;
            this.btnCFFSIO.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCFFSIO.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCFFSIO.FlatAppearance.BorderSize = 0;
            this.btnCFFSIO.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnCFFSIO.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCFFSIO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCFFSIO.Location = new System.Drawing.Point(6, 45);
            this.btnCFFSIO.Name = "btnCFFSIO";
            this.btnCFFSIO.Size = new System.Drawing.Size(184, 23);
            this.btnCFFSIO.TabIndex = 8;
            this.btnCFFSIO.Text = "Check for Placeholder";
            this.btnCFFSIO.UseVisualStyleBackColor = false;
            this.btnCFFSIO.Click += new System.EventHandler(this.BtnCFFSIO_Click);
            // 
            // grpBoxLoad
            // 
            this.grpBoxLoad.Controls.Add(this.prgsBarProgress);
            this.grpBoxLoad.Enabled = false;
            this.grpBoxLoad.Location = new System.Drawing.Point(6, 125);
            this.grpBoxLoad.Margin = new System.Windows.Forms.Padding(0);
            this.grpBoxLoad.Name = "grpBoxLoad";
            this.grpBoxLoad.Padding = new System.Windows.Forms.Padding(0);
            this.grpBoxLoad.Size = new System.Drawing.Size(501, 49);
            this.grpBoxLoad.TabIndex = 7;
            this.grpBoxLoad.TabStop = false;
            this.grpBoxLoad.Text = "Status";
            // 
            // prgsBarProgress
            // 
            this.prgsBarProgress.AccessibleName = "prgsBarProgress";
            this.prgsBarProgress.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.prgsBarProgress.BackColor = System.Drawing.Color.Gainsboro;
            this.prgsBarProgress.ForeColor = System.Drawing.Color.Orange;
            this.prgsBarProgress.Location = new System.Drawing.Point(6, 19);
            this.prgsBarProgress.Name = "prgsBarProgress";
            this.prgsBarProgress.Size = new System.Drawing.Size(489, 23);
            this.prgsBarProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgsBarProgress.TabIndex = 8;
            this.prgsBarProgress.UseWaitCursor = true;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 12);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(37, 13);
            this.lblVersion.TabIndex = 8;
            this.lblVersion.Text = "v6.1.0";
            // 
            // FrmRC
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(535, 383);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.grpBoxControlls);
            this.Controls.Add(this.grpRCFilepicker);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmRC";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ResourceCompare";
            this.grpRCFilepicker.ResumeLayout(false);
            this.grpRCFilepicker.PerformLayout();
            this.grpBoxControlls.ResumeLayout(false);
            this.grpBoxSpecialSettings.ResumeLayout(false);
            this.grpBoxLoad.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



        private System.Windows.Forms.Button btnCompareRC;
        private System.Windows.Forms.Label lblRCTop;
        private System.Windows.Forms.TextBox txtBoxRCTop;
        private System.Windows.Forms.GroupBox grpRCFilepicker;
        private System.Windows.Forms.TextBox txtBoxDestinationOfTxt;
        private System.Windows.Forms.TextBox txtBoxRCBottom;
        private System.Windows.Forms.Label lblDestinationOfTxt;
        private System.Windows.Forms.Label lblRCBottom;
        private System.Windows.Forms.GroupBox grpBoxControlls;
        private System.Windows.Forms.Button btnFilePickerTop;
        private System.Windows.Forms.Button btnFilepickerBottom;
        private System.Windows.Forms.Button btnTargetTxtDestination;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.GroupBox grpBoxLoad;
        public System.Windows.Forms.ProgressBar prgsBarProgress;
        private System.Windows.Forms.Button btnCFFSIO;
        private System.Windows.Forms.Button btnCheckfNTS;
        private System.Windows.Forms.GroupBox grpBoxSpecialSettings;
        private System.Windows.Forms.ComboBox cmBoxChooseSection;
        private System.Windows.Forms.Button btnSortResources;
    }
}

