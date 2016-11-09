using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Saber.Common
{
    public class XMLHelper
    {
        public static String GetElementValue(XElement element, String nName)
        {
            return (null == element.Element(nName)) ? null : element.Element(nName).Value.Trim();
        }
        public static XElement GetElement(XElement element, String nName)
        {
            return (null == element.Element(nName)) ? null : element.Element(nName);
        }
    }
}
