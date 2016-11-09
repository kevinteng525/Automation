using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class SupervisoryCode
    {

        public string Id { get; set; }

        public string Description { get; set; }

        public bool IsFinalStatus { get; set; }

        public string Name { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public QuarantineActionType QuarantineAction { get; set; }

        public bool CanBeRenamed { get; set; }

        public bool Selected { get; set; }
    }

    public enum QuarantineActionType
    {
        NoAction = 0,
        Release = 1,
        Delete = 2
    }

    public class ReportSupervisoryCodes
    {
        public List<SupervisoryCode> Codes { get; set; }

        public List<SupervisoryCode> DeletedCodes { get; set; }

    }
}