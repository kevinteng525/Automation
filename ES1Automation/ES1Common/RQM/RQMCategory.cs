using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMCategory
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public string CategoryType { get; private set; }

        public RQMCategory(XElement xmlString)
        {
            Id = xmlString.Element(RQMServer.XN3 + "identifier").Value;

            Title = xmlString.Element(RQMServer.XN3 + "title").Value;

            CategoryType = xmlString.Element(RQMServer.XN2 + "categoryType").Attribute("href").Value;
        }
    }
}
