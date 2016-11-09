using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class Member
    {
        //[DeserializeAs(Name="id")]
        public int Id { get; set; }

        //[DeserializeAs(Name="name")]
        public string Name { get; set; }

        //[DeserializeAs(Name="attr")]
        public string Attr { get; set; }

        public bool IsDeleted { get; set; }

        public bool Selected { get; set; }
    }

    public class ReviewGroupMembers
    {
        public List<Member> Members { get; set; }

        public List<Member> DeletedMembers { get; set; }
    }
}
