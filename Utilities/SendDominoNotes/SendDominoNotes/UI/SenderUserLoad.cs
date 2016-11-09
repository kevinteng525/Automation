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
    public partial class SenderUserLoad : Form
    {
        private List<NotesItem> mNotesList = null;
        private NotesItem mSelectedNotesItem = null;
        
        public SenderUserLoad(List<NotesItem> noteslist)
        {
            InitializeComponent();
            mNotesList = noteslist;
            InitializeAddressViewList();           
        }

        public NotesItem SelectNotesItem
        {
            get
            {
                return mSelectedNotesItem;
            }             
        }

        private void InitializeAddressViewList()
        { 
            if(null == mNotesList)
                return;
            foreach (NotesItem item in mNotesList)
            {                   
                ListViewItem listviewItem = new ListViewItem();
                listviewItem.Tag = item;

                object[] fullname = item.Values as object[];
                listviewItem.Text = fullname[1].ToString();
                this.SenderAddressListView.Items.Add(listviewItem);
            }

            //this.SenderAddressListView.SelectedIndexChanged += new EventHandler(SenderAddressListView_SelectedIndexChanged);
            this.SenderAddressListView.DoubleClick += new EventHandler(SenderAddressListView_DoubleClick);
        }

        void SenderAddressListView_DoubleClick(object sender, EventArgs e)
        {
            this.SenderAddressTextBox.Text = SenderAddressListView.SelectedItems[0].Text;
            this.SenderAddressTextBox.Tag = SenderAddressListView.SelectedItems[0].Tag;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NotesItem item = this.SenderAddressTextBox.Tag as NotesItem;
            if (null != item)
            {
                this.mSelectedNotesItem = item;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("No sender");
            }            
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
