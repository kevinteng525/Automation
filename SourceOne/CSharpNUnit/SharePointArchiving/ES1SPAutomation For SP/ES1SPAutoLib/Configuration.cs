using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using EMC.Interop.ExBase;

namespace ES1.ES1SPAutoLib
{
    public class Configuration
    {
        public static String ADServer;
        public static String ADUserName;
        public static String ADUserPassword;
        public static String ArchiveConnection;
        public static String ConnDBServer;
        public static String ConnDBName;
        public static String MCLocation;
        public static List<ArchiveFolder> ArchiveFolders = new List<ArchiveFolder>();
        public static List<MappedFolder> MappedFolders = new List<MappedFolder>();

        public static void FillInValues(string configXML)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configXML);
                XmlNodeList roots = doc.GetElementsByTagName("Configuration");
                XmlElement root = (XmlElement)roots[0];
                XmlNodeList nodes = root.ChildNodes;
                XmlElement elem = null;
                for (int i = 0; i < nodes.Count; i++)
                {
                    elem = (XmlElement)nodes[i];
                    if (elem.Name.ToLower().Equals("adserver"))
                    {
                        Configuration.ADServer = elem.GetAttribute("Name");
                        Configuration.ADUserName = elem.GetAttribute("UserName");
                        Configuration.ADUserPassword = elem.GetAttribute("Password");
                    }
                    else if (elem.Name.ToLower().Equals("archiveconnection"))
                    {
                        Configuration.ArchiveConnection = elem.GetAttribute("Name");
                        Configuration.ConnDBServer = elem.GetAttribute("DBServer");
                        Configuration.ConnDBName = elem.GetAttribute("DBName");
                    }
                    else if (elem.Name.ToLower().Equals("messagecenter"))
                    {
                        Configuration.MCLocation = elem.GetAttribute("MCLocation");
                    }
                    else if (elem.Name.ToLower().Equals("archivefolder"))
                    {
                        ArchiveFolder af = new ArchiveFolder();
                        af.Name = elem.GetAttribute("Name");
                        af.ArchiveLocation = elem.GetAttribute("ArchiveLocation");
                        af.IndexLocation = elem.GetAttribute("IndexLocation");
                        Configuration.ArchiveFolders.Add(af);
                    }
                    else if (elem.Name.ToLower().Equals("mappedfolder"))
                    {
                        MappedFolder mf = new MappedFolder();
                        mf.Name = elem.GetAttribute("Name");
                        mf.Description = elem.GetAttribute("Description");
                        mf.ArchiveFolder = elem.GetAttribute("ArchiveFolder");
                        String type = elem.GetAttribute("Type");
                        if(type.ToLower().Equals("organization"))
                            mf.Type = exBusinessFolderType.exBusinessFolderType_Organization;
                        else if(type.ToLower().Equals("community"))
                            mf.Type = exBusinessFolderType.exBusinessFolderType_Community;
                        else if (type.ToLower().Equals("personal"))
                            mf.Type = exBusinessFolderType.exBusinessFolderType_Personal;
                        else
                            mf.Type = exBusinessFolderType.exBusinessFolderType_Organization;

                        List<MappedFolderPermission> permissions = new List<MappedFolderPermission>();
                        XmlNodeList pNodes = elem.ChildNodes;
                        XmlElement telem = null;
                        for (int j = 0; j < pNodes.Count; j++)
                        {
                            telem = (XmlElement)pNodes[j];
                            if (telem.Name.ToLower().Equals("mfpermission"))
                            {
                                MappedFolderPermission perm = new MappedFolderPermission();
                                perm.UserName = telem.GetAttribute("UserName");
                                perm.UserType = telem.GetAttribute("UserType");
                                perm.Permission = Int32.Parse(telem.GetAttribute("Permission"));
                                permissions.Add(perm);
                            }
                        }
                        mf.mfPermissions = permissions;
                        
                        Configuration.MappedFolders.Add(mf);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("XML configuration error.");
                throw e;
            }
            

            //XmlNode rootNode = reader.g
        }
    }
}
