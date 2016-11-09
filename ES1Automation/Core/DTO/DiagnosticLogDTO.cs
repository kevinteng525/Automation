using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Core.DTO
{
    [DataContract()]
    public partial class DiagnosticLogDTO
    {
        [DataMember()]
        public Int32 LogId { get; set; }
        
        [DataMember()]
        public DateTime CreateTime { get; set; }
        
        [DataMember()]
        public string Component { get; set; }
        
        [DataMember()]
        public Int32 LogType { get; set; }
        
        [DataMember()]
        public string Message { get; set; }
        
        public DiagnosticLogDTO()
        { }
    }
}
