using System;
using System.Collections.Generic;
using System.Xml.Linq;
using VerifyLib.Utilities;

namespace VerifyLib
{
    public class VerifyGroup
    {
        public string GroupName { get; private set; }

        public List<VerifyItem> VerifyItems { get; set; }

        public VerifyEnvironment VerifyEnvironment { get; set; }

        public VerifyGroup(XElement groupNode, VerifyEnvironment environment)
        {
            VerifyEnvironment = environment;

            GroupName = XMLHelper.GetAttributeValue(groupNode, "groupName");

            VerifyItems = new List<VerifyItem>();

            string t = XMLHelper.GetAttributeValue(groupNode, "type");

            var type = VerifyType.NotDefined;

            // generate items
            if (Enum.TryParse(t, true, out type))
            {
                foreach (var itemNode in groupNode.Elements("Item"))
                {
                    GenerateVerifyItem(type, itemNode);
                }
            }
        }

        public VerifyItem GenerateVerifyItem(VerifyType type, XElement itemNode)
        {
            return VerifyItemFactory.CreateVerifyItem(type, itemNode, this);
        }

        public void Verify()
        {
            foreach (var item in VerifyItems)
            {
                item.DoValidation();
            }
        }
    }
}
