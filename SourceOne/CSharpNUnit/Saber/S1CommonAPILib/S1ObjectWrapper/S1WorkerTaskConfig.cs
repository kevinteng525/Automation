using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Saber.Common;

namespace Saber.S1CommonAPILib
{
    public class S1WorkerTaskConfig
    {
        public int WorkerId { get; set; }
        public int TaskTypeId { get; set; }
        public int Quota { get; set; }
        public int Status { get; set; }        
    }
}
