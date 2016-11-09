using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class FlaggedMessage
    {
        public ulong MessageId { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string Subject { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public FlaggedMsgReviewStatus Status { get; set; }

        public string To { get; set; }

        public DateTime ReviewDueDate { get; set; }

        public bool HasAttachments { get; set; }

        public bool HasComments { get; set; }

        public string MemberName { get; set; }

        public List<FlaggedMessageInfo> FlaggedMessageInfoList { get; set; }
    }

    public class FlaggedMessageInfo
    {
        public long SampleRecID { get; set; }
        public long BPID { get; set; }
        public long CurrentSupCodeID { get; set; }
        public long SamplePeriodID { get; set; }
        public bool IsFinalCode { get; set; }
    }

    public enum FlaggedMsgReviewStatus
    {
        Unreviewed = 0,
        Partial,
        Reviewed,
        Commented,
        Escalated
    };

    public enum CodeID
    {
        OK = 1,
        Escalated,
        SystemMarked,
        Reviewed,
        Other,
        None,
        Completely,
        Imcompletely,
        Partially
    };
}
