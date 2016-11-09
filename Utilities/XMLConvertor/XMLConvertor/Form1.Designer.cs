namespace XMLConvertor
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
            this.label2 = new System.Windows.Forms.Label();
            this.SourceXMLTextBox = new System.Windows.Forms.TextBox();
            this.XSLTFileTextBox = new System.Windows.Forms.TextBox();
            this.Convert = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SelXMLBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SelXSLTBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source XML";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "XSLTFile";
            // 
            // SourceXMLTextBox
            // 
            this.SourceXMLTextBox.Location = new System.Drawing.Point(84, 21);
            this.SourceXMLTextBox.Name = "SourceXMLTextBox";
            this.SourceXMLTextBox.Size = new System.Drawing.Size(134, 20);
            this.SourceXMLTextBox.TabIndex = 2;
            // 
            // XSLTFileTextBox
            // 
            this.XSLTFileTextBox.Location = new System.Drawing.Point(84, 69);
            this.XSLTFileTextBox.Name = "XSLTFileTextBox";
            this.XSLTFileTextBox.Size = new System.Drawing.Size(134, 20);
            this.XSLTFileTextBox.TabIndex = 3;
            // 
            // Convert
            // 
            this.Convert.Location = new System.Drawing.Point(49, 116);
            this.Convert.Name = "Convert";
            this.Convert.Size = new System.Drawing.Size(180, 23);
            this.Convert.TabIndex = 4;
            this.Convert.Text = "Convert";
            this.Convert.UseVisualStyleBackColor = true;
            this.Convert.Click += new System.EventHandler(this.Convert_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(274, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "The converted file will be put in C:\\Result.html";
            // 
            // SelXMLBtn
            // 
            this.SelXMLBtn.Location = new System.Drawing.Point(225, 21);
            this.SelXMLBtn.Name = "SelXMLBtn";
            this.SelXMLBtn.Size = new System.Drawing.Size(26, 23);
            this.SelXMLBtn.TabIndex = 6;
            this.SelXMLBtn.Text = "...";
            this.SelXMLBtn.UseVisualStyleBackColor = true;
            this.SelXMLBtn.Click += new System.EventHandler(this.SelXMLBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // SelXSLTBtn
            // 
            this.SelXSLTBtn.Location = new System.Drawing.Point(225, 69);
            this.SelXSLTBtn.Name = "SelXSLTBtn";
            this.SelXSLTBtn.Size = new System.Drawing.Size(26, 23);
            this.SelXSLTBtn.TabIndex = 7;
            this.SelXSLTBtn.Text = "...";
            this.SelXSLTBtn.UseVisualStyleBackColor = true;
            this.SelXSLTBtn.Click += new System.EventHandler(this.SelXSLTBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 204);
            this.Controls.Add(this.SelXSLTBtn);
            this.Controls.Add(this.SelXMLBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Convert);
            this.Controls.Add(this.XSLTFileTextBox);
            this.Controls.Add(this.SourceXMLTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "XML Convertor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SourceXMLTextBox;
        private System.Windows.Forms.TextBox XSLTFileTextBox;
        private System.Windows.Forms.Button Convert;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SelXMLBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button SelXSLTBtn;
    }
}

