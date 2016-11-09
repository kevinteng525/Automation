using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SendDominoNotes
{
    public partial class TaskNameForm : Form
    {
        private string mName = String.Empty;
        
        public TaskNameForm()
        {
            InitializeComponent();
        }

        public string TaskName
        {
            get { return mName; }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (this.TaskNameTextBox.Text == String.Empty)
            {
                MessageBox.Show("Please input the message name.");
                return;
            }
            mName = this.TaskNameTextBox.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
