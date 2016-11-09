using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMTestSuite
    {
        public string Title { get;  set; }

        public string SourceId { get;  set; }

        public bool SequentialExecution { get;  set; }

        public bool HaltOnFailure { get;  set; }

        public int Weight { get;  set; }

        public IDictionary<string, string> Categories { get;  set; }

        public IList<string> SubTestCaseSourceIds { get;  set; }

        public IList<string> SubTestSuiteSourceIds { get; set; }

        public RQMTestSuite()
        { }

        public RQMTestSuite(XElement xmlElement)
        {
            Title = xmlElement.Element(RQMServer.XN3 + "title").Value;

            SourceId = xmlElement.Element(RQMServer.XN2 + "webId").Value;

            Weight = int.Parse(xmlElement.Element(RQMServer.XN2 + "weight").Value);

            SequentialExecution = bool.Parse(xmlElement.Element(RQMServer.XN2 + "sequentialExecution").Value);

            SequentialExecution = bool.Parse(xmlElement.Element(RQMServer.XN2 + "haltOnFailure").Value);

            var categories = xmlElement.Elements(RQMServer.XN2 + "category");

            Categories = new Dictionary<string, string>();

            foreach (var cat in categories)
            {
                Categories.Add(cat.Attribute("term").Value, cat.Attribute("value").Value);
            }

            SubTestCaseSourceIds = new List<string>();

            var testElements = xmlElement.Element(RQMServer.XN2 + "suiteelements").Elements(RQMServer.XN2 + "suiteelement");

            foreach (var te in testElements)
            {
                var tc = te.Element(RQMServer.XN2 + "testcase");
                string link = tc.Attribute("href").Value;
                // the typical link of the test case is
                // https://jazzapps.otg.com:9443/qm/service/com.ibm.rqm.integration.service.IIntegrationService/resources/SourceOne+%28Quality+Management%29/testcase/urn:com.ibm.rqm:testcase:438
                string splitString = ":testcase:";
                if (link.LastIndexOf(splitString) > 0)
                {
                    string webId = link.Substring(link.LastIndexOf(splitString) + splitString.Length);
                    SubTestCaseSourceIds.Add(webId);
                }
                else
                {
                    //Todo:
                    // Something stange happened.
                    // We've met the issue that the link of the test case is as below, because the ranking of the test case is not within the ranking list
                    // https://jazzapps.otg.com:9443/qm/service/com.ibm.rqm.integration.service.IIntegrationService/resources/SourceOne+%28Quality+Management%29/testcase/Express_Sheet1_3.xml"
                }
            }
            //Currently RQM doesn't support one test suite to contain another, here we do nothing for this.
            SubTestSuiteSourceIds = new List<string>();
        }
    }
}
