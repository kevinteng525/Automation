using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Data.SqlClient;


namespace SPContactTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0} Begin to Gen the data...", DateTime.Now);
            GenerateTestingData(20, 2, 20, 10, 10, 10);
            DateTime endTime = DateTime.Now;
            Console.WriteLine("{0} End Gen the data.", DateTime.Now);


            //SPWebApplication webApplication = SPUtility.CreateContentWebApplication(SPFarm.Local, 65030, "ax2k8\administrator", "emcsiax@QA");
            //SPContentDatabase db = SPUtility.CreateContentDatabase(webApplication, "SQLForAuto", "mytestdatabase98765", null, null);
                   
        }

        public static void GenerateTestingData(int webApplicationNum, int contentDBNumForEachWebApplication, int siteCollectionNumberForEachDB, int subSiteNumber, int ListNumber, int itemNumber)
        {
            //Give the database info for the SharePoint DB, notice the username and password here should use SQL authentication.
            string strDatabaseServer = "100gsql2k8";
            string strDatabaseName = "TestDatabase";
            string strDatabaseUsername = "ax2k8\administrator";
            string strDatabasePassword = "emcsiax@QA";

            int basePortNumber = 65001;
            string baseSiteName = "TestSite";
            string baseSubSiteName = "TestSubSite";
            string baseListTitle = "TestList";
            string baseFileName = "TestFile";
            string testFilePath = "K20.docx";
            int currentWebApplicationPort;

            for (int i = 0; i < webApplicationNum; i++)
            {
                currentWebApplicationPort = basePortNumber + i;
                //Create a new web application.
                Console.WriteLine("Creating WebApplication{0}", i);
                SPUtility.DeleteContentWebApplication(currentWebApplicationPort);
                SPWebApplication webApplication = SPUtility.CreateContentWebApplication(SPFarm.Local, currentWebApplicationPort, strDatabaseUsername, strDatabasePassword);
                webApplication.ContentDatabases[0].Status = SPObjectStatus.Offline;
                for (int j = 0; j < contentDBNumForEachWebApplication; j++)
                {
                    //Create a new content database.
                    Console.WriteLine("Creating Content Database{0}", j);
                    //set the username and password to null when create the content db to use the application pool identity.
                    SPContentDatabase db = SPUtility.CreateContentDatabase(webApplication, strDatabaseServer, strDatabaseName + "_" + Guid.NewGuid(), null, null);
                    for (int k = 0; k < siteCollectionNumberForEachDB; k++)
                    {
                        //Create a new site collection.
                        Console.WriteLine("Creating site collection{0}", j + "_" + k);
                        SPSite siteCollection = SPUtility.CreateSite(webApplication, baseSiteName + j + "_" + k);
                        for (int m = 0; m < subSiteNumber; m++)
                        {
                            //Create a new site in site collection.
                            Console.WriteLine("Creating site{0}", k + "_" + m);
                            SPWeb site = SPUtility.CreateWeb(siteCollection, baseSubSiteName + k + "_" + m);
                            for (int n = 0; n < ListNumber; n++)
                            {
                                //Create a new list.
                                Console.WriteLine("Creating list{0}", n);
                                SPList list = SPUtility.CreateList(site, baseListTitle + n, SPListTemplateType.DocumentLibrary);
                                for (int p = 0; p < itemNumber; p++)
                                {
                                    //Create a new item in the list.
                                    Console.WriteLine("Creating file item{0}", p);
                                    SPItem item = SPUtility.CreateFile(list, baseFileName + p, testFilePath);
                                }
                            }
                        }
                    }
                    //Set the content db status to offline after use, and this make the new added sites into other new created dbs.
                    db.Status = SPObjectStatus.Offline;
                }
                webApplication.Update();
            }
        }
    }
}
