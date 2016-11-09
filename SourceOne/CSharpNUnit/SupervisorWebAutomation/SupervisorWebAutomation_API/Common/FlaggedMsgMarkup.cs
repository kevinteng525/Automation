using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class FlaggedMsgMarkup
    {
        public List<long> SampleRecIDs { get; set; }

        //public long SampleRecIDs { get; set; }

        public ulong MessageID { get; set; }

        public string BusinessPolicyID { get; set; }

        public string CodeID { get; set; }

        public string Comment { get; set; }

        public List<long> SamplePeriodIDs { get; set; }
    
    }
}
