using System;
using System.Collections.Generic;
using System.Text;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;

namespace ES1.ES1SPAutoLib
{
    public class ArchiveFolder
    {
        public String Name;
        public String ArchiveLocation;
        public String IndexLocation;
    }
    public class NativeArchiveOperator
    {
        public String NAConnectionName;
        

        public NativeArchiveOperator(String nac)
        {
            NAConnectionName = nac;
        }

        public void CreateArchiveFolder(ArchiveFolder af)
        {
            try
            {
                CoExASAdminAPI adminApi = new CoExASAdminAPI();
                adminApi.Initialize();
                CoExASRepository repo = (CoExASRepository)adminApi.GetRepository(NAConnectionName);

                CoExASFolderPlan folderPlan = (CoExASFolderPlan)repo.GetFolderPlan();
                CoExASArchiveFolderSet folderSet = (CoExASArchiveFolderSet)folderPlan.EnumerateArchiveFolders();
                foreach (CoExASArchiveFolder folder in folderSet)
                {
                    if (folder.FullPath.Equals(af.Name))
                    {
                        return;
                    }
                }

                CoExASArchiveFolder archiveFolder =
                    (CoExASArchiveFolder)repo.CreateNewObject(exASObjectType.exASObjectType_ArchiveFolder);

                archiveFolder.FullPath = af.Name;
                archiveFolder.ContainerLocation = af.ArchiveLocation;
                archiveFolder.Description = "Created by Program";
                archiveFolder.MaxIndexSize = 2048;
                archiveFolder.MaxVolumeSize = 100;
                archiveFolder.RetentionPeriod = 0;
                archiveFolder.FullTextEnabled = 1;
                archiveFolder.AutoDispose = true;
                archiveFolder.IndexLocation = af.IndexLocation;

                archiveFolder.Save();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        

        public static IExProviderTypeConfig GetActiveArchiveConnection(String archiveConnectionName)
        {
            CoExJDFAPIMgr apiMgr = new CoExJDFAPIMgr();
            IExVector configs = apiMgr.GetProviderTypeConfigs() as IExVector;
            if (configs == null)
            {
                return null;
            }
            foreach (IExProviderTypeConfig config in configs)
            {
                if (config.name.Equals(archiveConnectionName) && (config.state == exProviderConfigState.exProviderConfigState_Active))
                {
                    return config;
                }
            }
            return null;
        }

        public static IExASArchiveFolder2 GetArchiveFolder(String connectionName, int folderId)
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

        public static IExASArchiveFolder2 GetArchiveFolder(String connectionName, String folderName)
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

        public static CoExASArchiveFolderSet EnumerateArchiveFolders(String connectionName)
        {
            CoExASAdminAPI adminApi = new CoExASAdminAPI();
            adminApi.Initialize();

            CoExASRepository repo = (CoExASRepository)adminApi.GetRepository(connectionName);

            CoExASFolderPlan folderPlan = (CoExASFolderPlan)repo.GetFolderPlan();
            return (CoExASArchiveFolderSet)folderPlan.EnumerateArchiveFolders();
        }
    }
}
