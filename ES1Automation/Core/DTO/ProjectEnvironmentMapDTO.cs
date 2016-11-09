using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class ProjectEnvironmentMapDTO
    {
        [DataMember()]
        public Int32 MapId { get; set; }

        [DataMember()]
        public Int32 ProjectId { get; set; }

        [DataMember()]
        public Int32 EnvironmentId { get; set; }

        public ProjectEnvironmentMapDTO()
        {
        }
    }
}
