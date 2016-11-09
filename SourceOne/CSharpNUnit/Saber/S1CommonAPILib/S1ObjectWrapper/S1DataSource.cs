using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Saber.Common;

namespace Saber.S1CommonAPILib
{   
    public class S1DataSource : IS1Object
    {
        public S1DataSourceProviderType DataSourceProviderType { get; set; }
        public S1DataSourceChooseBy DataSourceChooseBy { get; set; }
        public String UserName { get; set; }
        public String DistributionGroup { get; set; }
        public ADObjectType Type { get; set; }
        public S1DataSource()
        {
            this.DataSourceProviderType = S1DataSourceProviderType.Exchange;
            this.DataSourceChooseBy = S1DataSourceChooseBy.AddressBook;
        }

        public S1DataSource(String userName, ADObjectType userType)
        {
            this.DataSourceProviderType = S1DataSourceProviderType.Exchange;
            this.DataSourceChooseBy = S1DataSourceChooseBy.AddressBook;
            this.Type = userType;
            switch (userType)
            {
                case ADObjectType.Group:
                    this.DistributionGroup = userName;
                    break;
                case ADObjectType.User:
                    this.UserName = userName;
                    break;
            }
        }

        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Cannot find the file: " + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String providertype = XMLHelper.GetElementValue(element, "providertype");
            if (!String.IsNullOrEmpty(providertype))
            {
                switch (providertype.ToLower())
                {
                    case "exchange":
                        this.DataSourceProviderType = S1DataSourceProviderType.Exchange;
                        break;
                    case "notes":
                        throw new Exception("The notes is not supportted yet.");

                    default:
                        throw new Exception("The type is not valid or not supportted yet.");
                }
            }
            else
            {
                throw new Exception("The provider type is required.");
            }

            String chooseby = XMLHelper.GetElementValue(element, "chooseby");
            if (!String.IsNullOrEmpty(chooseby))
            {
                switch (chooseby.ToLower())
                {
                    case "addressbook":
                        this.DataSourceChooseBy = S1DataSourceChooseBy.AddressBook;
                        break;
                    case "serverhierarchy":
                        throw new Exception("Choosing by ServerHierarchy is not supportted yet.");
                    case "ldap":
                        throw new Exception("Choosing by LDAP is not supportted yet.");
                    default:
                        throw new Exception("The type is not valid or not supportted yet.");
                }
            }

            String sourcetype = XMLHelper.GetElementValue(element, "sourcetype");
            if (String.IsNullOrEmpty(sourcetype))
            {
                throw new Exception("The source type is required.");
            }
            else
            {
                if (sourcetype.ToLower().Equals("user"))
                {
                    this.Type = ADObjectType.User;
                    String username = XMLHelper.GetElementValue(element, "username");
                    if (!String.IsNullOrEmpty(username))
                    {
                        this.UserName = username;
                    }
                }
                else if (sourcetype.ToLower().Equals("group"))
                {
                    this.Type = ADObjectType.Group;
                    String groupname = XMLHelper.GetElementValue(element, "groupname");
                    if (!String.IsNullOrEmpty(groupname))
                    {
                        this.DistributionGroup = groupname;
                    }
                }
                else
                {
                    throw new Exception("The source type is not valid.");
                }
            }
            return true;
        }
    }
}
