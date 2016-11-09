using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;

using Microsoft.SharePoint.Client;

namespace ES1OnlineTestDataImport
{
    class FileUploadImp
    {
        private ClientContext _clientContext;
        private Dictionary<string, byte[]> _dict = new Dictionary<string, byte[]>();
        private ConfigParser _cp;
        private StreamWriter _log; 
        private Action<Web> onSuccessWebCreated;
        private Action<Web, List> onSuccessLibraryCreated;

        public FileUploadImp(ClientContext context, string uploadDirectoryPath, ConfigParser cp, StreamWriter log)
        {
            this._clientContext = context;
            GetAllFiles(uploadDirectoryPath);
            this._cp = cp;
            this._log = log;
            this.onSuccessWebCreated = this.CreateLibrary;
            this.onSuccessLibraryCreated = this.CreateFile;
        }
        #region set local uploading fold info 
        private void GetAllFiles(string dirPath)
        {
            DirectoryInfo DI = new DirectoryInfo(dirPath);
            FileInfo[] FIs = DI.GetFiles();
            string FileName;
            for (int i = 0; i < FIs.Length; i++)
            {
                FileName = FIs[i].Name;
                this._dict[FileName] = GetTemplate(FIs[i].FullName);
            }
        }
        private byte[] GetTemplate(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffur = new byte[fs.Length];
            fs.Read(buffur, 0, (int)fs.Length);
            return buffur;
        }
        #endregion

        public void CreateOrOpenWebs()
        {
            NameValueCollection siteNameValueColl = this._cp.GetNodeAttibuteValue(@"//Site", "Name", "OpenMode");
            for (int i = 0; i < siteNameValueColl.Count; i++)
            {
                Web currentWeb = null;
                string name = siteNameValueColl.GetKey(i);
                string type = siteNameValueColl.GetValues(i)[0].ToLower();
                if (type == "create")
                {
                    Console.WriteLine("Create a Site " + name);
                    SPClientOMHelper.DeleteWeb(this._clientContext, name);
                    currentWeb = SPClientOMHelper.CreateWeb(this._clientContext, name);
                    Console.WriteLine("Create Site \"" + currentWeb.Title + "\" successfully.");
                }
                else
                {
                    currentWeb = SPClientOMHelper.GetWebByTitle(this._clientContext, name);
                }

                onSuccessWebCreated(currentWeb);
            }
        }

        public void CreateLibrary(Web targetWeb)
        {
            NameValueCollection listNameValueColl = this._cp.GetNodeAttibuteValue("//Site[@Name='" + targetWeb.Title + "']/List", "Name", "Type");
            for (int i = 0; i < listNameValueColl.Count; i++)
            {
                string name = listNameValueColl.GetKey(i);
                string type = listNameValueColl.GetValues(i)[0].ToLower();
                Console.WriteLine("Create a " + type +" " + name);
                List targetList = SPClientOMHelper.CreateLibraryByType(this._clientContext, targetWeb, type, name);
                Console.WriteLine("Create a " + type + " " + name + " successfully.");
                onSuccessLibraryCreated(targetWeb, targetList);                
            }
        }

        public void CreateFile(Web targetWeb, List targetList)
        {
            NameValueCollection fileNameValueColl = this._cp.GetNodeAttibuteValue("//List[@Name='" + targetList.Title + "']/Files", "Date", "Type");
            for (int i = 0; i < fileNameValueColl.Count; i++)
            {
                string date = fileNameValueColl.GetKey(i).ToLower();
                string type = fileNameValueColl.GetValues(i)[0].ToLower();
                if (type == "year")
                {
                    Console.WriteLine("Add a year " + date);
                    int year = int.Parse(date.Split(',')[0]);
                    AddYear(targetWeb, targetList, targetList.Title, year);
                }
                else
                {
                    Console.WriteLine("Add a month " + date);
                    int year = int.Parse(date.Split(',')[0]);
                    int month = int.Parse(date.Split(',')[1]);
                    AddMonth(targetWeb, targetList, targetList.Title, year, month);
                }
            }
        }

        public void AddYear(Web web, List lists, string fileNames, int year)
        {
            for (int i = 1; i <= 12; i++)
            {
                AddMonth(web, lists, fileNames, year, i);
            }
        }

        public void AddMonth(Web web, List lists, string fileNames, int year, int month)
        {
            int Days = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= Days; i++)
            {
                try
                {
                    AddFile(web, this._log, lists, fileNames, year, month, i);
                }
                catch(Exception ex)
                {
                    this._log.WriteLine(ex.Message);
                    continue;
                }

            }
        }

        public void AddFile(Web web, StreamWriter log, List list, string fileName, int year, int month, int day)
        {
            string fileExt = SetFileType(day);
            string fileSize = SetFileSize(day);
            DateTime createdDate = new DateTime(year, month, day, (year + month + day) % 24, (year + month + day) % 60, (year * month * day) % 60);
            fileName = fileName + year.ToString("D4") + month.ToString("D2") + day.ToString("D2");
            DateTime modifiedDate = createdDate.AddDays(5);

            int UniqueId = SPClientOMHelper.UploadFile(this._clientContext, web, list.Title, fileName + "." + fileExt, this._dict[fileSize + "." + fileExt], createdDate, modifiedDate);
            log.WriteLine(UniqueId.ToString() + "\t" + fileName + "\t" + list.Title + "\t" + fileExt.ToString() + "\t" + fileSize.ToString() + "\t" + createdDate.ToString() + "\t" + modifiedDate.ToString());
        }

        private bool CheckFileExistInLibrary(List list, string fileName, string fileExt)
        {
            var files = list.RootFolder.Files;
            this._clientContext.Load(files);
            this._clientContext.ExecuteQuery();

            var queryFile = from file in files.ToList()
                            where file.Name == fileName + "." + fileExt
                            select file.Name;
            if (queryFile != null && queryFile.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string SetFileSize(int day)
        {
            string fileSize = String.Empty;
            switch (day % 6)
            {
                case 0:
                    fileSize = "K20";
                    break;
                case 1:
                    fileSize = "K50";
                    break;
                case 2:
                    fileSize = "K100";
                    break;
                case 3:
                    fileSize = "K200";
                    break;
                case 4:
                    fileSize = "K500";
                    break;
                case 5:
                    fileSize = "K1000";
                    break;
            }
            return fileSize;
        }

        private string SetFileType(int day)
        {
            string fileExt = String.Empty;
            switch (day % 5)
            {
                case 0:
                    fileExt = "doc";
                    break;
                case 1:
                    fileExt = "jpg";
                    break;
                case 2:
                    fileExt = "tif";
                    break;
                case 3:
                    fileExt = "xlsx";
                    break;
                case 4:
                    fileExt = "docx";
                    break;
            }
            return fileExt;
        }
    }
}
