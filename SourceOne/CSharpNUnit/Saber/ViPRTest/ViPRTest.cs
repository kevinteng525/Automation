using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;
using Saber.Common;
using Saber.TestEnvironment;
using Saber.TestData;
using Saber.TestData.EWS;
using Saber.S1CommonAPILib;
using Saber.BaseTest;
using System.Threading;



namespace ViPRTest
{

    [TestFixture]
    public class ViPRTest : TestClassBase
    {
        public String metaDataFolder = String.Empty;
        //private String archiveDBName = "";
        //private String dbServer = "";
        //private String workerName = "";
        private S1ArchiveConnection connection = null;

        public ViPRTest()
            : base()
        {

        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            metaDataFolder = "TestMetadata";  //default
        }

        //[SetUp]    
        //public void CaseSetUp()
        //{
                       
        //        StopS1Services();

        //        // Clean DB
        //        CleanS1DB(metaDataFolder);
        //        // Run CleanShareFolder()
        //        CleanShareFolder(metaDataFolder);

        //        StartS1Services();
           
        //}

        
        public void CaseSetUp()
        {

            StopS1Services();

            // Clean DB
            CleanS1DB(metaDataFolder);
            // Run CleanShareFolder()
            CleanShareFolder(metaDataFolder);

            StartS1Services();

        }

        // Method for clean SourceOne Database
        public void CleanS1DB(String scriptSource)
        {
            Console.WriteLine("Running CleanS1DB()");
            SQLHelper sqlHelper = new SQLHelper();
            //string sqlConnectionString = "data source=Debug02; user id=sa; password=emcsiax@QA";
            String dataSource = "data source = " + TestEnvironmentHelper.DBServer + ";";

            Console.WriteLine("DBServer name: " + dataSource);
            
            // Using service account is ok, but should be login windows with that account to pass the SQL windows authentication.
            // So here to hard code user id = sa temporarily.
            String userId = "user id=sa;";

            Console.WriteLine("userId: " + userId);

            String password = "password=" + TestEnvironmentHelper.DomainAdminPassword ;

            Console.WriteLine("password: " + password);

            String sqlConString = dataSource + userId + password ;

            try
            {
                sqlHelper.RunSQLScript("", scriptSource+"\\CleanUpDB.sql", sqlConString);
            }
            catch
            {
                Console.WriteLine("Catched exception during RunSQLScript()");
                Thread.CurrentThread.Abort();
            }



        }

        // Method for clean share folder
        public void CleanShareFolder(String shareFolderSource)
        {
            String messageCenterLocation = "";
            String archiveLocation = "";
            String indexStorageLocations = "";

            // get messagecenterlocation
            S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
            naConfig.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, shareFolderSource, "S1NativeArchiveServerConfiguration.xml"));
            messageCenterLocation = naConfig.Archive_MessageCenterLocation;

            // get archiveFolderLocation and indexStorageLocations
            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, shareFolderSource, "S1NativeArchiveFolder.xml"));
            archiveLocation = (archiveFolder.Storage_ArchiveFolderConfig as S1NASContainerStorageConfig).ArchiveLocation;
            indexStorageLocations = archiveFolder.Index_IndexStorageLocations;

            Console.WriteLine("LOG: CleanShareFolder, mc location is " + messageCenterLocation);
            Console.WriteLine("LOG: CleanShareFolder, archive location is " + archiveLocation);
            Console.WriteLine("LOG: CleanShareFolder, index location is " + indexStorageLocations);

            
            // delete files and subFolders under share folders.
            FileHelper fileHelper = new FileHelper();

            fileHelper.DeleteFolder(messageCenterLocation);
            fileHelper.DeleteFolder(archiveLocation);
            fileHelper.DeleteFolder(indexStorageLocations);        
            
        }

        // Method for rename transaction files
        public void RenameFiles()
        {
            String messageCenterLocation = "";
            String archiveLocation = "";
            String indexStorageLocations = "";

            // get messagecenterlocation
            S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
            naConfig.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, "TestMetadata_Shortcut", "S1NativeArchiveServerConfiguration.xml"));
            messageCenterLocation = naConfig.Archive_MessageCenterLocation;

            // get archiveFolderLocation and indexStorageLocations
            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, "TestMetadata_Shortcut", "S1NativeArchiveFolder.xml"));
            archiveLocation = (archiveFolder.Storage_ArchiveFolderConfig as S1NASContainerStorageConfig).ArchiveLocation;
            indexStorageLocations = archiveFolder.Index_IndexStorageLocations;

            Console.WriteLine("RenameFiles(), mc location is " + messageCenterLocation);
            Console.WriteLine("RenameFiles(), archive location is " + archiveLocation);
            Console.WriteLine("RenameFiles(), index location is " + indexStorageLocations);

            FileHelper fileHelper = new FileHelper();
            // renames files and subFolders under share folders.
            fileHelper.ChangeFilesExtensionInFolder(messageCenterLocation, ".txvlts", "xvlts");

            ServiceHelper serviceHelper = new ServiceHelper();

            serviceHelper.RestartService("ExAsIndex");            
            

            System.Threading.Thread.Sleep(30 * 1000);

            fileHelper.ChangeFilesExtensionInFolder(indexStorageLocations, ".txvlts", "xvlts");

            serviceHelper.RestartService("ExAsIndex");           

        }

        public void StopS1Services()
        {
            List<String> S1ServicesName = new List<String>();
            S1ServicesName.Add("ExAddressCacheService");
            S1ServicesName.Add("ES1AddressResolutionService");
            S1ServicesName.Add("ExAsArchive");
            S1ServicesName.Add("DCWorkerService");
            S1ServicesName.Add("ExDocMgmtSvc");
            S1ServicesName.Add("ExAsIndex");
            S1ServicesName.Add("ExJobDispatcher");
            S1ServicesName.Add("ExJobScheduler");
            S1ServicesName.Add("ExDocMgmtSvcOA");
            S1ServicesName.Add("ExAsQuery");
            S1ServicesName.Add("ExSearchService");
            S1ServicesName.Add("ExAsAdmin");

            ServiceHelper serviceHelper = new ServiceHelper();
            foreach (String serviceName in S1ServicesName)
            {
                serviceHelper.StopService(serviceName);
            }
        }

        public void StartS1Services()
        {
            List<String> S1ServicesName = new List<String>();
            S1ServicesName.Add("ExAddressCacheService");
            S1ServicesName.Add("ES1AddressResolutionService");
            S1ServicesName.Add("ExAsArchive");
            S1ServicesName.Add("DCWorkerService");
            S1ServicesName.Add("ExDocMgmtSvc");
            S1ServicesName.Add("ExAsIndex");
            S1ServicesName.Add("ExJobDispatcher");
            S1ServicesName.Add("ExJobScheduler");
            S1ServicesName.Add("ExDocMgmtSvcOA");
            S1ServicesName.Add("ExAsQuery");
            S1ServicesName.Add("ExSearchService");
            S1ServicesName.Add("ExAsAdmin");

            ServiceHelper serviceHelper = new ServiceHelper();
            foreach (String serviceName in S1ServicesName)
            {
                serviceHelper.StartService(serviceName);
            }
        }              


        [Test, Description("Sample test cases for HA ViPR")]
        [Category("WebId=5459")]
        public void ViPRSampleTest()
        {
            //archiveDBName = TestEnvironmentHelper.ArchiveDB;
            //dbServer = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.SQLServer)[0].Name;
            //workerName = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.Worker)[0].Name;

            metaDataFolder = "TestMetadata_Shortcut";
            CaseSetUp();

            //Create connection
            Console.WriteLine("Start to create SourceOne connection");

            connection = new S1ArchiveConnection();
            connection.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1ArchiveConnection.xml"));
            S1ArchiveConnectionHelper.CreateArchiveConnection(connection);

            Console.WriteLine("SourceOne connection created with name: " + connection.Name.ToString());

            //Config the native archive servers
            foreach (S1ComponentHost host in TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.NativeArchive))
            {
                String name = host.FullName;
                S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
                naConfig.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1NativeArchiveServerConfiguration.xml"));
                S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer(connection.Name, name, naConfig);
            }
            //Create the archive folder
            Console.WriteLine("Start to create SourceOne archive folder");

            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1NativeArchiveFolder.xml"));
            S1NativeArchiveFolderHelper.CreateArchiveFolder(archiveFolder);

            Console.WriteLine("SourceOne archive folder created with name: " + archiveFolder.Name.ToString());



            //Create the mapped folder
            Console.WriteLine("Start to create SourceOne map folder");

            S1MappedFolder mappedFolder = new S1MappedFolder();
            mappedFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1MappedFolder.xml"));
            S1MappedFolderHelper.CreateS1MappedFolder(mappedFolder);

            Console.WriteLine("SourceOne map folder created with name: " + mappedFolder.Name.ToString());

            //Create the policy
            Console.WriteLine("Start to create SourceOne policy");

            S1OrganizationalPolicy policy = new S1OrganizationalPolicy();
            policy.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1OrganizationalPolicy.xml"));
            S1OrganizationalPolicyHelper.CreatePolicy(policy);

            Console.WriteLine("SourceOne policy created with name: " + policy.Name.ToString());

            //Create the historical archive activity
            Console.WriteLine("Start to create SourceOne historical archive activity");

            S1Activity activity = new S1Activity();
            activity.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1Activity.xml"),1);
            int activityId = S1ActivityHelper.CreateS1HistoricalArchiveActivity(activity);

            Console.WriteLine("SourceOne historical archive activity created with name: " + activity.Name);

            //Make sure the activity is created successfully
            // Assert


            Console.WriteLine("Assert activity name: " + activity.Name + " results = " + S1ActivityHelper.IsExistsByName(activity.Name));
            Assert.IsTrue(S1ActivityHelper.IsExistsByName(activity.Name));


            Console.WriteLine("Assert activity state, ID = " + activityId + " results = " + S1ActivityHelper.IsExistsByName(activity.Name));
            Assert.IsTrue((S1ActivityHelper.GetById(activityId) as S1Activity).State == S1ActivityState.Active);//Before it actural starts, it's active.

            Console.WriteLine("Wait for all jobs finish...");

            Assert.IsTrue(S1JobHelper.WaitAllJobsFinishForActivityWithId(activityId));      


            Console.WriteLine("Wait for Index complete...");            

            System.Threading.Thread.Sleep(30 * 1000);
            
            RenameFiles();

            Assert.IsTrue(S1IndexHelper.WaitForIndexAccomplishOfArchiveFolder(connection.Name, archiveFolder.Name, 30)); // timeout = 30 minutes
            
            Console.WriteLine("All done! Test case pass.");
            // todo : add assert for number of successed processed messages.

        }

        [Test, Description("Shortcut case")]
        [Category("WebId=124")]
        public void ShortcutSampleTest()
        {
            metaDataFolder = "TestMetadata_Shortcut";
            CaseSetUp();
            //Create connection
            Console.WriteLine("Start to create SourceOne connection");

            connection = new S1ArchiveConnection();
            connection.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1ArchiveConnection.xml"));
            S1ArchiveConnectionHelper.CreateArchiveConnection(connection);

            Console.WriteLine("SourceOne connection created with name: " + connection.Name.ToString());

            //Config the native archive servers
            foreach (S1ComponentHost host in TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.NativeArchive))
            {
                String name = host.FullName;
                S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
                naConfig.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1NativeArchiveServerConfiguration.xml"));
                S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer(connection.Name, name, naConfig);
            }
            //Create the archive folder
            Console.WriteLine("Start to create SourceOne archive folder");

            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1NativeArchiveFolder.xml"));
            S1NativeArchiveFolderHelper.CreateArchiveFolder(archiveFolder);

            Console.WriteLine("SourceOne archive folder created with name: " + archiveFolder.Name.ToString());

            //Create the mapped folder
            Console.WriteLine("Start to create SourceOne map folder");

            S1MappedFolder mappedFolder = new S1MappedFolder();
            mappedFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1MappedFolder.xml"));
            S1MappedFolderHelper.CreateS1MappedFolder(mappedFolder);

            Console.WriteLine("SourceOne map folder created with name: " + mappedFolder.Name.ToString());

            //Create the policy
            Console.WriteLine("Start to create SourceOne policy");

            S1OrganizationalPolicy policy = new S1OrganizationalPolicy();
            policy.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1OrganizationalPolicy.xml"));
            S1OrganizationalPolicyHelper.CreatePolicy(policy);

            Console.WriteLine("SourceOne policy created with name: " + policy.Name.ToString());

            //Create the historical archive activity
            Console.WriteLine("Start to create SourceOne shortcut activity");

            S1Activity activity = new S1Activity();
            activity.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1Activity.xml"), 1);
            int activityId = S1ActivityHelper.CreateS1HistoricalShortcutActivity(activity);

            Console.WriteLine("SourceOne historical archive activity created with name: " + activity.Name);

            //Make sure the activity is created successfully
            // Assert

            Console.WriteLine("Assert activity name: " + activity.Name + " results = " + S1ActivityHelper.IsExistsByName(activity.Name));
            Assert.IsTrue(S1ActivityHelper.IsExistsByName(activity.Name));


            Console.WriteLine("Assert activity state, ID = " + activityId + " results = " + S1ActivityHelper.IsExistsByName(activity.Name));
            Assert.IsTrue((S1ActivityHelper.GetById(activityId) as S1Activity).State == S1ActivityState.Active);//Before it actural starts, it's active.

            Console.WriteLine("Wait for all jobs finish...");
            Assert.IsTrue(S1JobHelper.WaitAllJobsFinishForActivityWithId(activityId));

            Console.WriteLine("Wait for Index complete...");

            System.Threading.Thread.Sleep(30 * 1000);

            RenameFiles();

            Assert.IsTrue(S1IndexHelper.WaitForIndexAccomplishOfArchiveFolder(connection.Name, archiveFolder.Name, 30)); // timeout = 30 minutes


            // Restore shortcut activity

            System.Threading.Thread.Sleep(30 * 1000);

            S1Activity activityRestore = new S1Activity();
            activityRestore.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, metaDataFolder, "S1Activity.xml"), 2);
            int activityId2 = S1ActivityHelper.CreateS1ShortcutRestoreActivity(activityRestore);

            Console.WriteLine("#####################################################################");
            Console.WriteLine("SourceOne Shortcut Restore activity created with name: " + activityRestore.Name);

            Console.WriteLine("Wait for all jobs finish...");
            Assert.IsTrue(S1JobHelper.WaitAllJobsFinishForActivityWithId(activityId2));
            Console.WriteLine("Activity successfully fininshed with id: " + activityId2);


            Console.WriteLine("All done! Test case pass.");
            // todo : add assert for number of successed processed messages.

        }

        [TearDown]
        public void CaseTearDown()
        {
            //TODO
        }

        private void PrepareTestData()
        {
            //TODO
            EWS ews = new EWS();
            ews.SendEmailByTSV(Path.Combine(AssemblyDirectory, "TestData", "email.tsv"));
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            //TODO, in future, we can investigate teh way to clean the environment to make sure we can reuse them.
        }
    }
}
