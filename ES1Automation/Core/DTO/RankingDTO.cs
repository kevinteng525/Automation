using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class RankingDTO
    {
        [DataMember()]
        public Int32 RankingId { get; set; }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public string Description { get; set; }

        public RankingDTO()
        { }
    }
}
