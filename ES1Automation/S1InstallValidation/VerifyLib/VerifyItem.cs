using System;
using System.Xml.Linq;

namespace VerifyLib
{
    public abstract class VerifyItem
    {
        private readonly XElement node;

        protected const string Exist = "EXIST";

        protected const string NotExist = "NOT EXIST";

        public const string LogFormat = "{0,-100}{1,-30}{2,-30}{3,-10}{4,-30}";

        public const string CSVFormat = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"";

        // The name of a verify item
        public string Name { get; protected set; }

        // The Display Name of a verify item
        public string DisplayName { get; protected set; }

        // The Actual Value of a criterion item
        public string ActualValue { get; protected set; }

        // The Expect Value of a verify item
        public string ExpectValue { get; protected set; }

        // The Check Value of a verify item
        public string CheckValue { get; protected set; }

        // The verify Result
        public VerifyResult VerifyResult { get; protected set; }

        // The verify Information
        public string Information { get; protected set; }

        // The verify group of this item
        public VerifyGroup VerifyGroup { get; protected set; }

        protected VerifyItem(XElement node, VerifyGroup group)
        {
            this.node = node;

            Name = GetAttributeValue("name");

            CheckValue = node.Value;

            VerifyGroup = group;

            VerifyGroup.VerifyItems.Add(this);
        }

        protected string GetAttributeValue(string attrName)
        {
            XAttribute attr = node.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            throw new Exception(string.Format("Config Error: attribute:{0} is not find !", attrName));
        }

        protected virtual void PrepareTest()
        {
            DisplayName = Name;

            ExpectValue = CheckValue;

            VerifyResult = VerifyResult.Unknow;
        }

        protected abstract void Verify();

        public void DoValidation()
        {
            try
            {
                PrepareTest();

                Verify();
            }
            catch (Exception ex)
            {
                VerifyResult = VerifyResult.Failed;

                Information = ex.Message.Trim(Environment.NewLine.ToCharArray());
            }
        }

        public XElement ToXML()
        {
            var xml = new XElement("result");

            xml.Add(new XAttribute("displayName", DisplayName??string.Empty));
            xml.Add(new XAttribute("expectValue", ExpectValue ?? string.Empty));
            xml.Add(new XAttribute("actualValue", ActualValue ?? string.Empty));
            xml.Add(new XAttribute("verifyResult", VerifyResult));
            xml.Add(new XAttribute("information", Information ?? string.Empty));

            return xml;
        }

    }
}
