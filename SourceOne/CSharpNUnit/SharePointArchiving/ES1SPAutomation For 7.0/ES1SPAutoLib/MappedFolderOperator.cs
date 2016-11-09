using System;
using System.Collections.Generic;
using System.Text;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace ES1.ES1SPAutoLib
{
    public class MappedFolder
    {
        public String Name;
        public String ArchiveFolder;
        public String NAConnection;
        public String Description;
        public exBusinessFolderType Type;
        public List<MappedFolderPermission> mfPermissions;
    }
    public class MappedFolderOperator
    {
        private static IExProviderGW_2 _providerGw = SourceOneContext.ProviderGW;
        private static IExFolderMgr_2 _folderMgr = SourceOneContext.FolderMgr;

        public static int CreateMappedFolder(MappedFolder mf)
        {
            if (IsFolderExist(mf.Name))
                return 0;
            CoExJanusFolder folder = GenerateMappedFolder(mf);
            IExDataSource dataSource = null;
            SearchResult entry = null;

            foreach (MappedFolderPermission item in mf.mfPermissions)
            {
                entry = ADOperator.ExecuteLDAPQueryFromLDAPServer("(cn=" + item.UserName + ")");

                if (item.UserType.ToLower().Equals("group")) 
                    dataSource = DataSourceOperator.BuildDataSoruceFromGroup(entry);
                else 
                    dataSource = DataSourceOperator.BuildDataSourceFromUser(entry);
                AddDataSource(folder, dataSource, (uint)item.Permission);
            }

            folder.Save();
            return folder.FolderId;
        }

        public static bool IsFolderExist(String folderName)
        {
            try
            {
                IExJanusFolder_2 folder = _folderMgr.FindFolderByName(folderName);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private static CoExJanusFolder GenerateMappedFolder(MappedFolder mf)
        {
            CoExJanusFolder newFolder = _folderMgr.CreateFolder(mf.Type);
            //IExASArchiveFolder2 af = NativeArchiveOperator.GetArchiveFolder(Configuration.ArchiveConnection, mf.ArchiveFolder);
            newFolder.Name = mf.Name;
            newFolder.Description = mf.Description;
            newFolder.ArchiveFolderPath = "\\" + mf.ArchiveFolder;
            newFolder.FolderId = -1;
            newFolder.ProviderConfigID = NativeArchiveOperator.GetActiveArchiveConnection(Configuration.ArchiveConnection).id;
            return newFolder;
        }

        private static void AddDataSource(CoExJanusFolder folder, IExDataSource dataSource, uint roleMask)
        {
            IExPrincipleRoles principleRoles = folder.GetPrincipleRoles();
            principleRoles.Add(dataSource.dataSourceName, dataSource.providerTypeID, -1, roleMask, (CoExDataSource)dataSource);
            folder.SetPrincipleRoles(principleRoles as CExPrincipleRoles);
        }
    }

    /*public class UserInfo
    {
        public String userName;
        public String userDN;
        public bool isGroup;
    }*/

    public class MappedFolderPermission
    {
        public String UserName;
        public String UserType;
        public int Permission;
    }

}
