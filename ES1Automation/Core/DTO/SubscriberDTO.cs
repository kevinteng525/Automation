using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class SubscriberDTO
    {
        [DataMember()]
        public Int32 SubscriberId { get; set; }

        [DataMember()]
        public Int32 ProjectId { get; set; }

        [DataMember()]
        public Int32 UserId { get; set; }

        [DataMember()]
        public DateTime CreateTime { get; set; }

        [DataMember()]
        public string Description { get; set; }

        [DataMember()]
        public int SubscriberType { get; set; }

        public SubscriberDTO()
        { }
    }
}
