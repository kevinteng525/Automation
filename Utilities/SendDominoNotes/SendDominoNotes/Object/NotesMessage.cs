using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domino;

namespace SendDominoNotes
{
    public class NotesMessage
    {        
        private string _from = string.Empty;
        private List<string> _to = null;//string.Empty;
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private List<NotesEmbeddedObject> _files = new List<NotesEmbeddedObject>(); //attachments
        private string _attachmentFolderPath = string.Empty;

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }
        
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public List<string> To
        {
            get { return _to; }
            set { _to = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public string AttachmentPath
        {
            get { return _attachmentFolderPath; }
            set { _attachmentFolderPath = value; }
        }
        
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private string _time = string.Empty;

        public List<NotesEmbeddedObject> Files
        {
            get { return _files; }
            set { _files = value; }
        }

    }
}
