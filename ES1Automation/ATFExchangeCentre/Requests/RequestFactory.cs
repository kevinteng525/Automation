using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ATFExchangeCentre.Requests
{
    public static class RequestFactory
    {
        public static Request CreateRequest(string requestString)
        {
            var requestRoot = XElement.Parse(requestString);

            switch (GetAttributeValue(requestRoot, "type").ToUpper())
            {
                case "CMD":
                    return new CMDRequest(requestRoot);

                case "SYSTEM":
                    return new SystemRequest(requestRoot);

                case "RUNJOB":
                    return new RunJobRequest(requestRoot);

                case "REPORTJOB":
                    return new RunJobRequest(requestRoot);

                default:
                    throw new Exception("request string is error");
            }
        }

        public static string GetAttributeValue(XElement requestRoot, string attrName)
        {
            XAttribute attr = requestRoot.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            throw new Exception(string.Format("Config Error: attribute:{0} is not find !", attrName));
        }

        public static string GetAttributeValueOptional(XElement requestRoot, string attrName)
        {
            XAttribute attr = requestRoot.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            return string.Empty;
        }

    }
}
