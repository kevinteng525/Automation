using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMCategoryType
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public string Scope { get; private set; }

        public string DefaultCategory { get; private set; }

        public IDictionary<string, List<string>> ValueSets { get; private set; }

        public RQMCategoryType(XElement xmlString)
        {
            Id = xmlString.Element(RQMServer.XN3 + "identifier").Value;

            Title = xmlString.Element(RQMServer.XN3 + "title").Value;

            Scope = xmlString.Element(RQMServer.XN2 + "scope").Value;

            DefaultCategory = xmlString.Element(RQMServer.XN2 + "defaultCategory") == null ? string.Empty : xmlString.Element(RQMServer.XN2 + "defaultCategory").Value;

            ValueSets =  new Dictionary<string, List<string>>();

            foreach (XElement valueSet in xmlString.Elements(RQMServer.XN2 + "valueset"))
            {                
                string key = valueSet.Element(RQMServer.XN2 + "key").Attribute("href").Value;
                List<string> values = new List<string>();
                foreach (XElement value in valueSet.Elements(RQMServer.XN2 + "value"))
                {
                    values.Add(value.Attribute("href").Value);
                }
                ValueSets.Add(key, values);
            }
        }
    }
}
