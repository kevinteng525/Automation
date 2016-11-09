namespace SendDominoNotes
{
    partial class SenderUserLoad
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
            this.SenderAddressListView = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.SenderAddressTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SenderAddressListView
            // 
            this.SenderAddressListView.Location = new System.Drawing.Point(16, 53);
            this.SenderAddressListView.Name = "SenderAddressListView";
            this.SenderAddressListView.Size = new System.Drawing.Size(243, 110);
            this.SenderAddressListView.TabIndex = 0;
            this.SenderAddressListView.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sender address:";
            // 
            // SenderAddressTextBox
            // 
            this.SenderAddressTextBox.Location = new System.Drawing.Point(102, 17);
            this.SenderAddressTextBox.Name = "SenderAddressTextBox";
            this.SenderAddressTextBox.Size = new System.Drawing.Size(156, 20);
            this.SenderAddressTextBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(102, 178);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(183, 178);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SenderUserLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 211);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SenderAddressTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SenderAddressListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SenderUserLoad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SenderUserLoad";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView SenderAddressListView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SenderAddressTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button CancelButton;
    }
}