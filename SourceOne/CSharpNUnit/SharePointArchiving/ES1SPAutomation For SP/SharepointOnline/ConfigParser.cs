using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

namespace SharepointOnline
{
    public class ConfigParser
    {
        private XmlDocument configDoc = new XmlDocument();

        public ConfigParser(string xmlPath)
        {
            configDoc.Load(xmlPath);
        }

        public NameValueCollection GetNodeAttibuteValue(string nodePath, string keyAttribute, string valueAttribute)
        {
            XmlNodeList siteNodeList = configDoc.SelectNodes(nodePath);
            NameValueCollection coll = new NameValueCollection();
            foreach (XmlNode siteNode in siteNodeList)
            {
                XmlElement xe = (XmlElement)siteNode;
                coll.Add(xe.GetAttribute(keyAttribute), xe.GetAttribute(valueAttribute));
            }
            return coll;
        }

        public string GetSingleNodeAttributeValue(string nodeName, string attributeName)
        {
            XmlNode siteNode = configDoc.SelectSingleNode("//" + nodeName);
            XmlElement xe = (XmlElement)siteNode;
            return xe.GetAttribute(attributeName).Trim();
        }

       
    }
}
