namespace CountItemSets
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
            this.trackBarMaxSupport = new System.Windows.Forms.TrackBar();
            this.trackBarMinSupport = new System.Windows.Forms.TrackBar();
            this.progressBarLoadingData = new System.Windows.Forms.ProgressBar();
            this.labelMaxSupport = new System.Windows.Forms.Label();
            this.labelMinSupport = new System.Windows.Forms.Label();
            this.trackBarMaxLift = new System.Windows.Forms.TrackBar();
            this.trackBarMaxConfidence = new System.Windows.Forms.TrackBar();
            this.trackBarMinLift = new System.Windows.Forms.TrackBar();
            this.trackBarMinConfidence = new System.Windows.Forms.TrackBar();
            this.labelMaxLift = new System.Windows.Forms.Label();
            this.labelMaxConfidence = new System.Windows.Forms.Label();
            this.labelMinLift = new System.Windows.Forms.Label();
            this.labelMinConfidence = new System.Windows.Forms.Label();
            this.listBoxConditionFilterLevel1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.groupBoxAssociationRules.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxSupport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinSupport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxLift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxConfidence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinLift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinConfidence)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filename:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(69, 13);
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
            this.browseButton1.Location = new System.Drawing.Point(282, 12);
            this.browseButton1.Name = "browseButton1";
            this.browseButton1.Size = new System.Drawing.Size(75, 23);
            this.browseButton1.TabIndex = 2;
            this.browseButton1.Text = "Browse...";
            this.browseButton1.UseVisualStyleBackColor = true;
            this.browseButton1.Click += new System.EventHandler(this.browseButton1_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(363, 12);
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
            this.labelTransactionCount.Location = new System.Drawing.Point(567, 15);
            this.labelTransactionCount.Name = "labelTransactionCount";
            this.labelTransactionCount.Size = new System.Drawing.Size(93, 13);
            this.labelTransactionCount.TabIndex = 6;
            this.labelTransactionCount.Text = "Nr of transactions:";
            // 
            // textBoxTransactionCount
            // 
            this.textBoxTransactionCount.Location = new System.Drawing.Point(666, 12);
            this.textBoxTransactionCount.Name = "textBoxTransactionCount";
            this.textBoxTransactionCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxTransactionCount.TabIndex = 7;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(772, 16);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(33, 13);
            this.labelTime.TabIndex = 8;
            this.labelTime.Text = "Time:";
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(811, 12);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.Size = new System.Drawing.Size(100, 20);
            this.textBoxTime.TabIndex = 9;
            // 
            // groupBoxAssociationRules
            // 
            this.groupBoxAssociationRules.Controls.Add(this.dataGridViewResults);
            this.groupBoxAssociationRules.Location = new System.Drawing.Point(12, 171);
            this.groupBoxAssociationRules.Name = "groupBoxAssociationRules";
            this.groupBoxAssociationRules.Size = new System.Drawing.Size(899, 481);
            this.groupBoxAssociationRules.TabIndex = 10;
            this.groupBoxAssociationRules.TabStop = false;
            this.groupBoxAssociationRules.Text = "Association Rules:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxConditionFilterLevel1);
            this.groupBox1.Controls.Add(this.labelMinConfidence);
            this.groupBox1.Controls.Add(this.labelMinLift);
            this.groupBox1.Controls.Add(this.labelMaxConfidence);
            this.groupBox1.Controls.Add(this.labelMaxLift);
            this.groupBox1.Controls.Add(this.trackBarMinConfidence);
            this.groupBox1.Controls.Add(this.trackBarMinLift);
            this.groupBox1.Controls.Add(this.trackBarMaxConfidence);
            this.groupBox1.Controls.Add(this.trackBarMaxLift);
            this.groupBox1.Controls.Add(this.labelMinSupport);
            this.groupBox1.Controls.Add(this.labelMaxSupport);
            this.groupBox1.Controls.Add(this.trackBarMinSupport);
            this.groupBox1.Controls.Add(this.trackBarMaxSupport);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 126);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Presentation Filters";
            // 
            // trackBarMaxSupport
            // 
            this.trackBarMaxSupport.Location = new System.Drawing.Point(770, 20);
            this.trackBarMaxSupport.Maximum = 100;
            this.trackBarMaxSupport.Name = "trackBarMaxSupport";
            this.trackBarMaxSupport.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxSupport.TabIndex = 0;
            this.trackBarMaxSupport.TickFrequency = 10;
            this.trackBarMaxSupport.Value = 100;
            this.trackBarMaxSupport.Scroll += new System.EventHandler(this.trackBarMaxSupport_Scroll);
            this.trackBarMaxSupport.ValueChanged += new System.EventHandler(this.trackBarMaxSupport_ValueChanged);
            // 
            // trackBarMinSupport
            // 
            this.trackBarMinSupport.Location = new System.Drawing.Point(770, 71);
            this.trackBarMinSupport.Maximum = 100;
            this.trackBarMinSupport.Name = "trackBarMinSupport";
            this.trackBarMinSupport.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinSupport.TabIndex = 1;
            this.trackBarMinSupport.TickFrequency = 10;
            this.trackBarMinSupport.Scroll += new System.EventHandler(this.trackBarMinSupport_Scroll);
            this.trackBarMinSupport.ValueChanged += new System.EventHandler(this.trackBarMinSupport_ValueChanged);
            // 
            // progressBarLoadingData
            // 
            this.progressBarLoadingData.Location = new System.Drawing.Point(444, 12);
            this.progressBarLoadingData.Name = "progressBarLoadingData";
            this.progressBarLoadingData.Size = new System.Drawing.Size(100, 23);
            this.progressBarLoadingData.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarLoadingData.TabIndex = 12;
            // 
            // labelMaxSupport
            // 
            this.labelMaxSupport.AutoSize = true;
            this.labelMaxSupport.Location = new System.Drawing.Point(808, 51);
            this.labelMaxSupport.Name = "labelMaxSupport";
            this.labelMaxSupport.Size = new System.Drawing.Size(40, 13);
            this.labelMaxSupport.TabIndex = 2;
            this.labelMaxSupport.Text = "1.0000";
            this.labelMaxSupport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMinSupport
            // 
            this.labelMinSupport.AutoSize = true;
            this.labelMinSupport.Location = new System.Drawing.Point(808, 103);
            this.labelMinSupport.Name = "labelMinSupport";
            this.labelMinSupport.Size = new System.Drawing.Size(40, 13);
            this.labelMinSupport.TabIndex = 3;
            this.labelMinSupport.Text = "0.0000";
            // 
            // trackBarMaxLift
            // 
            this.trackBarMaxLift.Location = new System.Drawing.Point(660, 19);
            this.trackBarMaxLift.Maximum = 100;
            this.trackBarMaxLift.Name = "trackBarMaxLift";
            this.trackBarMaxLift.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxLift.TabIndex = 4;
            this.trackBarMaxLift.TickFrequency = 10;
            this.trackBarMaxLift.Value = 100;
            this.trackBarMaxLift.Scroll += new System.EventHandler(this.trackBarMaxLift_Scroll);
            this.trackBarMaxLift.ValueChanged += new System.EventHandler(this.trackBarMaxLift_ValueChanged);
            // 
            // trackBarMaxConfidence
            // 
            this.trackBarMaxConfidence.Location = new System.Drawing.Point(550, 19);
            this.trackBarMaxConfidence.Maximum = 100;
            this.trackBarMaxConfidence.Name = "trackBarMaxConfidence";
            this.trackBarMaxConfidence.Size = new System.Drawing.Size(104, 45);
            this.trackBarMaxConfidence.TabIndex = 5;
            this.trackBarMaxConfidence.TickFrequency = 10;
            this.trackBarMaxConfidence.Value = 100;
            this.trackBarMaxConfidence.Scroll += new System.EventHandler(this.trackBarMaxConfidence_Scroll);
            this.trackBarMaxConfidence.ValueChanged += new System.EventHandler(this.trackBarMaxConfidence_ValueChanged);
            // 
            // trackBarMinLift
            // 
            this.trackBarMinLift.Location = new System.Drawing.Point(660, 70);
            this.trackBarMinLift.Maximum = 100;
            this.trackBarMinLift.Name = "trackBarMinLift";
            this.trackBarMinLift.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinLift.TabIndex = 6;
            this.trackBarMinLift.TickFrequency = 10;
            this.trackBarMinLift.Scroll += new System.EventHandler(this.trackBarMinLift_Scroll);
            this.trackBarMinLift.ValueChanged += new System.EventHandler(this.trackBarMinLift_ValueChanged);
            // 
            // trackBarMinConfidence
            // 
            this.trackBarMinConfidence.Location = new System.Drawing.Point(550, 70);
            this.trackBarMinConfidence.Maximum = 100;
            this.trackBarMinConfidence.Name = "trackBarMinConfidence";
            this.trackBarMinConfidence.Size = new System.Drawing.Size(104, 45);
            this.trackBarMinConfidence.TabIndex = 7;
            this.trackBarMinConfidence.TickFrequency = 10;
            this.trackBarMinConfidence.Scroll += new System.EventHandler(this.trackBarMinConfidence_Scroll);
            this.trackBarMinConfidence.ValueChanged += new System.EventHandler(this.trackBarMinConfidence_ValueChanged);
            // 
            // labelMaxLift
            // 
            this.labelMaxLift.AutoSize = true;
            this.labelMaxLift.Location = new System.Drawing.Point(693, 51);
            this.labelMaxLift.Name = "labelMaxLift";
            this.labelMaxLift.Size = new System.Drawing.Size(40, 13);
            this.labelMaxLift.TabIndex = 8;
            this.labelMaxLift.Text = "1.0000";
            this.labelMaxLift.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMaxConfidence
            // 
            this.labelMaxConfidence.AutoSize = true;
            this.labelMaxConfidence.Location = new System.Drawing.Point(584, 51);
            this.labelMaxConfidence.Name = "labelMaxConfidence";
            this.labelMaxConfidence.Size = new System.Drawing.Size(40, 13);
            this.labelMaxConfidence.TabIndex = 9;
            this.labelMaxConfidence.Text = "1.0000";
            this.labelMaxConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMinLift
            // 
            this.labelMinLift.AutoSize = true;
            this.labelMinLift.Location = new System.Drawing.Point(693, 103);
            this.labelMinLift.Name = "labelMinLift";
            this.labelMinLift.Size = new System.Drawing.Size(40, 13);
            this.labelMinLift.TabIndex = 10;
            this.labelMinLift.Text = "0.0000";
            // 
            // labelMinConfidence
            // 
            this.labelMinConfidence.AutoSize = true;
            this.labelMinConfidence.Location = new System.Drawing.Point(584, 103);
            this.labelMinConfidence.Name = "labelMinConfidence";
            this.labelMinConfidence.Size = new System.Drawing.Size(40, 13);
            this.labelMinConfidence.TabIndex = 11;
            this.labelMinConfidence.Text = "0.0000";
            // 
            // listBoxConditionFilterLevel1
            // 
            this.listBoxConditionFilterLevel1.FormattingEnabled = true;
            this.listBoxConditionFilterLevel1.Location = new System.Drawing.Point(18, 20);
            this.listBoxConditionFilterLevel1.Name = "listBoxConditionFilterLevel1";
            this.listBoxConditionFilterLevel1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxConditionFilterLevel1.Size = new System.Drawing.Size(138, 95);
            this.listBoxConditionFilterLevel1.TabIndex = 12;
            this.listBoxConditionFilterLevel1.SelectedIndexChanged += new System.EventHandler(this.listBoxConditionFilterLevel1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 664);
            this.Controls.Add(this.progressBarLoadingData);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxAssociationRules);
            this.Controls.Add(this.textBoxTime);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.textBoxTransactionCount);
            this.Controls.Add(this.labelTransactionCount);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.browseButton1);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "CountItemSets";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.groupBoxAssociationRules.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxSupport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinSupport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxLift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxConfidence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinLift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinConfidence)).EndInit();
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
    }
}

