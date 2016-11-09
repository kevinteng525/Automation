namespace SendDominoNotes
{
    partial class SenderMainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AttachmentBrowserButton = new System.Windows.Forms.Button();
            this.SelectReceiverButton = new System.Windows.Forms.Button();
            this.SelectSenderButton = new System.Windows.Forms.Button();
            this.MessageBodyRichTextBox = new System.Windows.Forms.RichTextBox();
            this.MessageAttachmentsTextBox = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.MessageSubjectTextBox = new System.Windows.Forms.TextBox();
            this.SenderButton = new System.Windows.Forms.Button();
            this.ReceiverTextBox = new System.Windows.Forms.TextBox();
            this.SenderTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.MailNumberTextBox = new System.Windows.Forms.TextBox();
            this.ScheduleGroupBox = new System.Windows.Forms.GroupBox();
            this.TaskAddButton = new System.Windows.Forms.Button();
            this.DefaultButton = new System.Windows.Forms.Button();
            this.AllDayRadioButton = new System.Windows.Forms.RadioButton();
            this.EndTimeDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.BetweenTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.StartTimeDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.MinutesTextBox = new System.Windows.Forms.TextBox();
            this.HoursTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.EndDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.StartDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.openFolderDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.TaskListGroupBox = new System.Windows.Forms.GroupBox();
            this.TaskListView = new System.Windows.Forms.ListView();
            this.TaskDeleteButton = new System.Windows.Forms.Button();
            this.TaskStopButton = new System.Windows.Forms.Button();
            this.TaskStartButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.ScheduleGroupBox.SuspendLayout();
            this.TaskListGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AttachmentBrowserButton);
            this.groupBox1.Controls.Add(this.SelectReceiverButton);
            this.groupBox1.Controls.Add(this.SelectSenderButton);
            this.groupBox1.Controls.Add(this.MessageBodyRichTextBox);
            this.groupBox1.Controls.Add(this.MessageAttachmentsTextBox);
            this.groupBox1.Controls.Add(this.CancelButton);
            this.groupBox1.Controls.Add(this.MessageSubjectTextBox);
            this.groupBox1.Controls.Add(this.SenderButton);
            this.groupBox1.Controls.Add(this.ReceiverTextBox);
            this.groupBox1.Controls.Add(this.SenderTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.MailNumberTextBox);
            this.groupBox1.Location = new System.Drawing.Point(7, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 342);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sender";
            // 
            // AttachmentBrowserButton
            // 
            this.AttachmentBrowserButton.Location = new System.Drawing.Point(246, 239);
            this.AttachmentBrowserButton.Name = "AttachmentBrowserButton";
            this.AttachmentBrowserButton.Size = new System.Drawing.Size(59, 23);
            this.AttachmentBrowserButton.TabIndex = 10;
            this.AttachmentBrowserButton.Text = "Browser";
            this.AttachmentBrowserButton.UseVisualStyleBackColor = true;
            this.AttachmentBrowserButton.Click += new System.EventHandler(this.AttachmentBrowserButton_Click);
            // 
            // SelectReceiverButton
            // 
            this.SelectReceiverButton.Location = new System.Drawing.Point(246, 55);
            this.SelectReceiverButton.Name = "SelectReceiverButton";
            this.SelectReceiverButton.Size = new System.Drawing.Size(59, 23);
            this.SelectReceiverButton.TabIndex = 9;
            this.SelectReceiverButton.Text = "Select";
            this.SelectReceiverButton.UseVisualStyleBackColor = true;
            this.SelectReceiverButton.Click += new System.EventHandler(this.SelectReceiverButton_Click);
            // 
            // SelectSenderButton
            // 
            this.SelectSenderButton.Location = new System.Drawing.Point(246, 24);
            this.SelectSenderButton.Name = "SelectSenderButton";
            this.SelectSenderButton.Size = new System.Drawing.Size(59, 23);
            this.SelectSenderButton.TabIndex = 8;
            this.SelectSenderButton.Text = "Select";
            this.SelectSenderButton.UseVisualStyleBackColor = true;
            this.SelectSenderButton.Click += new System.EventHandler(this.SelectSenderButton_Click);
            // 
            // MessageBodyRichTextBox
            // 
            this.MessageBodyRichTextBox.Location = new System.Drawing.Point(87, 124);
            this.MessageBodyRichTextBox.Name = "MessageBodyRichTextBox";
            this.MessageBodyRichTextBox.Size = new System.Drawing.Size(218, 103);
            this.MessageBodyRichTextBox.TabIndex = 7;
            this.MessageBodyRichTextBox.Text = "";
            // 
            // MessageAttachmentsTextBox
            // 
            this.MessageAttachmentsTextBox.Location = new System.Drawing.Point(87, 241);
            this.MessageAttachmentsTextBox.Name = "MessageAttachmentsTextBox";
            this.MessageAttachmentsTextBox.Size = new System.Drawing.Size(153, 20);
            this.MessageAttachmentsTextBox.TabIndex = 6;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(245, 306);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(60, 23);
            this.CancelButton.TabIndex = 10;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // MessageSubjectTextBox
            // 
            this.MessageSubjectTextBox.Location = new System.Drawing.Point(87, 89);
            this.MessageSubjectTextBox.Name = "MessageSubjectTextBox";
            this.MessageSubjectTextBox.Size = new System.Drawing.Size(218, 20);
            this.MessageSubjectTextBox.TabIndex = 4;
            // 
            // SenderButton
            // 
            this.SenderButton.Location = new System.Drawing.Point(179, 306);
            this.SenderButton.Name = "SenderButton";
            this.SenderButton.Size = new System.Drawing.Size(60, 23);
            this.SenderButton.TabIndex = 10;
            this.SenderButton.Text = "Send";
            this.SenderButton.UseVisualStyleBackColor = true;
            this.SenderButton.Click += new System.EventHandler(this.SenderButton_Click);
            // 
            // ReceiverTextBox
            // 
            this.ReceiverTextBox.Location = new System.Drawing.Point(87, 56);
            this.ReceiverTextBox.Name = "ReceiverTextBox";
            this.ReceiverTextBox.Size = new System.Drawing.Size(153, 20);
            this.ReceiverTextBox.TabIndex = 3;
            // 
            // SenderTextBox
            // 
            this.SenderTextBox.Location = new System.Drawing.Point(87, 26);
            this.SenderTextBox.Name = "SenderTextBox";
            this.SenderTextBox.Size = new System.Drawing.Size(153, 20);
            this.SenderTextBox.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 244);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Attachment:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Body:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Subject:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "From:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Mail Number:";
            // 
            // MailNumberTextBox
            // 
            this.MailNumberTextBox.Location = new System.Drawing.Point(87, 272);
            this.MailNumberTextBox.Name = "MailNumberTextBox";
            this.MailNumberTextBox.Size = new System.Drawing.Size(88, 20);
            this.MailNumberTextBox.TabIndex = 1;
            // 
            // ScheduleGroupBox
            // 
            this.ScheduleGroupBox.Controls.Add(this.TaskAddButton);
            this.ScheduleGroupBox.Controls.Add(this.DefaultButton);
            this.ScheduleGroupBox.Controls.Add(this.AllDayRadioButton);
            this.ScheduleGroupBox.Controls.Add(this.EndTimeDateTimePicker);
            this.ScheduleGroupBox.Controls.Add(this.BetweenTimeRadioButton);
            this.ScheduleGroupBox.Controls.Add(this.label15);
            this.ScheduleGroupBox.Controls.Add(this.StartTimeDateTimePicker);
            this.ScheduleGroupBox.Controls.Add(this.MinutesTextBox);
            this.ScheduleGroupBox.Controls.Add(this.HoursTextBox);
            this.ScheduleGroupBox.Controls.Add(this.label13);
            this.ScheduleGroupBox.Controls.Add(this.label12);
            this.ScheduleGroupBox.Controls.Add(this.EndDateDateTimePicker);
            this.ScheduleGroupBox.Controls.Add(this.StartDateDateTimePicker);
            this.ScheduleGroupBox.Controls.Add(this.label11);
            this.ScheduleGroupBox.Controls.Add(this.label10);
            this.ScheduleGroupBox.Controls.Add(this.label9);
            this.ScheduleGroupBox.Location = new System.Drawing.Point(331, 13);
            this.ScheduleGroupBox.Name = "ScheduleGroupBox";
            this.ScheduleGroupBox.Size = new System.Drawing.Size(295, 185);
            this.ScheduleGroupBox.TabIndex = 1;
            this.ScheduleGroupBox.TabStop = false;
            this.ScheduleGroupBox.Text = "Schedule";
            // 
            // TaskAddButton
            // 
            this.TaskAddButton.Location = new System.Drawing.Point(160, 154);
            this.TaskAddButton.Name = "TaskAddButton";
            this.TaskAddButton.Size = new System.Drawing.Size(60, 23);
            this.TaskAddButton.TabIndex = 18;
            this.TaskAddButton.Text = "Add";
            this.TaskAddButton.UseVisualStyleBackColor = true;
            this.TaskAddButton.Click += new System.EventHandler(this.TaskAddButton_Click);
            // 
            // DefaultButton
            // 
            this.DefaultButton.Location = new System.Drawing.Point(226, 154);
            this.DefaultButton.Name = "DefaultButton";
            this.DefaultButton.Size = new System.Drawing.Size(60, 23);
            this.DefaultButton.TabIndex = 14;
            this.DefaultButton.Text = "Default";
            this.DefaultButton.UseVisualStyleBackColor = true;
            this.DefaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // AllDayRadioButton
            // 
            this.AllDayRadioButton.AutoSize = true;
            this.AllDayRadioButton.Location = new System.Drawing.Point(9, 75);
            this.AllDayRadioButton.Name = "AllDayRadioButton";
            this.AllDayRadioButton.Size = new System.Drawing.Size(58, 17);
            this.AllDayRadioButton.TabIndex = 17;
            this.AllDayRadioButton.TabStop = true;
            this.AllDayRadioButton.Text = "All Day";
            this.AllDayRadioButton.UseVisualStyleBackColor = true;
            // 
            // EndTimeDateTimePicker
            // 
            this.EndTimeDateTimePicker.Location = new System.Drawing.Point(220, 50);
            this.EndTimeDateTimePicker.Name = "EndTimeDateTimePicker";
            this.EndTimeDateTimePicker.Size = new System.Drawing.Size(66, 20);
            this.EndTimeDateTimePicker.TabIndex = 16;
            // 
            // BetweenTimeRadioButton
            // 
            this.BetweenTimeRadioButton.AutoSize = true;
            this.BetweenTimeRadioButton.Location = new System.Drawing.Point(9, 52);
            this.BetweenTimeRadioButton.Name = "BetweenTimeRadioButton";
            this.BetweenTimeRadioButton.Size = new System.Drawing.Size(101, 17);
            this.BetweenTimeRadioButton.TabIndex = 15;
            this.BetweenTimeRadioButton.TabStop = true;
            this.BetweenTimeRadioButton.Text = "Between Times:";
            this.BetweenTimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(188, 54);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(26, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "And";
            // 
            // StartTimeDateTimePicker
            // 
            this.StartTimeDateTimePicker.Location = new System.Drawing.Point(116, 50);
            this.StartTimeDateTimePicker.Name = "StartTimeDateTimePicker";
            this.StartTimeDateTimePicker.Size = new System.Drawing.Size(66, 20);
            this.StartTimeDateTimePicker.TabIndex = 12;
            // 
            // MinutesTextBox
            // 
            this.MinutesTextBox.Location = new System.Drawing.Point(145, 25);
            this.MinutesTextBox.Name = "MinutesTextBox";
            this.MinutesTextBox.Size = new System.Drawing.Size(34, 20);
            this.MinutesTextBox.TabIndex = 9;
            // 
            // HoursTextBox
            // 
            this.HoursTextBox.Location = new System.Drawing.Point(69, 25);
            this.HoursTextBox.Name = "HoursTextBox";
            this.HoursTextBox.Size = new System.Drawing.Size(34, 20);
            this.HoursTextBox.TabIndex = 8;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 128);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "End Date:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Start Date:";
            // 
            // EndDateDateTimePicker
            // 
            this.EndDateDateTimePicker.Location = new System.Drawing.Point(88, 124);
            this.EndDateDateTimePicker.Name = "EndDateDateTimePicker";
            this.EndDateDateTimePicker.Size = new System.Drawing.Size(126, 20);
            this.EndDateDateTimePicker.TabIndex = 6;
            // 
            // StartDateDateTimePicker
            // 
            this.StartDateDateTimePicker.Location = new System.Drawing.Point(88, 96);
            this.StartDateDateTimePicker.Name = "StartDateDateTimePicker";
            this.StartDateDateTimePicker.Size = new System.Drawing.Size(126, 20);
            this.StartDateDateTimePicker.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(182, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Minutes";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(105, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Hours";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Run every";
            // 
            // TaskListGroupBox
            // 
            this.TaskListGroupBox.Controls.Add(this.TaskListView);
            this.TaskListGroupBox.Controls.Add(this.TaskDeleteButton);
            this.TaskListGroupBox.Controls.Add(this.TaskStopButton);
            this.TaskListGroupBox.Controls.Add(this.TaskStartButton);
            this.TaskListGroupBox.Location = new System.Drawing.Point(331, 204);
            this.TaskListGroupBox.Name = "TaskListGroupBox";
            this.TaskListGroupBox.Size = new System.Drawing.Size(295, 151);
            this.TaskListGroupBox.TabIndex = 2;
            this.TaskListGroupBox.TabStop = false;
            this.TaskListGroupBox.Text = "Task List";
            // 
            // TaskListView
            // 
            this.TaskListView.Location = new System.Drawing.Point(9, 20);
            this.TaskListView.Name = "TaskListView";
            this.TaskListView.Size = new System.Drawing.Size(205, 118);
            this.TaskListView.TabIndex = 15;
            this.TaskListView.UseCompatibleStateImageBehavior = false;
            // 
            // TaskDeleteButton
            // 
            this.TaskDeleteButton.Location = new System.Drawing.Point(231, 98);
            this.TaskDeleteButton.Name = "TaskDeleteButton";
            this.TaskDeleteButton.Size = new System.Drawing.Size(55, 23);
            this.TaskDeleteButton.TabIndex = 14;
            this.TaskDeleteButton.Text = "Delete";
            this.TaskDeleteButton.UseVisualStyleBackColor = true;
            this.TaskDeleteButton.Click += new System.EventHandler(this.TaskDeleteButton_Click);
            // 
            // TaskStopButton
            // 
            this.TaskStopButton.Location = new System.Drawing.Point(231, 65);
            this.TaskStopButton.Name = "TaskStopButton";
            this.TaskStopButton.Size = new System.Drawing.Size(55, 23);
            this.TaskStopButton.TabIndex = 13;
            this.TaskStopButton.Text = "Stop";
            this.TaskStopButton.UseVisualStyleBackColor = true;
            this.TaskStopButton.Click += new System.EventHandler(this.TaskStopButton_Click);
            // 
            // TaskStartButton
            // 
            this.TaskStartButton.Location = new System.Drawing.Point(231, 34);
            this.TaskStartButton.Name = "TaskStartButton";
            this.TaskStartButton.Size = new System.Drawing.Size(55, 23);
            this.TaskStartButton.TabIndex = 12;
            this.TaskStartButton.Text = "Start";
            this.TaskStartButton.UseVisualStyleBackColor = true;
            this.TaskStartButton.Click += new System.EventHandler(this.TaskStartButton_Click);
            // 
            // SenderMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(631, 366);
            this.Controls.Add(this.TaskListGroupBox);
            this.Controls.Add(this.ScheduleGroupBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SenderMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Domino Notes Sender";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ScheduleGroupBox.ResumeLayout(false);
            this.ScheduleGroupBox.PerformLayout();
            this.TaskListGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox MessageAttachmentsTextBox;
        private System.Windows.Forms.TextBox MessageSubjectTextBox;
        private System.Windows.Forms.TextBox ReceiverTextBox;
        private System.Windows.Forms.TextBox SenderTextBox;
        private System.Windows.Forms.RichTextBox MessageBodyRichTextBox;
        private System.Windows.Forms.Button SelectSenderButton;
        private System.Windows.Forms.Button SelectReceiverButton;
        private System.Windows.Forms.GroupBox ScheduleGroupBox;
        private System.Windows.Forms.TextBox MailNumberTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox MinutesTextBox;
        private System.Windows.Forms.TextBox HoursTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker EndDateDateTimePicker;
        private System.Windows.Forms.DateTimePicker StartDateDateTimePicker;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SenderButton;
        private System.Windows.Forms.Button AttachmentBrowserButton;
        private System.Windows.Forms.RadioButton AllDayRadioButton;
        private System.Windows.Forms.DateTimePicker EndTimeDateTimePicker;
        private System.Windows.Forms.RadioButton BetweenTimeRadioButton;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker StartTimeDateTimePicker;
        private System.Windows.Forms.FolderBrowserDialog openFolderDialog1;
        private System.Windows.Forms.GroupBox TaskListGroupBox;
        private System.Windows.Forms.Button TaskStopButton;
        private System.Windows.Forms.Button TaskStartButton;
        private System.Windows.Forms.Button TaskAddButton;
        private System.Windows.Forms.Button DefaultButton;
        private System.Windows.Forms.Button TaskDeleteButton;
        private System.Windows.Forms.ListView TaskListView;
    }
}

