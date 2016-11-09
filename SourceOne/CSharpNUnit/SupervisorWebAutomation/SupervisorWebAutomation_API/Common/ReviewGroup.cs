using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class ReviewGroup
    {
        //[DeserializeAs(Name="id")]
        public int Id { get; set; }

        //[DeserializeAs(Name="parentId")]
        public int ParentId { get; set; }

        //[DeserializeAs(Name="name")]
        public string Name { get; set; }

        //[DeserializeAs(Name="isActive")]
        public bool IsActive { get; set; }

        //[DeserializeAs(Name="hasBP")]
        public bool HasBP { get; set; }

        //this is for the post request to generate report
        public bool Checked { get; set; }
    }
}
