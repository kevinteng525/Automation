using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections.Specialized;

namespace ES1TestDataImport
{
    public class ConfigParser
    {
        private XmlDocument configDoc=new XmlDocument();

        public ConfigParser(string xmlPath)
        {
            configDoc.Load(xmlPath);
        }

        

        private XmlNodeList GetSiteNodeList(string port)
        {
            XmlNodeList webNodeList = configDoc.GetElementsByTagName("WebApp");
            foreach (XmlNode node in webNodeList)
            {
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("Port").ToLower() == port)
                {
                    return xe.GetElementsByTagName("Site");
                }
            }
            return null;
        }

        private XmlNodeList GetListNodeList(string port, string siteName)
        {
            XmlNodeList siteNodeList = GetSiteNodeList(port);
            foreach (XmlNode node in siteNodeList)
            {
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("Name").ToLower() == siteName)
                {
                    return xe.GetElementsByTagName("List");
                }
            }
            return null;
        }

        private XmlNodeList GetFileNodeList(string port, string siteName, string listName)
        {
            XmlNodeList siteNodeList = GetListNodeList(port,siteName);
            foreach (XmlNode node in siteNodeList)
            {
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("Name").ToLower() == listName)
                {
                    return xe.GetElementsByTagName("Files");
                }
            }
            return null;
        }

        public NameValueCollection WebAppNodeValueCol
        {
            get
            {
                NameValueCollection coll = new NameValueCollection();
                XmlNodeList nodeList = configDoc.GetElementsByTagName("WebApp");
                foreach (XmlNode node in nodeList)
                {
                    XmlElement xe = (XmlElement)node;
                    coll.Add(xe.GetAttribute("Port"), xe.GetAttribute("OpenMode"));
                }
                return coll;
            }
        }

        public NameValueCollection SiteNodeValueCol(string port)
        {
                NameValueCollection coll = new NameValueCollection();
                XmlNodeList nodeList = GetSiteNodeList(port);
                foreach (XmlNode node in nodeList)
                {
                    XmlElement xe = (XmlElement)node;
                    coll.Add(xe.GetAttribute("Name"), xe.GetAttribute("OpenMode"));
                }
                return coll;
        }

        public NameValueCollection ListNodeValueCol(string port, string siteName)
        {
            NameValueCollection coll = new NameValueCollection();
            XmlNodeList nodeList = GetListNodeList(port,siteName);
            foreach (XmlNode node in nodeList)
            {
                XmlElement xe = (XmlElement)node;
                coll.Add(xe.GetAttribute("Name"), xe.GetAttribute("Type"));
            }
            return coll;
        }

        public NameValueCollection FileNodeValueCol(string port, string siteName, string listName)
        {
            NameValueCollection coll = new NameValueCollection();
            XmlNodeList nodeList = GetFileNodeList(port, siteName, listName);
            foreach (XmlNode node in nodeList)
            {
                XmlElement xe = (XmlElement)node;
                coll.Add(xe.GetAttribute("Date"), xe.GetAttribute("Type"));
            }
            return coll;
        }
    }
}
