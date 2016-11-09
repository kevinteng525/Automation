using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Microsoft.SharePoint.Client;

namespace ES1OnlineTestDataImport
{
    public class SPClientOMHelper
    {
        public static Web CreateWeb(ClientContext clientContext, string webTitle)
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

        public static void DeleteWeb(ClientContext clientContext, string webTitle)
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

        public static Web GetWeb(ClientContext clientContext, string webTitle)
        { 
            Web rootWeb = clientContext.Web;
            clientContext.Load(rootWeb);
            var query = clientContext.LoadQuery(rootWeb.Webs.Where(w => w.Title == webTitle));
            clientContext.ExecuteQuery();
            return query.FirstOrDefault();
        }

        public static Web GetWebByTitle(ClientContext clientContext, string  webTitle)
        {
            Web rootWeb = clientContext.Web;
            clientContext.Load(rootWeb);
            var query = clientContext.LoadQuery(rootWeb.Webs.Where(w => w.Title == webTitle));
            clientContext.ExecuteQuery();
            return query.FirstOrDefault();
        }

        public static List CreateLibraryByType(ClientContext clientContext, Web web,  string type, string listTitle)
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

            if (type == "documentlibrary")
            {
                listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.DocumentLibrary); 
            }
            else
            {
                listCreationInfo.TemplateType = Convert.ToInt32(ListTemplateType.PictureLibrary);
            }

            List newList = lists.Add(listCreationInfo);
            clientContext.ExecuteQuery();
            return newList;
        }

        public static int UploadFile(ClientContext clientContext, Web targetWeb, string libraryName, string fileName, byte[] content, DateTime createdDate, DateTime modifiedDate)
        {
            FileCreationInformation newFileInfo = new FileCreationInformation();
            newFileInfo.Content = content;
            newFileInfo.Url = fileName;
            List docLibrary = targetWeb.Lists.GetByTitle(libraryName);
            File uploadedFile = docLibrary.RootFolder.Files.Add(newFileInfo);
            clientContext.Load(uploadedFile);
            clientContext.ExecuteQuery();
            
            ListItem item = uploadedFile.ListItemAllFields;
            item["Created"] = createdDate;
            item["Modified"] = modifiedDate;
            item.Update();
            clientContext.Load(item);
            clientContext.ExecuteQuery();
            return item.Id;
        }
    }
}
