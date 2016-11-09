using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EMC.SourceOne.SearchWS.ExSearchWebService;

namespace RestoreLib
{
    public class VirtualHierachy : SPRestore
    {
        public VirtualHierachy(string webAppUrl, string searchServiceURL, string userName, string password)
            : base(webAppUrl, searchServiceURL, userName, password)
        {
        }

        public XmlDocument GetArchiveFolderHierarchyXML(List<String> folderNames, string farmID)
        {
            List<String> folderIDs = GetBusGUIDs(folderNames);
            return GetArchiveFolderHierarchyXMLByIDs(folderIDs, farmID);       
        }

        public XmlDocument GetArchiveFolderHierarchyXMLByIDs(List<String> folderIDs, string farmID)
        {
            XmlDocument folderXMLDoc = new XmlDocument();
            XmlDocument folderFarmIDXML = new XmlDocument();
            XmlNode rootNode = folderFarmIDXML.CreateNode(XmlNodeType.Element, "FolderXML", null);
            XmlAttribute rootNodeAttribute = folderFarmIDXML.CreateAttribute("Version");
            rootNodeAttribute.Value = "1";
            rootNode.Attributes.Append(rootNodeAttribute);
            folderFarmIDXML.AppendChild(rootNode);

            XmlDeclaration xmldecl;
            xmldecl = folderFarmIDXML.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-16";
            folderFarmIDXML.InsertBefore(xmldecl, folderFarmIDXML.DocumentElement);
            XmlNode folderFarmIDNode = null;
            XmlNode domainIDNode = null;

            if (folderIDs.Count != 0)
            {
                foreach (String folderID in folderIDs)
                {

                    folderFarmIDNode = folderFarmIDXML.CreateElement("BusinessFolder");
                    XmlAttribute folderFarmIDAttribute = folderFarmIDXML.CreateAttribute("GUID");
                    folderFarmIDAttribute.Value = folderID;
                    folderFarmIDNode.Attributes.Append(folderFarmIDAttribute);
                    if (farmID != "")
                    {
                        domainIDNode = folderFarmIDXML.CreateElement("Domain");
                        XmlAttribute domainIDAttribute = folderFarmIDXML.CreateAttribute("Id");
                        domainIDAttribute.Value = farmID;
                        domainIDNode.Attributes.Append(domainIDAttribute);
                        folderFarmIDNode.AppendChild(domainIDNode);
                    }
                    rootNode.AppendChild(folderFarmIDNode);

                }
            }

            XmlDocument result = searchClientProxy.Client.GetArchivedFolderHierarchy(folderFarmIDXML.InnerXml);
            return result;
        }

        public XmlDocument GetArchiveFolderHierarchyXML(string folderXML)
        {
            XmlDocument result = searchClientProxy.Client.GetArchivedFolderHierarchy(folderXML);
            return result;
        }

        public String GetBusGUID(String busName)
        {
            String busGUID = null;
            XmlNode archiveFoldersNode = searchClientProxy.Client.EnumerateArchiveFolders(ExSearchType.Administrator);
            foreach (XmlNode archiveFolderNode in archiveFoldersNode)
            {
                if (archiveFolderNode.Attributes["folder"].Value == busName)
                    busGUID = archiveFolderNode.Attributes["folderid"].Value;
            }
            return busGUID;
        }

        public List<String> GetBusGUIDs(List<String> busNames)
        {
            List<String> busGUIDs = new List<string>();
            String busGUID = null;
            XmlNode archiveFoldersNode = searchClientProxy.Client.EnumerateArchiveFolders(ExSearchType.Administrator);
            foreach (String busName in busNames)
            {
                busGUID = GetBusGUID(busName);

                if (busGUID!=null)
                    busGUIDs.Add(busGUID);
            }
            return busGUIDs;
        }

        public List<String> GetLocations(XmlDocument vhXML, string folderName)
        {
            List<String> locations = new List<string>();
            string folderID = GetBusGUID(folderName);
            string xpath = @"//BusinessFolder[@GUID='" + folderID + @"']";
            XmlNode busFolderNode = vhXML.SelectSingleNode(xpath);
            if (busFolderNode == null||busFolderNode.FirstChild==null)
                return locations;
            XmlNodeList locationNodes = busFolderNode.FirstChild.ChildNodes;
            foreach (XmlNode locationNode in locationNodes)
            {
                locations.Add(locationNode.InnerText);
            }
            return locations;

        }
    }
}
