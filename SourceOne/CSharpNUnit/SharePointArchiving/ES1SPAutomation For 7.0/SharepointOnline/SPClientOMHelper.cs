using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Microsoft.SharePoint.Client;
using System.IO;


namespace SharepointOnline
{
    public class SPClientOMHelper
    {
        public static Web CreateSubWeb(ClientContext clientContext, Web parentWeb, string webTitle, string webTemplate)
        {
            clientContext.Load(parentWeb);
            WebCreationInformation webCreateInfo = new WebCreationInformation();
            webCreateInfo.WebTemplate = webTemplate;
            webCreateInfo.Title = webTitle;

            webCreateInfo.UseSamePermissionsAsParentSite = true;
            webCreateInfo.Url = HttpUtility.UrlEncode(webTitle);
            Web subWeb = parentWeb.Webs.Add(webCreateInfo);
            clientContext.ExecuteQuery();
            clientContext.Load(subWeb);
            clientContext.ExecuteQuery();
            return subWeb;
        }

        public static Web CreateSubWeb(ClientContext clientContext, string webTitle, string webTemplate)
        {
            Web rootWeb = clientContext.Web;
            return CreateSubWeb(clientContext, rootWeb, webTitle, webTemplate);
        }

        public static void DeleteSubWeb(ClientContext clientContext, Web targetWeb)
        {
            Web parentWeb = clientContext.Web;
            clientContext.Load(parentWeb);
            clientContext.Load(targetWeb);
            targetWeb.DeleteObject();
            parentWeb.Update();
            clientContext.Load(parentWeb);
            clientContext.ExecuteQuery();
        }

        public static void DeleteSubWeb(ClientContext clientContext, string webTitle)
        {
            Web rootWeb = clientContext.Web;
            clientContext.Load(rootWeb);
            var queryWeb = clientContext.LoadQuery(rootWeb.Webs.Where(w => w.Title == webTitle));
            clientContext.ExecuteQuery();
            if (null != queryWeb && null != queryWeb.FirstOrDefault())
                queryWeb.FirstOrDefault().DeleteObject();
            rootWeb.Update();
            clientContext.Load(rootWeb);
            clientContext.ExecuteQuery();
        }

        public static Web GetSubWeb(ClientContext clientContext, string webTitle)
        {
            Web rootWeb = clientContext.Web;

            return GetSubWeb(clientContext, rootWeb, webTitle);
        }

        public static Web GetSubWeb(ClientContext clientContext, Web parentWeb, string webTitle)
        {
            clientContext.Load(parentWeb);
            clientContext.Load(parentWeb.Webs);
            clientContext.ExecuteQuery();
            foreach (Web subWeb in parentWeb.Webs)
            {
                clientContext.Load(subWeb);
                
                clientContext.ExecuteQuery();
                if (subWeb.Title == webTitle)
                    return subWeb;
            }
            return null;
        }

        public static void EnableListVersion(ClientContext clientContext, Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQuery();

            List list = lists.GetByTitle(listTitle);
            list.EnableVersioning = true;
            list.Update();
            clientContext.Load(list);
            clientContext.ExecuteQuery();
        }

        public static List CreateLibraryByType(ClientContext clientContext, Web web, string type, string listTitle)
        {
            type = type.ToLower();
            clientContext.Load(web, w=>w.Lists);         
            clientContext.ExecuteQuery();

            ListCollection lists = web.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQuery();

            var queryList = from pList in lists.ToList()
                            where pList.Title == listTitle
                            select pList;
            var previousList = queryList.ToList<List>();
            if (null != previousList && previousList.Count > 0)
                return previousList[0];
            ListCreationInformation listCreationInfo = new ListCreationInformation();
            listCreationInfo.Title = listTitle;
            switch (type.ToLower())
            {
                case "documentlibrary":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.DocumentLibrary);
                    break;
                case "picuturelibrary":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.PictureLibrary);
                    break;
                case "contacts":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.Contacts);
                    break;
                case "announcements":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.Announcements);
                    break;
                case "tasks":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.Tasks);
                    break;
                case "links":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.Links);
                    break;
                case "discussionboards":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.DiscussionBoard);
                    break;
                case "events":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.Events);
                    break;
                case "pictures":
                    listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.PictureLibrary);
                    break;
            }

            List newList = lists.Add(listCreationInfo);
            clientContext.Load(newList);
            clientContext.ExecuteQuery();
            return newList;
        }

        public static void DeleteLibrary(ClientContext clientContext, Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQuery();
            List deletedList = lists.GetByTitle(listTitle);
            if (deletedList == null)
                throw new Exception("Can not find list: " + listTitle);
            deletedList.DeleteObject();
            web.Update();
            clientContext.Load(web);
            clientContext.ExecuteQuery();
        }

        private static byte[] GetContent(string filePath)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        private static Folder GetSubFolder(ClientContext clientContext, Folder parentFolder, string name)
        {
            clientContext.Load(parentFolder);
            clientContext.ExecuteQuery();
            clientContext.Load(parentFolder.Folders);
            clientContext.ExecuteQuery();
            foreach (Folder subFolder in parentFolder.Folders)
            {
                if (subFolder.Name == name)
                    return subFolder;
            }
            return null;
        }

        public static int UploadDocLibItem(ClientContext clientContext, Web targetWeb, string libraryName, string itemTitle, string testDataPath, String fileName, string folderName)
        {
            FileCreationInformation newFileInfo = new FileCreationInformation();
            newFileInfo.Content = GetContent(testDataPath + fileName);
            newFileInfo.Url = fileName;
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");
            Folder folder = GetSubFolder(clientContext, docLibrary.RootFolder, folderName);

            Microsoft.SharePoint.Client.File uploadedFile = folder.Files.Add(newFileInfo);
            
            clientContext.Load(uploadedFile);
            clientContext.ExecuteQuery();

            ListItem item = uploadedFile.ListItemAllFields;
            item["Title"] = itemTitle;
   
            item.Update();

            clientContext.Load(item);
            clientContext.ExecuteQuery();

            return item.Id;
        }

        public static int UploadDocLibItem(ClientContext clientContext, Web targetWeb, string libraryName, string itemTitle, string testDataPath, String fileName, string created, string modified)
        {
            FileCreationInformation newFileInfo = new FileCreationInformation();
            newFileInfo.Content = GetContent(testDataPath + fileName);
            newFileInfo.Url = fileName;
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");
            Microsoft.SharePoint.Client.File uploadedFile = docLibrary.RootFolder.Files.Add(newFileInfo);
            clientContext.Load(uploadedFile);
            clientContext.ExecuteQuery();

            ListItem item = uploadedFile.ListItemAllFields;
            item["Title"] = itemTitle;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
           
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            
            return item.Id;
        }

        public static int UploadBigDocLibItem(ClientContext clientContext, Web targetWeb, string libraryName, string itemTitle, string testDataPath, string fileName, string created, string modified)
        {
            List docLib = targetWeb.Lists.GetByTitle(libraryName);
            Folder docLibFolder = docLib.RootFolder;
            clientContext.Load(docLibFolder, f => f.ServerRelativeUrl);
            //clientContext.Load(docLib);
            clientContext.ExecuteQuery();

            if (docLib == null)
                throw new Exception("Can not find this list!");

            string filePath = testDataPath + fileName;
            string fileServerRelativeUrl = string.Format("{0}/{1}", docLibFolder.ServerRelativeUrl, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                clientContext.ExecuteQuery();
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileServerRelativeUrl, fs, true);
            }

            // query the item equals this size.
            FileInfo fi = new FileInfo(filePath);
            
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"
<View>
  <Query>
    <Where>
      <Eq>
        <FieldRef Name='File_x0020_Size' />
        <Value Type='Text'>{0}</Value>
      </Eq>
    </Where>
  </Query>
  <RowLimit>5</RowLimit>
</View>", fi.Length);
            ListItemCollection collListItem = docLib.GetItems(camlQuery);

            clientContext.Load(
                collListItem,
                items => items.Include(
                    item => item.File,
                    item => item.File.Name,
                    item => item.File.ServerRelativeUrl,
                    item => item.Id,
                    item => item.DisplayName));

            clientContext.ExecuteQuery();

            int itemId = -1;
            foreach (ListItem item in collListItem)
            {
                if (fileServerRelativeUrl == item.File.ServerRelativeUrl)
                {
                    itemId = item.Id;
                    break;
                }
            }

            return itemId;
        }

        public static Stream DownloadBigDocLibItem(ClientContext clientContext, Web targetWeb, string libraryName, string fileName)
        {
            List docLib = targetWeb.Lists.GetByTitle(libraryName);
            Folder docLibFolder = docLib.RootFolder;
            clientContext.Load(docLibFolder, f => f.ServerRelativeUrl);
            clientContext.ExecuteQuery();

            if (docLib == null)
                throw new Exception("Can not find this list!");

            string fileServerRelativeUrl = string.Format("{0}/{1}", docLibFolder.ServerRelativeUrl, fileName);
            
            FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, fileServerRelativeUrl);
            return fileInformation.Stream;
        }

        public static Stream DownloadDocLibItem(ClientContext clientContext, Web targetWeb, string libraryName, int itemId)
        {
            List docLib = targetWeb.Lists.GetByTitle(libraryName);
            ListItem item = docLib.GetItemById(itemId);
            clientContext.Load(item, i => i["FileRef"]);
            clientContext.ExecuteQuery();

            if (docLib == null)
                throw new Exception("Can not find this list!");

            FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, item["FileRef"].ToString());
            return fileInformation.Stream;
        }

        public static bool CheckItemExists(ClientContext clientContext, Web targetWeb, string libraryName, int itemId)
        {
            List docLib = targetWeb.Lists.GetByTitle(libraryName);
            ListItem item = docLib.GetItemById(itemId);
            Microsoft.SharePoint.Client.File file = item.File;

            try
            {
                clientContext.Load(item);
                clientContext.Load(file);
                clientContext.ExecuteQuery();

                if (docLib == null)
                    throw new Exception("Can not find this list!");

                return (item != null); // item exists
            }
            catch
            {
                return false;
            }
        }

        public static int AddFolder(ClientContext clientContext, Web targetWeb, string libraryName, string title)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            listItemCreationInformation.UnderlyingObjectType = FileSystemObjectType.Folder;
            listItemCreationInformation.LeafName = title;
            ListItem item = oList.AddItem(listItemCreationInformation);  
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddSubFolder(ClientContext clientContext, Web targetWeb, string libraryName, string parentFolderName, string title)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            listItemCreationInformation.UnderlyingObjectType = FileSystemObjectType.Folder;
            Folder parentFolder = GetSubFolder(clientContext, oList.RootFolder, parentFolderName);
            listItemCreationInformation.FolderUrl = parentFolder.ServerRelativeUrl;
            listItemCreationInformation.LeafName = title;
            ListItem item = oList.AddItem(listItemCreationInformation);
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        #region Add item into list.
        public static int AddAnnouncementItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string body, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = title;
            item["Body"] = body;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update(); 
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            
            return item.Id;
        }

        public static int AddTaskItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string folderUrl)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            listItemCreationInformation.FolderUrl = folderUrl;
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = title;
            
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddTaskItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = title;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddLinkItem(ClientContext clientContext, Web targetWeb, string libraryName, string url, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["URL"] = url;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddDiscussionBoardItem(ClientContext clientContext, Web targetWeb, string libraryName, string subject, string body, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = subject;
            item["Body"] = body;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddContactItem(ClientContext clientContext, Web targetWeb, string libraryName, string lastname, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = lastname;           //Modify "Title" later
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static int AddEventItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string created, string modified)
        {
            List oList = targetWeb.Lists.GetByTitle(libraryName);
            
            if (oList == null)
                throw new Exception("Can not find this list!");
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem item = oList.AddItem(listItemCreationInformation);
            item["Title"] = title;           
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static void AddListItemAttachment(ClientContext clientContext, string attachFilePath, string webName, string libraryName, int itemId)
        {
            FileStream fileStream = new FileStream(attachFilePath, FileMode.Open);
            string attachmentPath = "";
            if (webName == "")
            {
                attachmentPath = string.Format("/Lists/{0}/Attachments/{1}/{2}", libraryName, itemId, Path.GetFileName(attachFilePath));
            }
            else
            {
                attachmentPath = string.Format("/{0}/Lists/{1}/Attachments/{2}/{3}", webName, libraryName, itemId, Path.GetFileName(attachFilePath));
            }
            Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, attachmentPath, fileStream, true);

        }

        public static int AddPostItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string body, string categoryID, string created, string modified)
        {
            int itemId = 0;
            List postList = targetWeb.Lists.GetByTitle(libraryName);

            if (postList != null)
            {
                ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                ListItem item = postList.AddItem(listItemCreationInformation);
                item["Title"] = title;
                if (!string.IsNullOrEmpty(created))
                {
                    item["Created"] = created;
                }
                if (!string.IsNullOrEmpty(modified))
                {
                    item["Modified"] = modified;
                }
                item["Body"] = body;
                FieldLookupValue categoryLookupValue = new FieldLookupValue();
                categoryLookupValue.LookupId = Convert.ToInt32(categoryID);
                item["PostCategory"] = categoryLookupValue;
                item.Update();
                
                clientContext.Load(item);
                clientContext.ExecuteQuery();
                itemId = item.Id;
            }
            return itemId;
        }

        public static int AddCommentItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string body, string parentPostID, string created, string modified)
        {
            int itemId = 0;
            List commentList = targetWeb.Lists.GetByTitle(libraryName);
            if (commentList != null)
            {
                ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                ListItem item = commentList.AddItem(listItemCreationInformation);
                item["Title"] = title;
                if (!string.IsNullOrEmpty(created))
                {
                    item["Created"] = created;
                }
                if (!string.IsNullOrEmpty(modified))
                {
                    item["Modified"] = modified;
                }
                item["Body"] = body;
                FieldLookupValue postTitleLookupValue = new FieldLookupValue();
                postTitleLookupValue.LookupId = Convert.ToInt32(parentPostID);
                item["PostTitle"] = postTitleLookupValue;
                item.Update();
                clientContext.Load(item);
                clientContext.ExecuteQuery();
                itemId = item.Id;
            }
            return itemId;
        }

        public static int AddDocumentSetItem(ClientContext clientContext, Web targetWeb, string libraryName, string title, string created, string modified)
        {
            int itemId = 0;
            Web site = clientContext.Web;
            clientContext.Load(site);
            clientContext.ExecuteQuery();

            ContentTypeCollection contentTypes = site.ContentTypes;
            IEnumerable<ContentType> filteredTypes = clientContext.LoadQuery(contentTypes.Where(c => c.Name == "Document Set"));
            clientContext.ExecuteQuery();

            ContentTypeCreationInformation ctypeInfo = new ContentTypeCreationInformation();
            ctypeInfo.ParentContentType = filteredTypes.First();
            ctypeInfo.Group = "Custom Content Types";
            ctypeInfo.Name = "Document Set";

            List documentLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (documentLibrary != null)
            {
                ContentTypeCreationInformation documentSetContentType = new ContentTypeCreationInformation();
                documentLibrary.ContentTypes.Add(ctypeInfo);
                documentLibrary.ContentTypesEnabled = true;
                clientContext.Load(documentLibrary);
                clientContext.ExecuteQuery();

                ContentTypeCollection listContentTypes = documentLibrary.ContentTypes;
                clientContext.Load(listContentTypes, types => types.Include
                                                    (type => type.Id, type => type.Name,
                                                    type => type.Parent));

                var result = clientContext.LoadQuery(listContentTypes.Where
                    (c => c.Name == "Document Set"));

                clientContext.ExecuteQuery();

                if (null != result && null != result.FirstOrDefault())
                {
                    ContentType targetDocumentSetContentType = result.FirstOrDefault();

                    ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                    newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                    newItemInfo.LeafName = title;
                    ListItem newListItem = documentLibrary.AddItem(newItemInfo);

                    newListItem["ContentTypeId"] = targetDocumentSetContentType.Id.ToString();
                    newListItem["Title"] = title;
                    newListItem.Update();

                    clientContext.Load(newListItem);
                    clientContext.ExecuteQuery();
                    itemId = newListItem.Id;
                }
            }
            return itemId;
        }
        #endregion

        public static int ModifyItem(ClientContext clientContext, Web targetWeb, string libraryName, int id)
        {
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");
            ListItem item = docLibrary.GetItemById(id);
            
            item["Modified"] = DateTime.Now;

            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public static void DeleteItem(ClientContext clientContext, Web targetWeb, string libraryName, int id)
        {
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");
            ListItem item = docLibrary.GetItemById(id);
   
            item.DeleteObject();
            clientContext.ExecuteQuery();
        }

        public static ListItemCollection RetrieveItems(ClientContext clientContext, Web targetWeb, string libraryName, CamlQuery query)
        {

            List list = targetWeb.Lists.GetByTitle(libraryName);
            if (list == null)
                throw new Exception("Can not find this list!");
            ListItemCollection items = list.GetItems(query);
            clientContext.Load(items);
            clientContext.ExecuteQuery();
            return items;
        }

        public static string GetGUID(ClientContext clientContext, Web targetWeb, string libraryName, int id)
        {
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");
            ListItem item = docLibrary.GetItemById(id);
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            string guid = item.FieldValues["UniqueId"].ToString();
            return guid;
        }

        public static int GetListItemCount(ClientContext clientContext, Web targetWeb, string libraryName)
        {
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");

            clientContext.Load(docLibrary, List => List.ItemCount);
            clientContext.ExecuteQuery();
            int itemCount = docLibrary.ItemCount;
            return itemCount;
        }

        public static void UpdateItem(ClientContext clientContext, Web web, string listTitle, int itemID, System.Collections.Hashtable fieldValueCollection)
        {
            ListCollection lists = web.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQuery();

            List list = lists.GetByTitle(listTitle);
            clientContext.Load(list);
            clientContext.ExecuteQuery();

            ListItem item = list.GetItemById(itemID);
            foreach(string key in fieldValueCollection.Keys)
            {
                item[key] = fieldValueCollection[key];
            }

            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Check if the folder exists in the target list， if it does not， create the folders level by level.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listName"></param>
        /// <param name="folderServerRelativeUrl">folder1/subFolder1/subsubfolder1</param>
        public static void EnsureFolderExist(ClientContext clientContext, Web targetWeb, string listName, string folderServerRelativeUrl)
        {
            // Remove the last character ""/""  the string folderServerRelativeUrl.
            
            if(folderServerRelativeUrl.Length > 0 && folderServerRelativeUrl.Last().Equals("/"))
            {
                folderServerRelativeUrl = folderServerRelativeUrl.Substring(0, folderServerRelativeUrl.Length - 1);
            }

            folderServerRelativeUrl = listName + "/" + folderServerRelativeUrl;
            Folder folder = FindExistFolder(clientContext, targetWeb, listName, folderServerRelativeUrl);

            if(folder != null)
            {
                // Get the new folders path string part.
                string s = folderServerRelativeUrl.Replace(folder.ServerRelativeUrl, string.Empty);
                if(s.Length > 0 && s.First().Equals("/"))
                {
                    s = s.Substring(1, s.Length - 1);
                }

                string[] arr = s.Split('/');
                if(arr.Length > 0)
                {
                    string tmp = string.Empty;
                    // Create new folders level by level.
                    for(int i = 1; i < arr.Length; i++)
                    {
                        if(arr[i].Trim().Length > 0)
                        {
                            tmp += "/" + arr[i];
                            folder.Folders.Add(folder.ServerRelativeUrl + tmp);
                            clientContext.Load(folder);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find the exist folder in the given URL.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listName"></param>
        /// <param name="folderServerRelativeUrl"></param>
        /// <returns>Returns the existed SharePoint Folder object.</returns>
        private static Folder FindExistFolder(ClientContext clientContext, Web targetWeb, string listName, string folderServerRelativeUrl)
        {
            Folder folder = GetFolderInList(clientContext, targetWeb, listName, folderServerRelativeUrl);

            if(folder == null)
            {
                int iLastSlashPos = folderServerRelativeUrl.LastIndexOf("/");
                if(iLastSlashPos > 0)
                {
                    // if current folder does not exist， back to the parent folder.
                    string parentFolderUrl = folderServerRelativeUrl.Substring(0,iLastSlashPos);
                    return FindExistFolder(clientContext, targetWeb, listName, parentFolderUrl);
                }
            }
            return folder;
        }

        /// <summary>
        /// Get folder in the specific SharePoint List.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listName"></param>
        /// <param name="folderServerRelativeUrl"></param>
        /// <returns> If the folder does not exist in the specific SharePoint List return null， else return the folder object.</returns>
        public static Folder GetFolderInList(ClientContext clientContext, Web targetWeb, String listName, String folderServerRelativeUrl)
        {
            Folder existingFolder = null;
                         
            existingFolder = targetWeb.GetFolderByServerRelativeUrl(folderServerRelativeUrl);
            clientContext.Load(existingFolder);

            try
            {
                clientContext.ExecuteQuery();
            }
            catch 
            {
                existingFolder = null;
            }

            return existingFolder;
        }


        public static int AddFileForFolder(ClientContext clientContext, Web targetWeb, string libraryName, string itemTitle, string targetFolderUrl, string filePath, string fileName, string created, string modified)
        {
            FileCreationInformation newFileInfo = new FileCreationInformation();
            newFileInfo.Content = GetContent(filePath + fileName);
            newFileInfo.Url = fileName;
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            if (docLibrary == null)
                throw new Exception("Can not find this list!");

            Folder newFolder = targetWeb.GetFolderByServerRelativeUrl(libraryName + "/" + targetFolderUrl);
          
            Microsoft.SharePoint.Client.File uploadedFile = newFolder.Files.Add(newFileInfo);
            clientContext.Load(newFolder);
            clientContext.ExecuteQuery();
            ListItem item = uploadedFile.ListItemAllFields;
            item["Title"] = itemTitle;
            if (created != "")
            {
                item["Created"] = created;
            }
            if (modified != "")
            {
                item["Modified"] = modified;
            }
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();

            return item.Id;
   
        }
    }
}
