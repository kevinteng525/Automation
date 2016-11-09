using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ES1Common.RQM
{
    public class RQMFeature
    {
        public string Id { get;  set; }

        public string Title { get;  set; }

        public IList<RQMFeature> SubFeatures { get;  set; }

    }
}
