using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class MessageAttachment
    {
        public string AttachmentId { get; set; }

        public string FileId { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string MatchLocation { get; set; }

        public string Name { get; set; }
    }

    public enum AttachmentMatchLocation
    {
        Name = 1,
        Body = 2,
        UdxAttach = 4
    }
}
