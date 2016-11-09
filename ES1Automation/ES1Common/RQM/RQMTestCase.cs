using System.Collections.Generic;
using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMTestCase
    {
        public string Title { get; private set; }

        public string WebId { get; private set; }

        public int Weight { get; private set; }

        public IDictionary<string, string> Categories { get; private set; }

        public IDictionary<string, string> CustomAttributes { get; private set; }

        public RQMTestCase(XElement xmlString)
        {
            Title = xmlString.Element(RQMServer.XN3 + "title").Value;

            WebId = xmlString.Element(RQMServer.XN2 + "webId").Value;

            Weight = int.Parse(xmlString.Element(RQMServer.XN2 + "weight").Value);

            Categories = new Dictionary<string, string>();

            var categories = xmlString.Elements(RQMServer.XN2 + "category");

            foreach (var cat in categories)
            {
                Categories.Add(cat.Attribute("term").Value, cat.Attribute("value").Value);
            }

            CustomAttributes = new Dictionary<string, string>();

            if (xmlString.Element(RQMServer.XN2 + "customAttributes") != null)
            {
                var attributes = xmlString.Element(RQMServer.XN2 + "customAttributes").Elements(RQMServer.XN2 + "customAttribute");

                foreach (var attr in attributes)
                {
                    CustomAttributes.Add(attr.Element(RQMServer.XN2 + "name").Value, attr.Element(RQMServer.XN2 + "value").Value);
                }
            }
        }
    }
}
