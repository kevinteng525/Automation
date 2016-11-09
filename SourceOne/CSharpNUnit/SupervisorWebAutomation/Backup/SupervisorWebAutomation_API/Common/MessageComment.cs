using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class MessageComment
    {
        //[DeserializeAs(Name="messageId")]
        public int MessageId { get; set; }

        //[DeserializeAs(Name="comment")]
        public string Comment { get; set; }
    }

    public class AddMessageComments
    {
        public List<MessageComment> MessageComments { get; set; }
    }
}
