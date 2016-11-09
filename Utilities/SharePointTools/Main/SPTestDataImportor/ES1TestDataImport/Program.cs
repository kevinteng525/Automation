using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPUtilities.Extensions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.IO;

namespace ES1TestDataImport
{
    class Program
    {
        public static ConfigParser cp = new ConfigParser(@"..\..\Config.xml");
        public static StreamWriter log = new StreamWriter("TestData.tsv");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801")]
        static void Main(string[] args)
        {
            SPImporter.GetAllFiles(@"..\..\files");
            log.AutoFlush = true;
            log.WriteLine("Item ID\tFile Name\tList Title\tFile Extension\tFile Size\tCreated Date\tModified Date");
            CreateWebApps();
            
            Console.WriteLine("All Done! Have fun!");
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        public static void CreateWebApps()
        {
            int WebAppCount = cp.WebAppNodeValueCol.Count;
            for (int i = 0; i < WebAppCount; i++)
            {
                string port = cp.WebAppNodeValueCol.GetKey(i).ToLower();
                string type = cp.WebAppNodeValueCol.GetValues(i)[0].ToLower();
                if (type == "create")
                {
                    Console.WriteLine("Create WebApp " + port + "...");
                    SPWebApplication webApplication = SPFarm.Local.RemoveContentWebApplication(int.Parse(port)).CreateContentWebApplication(int.Parse(port));              
                    Console.WriteLine("Create Application successfully!");
                    CreateSites(webApplication,port);
                }
                else
                {
                    Console.WriteLine("Open WebApp " + port + "...");
                    SPWebApplication webApplication = SPFarm.Local.GetContentWebApplication(int.Parse(port));
                    Console.WriteLine("Open Application successfully!");
                    CreateSites(webApplication,port);
                }
            }
        }

        public static void CreateSites(SPWebApplication webApp, string port)
        {
            int SiteCount = cp.SiteNodeValueCol(port).Count;
            for (int i = 0; i < SiteCount; i++)
            {
                string name = cp.SiteNodeValueCol(port).GetKey(i).ToLower();
                string type = cp.SiteNodeValueCol(port).GetValues(i)[0].ToLower();
                if (type == "create")
                {
                    Console.WriteLine("Create a Site " + name);
                    SPWeb web = webApp.RemoveSite("/sites/" + name, false)
                    .CreateSite("/sites/" + name, false, name).RootWeb;
                    Console.WriteLine("Create Site successfully!");
                    CreateLists(web,port, name);
                }
                else
                {
                    Console.WriteLine("Open a Site " + name);
                    SPWeb web = webApp.GetSite("/sites/" + name,false).RootWeb;
                 
                    Console.WriteLine("Open Site successfully!");
                    CreateLists(web,port, name);
                }
            }
        }

        public static void CreateLists(SPWeb web, string port, string siteName)
        {
            int ListCount = cp.ListNodeValueCol(port, siteName).Count;
            for (int i = 0; i < ListCount; i++)
            {
                string name = cp.ListNodeValueCol(port, siteName).GetKey(i).ToLower();
                string type = cp.ListNodeValueCol(port, siteName).GetValues(i)[0].ToLower();
                if (type == "documentlibrary")
                {
                    Console.WriteLine("Create a DocumentLibrary " + name);
                    SPList docLib = web.CreateDocumentLibrary(name);
                    Console.WriteLine("Create DocumentLibrary successfully!");
                    CreateFiles(docLib, port, siteName, name);
                }
                else
                {
                    Console.WriteLine("Create a PictureLibrary " + name);
                    SPList picLib = web.CreatePictureLibrary(name);
                    Console.WriteLine("Create PictureLibrary successfully!");
                    CreateFiles(picLib,port, siteName, name);
                }
            }
        }

        
        public static void CreateFiles(SPList list, string port, string siteName, string listName)
        {
            int ListCount = cp.FileNodeValueCol(port, siteName, listName).Count;
            for (int i = 0; i < ListCount; i++)
            {
                string date = cp.FileNodeValueCol(port, siteName, listName).GetKey(i).ToLower();
                string type = cp.FileNodeValueCol(port, siteName, listName).GetValues(i)[0].ToLower();
                if (type == "year")
                {
                    Console.WriteLine("Add a year " + date);
                    int year = int.Parse(date.Split(',')[0]);
                    SPImporter.AddYear(log, list, list.Title, year);
                }
                else
                {
                    Console.WriteLine("Add a month " + date);
                    int year = int.Parse(date.Split(',')[0]);
                    int month = int.Parse(date.Split(',')[1]);
                    SPImporter.AddMonth(log, list, list.Title, year,month);
                }
            }
        }

    }
}
