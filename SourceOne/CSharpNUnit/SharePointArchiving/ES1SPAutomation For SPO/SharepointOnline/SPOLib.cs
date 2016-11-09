using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.SharePoint.Client;

namespace SharepointOnline
{
    public class SPOLib
    {
        public ConfigParser configParser;
        public ClientContext clientContext;
        private User currentUser;

        public SPOLib(string configPath)
        {
            try
            {
                configParser = new ConfigParser(configPath);
                currentUser = new User()
                {
                    UserName = configParser.GetNodeAttibuteValue(@"//User", "Name", "Password").GetKey(0).Trim(),
                    Password = configParser.GetNodeAttibuteValue(@"//User", "Name", "Password").GetValues(0)[0].Trim()
                };

                BaseAuthentication authentication = Authenticate(configParser, currentUser);
                clientContext = authentication.clientContext;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--->" + ex.StackTrace);
            }
        }

        private static BaseAuthentication Authenticate(ConfigParser configParser, User currentUser)
        {
            try
            {
                BaseAuthentication authentication = CreateAuthentication(configParser, currentUser);
                authentication.Authenticate();
                Web web = authentication.clientContext.Web;
                authentication.clientContext.Load(web);
                authentication.clientContext.ExecuteQuery();
                return authentication;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static BaseAuthentication CreateAuthentication(ConfigParser configParser, User currentUser)
        {
            string targetWebAppLocation = configParser.GetSingleNodeAttributeValue("WebApp", "URL");
            NameValueCollection metaHandlerInfo = configParser.GetNodeAttibuteValue(@"//Identity", "Type", "Model");
            Type t = Type.GetType(metaHandlerInfo.GetValues(0)[0].Trim());
            BaseAuthentication authentication = System.Activator.CreateInstance(t, new object[] { currentUser, targetWebAppLocation }) as BaseAuthentication;
            return authentication;
        }

        public List CreateDocumentLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "documentlibrary", libraryName);
        }

        public List CreateContactLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "contacts", libraryName);
        }

        public List CreateAnnouncementLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "announcements", libraryName);
        }

        public List CreateTaskLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "tasks", libraryName);
        }

        public List CreateLinkLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "links", libraryName);
        }

        public List CreateDiscussionBoardLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "discussionboards", libraryName);
        }

        public List CreateEventLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "events", libraryName);
        }

        public List CreatePictureLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "pictures", libraryName);
        }

        public void DeleteLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            SPClientOMHelper.DeleteLibrary(clientContext, targetWeb, libraryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteTitle">If "root", just upload on root Web</param>
        /// <param name="libraryName"></param>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int UploadFile(string siteTitle, string libraryName, string itemTitle, string testDataPath, string fileName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.UploadDocLibItem(clientContext, targetWeb, libraryName, itemTitle, testDataPath, fileName,"","");
        }

        public int UploadFile(string siteTitle, string libraryName, string itemTitle, string testDataPath, string fileName, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.UploadDocLibItem(clientContext, targetWeb, libraryName, itemTitle, testDataPath, fileName, created, modified);
        }

        public int AddAnnouncementItem(string siteTitle, string libraryName, string title, string body)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddAnnouncementItem(clientContext, targetWeb, libraryName, title, body,"","");
        }

        public int AddAnnouncementItem(string siteTitle, string libraryName, string title, string body, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddAnnouncementItem(clientContext, targetWeb, libraryName, title, body, created, modified);
        }

        public int AddTaskItem(string siteTitle, string libraryName, string title)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddTaskItem(clientContext, targetWeb, libraryName, title, "", "");
        }

        public int AddTaskItem(string siteTitle, string libraryName, string title, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddTaskItem(clientContext, targetWeb, libraryName, title, created, modified);
        }

        public int AddLinkItem(string siteTitle, string libraryName, string url)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddLinkItem(clientContext, targetWeb, libraryName, url, "", "");
        }

        public int AddLinkItem(string siteTitle, string libraryName, string url, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddLinkItem(clientContext, targetWeb, libraryName, url, created, modified);
        }

        public int AddDiscussionBoardItem(string siteTitle, string libraryName, string subject, string body)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddDiscussionBoardItem(clientContext, targetWeb, libraryName, subject, body, "", "");
        }

        public int AddDiscussionBoardItem(string siteTitle, string libraryName, string subject, string body, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddDiscussionBoardItem(clientContext, targetWeb, libraryName, subject, body, created, modified);
        }

        public int AddContactItem(string siteTitle, string libraryName, string lastname)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddContactItem(clientContext, targetWeb, libraryName, lastname, "", "");
        }

        public int AddContactItem(string siteTitle, string libraryName, string lastname, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddContactItem(clientContext, targetWeb, libraryName, lastname, created, modified);
        }

        public int AddEventItem(string siteTitle, string libraryName, string title)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddEventItem(clientContext, targetWeb, libraryName, title, "", "");
        }

        public int AddEventItem(string siteTitle, string libraryName, string title, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddEventItem(clientContext, targetWeb, libraryName, title, created, modified);
        }

        public int ModifyItem(string siteTitle, string libraryName, int itemID)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.ModifyItem(clientContext, targetWeb, libraryName, itemID);
        }

        public void DeleteItem(string siteTitle, string libraryName, int itemID)
        {
            try
            {
                Web targetWeb = FindTargetWeb(siteTitle);
                SPClientOMHelper.DeleteItem(clientContext, targetWeb, libraryName, itemID);
            }
            catch
            {
                Console.WriteLine("No Such Item can be deleted!");
            }
        }

        public int GetItemCount(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.GetListItemCount(clientContext, targetWeb, libraryName);
        }

        private Web FindTargetWeb(string siteTitle)
        {
            Web targetWeb = null;
            if (siteTitle.ToLower() == "root")
            {
                targetWeb = clientContext.Web;
            }
            else
            {
                targetWeb = clientContext.Web;
                if (targetWeb.Title != siteTitle)
                {
                    targetWeb = SPClientOMHelper.GetSubWeb(clientContext, siteTitle);
                }
            }
            return targetWeb;
        }
    }
}
