namespace SMTPEmailSenderGUIApp
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
            this.csvButton = new System.Windows.Forms.Button();
            this.patternTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.domainTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.folderBrowserTextbox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.totalMailNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.mailboxlNum = new System.Windows.Forms.NumericUpDown();
            this.LogFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.totalMailNumUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailboxlNum)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(223, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a pattern file contains the mail box info.";
            // 
            // csvButton
            // 
            this.csvButton.Location = new System.Drawing.Point(235, 25);
            this.csvButton.Name = "csvButton";
            this.csvButton.Size = new System.Drawing.Size(40, 23);
            this.csvButton.TabIndex = 2;
            this.csvButton.Text = "...";
            this.csvButton.UseVisualStyleBackColor = true;
            this.csvButton.Click += new System.EventHandler(this.csvButton_Click);
            // 
            // patternTextBox
            // 
            this.patternTextBox.Location = new System.Drawing.Point(12, 25);
            this.patternTextBox.Name = "patternTextBox";
            this.patternTextBox.Size = new System.Drawing.Size(217, 20);
            this.patternTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(182, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Mail box number for each group type.";
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(63, 327);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(128, 23);
            this.sendButton.TabIndex = 12;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 298);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(268, 15);
            this.progressBar1.TabIndex = 6;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(222, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Input the domain info (Ex: yourcompany.com).";
            // 
            // domainTextBox
            // 
            this.domainTextBox.Location = new System.Drawing.Point(12, 69);
            this.domainTextBox.Name = "domainTextBox";
            this.domainTextBox.Size = new System.Drawing.Size(100, 20);
            this.domainTextBox.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(271, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Input the folder path which contains all the attachments.";
            // 
            // folderBrowserTextbox
            // 
            this.folderBrowserTextbox.Location = new System.Drawing.Point(12, 169);
            this.folderBrowserTextbox.Name = "folderBrowserTextbox";
            this.folderBrowserTextbox.Size = new System.Drawing.Size(217, 20);
            this.folderBrowserTextbox.TabIndex = 8;
            // 
            // folderBrowserButton
            // 
            this.folderBrowserButton.Location = new System.Drawing.Point(235, 169);
            this.folderBrowserButton.Name = "folderBrowserButton";
            this.folderBrowserButton.Size = new System.Drawing.Size(40, 23);
            this.folderBrowserButton.TabIndex = 9;
            this.folderBrowserButton.Text = "...";
            this.folderBrowserButton.UseVisualStyleBackColor = true;
            this.folderBrowserButton.Click += new System.EventHandler(this.folderBrowserButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Total number of eamils you want to send.";
            // 
            // totalMailNumUpDown
            // 
            this.totalMailNumUpDown.Location = new System.Drawing.Point(12, 217);
            this.totalMailNumUpDown.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.totalMailNumUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.totalMailNumUpDown.Name = "totalMailNumUpDown";
            this.totalMailNumUpDown.Size = new System.Drawing.Size(120, 20);
            this.totalMailNumUpDown.TabIndex = 11;
            this.totalMailNumUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mailboxlNum
            // 
            this.mailboxlNum.Location = new System.Drawing.Point(12, 119);
            this.mailboxlNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.mailboxlNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mailboxlNum.Name = "mailboxlNum";
            this.mailboxlNum.Size = new System.Drawing.Size(120, 20);
            this.mailboxlNum.TabIndex = 6;
            this.mailboxlNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LogFilePathTextBox
            // 
            this.LogFilePathTextBox.Location = new System.Drawing.Point(12, 268);
            this.LogFilePathTextBox.Name = "LogFilePathTextBox";
            this.LogFilePathTextBox.Size = new System.Drawing.Size(219, 20);
            this.LogFilePathTextBox.TabIndex = 13;
            this.LogFilePathTextBox.Text = "C:\\SMTPEmailSenderErrorLog.txt";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Log file path.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 358);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LogFilePathTextBox);
            this.Controls.Add(this.mailboxlNum);
            this.Controls.Add(this.totalMailNumUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.folderBrowserButton);
            this.Controls.Add(this.folderBrowserTextbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.domainTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.patternTextBox);
            this.Controls.Add(this.csvButton);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Email Sender";
            ((System.ComponentModel.ISupportInitialize)(this.totalMailNumUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailboxlNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button csvButton;
        private System.Windows.Forms.TextBox patternTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox domainTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox folderBrowserTextbox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button folderBrowserButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown totalMailNumUpDown;
        private System.Windows.Forms.NumericUpDown mailboxlNum;
        private System.Windows.Forms.TextBox LogFilePathTextBox;
        private System.Windows.Forms.Label label6;
    }
}

