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
    public partial class ReceiverUserLoad : Form
    {
        private List<NotesItem> mNotesList = null;
        private List<NotesItem> mSelectedNotesList = null;

        public ReceiverUserLoad(List<NotesItem> noteslist)
        {
            InitializeComponent();
            mNotesList = noteslist;
            mSelectedNotesList = new List<NotesItem>();
            InitializeAddressViewList();           
        }

        public List<NotesItem> SelectNotesList
        {
            get
            {
                return mSelectedNotesList;
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
            NotesItem item = SenderAddressListView.SelectedItems[0].Tag as NotesItem;
            mSelectedNotesList.Add(item);
            if (this.SenderAddressTextBox.Text == String.Empty)
            {
                this.SenderAddressTextBox.Text = SenderAddressListView.SelectedItems[0].Text;
            }
            else
            {
                this.SenderAddressTextBox.Text += "; " + SenderAddressListView.SelectedItems[0].Text;
            }
            this.SenderAddressTextBox.Tag = mSelectedNotesList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<NotesItem> list = this.SenderAddressTextBox.Tag as List<NotesItem>;
            if (null != list)
            {
                this.mSelectedNotesList = list;
            }

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
