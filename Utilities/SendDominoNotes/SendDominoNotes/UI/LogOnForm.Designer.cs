namespace SendDominoNotes
{
    partial class LogOnForm
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
            this.ServerNameComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PwdTextBox = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.LogInButton = new System.Windows.Forms.Button();
            this.SavePwdCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // ServerNameComboBox
            // 
            this.ServerNameComboBox.FormattingEnabled = true;
            this.ServerNameComboBox.Location = new System.Drawing.Point(98, 12);
            this.ServerNameComboBox.Name = "ServerNameComboBox";
            this.ServerNameComboBox.Size = new System.Drawing.Size(177, 21);
            this.ServerNameComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // PwdTextBox
            // 
            this.PwdTextBox.Location = new System.Drawing.Point(98, 45);
            this.PwdTextBox.Name = "PwdTextBox";
            this.PwdTextBox.Size = new System.Drawing.Size(177, 20);
            this.PwdTextBox.TabIndex = 3;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(200, 76);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // LogInButton
            // 
            this.LogInButton.Location = new System.Drawing.Point(122, 76);
            this.LogInButton.Name = "LogInButton";
            this.LogInButton.Size = new System.Drawing.Size(75, 23);
            this.LogInButton.TabIndex = 5;
            this.LogInButton.Text = "Log In";
            this.LogInButton.UseVisualStyleBackColor = true;
            this.LogInButton.Click += new System.EventHandler(this.LogInButton_Click);
            // 
            // SavePwdCheckBox
            // 
            this.SavePwdCheckBox.AutoSize = true;
            this.SavePwdCheckBox.Location = new System.Drawing.Point(17, 80);
            this.SavePwdCheckBox.Name = "SavePwdCheckBox";
            this.SavePwdCheckBox.Size = new System.Drawing.Size(99, 17);
            this.SavePwdCheckBox.TabIndex = 6;
            this.SavePwdCheckBox.Text = "Save password";
            this.SavePwdCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogOnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 107);
            this.Controls.Add(this.SavePwdCheckBox);
            this.Controls.Add(this.LogInButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.PwdTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServerNameComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LogOnForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LogInForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ServerNameComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PwdTextBox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button LogInButton;
        private System.Windows.Forms.CheckBox SavePwdCheckBox;
    }
}