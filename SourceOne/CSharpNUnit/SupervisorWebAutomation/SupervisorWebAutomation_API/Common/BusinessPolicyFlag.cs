using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class BusinessPolicyFlag
    {
        public string AppliedCodeID { get; set; }

        public string AppliedCodeName { get; set; }

        public string Id { get; set; }

        public string HighlightName { get; set; }

        public MessageMatchLocation MatchLocation { get; set; }

        public string Name { get; set; }

        public SamplingType SamplingType { get; set; }

        public List<SupervisoryCode> AvailableBPFlags { get; set; }
    }

    public enum MessageMatchLocation
    {
        Subject = 1,
        Body = 2,
        Attachment = 4
    }

    public enum SamplingType
    {
        Random = 1,
        Lexicon = 2,
        LexiconWeighted = 3,
        LexiconLimited = 4
    }
}
