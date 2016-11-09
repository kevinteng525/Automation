using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saber.S1CommonAPILib
{
    public class S1TaskType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int TaskTypeId { get; set; }
        public int State { get; set; }
        public int RuntimeEnvironmentMask { get; set; }
    }
}
