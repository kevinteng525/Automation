using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class MessagePreview
    {
        public int MessageId { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string Subject { get; set; }

        private List<Recipient> toList;
        //[JsonIgnore]
        public List<Recipient> ToList
        {
            get
            {
                return toList;
            }
            set
            {
                toList = value;
                To = GetRecipientListDisplayString(value);
            }
        }

        private List<Recipient> ccList;
        //[JsonIgnore]
        public List<Recipient> CCList
        {
            get
            {
                return ccList;
            }
            set
            {
                ccList = value;
                CC = GetRecipientListDisplayString(value);
            }
        }

        private List<Recipient> bccList;
        //[JsonIgnore]
        public List<Recipient> BCCList
        {
            get
            {
                return bccList;
            }
            set
            {
                bccList = value;
                BCC = GetRecipientListDisplayString(value);
            }
        }

        private List<Recipient> fromList;
        //[JsonIgnore]
        public List<Recipient> FromList
        {
            get
            {
                return fromList;
            }
            set
            {
                fromList = value;
                From = GetRecipientListDisplayString(value);
            }
        }

        public List<BusinessPolicyFlag> BusinessPolicyFlags { get; set; }

        public List<MessageAttachment> Attachments { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string From { get; set; }

        public string Body { get; set; }

        private string GetRecipientListDisplayString(List<Recipient> list)
        {
            var str = String.Empty;
            foreach (var recipient in list)
            {
                str += recipient.Name ?? String.Empty + "; ";
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 2);
            }

            return str;
        }
    }
}
