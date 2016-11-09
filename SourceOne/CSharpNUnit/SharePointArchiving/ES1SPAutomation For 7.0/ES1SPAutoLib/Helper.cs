﻿using System;
using System.Collections.Generic;
using System.Text;
//using EMC.Interop.ExConnector;
using EMC.Interop.ExProvider_2;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;
using ES1SPAutoLib;
using ES1SPAutoLib.Activity;
using System.IO;

namespace ES1.ES1SPAutoLib
{
    public class Helper
    {
        private static int count = 0;

        static void Main(string[] args)
        {
            Configuration.FillInValues("Configuration.xml");
            //ConfigES1();
     
            ES1Activity activity1 = ActivityFactory.CreateActivity("SPActivity.xml", ActivityTypes.Sharepoint);
            activity1.Create();
            //Console.WriteLine(activity1.IsActivityRunSuccess(0,600));
            //activity1.Delete();

            //DateTime date1;
            //bool res = DateTime.TryParse("2010-11-15 16:06:40", out date1);
            //System.Console.WriteLine(date1);

            //CreateActivity("SPOActivityTest1.xml");

            /*bool res = MappedFolderOperator.IsFolderExist("Good");
            res = MappedFolderOperator.IsFolderExist("MDAF1");
            res = MappedFolderOperator.IsFolderExist("mdaf1");
            System.Console.WriteLine("Good job.");*/
        }

        public static void ConfigES1()
        { 
            CreateConnection();
            CreateAFs();
            CreateMFs();
            ConfigNativeServer();
        }

        public static void CreateConnection()
        {
            ConnectionOperator.CreateArchiveConnection(Configuration.ArchiveConnection, Configuration.ConnDBServer, Configuration.ConnDBName);
        }

        public static void CreateAFs()
        {
            NativeArchiveOperator naService = new NativeArchiveOperator(Configuration.ArchiveConnection);
            foreach (ArchiveFolder af in Configuration.ArchiveFolders)
            {
                naService.CreateArchiveFolder(af);
            }
        }

       public static void CreateMFs()
        {
            foreach (MappedFolder mf in Configuration.MappedFolders)
            {
                MappedFolderOperator.CreateMappedFolder(mf);
            }
        }

        public static void ConfigNativeServer()
        {
            NativeServerOperator.DefaultConfigAllNativeServer(Configuration.ArchiveConnection, Configuration.MCLocation);
        }

        public static void WaitForIndex(string mcLocation)
        {
            WaitForIndex(mcLocation, 900);
        }

        public static void WaitForIndex(string mcLocation, int timeout)
        {
            DirectoryInfo unPack = new DirectoryInfo(mcLocation+"\\Unpack_Area");
            int time = 0;
            count = 0;
            System.Console.WriteLine("Start waiting...");
            int fileCount = GetAllFiles(unPack);
            while(fileCount!=0)
            {
                System.Threading.Thread.Sleep(1000);
                count = 0;  //reset count before get files
                fileCount = GetAllFiles(unPack);
                System.Console.WriteLine(time.ToString() + ":" + fileCount.ToString());
                time++;
                if (time > timeout)
                {
                    break;
                }
            }
        }

        private static int GetAllFiles(DirectoryInfo dir)
        {
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    GetAllFiles((DirectoryInfo)i);
                }
                else if(!i.Attributes.ToString().Contains("Hidden"))    //only count those un hidden files
                {
                    count++;
                }
            }
            return count;
        }
    }
}