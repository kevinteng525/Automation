using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using System.IO;
using System.Data.SqlClient;

namespace SPContactTest
{
    public class SPUtility
    {
            
        #region Farm and WebApplication
        public static SPWebApplication GetContentWebApplication(int port)
        {
            return SPWebApplication.Lookup(new Uri("http://localhost:" + port));
        }

        public static SPWebApplication CreateContentWebApplication(SPFarm farm, int port, string username, string password)
        {
            SPWebApplication webApplication = null;

            if (GetContentWebApplication(port) == null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPWebApplicationBuilder builder = new SPWebApplicationBuilder(farm);
                    builder.Port = port;
                    builder.AutomaticallyResetDefaultsAfterCreation = true;
                    builder.ServerComment = "SharePoint" + " - " + port;
                    builder.CreateNewDatabase = true;
                    builder.UseNTLMExclusively = true;
                    builder.AllowAnonymousAccess = false;
                    builder.UseSecureSocketsLayer = false;

                    if (username == null || username == "")
                    {
                        builder.IdentityType = IdentityType.NetworkService;
                    }
                    else
                    {
                        builder.ApplicationPoolUsername = username;
                        System.Security.SecureString securePassword = new System.Security.SecureString();
                        char[] passCharArray = password.ToCharArray();
                        foreach (char c in passCharArray)
                        {
                            securePassword.AppendChar(c);
                        }
                        builder.ApplicationPoolPassword = securePassword;                        
                        builder.IdentityType = IdentityType.SpecificUser;
                    }


                    webApplication = builder.Create();
                    webApplication.Provision();                   

                });
            }
            else
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "WebApplication on port {0} already exists.", port));
            }

            return webApplication;
        }

        public static void DeleteContentWebApplication(int port)
        {
            SPWebApplication webApplication = GetContentWebApplication(port);
            if (webApplication != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    webApplication.Delete();
                    webApplication.Unprovision();
                });
            }
        }

        public static SPContentDatabase CreateContentDatabase(SPWebApplication webApplication, string strDatabaseServer, string strDatabaseName, string strDatabaseUsername, string strDatabasePassword)
        {
            SPContentDatabase db = null;
            db = webApplication.ContentDatabases.Add(strDatabaseServer, strDatabaseName, strDatabaseUsername, strDatabasePassword, 9000, 15000, 0);
            return db;            
        }

        
        #endregion

        #region SiteCollection
        public static readonly string SiteNamePrefix = "sites";

        public static SPSite GetSite(SPWebApplication webApplication, string siteName, bool needPrefix)
        {
            SPSite site = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                siteName = siteName.TrimStart(new char[] { '/' });

                if (needPrefix == true)
                    siteName = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", SiteNamePrefix, siteName);

                if (webApplication.Sites.Names.Contains(siteName, StringComparer.InvariantCultureIgnoreCase) == true)
                    site = webApplication.Sites[siteName];
            });

            return site;
        }

        public static SPSite GetSite(SPWebApplication webApplication, string siteName)
        {
            return GetSite(webApplication, siteName, true);
        }

        public static SPSite CreateSite(SPWebApplication webApplication, string siteName, bool needPrefix, string title)
        {
            siteName = siteName.TrimStart(new char[] { '/' });

            if (needPrefix == true)
                siteName = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", SiteNamePrefix, siteName);

            SPSite site = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                site = webApplication.Sites.Add(siteName, title, string.Empty, 1033, "STS#0",
                    System.Security.Principal.WindowsIdentity.GetCurrent().Name, System.Security.Principal.WindowsIdentity.GetCurrent().Name, string.Empty);
            });

            return site;
        }

        public static SPSite CreateSite(SPWebApplication webApplication, string siteName)
        {
            return CreateSite(webApplication, siteName, true, siteName);
        }

        public static void DeleteSite(SPWebApplication webApplication, string siteName, bool needPrefix)
        {
            SPSite site = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                site = GetSite(webApplication, siteName, needPrefix);
                if (site != null)
                    site.Delete();
            });
        }

        public static void DeleteSite(SPWebApplication webApplication, string siteName)
        {
            DeleteSite(webApplication, siteName, true);
        }

        public static SPWebApplication AddSite(SPWebApplication webApplication, string siteName, bool needPrefix, string title)
        {
            CreateSite(webApplication, siteName, needPrefix, title);
            return webApplication;
        }

        public static SPWebApplication AddSite(SPWebApplication webApplication, string siteName)
        {
            return AddSite(webApplication, siteName, true, siteName);
        }

        public static SPWebApplication RemoveSite(SPWebApplication webApplication, string siteName)
        {
            DeleteSite(webApplication, siteName);
            return webApplication;
        }

        public static SPWebApplication RemoveSite(SPWebApplication webApplication, string siteName, bool needPrefix)
        {
            DeleteSite(webApplication, siteName, needPrefix);
            return webApplication;
        }

        #endregion

        #region Site(Web)

        public static SPWeb GetWeb(SPSite site, string webName)
        {
            SPWeb web = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                web = site.RootWeb.Webs.Cast<SPWeb>().FirstOrDefault(w => w.Name.Equals(webName, StringComparison.OrdinalIgnoreCase) == true);
            });
            return web;
        }

        public static SPWeb CreateWeb(SPSite site, string webName)
        {
            SPWeb web = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                web = site.RootWeb.Webs.Add(webName, webName, string.Empty, site.RootWeb.Language, SPWebTemplate.WebTemplateSTS, false, false);
            });

            return web;
        }

        public static void DeleteWeb(SPSite site, string webName)
        {
            SPWeb web = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                web = GetWeb(site, webName);
                if (web != null)
                    web.Delete();
            });
        }

        public static SPSite AddWeb(SPSite site, string webName)
        {
            CreateWeb(site, webName);
            return site;
        }

        public static SPSite RemoveWeb(SPSite site, string webName)
        {
            DeleteWeb(site, webName);
            return site;
        }
        #endregion

        #region List
        public static SPList GetListByTitle(SPWeb web, string listTitle)
        {
            SPList list = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                list = web.Lists.Cast<SPList>().FirstOrDefault(ls => ls.Title.Equals(listTitle, StringComparison.OrdinalIgnoreCase) == true);
            });

            return list;
        }
        
        public static SPList CreateList(SPWeb web, string listTitle, SPListTemplateType templateType)
        {
            SPList list = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Guid listId = web.Lists.Add(listTitle, listTitle, templateType);
                list = web.Lists[listId];
            });

            return list;
        }
        public static void DeleteList(SPWeb web, string listTitle)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList list = GetListByTitle(web, listTitle);
                if (list != null)
                    list.Delete();
            });
        }
        #endregion

        #region Item

        public static SPListItem CreateFile(SPList list, string fileName, string filePath)
        {
            SPFile file = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                byte[] fileContent = File.ReadAllBytes(filePath);
                file = list.RootFolder.Files.Add(fileName, fileContent);
                file.Update();
            });

            return file.Item;
        }

        public static void DeleteFile(SPList list, string fileName)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (GetFile(list, fileName) != null)
                    list.RootFolder.Files.Delete(fileName);
            });
        }

        public static SPListItem GetFile(SPList list, string fileName)
        {
            SPFile file = list.RootFolder.Files.Cast<SPFile>().FirstOrDefault(f => string.Equals(f.Name, fileName, StringComparison.OrdinalIgnoreCase) == true);
            if (file != null)
                return file.Item;
            else
                return null;
        }
        #endregion
    }
}
