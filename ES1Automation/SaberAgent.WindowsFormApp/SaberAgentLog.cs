using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace SaberAgent.WindowsFormApp
{  

    public partial class ProgressDetail : Form
    {        

        public ProgressDetail()
        {
            InitializeComponent();
            Task backgroundTask = Task.Factory.StartNew(() => {
                SaberAgent agent = new SaberAgent();                
                agent.Start();
            });
            

        }

        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            base.OnLoad(e);
        }


        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ProgressMenu_Click(object sender, EventArgs e)
        {
            BindingSource source = new BindingSource();
            string path = @"C:\SaberAgent\Logs\SaberAgentMgrWinService.log";
            if (System.IO.File.Exists(path))
            {
                string content = string.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite)))
                {                    
                    string line = string.Empty;
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (null != line)
                        {
                            string[] segments = line.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                            if (segments.Length == 6)
                            {
                                source.Add(new LogItem() { Time = segments[0], Module = segments[1], Level = segments[2], Type = segments[3], Line = segments[4], Message = segments[5] });
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }   
            
            this.ProgressLogGridView.DataSource = source;

            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Activate();
        }

        private void HelpMenu_Click(object sender, EventArgs e)
        {

        }

        private void Form_Closing(object sender, FormClosingEventArgs  e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
       
    }

    public class LogItem
    {
        public string Time { get; set; }
        public string Module { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public string Line { get; set; }
        public string Message { get; set; }
    }
}
