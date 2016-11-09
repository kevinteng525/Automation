using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Saber.Common;


namespace Saber.S1CommonAPILib
{
    public class S1BCEConfig : IS1Object
    {
        public String Name { get; set; }
        public List<S1AddressFilteringRule> Rules { get; set; }
        public S1BCEConfig()
        {
            this.Name = "Created By Saber at " + DateTime.Now.ToString();
            this.Rules = new List<S1AddressFilteringRule>();
        }


        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Can not find the file specified!");
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String name = XMLHelper.GetElementValue(element, "name");
            XElement addressfilteringrules = XMLHelper.GetElement(element, "addressfilteringrules");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            List<S1AddressFilteringRule> rules = new List<S1AddressFilteringRule>();
            if (null != addressfilteringrules)
            {
                foreach (XElement rule in addressfilteringrules.Elements())
                {
                    S1AddressFilteringRule r = new S1AddressFilteringRule();
                    r.DeserializeFromXElement(rule);
                    rules.Add(r);
                }
            }
            else
            {
                throw new Exception("At lease one rule is required.");
            }
            if (rules.Count > 0)
            {
                this.Rules = rules;
            }
            else
            {
                throw new Exception("At lease one rule is required.");
            }
            return true;
        }
    }
}
