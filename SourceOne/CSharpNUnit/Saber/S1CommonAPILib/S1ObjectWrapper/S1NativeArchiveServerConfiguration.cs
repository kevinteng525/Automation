using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Saber.Common;

namespace Saber.S1CommonAPILib
{
     public class S1NativeArchiveServerConfiguration : IS1Object
    {
        //archive config
        public bool Archive_Enabled { get; set; }
        public String Archive_MessageCenterLocation { get; set; }
        public uint Archive_VolumeIdleTime { get; set; }
        //Index
        public bool Index_Enabled { get; set; }
        public List<String> Index_ArchiveServersToIndex { get; set; }
        public uint Index_RunThreshold { get; set; }

        public uint Index_ComponentLimitPerIndexAction_Add { get; set; }
        public uint Index_ComponentLimitPerIndexAction_Delete { get; set; }
        public uint Index_ComponentLimitPerIndexAction_Repair { get; set; }
        public uint Index_ComponentLimitPerIndexAction_Update { get; set; }
        //Search
        public bool Search_Enabled { get; set; }
        public uint Search_MemoryAllocated { get; set; }
        //Retrieval
        public bool Retrieval_Enabled { get; set; }


        public S1NativeArchiveServerConfiguration()
        { }
        public S1NativeArchiveServerConfiguration(String mcLocation )
        {
            this.Archive_Enabled = true;
            this.Archive_VolumeIdleTime = 172800;
            this.Archive_MessageCenterLocation = mcLocation;

            this.Index_Enabled = true;
            this.Index_ArchiveServersToIndex = new List<string>();
            this.Index_ComponentLimitPerIndexAction_Add = 4;
            this.Index_ComponentLimitPerIndexAction_Delete = 2;
            this.Index_ComponentLimitPerIndexAction_Repair = 2;
            this.Index_ComponentLimitPerIndexAction_Update = 4;
            this.Index_RunThreshold = 45;

            this.Search_Enabled = true;
            this.Search_MemoryAllocated = 50;

            this.Retrieval_Enabled = true;

        }


        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("File not found: " + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            XElement archive = element.Element("archive");
            XElement index = element.Element("index");
            XElement search = element.Element("search");
            XElement retrieval = element.Element("retrieval");
            if (null != archive)
            {
                String enable = XMLHelper.GetElementValue(archive, "enable");
                String messagecenterlocation = XMLHelper.GetElementValue(archive, "messagecenterlocation");
                String volumeidletime = XMLHelper.GetElementValue(archive, "volumeidletime"); 
                if (!String.IsNullOrEmpty(enable))
                {
                    this.Archive_Enabled = bool.Parse(enable);
                }                
                if (!String.IsNullOrEmpty(messagecenterlocation))
                {
                    this.Archive_MessageCenterLocation = messagecenterlocation;
                }
                if (!String.IsNullOrEmpty(volumeidletime))
                {
                    this.Archive_VolumeIdleTime = uint.Parse(volumeidletime);
                }
            }
            if (null != index)
            {
                String enable = XMLHelper.GetElementValue(index, "enable");
                if (!String.IsNullOrEmpty(enable))
                {
                    this.Index_Enabled = bool.Parse(enable);
                }               
                XElement archiveServers = index.Element("archiveservers");
                if (null != archiveServers)
                {
                    List<String> servers = new List<String>();
                    foreach (XElement server in archiveServers.Elements())
                    {
                        if (!String.IsNullOrEmpty(server.Value))
                        {
                            servers.Add(server.Value);
                        }
                    }
                    if (servers.Count > 0)
                    {
                        this.Index_ArchiveServersToIndex = servers;
                    }
                }
                String addLimit = XMLHelper.GetElementValue(index, "componentlimitperindexaction_add");
                String deleteLimit = XMLHelper.GetElementValue(index, "componentlimitperindexaction_delete"); 
                String repareLimit = XMLHelper.GetElementValue(index, "componentlimitperindexaction_repair"); 
                String updateLimit = XMLHelper.GetElementValue(index, "componentlimitperindexaction_update");

                if (!String.IsNullOrEmpty(addLimit))
                {
                    this.Index_ComponentLimitPerIndexAction_Add = uint.Parse(addLimit);
                }
                if (!String.IsNullOrEmpty(deleteLimit))
                {
                    this.Index_ComponentLimitPerIndexAction_Delete = uint.Parse(deleteLimit);
                }
                if (!String.IsNullOrEmpty(repareLimit))
                {
                    this.Index_ComponentLimitPerIndexAction_Repair = uint.Parse(repareLimit);
                }
                if (!String.IsNullOrEmpty(updateLimit))
                {
                    this.Index_ComponentLimitPerIndexAction_Update = uint.Parse(updateLimit);
                }
                String runthreshod = XMLHelper.GetElementValue(index, "runthreshod");

                if (!String.IsNullOrEmpty(runthreshod))
                {
                    this.Index_RunThreshold = uint.Parse(runthreshod);
                }

            }
            if (null != search)
            {
                String enable = XMLHelper.GetElementValue(search, "enable"); 
                if (!String.IsNullOrEmpty(enable))
                {
                    this.Search_Enabled = bool.Parse(enable);
                }
                String memoryallocated = XMLHelper.GetElementValue(search, "memoryallocated"); 
                if (!String.IsNullOrEmpty(memoryallocated))
                {
                    this.Search_MemoryAllocated = uint.Parse(memoryallocated);
                }
            }
            if (null != retrieval)
            {
                String enable = XMLHelper.GetElementValue(retrieval, "enable"); 
                if (!String.IsNullOrEmpty(enable))
                {
                    this.Retrieval_Enabled = bool.Parse(enable);
                }                
            }
            return true;
        }
    }
}
