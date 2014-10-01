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
            this.label2 = new System.Windows.Forms.Label();
            this.labelTransactionCount = new System.Windows.Forms.Label();
            this.textBoxTransactionCount = new System.Windows.Forms.TextBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
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
            this.textBoxFileName.Size = new System.Drawing.Size(554, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // browseButton1
            // 
            this.browseButton1.Location = new System.Drawing.Point(629, 12);
            this.browseButton1.Name = "browseButton1";
            this.browseButton1.Size = new System.Drawing.Size(75, 23);
            this.browseButton1.TabIndex = 2;
            this.browseButton1.Text = "Browse...";
            this.browseButton1.UseVisualStyleBackColor = true;
            this.browseButton1.Click += new System.EventHandler(this.browseButton1_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(629, 54);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.Location = new System.Drawing.Point(69, 100);
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.Size = new System.Drawing.Size(635, 420);
            this.dataGridViewResults.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Results:";
            // 
            // labelTransactionCount
            // 
            this.labelTransactionCount.AutoSize = true;
            this.labelTransactionCount.Location = new System.Drawing.Point(14, 59);
            this.labelTransactionCount.Name = "labelTransactionCount";
            this.labelTransactionCount.Size = new System.Drawing.Size(93, 13);
            this.labelTransactionCount.TabIndex = 6;
            this.labelTransactionCount.Text = "Nr of transactions:";
            // 
            // textBoxTransactionCount
            // 
            this.textBoxTransactionCount.Location = new System.Drawing.Point(113, 56);
            this.textBoxTransactionCount.Name = "textBoxTransactionCount";
            this.textBoxTransactionCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxTransactionCount.TabIndex = 7;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(233, 59);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(33, 13);
            this.labelTime.TabIndex = 8;
            this.labelTime.Text = "Time:";
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(273, 56);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.Size = new System.Drawing.Size(100, 20);
            this.textBoxTime.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 532);
            this.Controls.Add(this.textBoxTime);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.textBoxTransactionCount);
            this.Controls.Add(this.labelTransactionCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewResults);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.browseButton1);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "CountItemSets";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTransactionCount;
        private System.Windows.Forms.TextBox textBoxTransactionCount;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.TextBox textBoxTime;
    }
}

