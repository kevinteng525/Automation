using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saber.TestData
{
    public class MessagesQuery
    {
        public class MessageTitle
        {
            public String Contains { get; set; }
            public int Size { get; set; }
            public String Language { get; set; }
            public String Equal { get; set; }
        }
        public class MessageBody
        {
            public String contains { get; set; }
            public int size { get; set; }
            public String language { get; set; }
            public String equal { get; set; }
        }
        public class MessageAttachment
        {
            public int size { get; set; }
            public int count { get; set; }
            public String filePath { get; set; }
        }
        public int countNeeded { get; set; }
        public MessageTitle Title
        {
            get;
            set;
        }
        public MessageBody body
        {
            get;
            set;
        }
        public MessageAttachment attachment
        {
            get;
            set;
        }
        
    }
}
