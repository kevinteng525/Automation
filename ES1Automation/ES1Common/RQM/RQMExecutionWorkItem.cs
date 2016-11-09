using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMExecutionWorkItem
    {
        public string WebId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Creator { get; private set; }

        public string Owner { get; private set; }

        public string TestCaseUrl { get; private set; }

        public RQMExecutionWorkItem(XElement xmlString)
        {
            WebId = xmlString.Element(RQMServer.XN2 + "webId").Value;

            Title = xmlString.Element(RQMServer.XN3 + "title").Value;

            Description = xmlString.Element(RQMServer.XN3 + "description").Value;

            Creator = xmlString.Element(RQMServer.XN3 + "creator").Value;

            Owner = xmlString.Element(RQMServer.XN5 + "owner").Value;

            TestCaseUrl = xmlString.Element(RQMServer.XN2 + "testcase").Attribute("href").Value;
        }
    }
}
