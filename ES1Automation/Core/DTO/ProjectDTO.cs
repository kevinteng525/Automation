using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class ProjectDTO
    {
        [DataMember()]
        public Int32 ProjectId { get; set; }

        [DataMember()]
        public String Name { get; set; }

        [DataMember()]
        public String Description { get; set; }

        [DataMember()]
        public Int32 VCSType { get; set; }

        [DataMember()]
        public string VCSServer { get; set; }

        [DataMember()]
        public string VCSUser { get; set; }

        [DataMember()]
        public string VCSPassword { get; set; }

        [DataMember()]
        public string VCSRootPath { get; set; }

        [DataMember()]
        public string RunTime { get; set; }


        public ProjectDTO()
        {
        }

    }
}
