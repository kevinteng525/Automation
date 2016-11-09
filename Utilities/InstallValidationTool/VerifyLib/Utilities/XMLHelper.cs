using System;
using System.Xml.Linq;

namespace VerifyLib.Utilities
{
    public static class XMLHelper
    {
        public static string GetAttributeValue(XElement groupNode, string attrName)
        {
            XAttribute attr = groupNode.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            throw new Exception(string.Format("Config Error: attribute:{0} is not find !", attrName));
        }
    }
}
