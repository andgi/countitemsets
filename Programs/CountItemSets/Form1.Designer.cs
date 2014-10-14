﻿namespace CountItemSets
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.browseButton1 = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.labelTransactionCount = new System.Windows.Forms.Label();
            this.textBoxTransactionCount = new System.Windows.Forms.TextBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            this.groupBoxAssociationRules = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxThenFilterLevel1 = new System.Windows.Forms.ListBox();
            this.listBoxConditionFilterLevel1 = new System.Windows.Forms.ListBox();
            this.labelMinConfidence = new System.Windows.Forms.Label();
            this.labelMinLift = new System.Windows.Forms.Label();
            this.labelMaxConfidence = new System.Windows.Forms.Label();
            this.labelMaxLift = new System.Windows.Forms.Label();
            this.trackBarMinConfidence = new System.Windows.Forms.TrackBar();
            this.trackBarMinLift = new System.Windows.Forms.TrackBar();
            this.trackBarMaxConfidence = new System.Windows.Forms.TrackBar();
            this.trackBarMaxLift = new System.Windows.Forms.TrackBar();
            this.labelMinSupport = new System.Windows.Forms.Label();
            this.labelMaxSupport = new System.Windows.Forms.Label();
            this.trackBarMinSupport = new System.Windows.Forms.TrackBar();
            this.trackBarMaxSupport = new System.Windows.Forms.TrackBar();
            this.progressBarLoadingData = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.groupBoxAssociationRules.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinConfidence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinLift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxConfidence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxLift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinSupport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxSupport)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filename:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(64, 37);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(207, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // browseButton1
            // 
            this.browseButton1.Location = new System.Drawing.Point(277, 35);
            this.browseButton1.Name = "browseButton1";
            this.browseButton1.Size = new System.Drawing.Size(75, 23);
            this.browseButton1.TabIndex = 2;
            this.browseButton1.Text = "Browse...";
            this.browseButton1.UseVisualStyleBackColor = true;
            this.browseButton1.Click += new System.EventHandler(this.browseButton1_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(358, 35);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.AllowUserToAddRows = false;
            this.dataGridViewResults.AllowUserToDeleteRows = false;
            this.dataGridViewResults.AllowUserToResizeRows = false;
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.ReadOnly = true;
            this.dataGridViewResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewResults.ShowCellErrors = false;
            this.dataGridViewResults.Size = new System.Drawing.Size(885, 451);
            this.dataGridViewResults.TabIndex = 4;
            // 
            // labelTransactionCount
            // 
            this.labelTransactionCount.AutoSize = true;
            this.labelTransactionCount.Location = new System.Drawing.Point(545, 40);
            this.labelTransactionCount.Name = "labelTransactionCount";
            this.labelTransactionCount.Size = new System.Drawing.Size(93, 13);
            this.labelTransactionCount.TabIndex = 6;
            this.labelTransactionCount.Text = "Nr of transactions:";
            // 
            // textBoxTransactionCount
            // 
            this.textBoxTransactionCount.Location = new System.Drawing.Point(644, 37);
            this.textBoxTransactionCount.Name = "textBoxTransactionCount";
            this.textBoxTransactionCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxTransactionCount.TabIndex = 7;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(750, 40);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(33, 13);
            this.labelTime.TabIndex = 8;
            this.labelTime.Text = "Time:";
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(789, 37);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.Size = new System.Drawing.Size(100, 20);
            this.textBoxTime.TabIndex = 9;
            // 
            // groupBoxAssociationRules
            // 
            this.groupBoxAssociationRules.Controls.Add(this.dataGridViewResults);
            this.groupBoxAssociationRules.Location = new System.Drawing.Point(3, 182);
            this.groupBoxAssociationRules.Name = "groupBoxAssociationRules";
            this.groupBoxAssociationRules.Size = new System.Drawing.Size(899, 481);
            this.groupBoxAssociationRules.TabIndex = 10;
            this.groupBoxAssociationRules.TabStop = false;
            this.groupBoxAssociationRules.Text = "Association Rules:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.listBoxThenFilterLevel1);
            this.groupBox1.Controls.Add(this.listBoxConditionFilterLevel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(896, 142);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Presentation Filters";
            // 
            // listBoxThenFilterLevel1
            // 
            this.listBoxThenFilterLevel1.FormattingEnabled = true;
            this.listBoxThenFilterLevel1.Location = new System.Drawing.Point(188, 28);
            this.listBoxThenFilterLevel1.Name = "listBoxThenFilterLevel1";
            this.listBoxThenFilterLevel1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxThenFilterLevel1.Size = new System.Drawing.Size(138, 95);
            this.listBoxThenFilterLevel1.TabIndex = 13;
            this.listBoxThenFilterLevel1.SelectedIndexChanged += new System.EventHandler(this.listBoxThenFilterLevel1_SelectedIndexChanged);
            // 
            // listBoxConditionFilterLevel1
            // 
            this.listBoxConditionFilterLevel1.FormattingEnabled = true;
            this.listBoxConditionFilterLevel1.Location = new System.Drawing.Point(6, 28);
            this.listBoxConditionFilterLevel1.Name = "listBoxConditionFilterLevel1";
            this.listBoxConditionFilterLevel1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxConditionFilterLevel1.Size = new System.Drawing.Size(138, 95);
            this.listBoxConditionFilterLevel1.TabIndex = 12;
            this.listBoxConditionFilterLevel1.SelectedIndexChanged += new System.EventHandler(this.listBoxConditionFilterLevel1_SelectedIndexChanged);
            // 
            // labelMinConfidence
            // 
            this.labelMinConfidence.AutoSize = true;
            this.labelMinConfidence.Location = new System.Drawing.Point(40, 99);
            this.labelMinConfidence.Name = "labelMinConfidence";
            this.labelMinConfidence.Size = new System.Drawing.Size(40, 13);
            this.labelMinConfidence.TabIndex = 11;
            this.labelMinConfidence.Text = "0.0000";
            // 
            // labelMinLift
            // 
            this.labelMinLift.AutoSize = true;
            this.labelMinLift.Location = new System.Drawing.Point(37, 98);
            this.labelMinLift.Name = "labelMinLift";
            this.labelMinLift.Size = new System.Drawing.Size(40, 13);
            this.labelMinLift.TabIndex = 10;
            this.labelMinLift.Text = "0.0000";
            this.labelMinLift.Click += new System.EventHandler(this.labelMinLift_Click);
            // 
            // labelMaxConfidence
            // 
            this.labelMaxConfidence.AutoSize = true;
            this.labelMaxConfidence.Location = new System.Drawing.Point(40, 51);
            this.labelMaxConfidence.Name = "labelMaxConfidence";
            this.labelMaxConfidence.Size = new System.Drawing.Size(40, 13);
            this.labelMaxConfidence.TabIndex = 9;
            this.labelMaxConfidence.Text = "1.0000";
            this.labelMaxConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMaxLift
            // 
            this.labelMaxLift.AutoSize = true;
            this.labelMaxLift.Location = new System.Drawing.Point(37, 46);
            this.labelMaxLift.Name = "labelMaxLift";
            this.labelMaxLift.Size = new System.Drawing.Size(40, 13);
            this.labelMaxLift.TabIndex = 8;
            this.labelMaxLift.Text = "1.0000";
            this.labelMaxLift.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMaxLift.Click += new System.EventHandler(this.labelMaxLift_Click);
            // 
            // trackBarMinConfidence
            // 
            this.trackBarMinConfidence.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMinConfidence.Location = new System.Drawing.Point(6, 67);
            this.trackBarMinConfidence.Maximum = 100;
            this.trackBarMinConfidence.Name = "trackBarMinConfidence";
            this.trackBarMinConfidence.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinConfidence.TabIndex = 7;
            this.trackBarMinConfidence.TickFrequency = 10;
            this.trackBarMinConfidence.Scroll += new System.EventHandler(this.trackBarMinConfidence_Scroll);
            this.trackBarMinConfidence.ValueChanged += new System.EventHandler(this.trackBarMinConfidence_ValueChanged);
            // 
            // trackBarMinLift
            // 
            this.trackBarMinLift.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMinLift.Location = new System.Drawing.Point(4, 65);
            this.trackBarMinLift.Maximum = 100;
            this.trackBarMinLift.Name = "trackBarMinLift";
            this.trackBarMinLift.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinLift.TabIndex = 6;
            this.trackBarMinLift.TickFrequency = 10;
            this.trackBarMinLift.Scroll += new System.EventHandler(this.trackBarMinLift_Scroll);
            this.trackBarMinLift.ValueChanged += new System.EventHandler(this.trackBarMinLift_ValueChanged);
            // 
            // trackBarMaxConfidence
            // 
            this.trackBarMaxConfidence.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMaxConfidence.Location = new System.Drawing.Point(6, 19);
            this.trackBarMaxConfidence.Maximum = 100;
            this.trackBarMaxConfidence.Name = "trackBarMaxConfidence";
            this.trackBarMaxConfidence.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxConfidence.TabIndex = 5;
            this.trackBarMaxConfidence.TickFrequency = 10;
            this.trackBarMaxConfidence.Value = 100;
            this.trackBarMaxConfidence.Scroll += new System.EventHandler(this.trackBarMaxConfidence_Scroll);
            this.trackBarMaxConfidence.ValueChanged += new System.EventHandler(this.trackBarMaxConfidence_ValueChanged);
            // 
            // trackBarMaxLift
            // 
            this.trackBarMaxLift.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMaxLift.Location = new System.Drawing.Point(4, 14);
            this.trackBarMaxLift.Maximum = 100;
            this.trackBarMaxLift.Name = "trackBarMaxLift";
            this.trackBarMaxLift.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxLift.TabIndex = 4;
            this.trackBarMaxLift.TickFrequency = 10;
            this.trackBarMaxLift.Value = 100;
            this.trackBarMaxLift.Scroll += new System.EventHandler(this.trackBarMaxLift_Scroll);
            this.trackBarMaxLift.ValueChanged += new System.EventHandler(this.trackBarMaxLift_ValueChanged);
            // 
            // labelMinSupport
            // 
            this.labelMinSupport.AutoSize = true;
            this.labelMinSupport.Location = new System.Drawing.Point(42, 97);
            this.labelMinSupport.Name = "labelMinSupport";
            this.labelMinSupport.Size = new System.Drawing.Size(40, 13);
            this.labelMinSupport.TabIndex = 3;
            this.labelMinSupport.Text = "0.0000";
            // 
            // labelMaxSupport
            // 
            this.labelMaxSupport.AutoSize = true;
            this.labelMaxSupport.Location = new System.Drawing.Point(42, 45);
            this.labelMaxSupport.Name = "labelMaxSupport";
            this.labelMaxSupport.Size = new System.Drawing.Size(40, 13);
            this.labelMaxSupport.TabIndex = 2;
            this.labelMaxSupport.Text = "1.0000";
            this.labelMaxSupport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBarMinSupport
            // 
            this.trackBarMinSupport.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMinSupport.Location = new System.Drawing.Point(4, 65);
            this.trackBarMinSupport.Maximum = 100;
            this.trackBarMinSupport.Name = "trackBarMinSupport";
            this.trackBarMinSupport.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinSupport.TabIndex = 1;
            this.trackBarMinSupport.TickFrequency = 10;
            this.trackBarMinSupport.Scroll += new System.EventHandler(this.trackBarMinSupport_Scroll);
            this.trackBarMinSupport.ValueChanged += new System.EventHandler(this.trackBarMinSupport_ValueChanged);
            // 
            // trackBarMaxSupport
            // 
            this.trackBarMaxSupport.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBarMaxSupport.Location = new System.Drawing.Point(4, 14);
            this.trackBarMaxSupport.Maximum = 100;
            this.trackBarMaxSupport.Name = "trackBarMaxSupport";
            this.trackBarMaxSupport.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxSupport.TabIndex = 0;
            this.trackBarMaxSupport.TickFrequency = 10;
            this.trackBarMaxSupport.Value = 100;
            this.trackBarMaxSupport.Scroll += new System.EventHandler(this.trackBarMaxSupport_Scroll);
            this.trackBarMaxSupport.ValueChanged += new System.EventHandler(this.trackBarMaxSupport_ValueChanged);
            // 
            // progressBarLoadingData
            // 
            this.progressBarLoadingData.Location = new System.Drawing.Point(439, 35);
            this.progressBarLoadingData.Name = "progressBarLoadingData";
            this.progressBarLoadingData.Size = new System.Drawing.Size(100, 23);
            this.progressBarLoadingData.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarLoadingData.TabIndex = 12;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(976, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.printToolStripMenuItem.Text = "&Print";
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
            this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(976, 692);
            this.tabControl1.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBoxTime);
            this.tabPage1.Controls.Add(this.progressBarLoadingData);
            this.tabPage1.Controls.Add(this.labelTime);
            this.tabPage1.Controls.Add(this.textBoxFileName);
            this.tabPage1.Controls.Add(this.textBoxTransactionCount);
            this.tabPage1.Controls.Add(this.browseButton1);
            this.tabPage1.Controls.Add(this.labelTransactionCount);
            this.tabPage1.Controls.Add(this.buttonStart);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(968, 666);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Market Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.groupBoxAssociationRules);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(968, 666);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Association Rules Browser";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelMaxConfidence);
            this.groupBox2.Controls.Add(this.labelMinConfidence);
            this.groupBox2.Controls.Add(this.trackBarMaxConfidence);
            this.groupBox2.Controls.Add(this.trackBarMinConfidence);
            this.groupBox2.Location = new System.Drawing.Point(536, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(114, 121);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Confidence";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelMinLift);
            this.groupBox3.Controls.Add(this.labelMaxLift);
            this.groupBox3.Controls.Add(this.trackBarMinLift);
            this.groupBox3.Controls.Add(this.trackBarMaxLift);
            this.groupBox3.Location = new System.Drawing.Point(656, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(114, 121);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lift";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelMaxSupport);
            this.groupBox4.Controls.Add(this.labelMinSupport);
            this.groupBox4.Controls.Add(this.trackBarMinSupport);
            this.groupBox4.Controls.Add(this.trackBarMaxSupport);
            this.groupBox4.Location = new System.Drawing.Point(776, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(114, 121);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Support";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 715);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Market Basket Analyzer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.groupBoxAssociationRules.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinConfidence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinLift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxConfidence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxLift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinSupport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxSupport)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browseButton1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.DataGridView dataGridViewResults;
        private System.Windows.Forms.Label labelTransactionCount;
        private System.Windows.Forms.TextBox textBoxTransactionCount;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.GroupBox groupBoxAssociationRules;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBarMinSupport;
        private System.Windows.Forms.TrackBar trackBarMaxSupport;
        private System.Windows.Forms.ProgressBar progressBarLoadingData;
        private System.Windows.Forms.Label labelMinSupport;
        private System.Windows.Forms.Label labelMaxSupport;
        private System.Windows.Forms.Label labelMinConfidence;
        private System.Windows.Forms.Label labelMinLift;
        private System.Windows.Forms.Label labelMaxConfidence;
        private System.Windows.Forms.Label labelMaxLift;
        private System.Windows.Forms.TrackBar trackBarMinConfidence;
        private System.Windows.Forms.TrackBar trackBarMinLift;
        private System.Windows.Forms.TrackBar trackBarMaxConfidence;
        private System.Windows.Forms.TrackBar trackBarMaxLift;
        private System.Windows.Forms.ListBox listBoxConditionFilterLevel1;
        private System.Windows.Forms.ListBox listBoxThenFilterLevel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

