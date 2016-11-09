using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Domino;

namespace SendDominoNotes
{
    public partial class LogOnForm : Form
    {
        NotesGenerator mNotesGenerator = null;
        private string mServer = String.Empty;//"Mail01/BRS";
        private string mPassword = String.Empty;//"emcsiax@QA";       

        public LogOnForm()
        {
            InitializeComponent();
            InitializeServerNames();
            this.PwdTextBox.Text = ReadPassword();
            this.ServerNameComboBox.Text = mServer;
            this.Dock = DockStyle.Fill;
        }

        public NotesGenerator NotesGenerator
        {
            get { return mNotesGenerator; }
        }

        private void InitializeServerNames()
        {
            string path64 = @"C:\Program Files (x86)\IBM\Lotus\Notes\notes.ini";
            string path86 = @"C:\Program Files\IBM\Lotus\Notes\notes.ini";
            string inipath = String.Empty;
            if (File.Exists(path86))
            {
                inipath = path86;
            }
            else if (File.Exists(path64))
            {
                inipath = path64;
            }
            else
            {
                inipath = String.Empty;
            }

            if (inipath != String.Empty)
            {
                string[] str = UtilManager.ReadIni("Notes", "MailServer", inipath).Split('=');
                mServer = str[1].Remove(str[1].Length - 1) + str[2];
            }
        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            mServer = this.ServerNameComboBox.Text;
            mPassword = this.PwdTextBox.Text;

            try
            {
                mNotesGenerator = NotesGenerator.Instance(mServer, mPassword);
                if (mNotesGenerator != null)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    if(this.SavePwdCheckBox.Checked)
                    {
                        SavePassword();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The server name or the password is wrong.");
                    mNotesGenerator = null;
                }
            }
            catch
            {
                MessageBox.Show("The server name or the password is wrong.");
                mNotesGenerator = null;
            }            
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SavePassword()
        {
            string pwdstr = this.PwdTextBox.Text;
            string filepath = Application.StartupPath + "\\" + "Configure.ini";
            if (!File.Exists(filepath))
            {
                File.Create(filepath);                
            }

            UtilManager.WriteIni("LogInfo", "Password", pwdstr, filepath);
        }

        private string ReadPassword()
        {
            string filepath = Application.StartupPath + "\\" + "Configure.ini";
            if (File.Exists(filepath))
            {
                return UtilManager.ReadIni("LogInfo", "Password", filepath);
            }
            else
            {
                return String.Empty;
            }
        }

        private void CreateConfigureIniFile(string filepath)
        {            
            if (!File.Exists(filepath))
            {
                File.Create(filepath);
            }
        }
    }
}
