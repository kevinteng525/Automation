using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;
using EMC.Interop.ExASBaseAPI;

namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1ArchiveFolderStorageType
    {
        UnDefined = -1,
        [Description("exASStorageType_NASContainer")]
        NASContainer = 0,
        [Description("exASStorageType_DXContainer")]
        DXContainer = 2,
        [Description("exASStorageType_CenteraContainer")]
        CenteraContainer = 4,
        [Description("exASStorageType_CelerraContainer")]
        CelerraContainer = 6,
        [Description("exASStorageType_SnapLockContainer")]
        SnapLockContainer = 8,//TBD
        [Description("exASStorageType_DataDomainContainer")]
        DataDomainContainer = 10,
        [Description("exASStorageType_VirtualContainer")]
        VirtualContainer = 12,
        [Description("exASStorageType_AtmosContainer")]
        AtmosContainer = 14,
        [Description("exASStorageType_IsilonContainer")]
        IsilonContainer = 16,

    }

    public enum S1ArchiveFolderOrganizationMethod
    {
        By_Date = 0,
        By_Matter = 1,
    }

    public class S1NativeArchiveFolderHelper
    {
        public static int CreateArchiveFolder(S1NativeArchiveFolder archiveFolder)
        {
            try
            {
                CoExASAdminAPI adminApi = new CoExASAdminAPI();
                adminApi.Initialize();
                CoExASRepository repo = (CoExASRepository)adminApi.GetRepository(archiveFolder.ArchiveConnectionName);

                CoExASArchiveFolder tempFolder = GetArchiveFolderByName(archiveFolder.ArchiveConnectionName, archiveFolder.Name);
                
                if(null != tempFolder)
                {
                    return tempFolder.NodeId;
                }
                else
                {
                    CoExASArchiveFolder folder =
                    (CoExASArchiveFolder)repo.CreateNewObject(exASObjectType.exASObjectType_ArchiveFolder);
                    //connection?
                    
                    //general
                    folder.FullPath = archiveFolder.Name;
                    folder.Description = archiveFolder.Description;
                    //storage
                    folder.MaxVolumeSize = archiveFolder.Storage_MaximumVolumeSize;
                    folder.RetentionPeriod = archiveFolder.Storage_MonthesToRetain;
                    folder.AutoDispose = archiveFolder.Storage_EnableAutoDisposition;
                    switch (archiveFolder.Storage_Type)
                    {
                        case S1ArchiveFolderStorageType.NASContainer:
                            folder.StorageType = exASFolderStorageType.exASStorageType_NASContainer;
                            folder.ContainerLocation = (archiveFolder.Storage_ArchiveFolderConfig as S1NASContainerStorageConfig).ArchiveLocation;

                            break;
                        default:
                            throw new Exception("Not support yet.");
                    }
                    
                    //large content
                    if (!archiveFolder.LargeContent_StoreAllContentInsideContainer)
                    {
                        folder.CompressLargeAttachments = archiveFolder.LargeContent_CompressLargeAttachments;
                        folder.AttachmentSizeThreshold = archiveFolder.LargeContent_StoreContentWhenLargerThan;
                    }

                    //organization options
                    folder.ConceptBasedFolderType = (int)archiveFolder.Organization_OrganizationMethod;

                    //index
                    folder.FullTextEnabled = archiveFolder.Index_EnabledIndexing ? 1 : 0;//TO Be Verified
                    folder.MaxIndexSize = archiveFolder.Index_MaximumIndexSize;
                    folder.ContentCache = archiveFolder.Index_EnableContentCache ? 1 : 0;//TO Be Verified
                    folder.AttachmentIndexing = archiveFolder.Index_AttachmentAndFileIndexing ? 0 : 1;//TO Be Verified
                    folder.NestedContainers = archiveFolder.Index_IndexNestedSubContains ? 1 : 0;//TO Be Verified
                    folder.IndexLocation = archiveFolder.Index_IndexStorageLocations;

                    //persistence
                    folder.Save();
                    return folder.NodeId;

                }                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal static CoExASArchiveFolder GetArchiveFolderById(String connectionName, int folderId)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                //IExVector configs = SourceOneContext.JDFAPIMgr.GetProviderTypeConfigs() as IExVector;

            }
            else
            {
                CoExASArchiveFolderSet archiveFolderSet = EnumerateArchiveFolders(connectionName);
                foreach (CoExASArchiveFolder folder in archiveFolderSet)
                {
                    if (folder.NodeId == folderId)
                    {
                        return folder;
                    }
                }
            }
            return null;
        }

        internal static CoExASArchiveFolder GetArchiveFolderByName(String connectionName, String folderName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {

            }
            else
            {
                CoExASArchiveFolderSet archiveFolderSet = EnumerateArchiveFolders(connectionName);
                foreach (CoExASArchiveFolder folder in archiveFolderSet)
                {
                    if (folder.FullPath.Equals(folderName))
                    {
                        return folder;
                    }
                }
            }
            return null;
        }

        internal static CoExASArchiveFolderSet EnumerateArchiveFolders(String connectionName)
        {
            CoExASAdminAPI adminApi = new CoExASAdminAPI();
            adminApi.Initialize();

            CoExASRepository repo = (CoExASRepository)adminApi.GetRepository(connectionName);

            CoExASFolderPlan folderPlan = (CoExASFolderPlan)repo.GetFolderPlan();
            return (CoExASArchiveFolderSet)folderPlan.EnumerateArchiveFolders();
        }

        internal static List<CoExASArchiveFolder> GetAllArchiveFolders(String connectionName)
        {
            CoExASArchiveFolderSet folders = EnumerateArchiveFolders(connectionName);
            List<CoExASArchiveFolder> fs = new List<CoExASArchiveFolder>();
            foreach (CoExASArchiveFolder folder in folders)
            {
                fs.Add(folder);
            }
            return fs;
        }

    }
}
