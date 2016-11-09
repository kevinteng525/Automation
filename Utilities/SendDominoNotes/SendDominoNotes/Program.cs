using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SendDominoNotes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LogOnForm form = new LogOnForm();
            if (form.ShowDialog() == DialogResult.OK)
            {                
                Application.Run(new SenderMainForm(form.NotesGenerator));
            }
        }
    }
}
