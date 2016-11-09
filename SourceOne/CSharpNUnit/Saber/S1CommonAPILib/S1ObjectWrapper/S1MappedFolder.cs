using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using System.Management;
using System.Management.Instrumentation;

using Saber.Common;

using EMC.Interop.ExBase;

namespace Saber.S1CommonAPILib
{
    public class S1MappedFolder : IS1Object
    {
        public String ArchiveConnection { get; set; }
        
        public String Name { get; set; }
        public String Description { get; set; }
        public String ArchiveFolder { get; set; }
        public S1BusinessFolderType FolderType { get; set; }
        public List<S1MappedFolderPermission> Permissions { get; set; }

        public S1MappedFolder()
        {
            this.Permissions = new List<S1MappedFolderPermission>();
        }

        public S1MappedFolder(string name, string connectionName, string archiveFolderName, string description)
        {
            this.Name = name;
            this.ArchiveConnection = connectionName;
            this.ArchiveFolder = archiveFolderName;
            this.Description = description;
            this.Permissions = new List<S1MappedFolderPermission>();
        }

        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Can not find the file: " + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String name = XMLHelper.GetElementValue(element, "name");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            else
            {
                throw new Exception("The name is required for mapped folder.");
            }
            String description = XMLHelper.GetElementValue(element, "description");
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }
            String archiveconnection = XMLHelper.GetElementValue(element, "archiveconnection");
            if (!String.IsNullOrEmpty(archiveconnection))
            {
                this.ArchiveConnection = archiveconnection;
            }
            else
            {
                throw new Exception("The archive connection of the archive folder is required for mapped folder.");
            }
            String archivefolder = XMLHelper.GetElementValue(element, "archivefolder");
            if (!String.IsNullOrEmpty(archivefolder))
            {
                this.ArchiveFolder = archivefolder;
            }
            else
            {
                throw new Exception("The archive folder is required for mapped folder.");
            }
            String businessfoldertype = XMLHelper.GetElementValue(element, "businessfoldertype");
            if (!String.IsNullOrEmpty(businessfoldertype))
            {                
                switch (businessfoldertype.ToLower())
                {
                    case "organization":
                        this.FolderType = S1BusinessFolderType.Organization;
                        break;
                    case "legalhold":
                        this.FolderType = S1BusinessFolderType.LegalHold;
                        break;
                    case "personal":
                        this.FolderType = S1BusinessFolderType.Personal;
                        break;
                    case "community":
                        this.FolderType = S1BusinessFolderType.Community;
                        break;                        
                    default:
                        throw new Exception("The folder type: " + businessfoldertype + " is not valid.");
                }
                
            }
            else
            {
                throw new Exception("The business folder type is required for mapped folder.");
            }
            XElement permissions = element.Element("permissions");
            if (null != permissions)
            {
                List<S1MappedFolderPermission> permissionList = new List<S1MappedFolderPermission>();
                foreach (XElement permission in permissions.Elements())
                {
                    permissionList.Add(new S1MappedFolderPermission(permission));
                }
                this.Permissions = permissionList;
            }
            return true;
        }
    }
}
