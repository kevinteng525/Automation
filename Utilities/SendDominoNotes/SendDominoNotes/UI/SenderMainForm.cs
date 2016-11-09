using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Domino;

namespace SendDominoNotes
{
    public partial class SenderMainForm : Form
    {
        //private string mServer = "Mail01/BRS";
        //private string mPassword = "emcsiax@QA";
        private List<string> receiversList = null;
        private NotesGenerator mNotesGenerator = null;

        public SenderMainForm(NotesGenerator noteGenerator)
        {
            InitializeComponent();
            InitialSenderControls();
            InitialScheduleControls();
            InitialTaskListViewControl();
            //mNotesGenerator = NotesGenerator.Instance(mServer, mPassword);
            mNotesGenerator = noteGenerator;
            this.FormClosed += new FormClosedEventHandler(SenderMainForm_FormClosed);
        }

        public NotesGenerator NotesGenerator
        {
            get { return mNotesGenerator; }
        }

        private void InitialSenderControls()
        {
            this.SenderTextBox.Enabled = false;
            this.SelectSenderButton.Enabled = false;
        }

        private void InitialDateTimeControls()
        {
            this.StartDateDateTimePicker.Format = DateTimePickerFormat.Custom;
            this.EndDateDateTimePicker.Format = DateTimePickerFormat.Custom;

            this.StartTimeDateTimePicker.ShowUpDown = true;
            this.EndTimeDateTimePicker.ShowUpDown = true;
            this.StartTimeDateTimePicker.Format = DateTimePickerFormat.Custom;
            StartTimeDateTimePicker.CustomFormat = "H:mm";

            this.EndTimeDateTimePicker.Format = DateTimePickerFormat.Custom;
            EndTimeDateTimePicker.CustomFormat = "H:mm";

            this.StartDateDateTimePicker.Value = DateTime.Now;
            this.EndDateDateTimePicker.Value = DateTime.Now;
            this.StartTimeDateTimePicker.Value = DateTime.Now;
            this.EndTimeDateTimePicker.Value = DateTime.Now;

            InitialRadioControls();
        }

        private void InitialTaskListViewControl()
        {
            this.TaskListView.Items.Clear();
            this.TaskListView.View = View.Details;
            this.TaskListView.Columns.Add("Name",50);
            this.TaskListView.Columns.Add("Schedule",100);
            this.TaskListView.Columns.Add("Status",50);

            this.TaskListView.SelectedIndexChanged += new EventHandler(TaskListView_SelectedIndexChanged);

            InitialTaskListViewButtons();
        }

        private void InitialTaskListViewButtons()
        {
            this.TaskStartButton.Enabled = false;
            this.TaskDeleteButton.Enabled = false;
            this.TaskStopButton.Enabled = false;
            TaskListView.MultiSelect = false;
        }        

        private void InitialScheduleControls()
        {
            this.HoursTextBox.Text = "0";
            this.MinutesTextBox.Text = "5";
            this.BetweenTimeRadioButton.Checked = true;
            InitialDateTimeControls();
            
        }

        private void InitialRadioControls()
        {
            this.BetweenTimeRadioButton.Checked = true;
            this.AllDayRadioButton.Checked = false;
            this.AllDayRadioButton.CheckedChanged += new EventHandler(AllDayRadioButton_CheckedChanged);
        }

        void AllDayRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AllDayRadioButton.Checked == true)
            {
                this.BetweenTimeRadioButton.Checked = false;
                this.StartTimeDateTimePicker.Enabled = false;
                this.EndTimeDateTimePicker.Enabled = false;
            }
            else
            {
                this.BetweenTimeRadioButton.Checked = true;
                this.StartTimeDateTimePicker.Enabled = true;
                this.EndTimeDateTimePicker.Enabled = true;
            }
        }

        private void AddListViewItem(TaskItem task)
        {            
            TaskListView.BeginUpdate();

            ListViewItem item = new ListViewItem(task.TaskName);
            item.SubItems.Add(task.MessageSchedule.StartTime.ToString());
            item.SubItems.Add(task.Status.ToString());
            item.Tag = task;
            TaskListView.Items.Add(item);

            TaskListView.EndUpdate();
        }

        private void AddListViewItem(string name, string schedule, string status)
        {
            TaskListView.BeginUpdate();

            ListViewItem item = new ListViewItem(name);            
            item.SubItems.Add(schedule);
            item.SubItems.Add(status);
            TaskListView.Items.Add(item);

            TaskListView.EndUpdate();
        }

        private void DeleteListViewItem(ListViewItem item)
        {
            TaskListView.BeginUpdate();
            
            TaskListView.Items.Remove(item);            

            TaskListView.EndUpdate();

            UpdateTaskButtonsStatus();
        }

        private void UpdateTaskButtonsStatus()
        {
            if (TaskListView.Items.Count == 0 || TaskListView.FocusedItem == null)
            {
                this.TaskStartButton.Enabled = false;
                this.TaskStopButton.Enabled = false;
                this.TaskDeleteButton.Enabled = false;
            }
        }

        private void UpdateTaskButtonsStatus(TaskItem item)
        {
            switch (item.Status)
            {
                case TaskStatus.Running:
                    this.TaskStartButton.Enabled = false;
                    this.TaskStopButton.Enabled = true;
                    break;
                case TaskStatus.Stopped:
                    this.TaskStartButton.Enabled = true;
                    this.TaskStopButton.Enabled = false;
                    break;
                case TaskStatus.Expired:
                    this.TaskStartButton.Enabled = false;
                    this.TaskStopButton.Enabled = false;
                    break;
                default:
                    this.TaskStartButton.Enabled = true;
                    this.TaskStopButton.Enabled = false;
                    break;
            }
        }

        private void TaskListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskItem item = GetSeletedTaskItem();
            if (item == null)
            {
                return;
            }
            UpdateTaskButtonsStatus(item);
            this.TaskDeleteButton.Enabled = true;
        }        

        private void SenderMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mNotesGenerator.CloseSession();
        }

        private void SelectSenderButton_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                MessageBox.Show("The server is disconnected or no user.");
                return;
            }

            List<NotesItem> noteslist = mNotesGenerator.User_Load();
            SenderUserLoad selectUserDialog = new SenderUserLoad(noteslist);
            if (selectUserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                
                if (selectUserDialog.SelectNotesItem != null)
                {
                    SenderTextBox.Tag = selectUserDialog.SelectNotesItem;
                    object[] fullname = selectUserDialog.SelectNotesItem.Values as object[];
                    SenderTextBox.Text = fullname[1].ToString();
                }
            }
        }

        private void SelectReceiverButton_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                MessageBox.Show("The server is disconnected or no user.");
                return;
            }

            List<NotesItem> noteslist = mNotesGenerator.User_Load();
            ReceiverUserLoad selectUserDialog = new ReceiverUserLoad(noteslist);
            if (selectUserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReceiverTextBox.Tag = selectUserDialog.SelectNotesList;
                foreach (NotesItem item in selectUserDialog.SelectNotesList)
                {
                    object[] fullname = item.Values as object[];
                    if (this.ReceiverTextBox.Text == String.Empty)
                    {
                        ReceiverTextBox.Text = fullname[1].ToString();
                    }
                    else 
                    {
                        ReceiverTextBox.Text += "; " + fullname[1].ToString();
                    }
                    if (null == receiversList)
                    {
                        receiversList = new List<string>();
                    }
                    receiversList.Add(fullname[1].ToString());
                }
            }
        }

        private void SenderButton_Click(object sender, EventArgs e)
        {
            string errormessage = CheckStatus();
            if (errormessage != String.Empty)
            {
                MessageBox.Show(errormessage);
                return;
            }

            mNotesGenerator.SendNotes(GetMessage(), GetNotesCount());
            MessageBox.Show("Successfully!");
        }

        private NotesMessage GetMessage()
        {
            NotesMessage notesMessage = new NotesMessage();
            notesMessage.Subject = this.MessageSubjectTextBox.Text;
            List<NotesItem> list = this.ReceiverTextBox.Tag as List<NotesItem>;
            if (list.Count == 0)
            {
                return null;
            }
            object[] fullname = list[0].Values as object[];
            notesMessage.To = receiversList;
            notesMessage.Body = MessageBodyRichTextBox.Text;
            notesMessage.From = SenderTextBox.Text;
            notesMessage.AttachmentPath = this.MessageAttachmentsTextBox.Text;

            return notesMessage;
        }

        private int GetNotesCount()
        {
            int notesCount = 0;
            if (String.Empty != MailNumberTextBox.Text)
            {
                notesCount = int.Parse(MailNumberTextBox.Text);
            }
            else
            {
                notesCount = 1;
            }
            return notesCount;
        }

        private void AttachmentBrowserButton_Click(object sender, EventArgs e)
        {
            if (openFolderDialog1.ShowDialog() == DialogResult.OK)
            {
                this.MessageAttachmentsTextBox.Text = openFolderDialog1.SelectedPath;
            }
        }

        private string CheckStatus()
        {
            string errorMessage = String.Empty;
            if (this.ReceiverTextBox.Text == String.Empty)
            {
                errorMessage = "No receiver";
            }
            if(this.MessageSubjectTextBox.Text == String.Empty)
            {
                if (errorMessage == String.Empty)
                {
                    errorMessage = "No subject";
                }
                else
                {
                    errorMessage += ", no subject";
                }
            }
            if (this.MessageBodyRichTextBox.Text == String.Empty)
            {
                if (errorMessage == String.Empty)
                {
                    errorMessage = "No body";
                }
                else
                {
                    errorMessage += ", no body";
                }
            }
            return errorMessage;
        }

        private int GetIntervalTime()
        {
            int h = 0;
            int m = 0;
            try
            {
                h = int.Parse(this.HoursTextBox.Text);
                m = int.Parse(this.MinutesTextBox.Text);
            }
            catch
            {                
                return 0;
            }            

            return h * 60 + m;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to ignore all settings?", "Restore", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                InitialScheduleControls();
            }
        }

        private void TaskAddButton_Click(object sender, EventArgs e)
        {
            string errormessage = CheckStatus();
            if (errormessage != String.Empty)
            {
                MessageBox.Show(errormessage);
                return;
            }

            int intervalTime = GetIntervalTime();
            if (intervalTime <= 0)
            {
                MessageBox.Show("Please input a positive integer number in the interval textbox.");
                return;
            }

            string taskname = String.Empty;
            TaskNameForm taskNameForm = new TaskNameForm();
            if (taskNameForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                taskname = taskNameForm.TaskName;
            }
            else
            {
                return;
            }

            if (TaskManager.Instance.TaskDictionary.ContainsKey(taskname))
            {
                MessageBox.Show("The name has been existed, please change the task name.");
                return;
            }

            Schedule schedule = new Schedule();
            schedule.IntervalTime = intervalTime;

            if (this.AllDayRadioButton.Checked == false)
            {             
                schedule.StartTime = this.StartDateDateTimePicker.Value.Date.Add(this.StartTimeDateTimePicker.Value.TimeOfDay);
                schedule.EndTime = this.EndDateDateTimePicker.Value.Date.Add(this.EndTimeDateTimePicker.Value.TimeOfDay);
            }
            else
            {                
                schedule.StartTime = this.StartDateDateTimePicker.Value.Date;
                schedule.EndTime = this.EndDateDateTimePicker.Value.Date.AddDays(1.0);
            }

            TaskItem task = new TaskItem(taskname, GetMessage(), GetNotesCount(), schedule, mNotesGenerator);
            
            TaskManager.Instance.AddTaskItem(task);
            AddListViewItem(task);
        }        
        
        private void TaskStartButton_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = this.GetSeletedListViewItem();
            TaskItem item = GetSeletedTaskItem();
            if (item == null)
            {
                MessageBox.Show("The task item is null.");
                return;
            }

            item.Start();

            UpdateTaskButtonsStatus(item);

            TaskListView.BeginUpdate();
            this.TaskListView.FocusedItem = selectedItem;
            selectedItem.SubItems[2].Text = item.Status.ToString();
            TaskListView.EndUpdate();
        }

        private void TaskStopButton_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = this.GetSeletedListViewItem();
            TaskItem item = GetSeletedTaskItem();
            if (item == null)
            {
                MessageBox.Show("The task item is null.");
                return;
            }

            item.Stop();
            this.TaskStartButton.Enabled = true;
            this.TaskStopButton.Enabled = false;

            TaskListView.BeginUpdate();
            this.TaskListView.FocusedItem = selectedItem;
            selectedItem.SubItems[2].Text = item.Status.ToString();
            TaskListView.EndUpdate();
        }

        private void TaskDeleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete the task?","Delete Task",MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                //ListViewItem listViewItem = GetSeletedListViewItem();
                TaskItem item = GetSeletedTaskItem();
                if (item == null)
                {
                    MessageBox.Show("The task item is null.");
                    return;
                }

                item.Stop();
                TaskManager.Instance.DeleteTaskItem(item);
                this.DeleteListViewItem(GetSeletedListViewItem());
            }
        }

        private TaskItem GetSeletedTaskItem()
        {
            ListViewItem selectedItem = this.GetSeletedListViewItem();
            if (selectedItem == null)
            {
                //MessageBox.Show("No selected task");
                return null;
            }

            TaskItem item = selectedItem.Tag as TaskItem;
            return item;
        }

        private ListViewItem GetSeletedListViewItem()
        {
            //if (this.TaskListView.SelectedItems == null || 
            //    this.TaskListView.SelectedItems.Count == 0)
            //{
            //    return null;
            //}
            
            return this.TaskListView.FocusedItem;            

            //return this.TaskListView.SelectedItems[0];
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure to close the form?", "Close", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}
