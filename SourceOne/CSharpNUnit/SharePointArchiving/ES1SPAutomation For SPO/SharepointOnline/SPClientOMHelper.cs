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
        public static Web CreateSubWeb(ClientContext clientContext, string webTitle)
        {
            Web rootWeb = clientContext.Web;
            clientContext.Load(rootWeb);
            WebCreationInformation webCreateInfo = new WebCreationInformation();
            webCreateInfo.Title = webTitle;

            webCreateInfo.UseSamePermissionsAsParentSite = true;
            webCreateInfo.Url = HttpUtility.UrlEncode(webTitle);
            rootWeb = rootWeb.Webs.Add(webCreateInfo);
            clientContext.ExecuteQuery();
            return rootWeb;
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
            clientContext.Load(rootWeb);
            var query = clientContext.LoadQuery(rootWeb.Webs.Where(w => w.Title == webTitle));
            clientContext.ExecuteQuery();
            return query.FirstOrDefault();
        }

        public static List CreateLibraryByType(ClientContext clientContext, Web web, string type, string listTitle)
        {
            type = type.ToLower();

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
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffur = new byte[fs.Length];
            fs.Read(buffur, 0, (int)fs.Length);
            return buffur;
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
    }
}
