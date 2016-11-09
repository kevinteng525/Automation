using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{

    public class MessageAction
    {
        //[DeserializeAs(Name="bpName")]
        public string BpName { get; set; }

        //[DeserializeAs(Name="reviewerName")]
        public string ReviewerName { get; set; }

        //[DeserializeAs(Name="description")]
        public string Description { get; set; }

        //[DeserializeAs(Name="date")]
        public DateTime Date { get; set; }
    }

    public class MessageHistory
    {
        public List<MessageAction> Actions { get; set; }
    }
}
