using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.SharePoint.Client;
using System.IO;

namespace SharepointOnline
{
    public class SPOLib
    {
        public ConfigParser configParser;
        public ClientContext clientContext;
        public User currentUser;
        public string webUrl;

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
                webUrl = clientContext.Url;
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

        public void ReNewClientContext(string url)
        {
            BaseAuthentication authentication = Authenticate(url);
            clientContext = authentication.clientContext;
        }

        private BaseAuthentication Authenticate(string url)
        {
            try
            {
                BaseAuthentication authentication = CreateAuthentication(url);
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

        private BaseAuthentication CreateAuthentication(string url)
        {
            string targetWebAppLocation = url;
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

        public List CreateDocumentLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "documentlibrary", libraryName);
        }

        public List CreateContactLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "contacts", libraryName);
        }

        public List CreateContactLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "contacts", libraryName);
        }

        public List CreateAnnouncementLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "announcements", libraryName);
        }

        public List CreateAnnouncementLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "announcements", libraryName);
        }

        public List CreateTaskLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "tasks", libraryName);
        }

        public List CreateTaskLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "tasks", libraryName);
        }

        public List CreateLinkLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "links", libraryName);
        }

        public List CreateLinkLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "links", libraryName);
        }

        public List CreateDiscussionBoardLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "discussionboards", libraryName);
        }

        public List CreateDiscussionBoardLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "discussionboards", libraryName);
        }

        public List CreateEventLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "events", libraryName);
        }

        public List CreateEventLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "events", libraryName);
        }

        public List CreatePictureLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "pictures", libraryName);
        }

        public List CreatePictureLib(Web targetWeb, string libraryName)
        {
            return SPClientOMHelper.CreateLibraryByType(clientContext, targetWeb, "pictures", libraryName);
        }

        public void DeleteLib(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            SPClientOMHelper.DeleteLibrary(clientContext, targetWeb, libraryName);
        }

        /// <summary>
        /// Here, can't use the client object to add the attache, so call the webservice
        /// </summary>
        public void AddAttachment(string serviceURL, string listName, string attachedFilePath, string attachedItemID, string attachmentName)
        {
            byte[] contents = null;
            using (FileStream oFileStream = new FileStream(attachedFilePath, FileMode.Open, FileAccess.Read))
            {
                contents = new byte[oFileStream.Length];
                oFileStream.Read(contents, 0, (int)oFileStream.Length);
                oFileStream.Close();
            }

            ServiceReference.Lists listService = new ServiceReference.Lists();
            
            if (!string.IsNullOrEmpty(serviceURL))
            {
                listService.Url = serviceURL;
            }

            listService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            string addAttach = listService.AddAttachment(listName, attachedItemID, attachmentName, contents);
        }

        /// <summary>
        /// Here, can't use the client object to add the attache, so call the webservice
        /// </summary>
        public void AddAttachment(string listName, string attachedFilePath, string attachedItemID, string attachmentName)
        {
            AddAttachment(null, listName, attachedFilePath, attachedItemID, attachmentName);
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

        /// <summary>
        /// upload file greater than 5M
        /// </summary>
        public int UploadBigFile(string siteTitle, string libraryName, string itemTitle, string testDataPath, string fileName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.UploadBigDocLibItem(clientContext, targetWeb, libraryName, itemTitle, testDataPath, fileName, "", "");
        }

        /// <summary>
        /// download file greater than 5M
        /// </summary>
        public Stream DownloadBigFile(string siteTitle, string libraryName, string fileName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.DownloadBigDocLibItem(clientContext, targetWeb, libraryName, fileName);
        }

        /// <summary>
        /// download a small file
        /// </summary>
        public Stream DownloadFile(string siteTitle, string libraryName, int itemId)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.DownloadDocLibItem(clientContext, targetWeb, libraryName, itemId);
        }

        public bool CheckItemExists(string siteTitle, string libraryName, int itemId)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.CheckItemExists(clientContext, targetWeb, libraryName, itemId);
        }

        public int UploadFile(string siteTitle, string libraryName, string itemTitle, string testDataPath, string fileName, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.UploadDocLibItem(clientContext, targetWeb, libraryName, itemTitle, testDataPath, fileName, created, modified);
        }
        
        public int UploadFile(string siteTitle, string libraryName, string itemTitle, string testDataPath, string fileName, string folderName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.UploadDocLibItem(clientContext, targetWeb, libraryName, itemTitle, testDataPath, fileName, folderName);
        }

        public int AddFolder(string siteTitle, string libraryName, string folderName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddFolder(clientContext, targetWeb, libraryName, folderName);
        }

        public int AddSubFolder(string siteTitle, string libraryName, string parentFolderName, string folderName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddSubFolder(clientContext, targetWeb, libraryName, parentFolderName, folderName);
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

        public int AddTaskItem(string siteTitle, string libraryName, string title, string folderURL)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddTaskItem(clientContext, targetWeb, libraryName, title, folderURL);
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

        public int AddPostItem(string siteTitle, string libraryName, string title, string body, string categoryID, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddPostItem(clientContext, targetWeb, libraryName, title, body, categoryID, created, modified);
        }

        public int AddCommentItem(string siteTitle, string libraryName, string title, string body, string parentPostID, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddCommentItem(clientContext, targetWeb, libraryName, title, body, parentPostID, created, modified);
        }

        public int AddDocumentSetItem(string siteTitle, string libraryName, string title, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddDocumentSetItem(clientContext, targetWeb, libraryName, title, created, modified);
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

        public ListItemCollection RetrieveItems(string siteTitle, string libraryName, string camlFile)
        {
            Web targetWeb = FindTargetWeb(siteTitle, true);
            CamlQuery query = new CamlQuery();
            query.ViewXml = System.IO.File.ReadAllText(camlFile);

            return SPClientOMHelper.RetrieveItems(clientContext, targetWeb, libraryName, query);
        }

        public Boolean ValidateRestore(string siteTitle, string listTitle, string camlFile, int count)
        {
            ListItemCollection items = RetrieveItems(siteTitle, listTitle, camlFile);
            int index = 0;
            foreach (ListItem oListItem in items)
            {
                index++;
            }
            return (index == count);
        }

        public Boolean ValidateRestoreNewVersion(string siteTitle, string listTitle, string camlFile, Hashtable resultItems)
        {
            ListItemCollection items = RetrieveItems(siteTitle, listTitle, camlFile);
            bool allSet = false;
            if (items.Count > 0)
            {
                allSet = true;
            }
            foreach (ListItem oListItem in items)
            {
                clientContext.Load(oListItem);
                clientContext.ExecuteQuery();
                if (resultItems[oListItem.Id] != null)      //Only check those items that have been passed into
                {
                    string latestVersion = oListItem["_UIVersionString"].ToString();
                    if (!latestVersion.Equals(resultItems[oListItem.Id].ToString()))
                    {
                        allSet = false;
                        break;
                    }
                }
            }
            return allSet;
        }

        public string GetGUID(string siteTitle, string libraryName, int itemID)
        {
            try
            {
                Web targetWeb = FindTargetWeb(siteTitle);
                string guid = SPClientOMHelper.GetGUID(clientContext, targetWeb, libraryName, itemID);
                return guid;
            }
            catch
            {
                Console.WriteLine("No Such Item can be deleted!");
                return "";
            }
        }

        public int GetItemCount(string siteTitle, string libraryName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.GetListItemCount(clientContext, targetWeb, libraryName);
        }

        public void EnableListVersion(string siteTitle, string listName)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            SPClientOMHelper.EnableListVersion(clientContext, targetWeb, listName);
        }

        public void UpdateItem(string siteTitle, string listName, int itemID, Hashtable fieldValueCollection)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            SPClientOMHelper.UpdateItem(clientContext, targetWeb, listName, itemID, fieldValueCollection);
        }

        public Web FindTargetWeb(string siteTitle)
        {
            return FindTargetWeb(siteTitle, false);
        }

        public Web FindTargetWeb(string siteTitle, bool refresh)
        {
            Web targetWeb = null;
            if (refresh)
            {
                BaseAuthentication authentication = Authenticate(configParser, currentUser);
                clientContext = authentication.clientContext;
            }
            targetWeb = clientContext.Web;
            clientContext.Load(targetWeb);
            clientContext.ExecuteQuery();
            if (!string.Equals(siteTitle, "root", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.Equals(targetWeb.Title, siteTitle, StringComparison.InvariantCultureIgnoreCase))
                {
                    targetWeb = SPClientOMHelper.GetSubWeb(clientContext, siteTitle);
                }
            }
            
            return targetWeb;
        }

        public void DeleteWeb(string webTitle)
        {
            SPClientOMHelper.DeleteSubWeb(clientContext, webTitle);
        }

        public void DeleteWeb(Web targetWeb)
        {
            SPClientOMHelper.DeleteSubWeb(clientContext, targetWeb);
        }

        public Web CreateWeb(string webTitle, string webTemplate)
        {
            return SPClientOMHelper.CreateSubWeb(clientContext, webTitle, webTemplate);
        }

        public Web CreateWeb(Web parentWeb, string webTitle, string webTemplate)
        {
            return SPClientOMHelper.CreateSubWeb(clientContext, parentWeb, webTitle, webTemplate);
        }

        /// <summary>
        /// create folder level by level
        /// </summary>
        /// <param name="siteTitle"></param>
        /// <param name="libraryName"></param>
        /// <param name="folderServerRelativeUrl">folder1/subfolder1/subsubfolder1</param>
        public void CreateFolderLevelByLevel(string siteTitle, string libraryName, string folderServerRelativeUrl)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            SPClientOMHelper.EnsureFolderExist(clientContext, targetWeb, libraryName, folderServerRelativeUrl);
        }

        /// <summary>
        /// upload file for folder
        /// </summary>
        /// <param name="siteTitle"></param>
        /// <param name="libraryName"></param>
        /// <param name="itemTitle"></param>
        /// <param name="targetFolderUrl">folder1/subfolder1/subsubfolder1</param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public int AddFileForFolder(string siteTitle, string libraryName, string itemTitle, string targetFolderUrl, string filePath, string fileName, string created, string modified)
        {
            Web targetWeb = FindTargetWeb(siteTitle);
            return SPClientOMHelper.AddFileForFolder(clientContext, targetWeb, libraryName, itemTitle, targetFolderUrl, filePath, fileName, created, modified);
            
        }
    }
}
