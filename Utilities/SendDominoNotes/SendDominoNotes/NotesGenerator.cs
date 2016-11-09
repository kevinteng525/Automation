using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domino;
using System.Windows.Forms;
using System.IO;

namespace SendDominoNotes
{
    public class NotesGenerator
    {
        private static NotesSession ns;
        private static NotesDatabase ndb;
        private static NotesGenerator senderInstance;
        private string mServer = String.Empty;
        private string mPassword = String.Empty;

        private NotesGenerator(string serverName, string password)
        {
            //Initialize NotesSession
            OpenSession(serverName,password);
        }

        public static NotesGenerator Instance(string serverName, string password)
        {
            if (null == senderInstance)
            {
                senderInstance = new NotesGenerator(serverName, password);
                if (ndb == null)
                {
                    senderInstance = null;
                }
            }
            return senderInstance;                                                
        }

        public List<NotesItem> User_Load()
        {
            try
            {                                
                //Initialize NotesDatabase                          
                NotesView vw = ndb.GetView("People");

                NotesDocument doc = vw.GetFirstDocument();
                List<NotesItem> notesList = new List<NotesItem>();
                while (doc != null)
                {
                    notesList.Add(doc.GetFirstItem("FullName"));                  
                    doc = vw.GetNextDocument(doc);
                }
                               
                return notesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
                return null;
            }
        }

        public void SendNotes(NotesMessage message)
        {
            try
            {
                if (ns != null)
                {
                    NotesDocument doc = ndb.CreateDocument();
                    doc.ReplaceItemValue("Form", "Memo");

                    //sent notes memo fields (To: CC: Bcc: Subject etc) 
                    doc.ReplaceItemValue("From", message.From);
                    doc.ReplaceItemValue("SendTo", message.To.ToArray());
                    doc.ReplaceItemValue("Subject", message.Subject); 

                    NotesRichTextItem rt = doc.CreateRichTextItem("Body");
                    rt.AppendText(message.Body);

                    if (!string.IsNullOrEmpty(message.AttachmentPath))
                    {
                        NotesRichTextItem attachment = doc.CreateRichTextItem("attachment");
                        List<string> filelist = GetFileList(message.AttachmentPath);
                        foreach (string filepath in filelist)
                        {
                            attachment.EmbedObject(EMBED_TYPE.EMBED_ATTACHMENT, "", filepath, "attachment");
                        }
                    }

                    object obj = doc.GetItemValue("SendTo");
                    doc.Send(false, ref obj);

                    doc = null;                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }

        }

        public void SendNotes(NotesMessage message, int count)
        {
            if (0 == count)
            {
                MessageBox.Show("The message count shouldn't be 0.");
                return;
            }

            while (0 != count)
            {
                SendNotes(message);
                count--;
            }
        }

        private List<string> GetFileList(string FolderPath)
        {
            List<string> filelist = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(FolderPath);
            List<FileInfo> files = dir.GetFiles().ToList();
            foreach(FileInfo file in files)
            {
                filelist.Add(file.FullName);
            }
            return filelist;
        }

        private bool OpenSession(string serverName, string password)
        {
            mServer = serverName;
            mPassword = password;
            ns = new NotesSession();            
            ns.Initialize(mPassword);
            string servername = ns.GetEnvironmentString("MailServer");
            ndb = ns.GetDatabase(mServer, "names.nsf", false);
            if (ndb == null)
            {
                return false;
            }
            return true;
        }

        public void CloseSession()
        {
            ndb = null;
            ns = null;
        }
    }
}
