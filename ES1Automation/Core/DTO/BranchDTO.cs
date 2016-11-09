using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class BranchDTO
    {
        [DataMember()]
        public Int32 BranchId { get; set; }

        [DataMember()]
        public Int32 ProductId { get; set; }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public string Path { get; set; }

        [DataMember()]
        public string Description { get; set; }

        public BranchDTO()
        { }
    }
}
