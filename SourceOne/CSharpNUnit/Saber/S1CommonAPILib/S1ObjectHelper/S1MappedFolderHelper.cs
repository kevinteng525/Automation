using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.ComponentModel;
using System.Xml.Linq;

using Saber.Common;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;


namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1BusinessFolderType
    {
        [Description("exBusinessFolderType_All")]
        All = -1,
        [Description("exBusinessFolderType_Organization")]
        Organization = 1,
        [Description("exBusinessFolderType_LegalHold")]
        LegalHold = 2,
        [Description("exBusinessFolderType_Personal")]
        Personal = 4,
        [Description("exBusinessFolderType_Community")]
        Community = 8,
    }
    public enum S1MappedFolderPermissionRoles
    {
        ROLE_Undefined = 0,
        ROLE_Administrator = 1,
        ROLE_ReadAll = 2,
        ROLE_Contributor = 4,
        ROLE_Owner = 8,
        ROLE_ACL = 16,
        ROLE_USERDELETE = 32,
        ROLE_MATTERMANAGER = 64,
        ROLE_MAX_ROLEID = 128,
    }
     
    public class S1MappedFolderPermission
    {
        public String UserName;
        public ADObjectType UserType;
        public int Permission;

        public S1MappedFolderPermission(String name, ADObjectType type, int permission)
        {
            this.UserName = name;
            this.UserType = type;
            this.Permission = permission;
        }

        public S1MappedFolderPermission(String name, ADObjectType type)
        {
            this.UserName = name;
            this.UserType = type;
            this.Permission = (int)S1MappedFolderPermissionRoles.ROLE_Administrator |
                (int)S1MappedFolderPermissionRoles.ROLE_ReadAll |
                (int)S1MappedFolderPermissionRoles.ROLE_Contributor |
                (int)S1MappedFolderPermissionRoles.ROLE_Owner |
                (int)S1MappedFolderPermissionRoles.ROLE_ACL;
        }

        public S1MappedFolderPermission(XElement element)
        {
            String username = XMLHelper.GetElementValue(element, "username");
            if (!String.IsNullOrEmpty(username))
            {
                this.UserName = username;
            }
            else
            {
                throw new Exception("User name is required for mapped folder permission.");
            }

            String usertype = XMLHelper.GetElementValue(element, "usertype");
            if (!String.IsNullOrEmpty(usertype))
            {
                switch (usertype.ToLower())
                {
                    case "user":
                        this.UserType = ADObjectType.User;
                        break;
                    case "group":
                        this.UserType = ADObjectType.Group;
                        break;
                    default:
                        throw new Exception("The type: " + usertype + " is not valid.");
                }
                
            }
            else
            {
                throw new Exception("User type is required for mapped folder permission.");
            }

            String permission = XMLHelper.GetElementValue(element, "permission");
            if (!String.IsNullOrEmpty(permission))
            {
                int p = 0;
                string[] permissionSlice = permission.Split(',');
                foreach (string temp in permissionSlice)
                {

                    if (temp.ToLower().Equals("administrator"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_Administrator;
                    }
                    else if (temp.ToLower().Equals("readall"))
                    {

                        p |= (int)S1MappedFolderPermissionRoles.ROLE_ReadAll;
                    }
                    else if (temp.ToLower().Equals("contributor"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_Contributor;
                    }
                    else if (temp.ToLower().Equals("owner"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_Owner;
                    }
                    else if (temp.ToLower().Equals("myfile"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_ACL;
                    }
                    else if (temp.ToLower().Equals("delete"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_USERDELETE;
                    }
                    else if (temp.ToLower().Equals("mattermanager"))
                    {
                        p |= (int)S1MappedFolderPermissionRoles.ROLE_MATTERMANAGER;
                    }
                    else
                    {
                        throw new Exception("The permission role: " + temp + " is not valid.");
                    }

                }
                this.Permission = p;
            }
            else
            {
                throw new Exception("permission is required for mapped folder permission.");
            }

            
        }
    }

    public class S1MappedFolderHelper
    {
        public static void ConfigMappedFolderPermissions(CoExJanusFolder folder, List<S1MappedFolderPermission> mfPermissions)
        {
            IExDataSource dataSource = null;
            foreach (S1MappedFolderPermission item in mfPermissions)
            {
                if (item.UserType == ADObjectType.Group) 
                    dataSource = S1DataSourceHelper.BuildDataSourceFromGroup(item.UserName);
                else
                    dataSource = S1DataSourceHelper.BuildDataSourceFromUser(item.UserName);
                AddDataSource(folder, dataSource, (uint)item.Permission);
            }
        }

        public static bool IsMappedFolderExist(String folderName)
        {
            try
            {
                IExJanusFolder_2 folder = S1Context.FolderMgr.FindFolderByName(folderName);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        internal static CoExJanusFolder GetByName(String mappedFolderName)
        {
            CoExJanusFolder folder = null;
            try
            {
                folder = S1Context.FolderMgr.FindFolderByName(mappedFolderName);
            }
            catch (Exception e)
            {
                return null;
            }
            return folder;
        }

        internal static CoExJanusFolder GetById(int mappedFolderId)
        {
            CoExJanusFolder folder = null;
            try
            {
                folder = S1Context.FolderMgr.FindFolderByID(mappedFolderId);
            }
            catch (Exception e)
            {
                return null;
            }
            return folder;
        }

        internal static List<CoExJanusFolder> GetAll()
        {
            CoExJanusFolderSet folders = S1Context.FolderMgr.EnumerateFolders(eFolderInfoLevel.FolderEverything, (int)S1BusinessFolderType.All);
            List<CoExJanusFolder> folderList = new List<CoExJanusFolder>();
            foreach (CoExJanusFolder folder in folders)
            {
                folderList.Add(folder);
            }
            return folderList;
        }

        public static int CreateS1MappedFolder(S1MappedFolder mappedFolder)
        {
            if (IsMappedFolderExist(mappedFolder.Name))
                return GetByName(mappedFolder.Name).FolderId;
            CoExJanusFolder newFolder = S1Context.FolderMgr.CreateFolder((exBusinessFolderType)Enum.Parse(typeof(exBusinessFolderType), EnumToStringUsingDescription.GetS1EnumByDescription(mappedFolder.FolderType)));
            newFolder.Name = mappedFolder.Name;
            newFolder.Description = mappedFolder.Description;
            newFolder.ArchiveFolderPath = "\\" + mappedFolder.ArchiveFolder;
            newFolder.FolderId = -1;
            newFolder.ProviderConfigID = S1ArchiveConnectionHelper.GetByName(mappedFolder.ArchiveConnection).id;
            ConfigMappedFolderPermissions(newFolder, mappedFolder.Permissions);
            newFolder.Save();
            return newFolder.FolderId;
        }

        private static void AddDataSource(CoExJanusFolder folder, IExDataSource dataSource, uint roleMask)
        {
            IExPrincipleRoles principleRoles = folder.GetPrincipleRoles(); 
            principleRoles.Add(dataSource.dataSourceName, dataSource.providerTypeID, -1, roleMask, (CoExDataSource)dataSource);
            folder.SetPrincipleRoles(principleRoles as CExPrincipleRoles);
        }
    
    }

}
