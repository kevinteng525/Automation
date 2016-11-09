using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Saber.Common;

namespace Saber.S1CommonAPILib
{
    public class S1ArchiveFolderStorageConfig
    { }

    public class S1NASContainerStorageConfig : S1ArchiveFolderStorageConfig
    {
        //To Be Detailed 
        public String ArchiveLocation { get; set; }
        public S1NASContainerStorageConfig()
        {
 
        }
        public S1NASContainerStorageConfig(XElement element)
        {
            String archivelocation = XMLHelper.GetElementValue(element, "archivelocation");
            if (!String.IsNullOrEmpty(archivelocation))
            {
                this.ArchiveLocation = archivelocation;
            }
            else
            {
                throw new Exception("Please specify the archive location for the NAS storage.");
            }
        }

    }

    public class S1NativeArchiveFolder : IS1Object
    {
        //hidden
        public String ArchiveConnectionName { get; set; }
        //general
        public String Name{get; set;}
        public String Description { get; set; }
        //storage options
        public S1ArchiveFolderStorageType Storage_Type { get; set; }
        public S1ArchiveFolderStorageConfig Storage_ArchiveFolderConfig { get; set; }
        public int Storage_MaximumVolumeSize { get; set; }
        public int Storage_MonthesToRetain { get; set; }
        public bool Storage_EnableAutoDisposition { get; set; }
        public bool Storage_CompressContentInContainers { get; set; }
        //large content
        public bool LargeContent_StoreAllContentInsideContainer { get; set; }
        public bool LargeContent_CompressLargeAttachments { get; set; }
        public int LargeContent_StoreContentWhenLargerThan { get; set; }        
        //organization options
        public S1ArchiveFolderOrganizationMethod Organization_OrganizationMethod { get; set; }
        //indexing
        public bool Index_EnabledIndexing { get; set; }
        public int Index_MaximumIndexSize { get; set; }        
        public bool Index_EnableContentCache { get; set; }
        public bool Index_AttachmentAndFileIndexing { get; set; }
        public bool Index_IndexNestedSubContains { get; set; }
        public String Index_IndexStorageLocations { get; set; }
        

        public S1NativeArchiveFolder()
        { }

        public S1NativeArchiveFolder(String archiveConnectionName)
        {
            this.ArchiveConnectionName = archiveConnectionName;
        }

        public S1NativeArchiveFolder(String archiveConnectionName, S1ArchiveFolderStorageType storageType)
        {
            this.ArchiveConnectionName = archiveConnectionName;
            this.Storage_Type = storageType;
            switch (storageType)
            {
                case S1ArchiveFolderStorageType.NASContainer:
                    this.Storage_ArchiveFolderConfig = new S1NASContainerStorageConfig();
                    break;
                default:
                    throw new Exception("This kind of container is not implemented yet.");
            }
        }



        public bool DeserializeFromXMLFile(String filePath)
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
            //hidden
            String connection = XMLHelper.GetElementValue(element, "archiveconnection");
            if (String.IsNullOrEmpty(connection))
            {
                throw new Exception("The archive connection name is required.");
            }
            this.ArchiveConnectionName = connection;
            //general
            String name = XMLHelper.GetElementValue(element, "name");
            if (String.IsNullOrEmpty(name))
            {
                throw new Exception("The archive folder name is required.");
            }
            else
            {
                this.Name = name;
            }
            String description = XMLHelper.GetElementValue(element, "description");
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }
            //storage options
            String storageType = XMLHelper.GetElementValue(element, "storagetype");
            if (String.IsNullOrEmpty(storageType))
            {
                throw new Exception("The archive folder storage type is required.");
            }
            this.Storage_Type = GetStorageType(storageType);
            XElement storageconfig = element.Element("storageconfig");
            if (null != storageconfig)
            {
                this.Storage_ArchiveFolderConfig = new S1NASContainerStorageConfig(storageconfig);
            }
            String maximumvolumesize = XMLHelper.GetElementValue(element, "maximumvolumesize");
            String monthestoretain = XMLHelper.GetElementValue(element, "monthestoretain");
            String enableautodisposition = XMLHelper.GetElementValue(element, "enableautodisposition");
            String compresscontentincontainers = XMLHelper.GetElementValue(element, "compresscontentincontainers");
            if (!String.IsNullOrEmpty(maximumvolumesize))
            {
                this.Storage_MaximumVolumeSize = int.Parse(maximumvolumesize);
            }
            if (!String.IsNullOrEmpty(monthestoretain))
            {
                this.Storage_MonthesToRetain = int.Parse(monthestoretain);
            }
            if (!String.IsNullOrEmpty(enableautodisposition))
            {
                this.Storage_EnableAutoDisposition = bool.Parse(enableautodisposition);
            }
            if (!String.IsNullOrEmpty(compresscontentincontainers))
            {
                this.Storage_CompressContentInContainers = bool.Parse(compresscontentincontainers);
            }
            //large content options
            String storeallcontentinsidecontainer = XMLHelper.GetElementValue(element, "storeallcontentinsidecontainer");
            if (!String.IsNullOrEmpty(storeallcontentinsidecontainer))
            {
                this.LargeContent_StoreAllContentInsideContainer = bool.Parse(storeallcontentinsidecontainer);
            }
            String compresslargeattachments = XMLHelper.GetElementValue(element, "compresslargeattachments");
            if (!String.IsNullOrEmpty(compresslargeattachments))
            {
                this.LargeContent_CompressLargeAttachments = bool.Parse(compresslargeattachments);
            }
            String storecontentwhenlargerthan = XMLHelper.GetElementValue(element, "storecontentwhenlargerthan");
            if (!String.IsNullOrEmpty(storecontentwhenlargerthan))
            {
                this.LargeContent_StoreContentWhenLargerThan = int.Parse(storecontentwhenlargerthan);
            }
            //organizing method
            String organizationmethod = XMLHelper.GetElementValue(element, "organizationmethod");
            if (!String.IsNullOrEmpty(organizationmethod))
            {
                switch (organizationmethod.ToLower())
                {
                    case "bydate":
                        this.Organization_OrganizationMethod = S1ArchiveFolderOrganizationMethod.By_Date;
                        break;
                    case "bymatter":
                        this.Organization_OrganizationMethod = S1ArchiveFolderOrganizationMethod.By_Matter;
                        break;
                    default:
                        throw new Exception("The organizing method: " + organizationmethod + " is not valid.");
                }
            }
            //indexing
            String enabledindexing = XMLHelper.GetElementValue(element, "enabledindexing");
            if (!String.IsNullOrEmpty(enabledindexing))
            {
                this.Index_EnabledIndexing = bool.Parse(enabledindexing);
            }
            String maximumindexsize = XMLHelper.GetElementValue(element, "maximumindexsize");
            if (!String.IsNullOrEmpty(maximumindexsize))
            {
                this.Index_MaximumIndexSize = int.Parse(maximumindexsize);
            }
            String enabledcontentcache = XMLHelper.GetElementValue(element, "enabledcontentcache");
            if (!String.IsNullOrEmpty(enabledcontentcache))
            {
                this.Index_EnableContentCache = bool.Parse(enabledcontentcache);
            }
            String attachmentandfileindexing = XMLHelper.GetElementValue(element, "attachmentandfileindexing");
            if (!String.IsNullOrEmpty(attachmentandfileindexing))
            {
                this.Index_AttachmentAndFileIndexing = bool.Parse(attachmentandfileindexing);
            }
            String indexnestedsubcontains = XMLHelper.GetElementValue(element, "indexnestedsubcontains");
            if (!String.IsNullOrEmpty(indexnestedsubcontains))
            {
                this.Index_IndexNestedSubContains = bool.Parse(indexnestedsubcontains);
            }
            String indexstoragelocations = XMLHelper.GetElementValue(element, "indexstoragelocations");
            if (!String.IsNullOrEmpty(indexstoragelocations))
            {
                this.Index_IndexStorageLocations = indexstoragelocations;
            }
            return true;
        }

        private S1ArchiveFolderStorageType GetStorageType(String type)
        {
            S1ArchiveFolderStorageType ret = S1ArchiveFolderStorageType.UnDefined;
            switch (type)
            {
                case "NASContainer":
                    ret = S1ArchiveFolderStorageType.NASContainer;
                    break;
                case "DXContainer":
                    ret = S1ArchiveFolderStorageType.DXContainer;
                    break;
                case "CenteraContainer":
                    ret = S1ArchiveFolderStorageType.CenteraContainer;
                    break;
                case "CelerraContainer":
                    ret = S1ArchiveFolderStorageType.CelerraContainer;
                    break;
                case "SnapLockContainer":
                    ret = S1ArchiveFolderStorageType.SnapLockContainer;
                    break;
                case "DataDomainContainer":
                    ret = S1ArchiveFolderStorageType.DataDomainContainer;
                    break;
                case "VirtualContainer":
                    ret = S1ArchiveFolderStorageType.VirtualContainer;
                    break;
                case "AtmosContainer":
                    ret = S1ArchiveFolderStorageType.AtmosContainer;
                    break;
                case "IsilonContainer":
                    ret = S1ArchiveFolderStorageType.IsilonContainer;
                    break;
                default:
                    throw new Exception("The storage type for the archive folder specify by: " + type + " is not valid.");
            }
            return ret;
        }
    }

    
}
