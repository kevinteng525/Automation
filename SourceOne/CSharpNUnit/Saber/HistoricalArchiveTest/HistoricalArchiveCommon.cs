using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

using Saber.TestEnvironment;
using Saber.TestData;
using Saber.TestData.EWS;
using Saber.S1CommonAPILib;
using Saber.BaseTest;

namespace HistoricalArchiveTest
{
    [TestFixture]
    public class HistoricalArchiveCommon : TestClassBase
    {
        private String archiveDBName = "";
        private String dbServer = "";
        private String workerName = "";
        private S1ArchiveConnection connection = null;

        public HistoricalArchiveCommon() : base()
        {
            
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        { 
            
        }

        [SetUp]
        public void CaseSetUp()
        {
            //PrepareTestData();
        }
        
        [Test, Description("Sample test cases for historical archive")]
        [Category("WebId=987")]
        public void HASampleTest()
        {
            archiveDBName = TestEnvironmentHelper.ArchiveDB;
            dbServer = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.SQLServer)[0].Name;
            workerName = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.Worker)[0].Name;

            //Create connection
            connection = new S1ArchiveConnection();
            connection.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, "TestMetadata", "S1ArchiveConnection.xml"));
            S1ArchiveConnectionHelper.CreateArchiveConnection(connection);
            //Config the native archive servers
            foreach (S1ComponentHost host in TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.NativeArchive))
            {
                String name = host.FullName;
                S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
                naConfig.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory, "TestMetadata", "S1NativeArchiveServerConfiguration.xml"));
                S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer(connection.Name, name, naConfig);
            }
            //Create the archive folder
            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory,"TestMetadata","S1NativeArchiveFolder.xml"));           
            S1NativeArchiveFolderHelper.CreateArchiveFolder(archiveFolder);
            //Create the mapped folder
            S1MappedFolder mappedFolder = new S1MappedFolder();
            mappedFolder.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory,"TestMetadata","S1MappedFolder.xml"));           
            S1MappedFolderHelper.CreateS1MappedFolder(mappedFolder);
            //Create the policy
            S1OrganizationalPolicy policy = new S1OrganizationalPolicy();
            policy.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory,"TestMetadata","S1OrganizationalPolicy.xml"));           
            S1OrganizationalPolicyHelper.CreatePolicy(policy);
            //Create the historical archive activity
            S1Activity activity = new S1Activity();
            activity.DeserializeFromXMLFile(Path.Combine(AssemblyDirectory,"TestMetadata","S1Activity.xml"));            
            int activityId = S1ActivityHelper.CreateS1HistoricalArchiveActivity(activity); 
            //Make sure the activity is created successfully
            Assert.IsTrue(S1ActivityHelper.IsExistsByName(activity.Name));
            Assert.IsTrue((S1ActivityHelper.GetById(activityId) as S1Activity).State == S1ActivityState.Active);//Before it actural starts, it's active.
            S1JobHelper.WaitAllJobsFinishForActivityWithId(activityId);
            S1IndexHelper.WaitForIndexAccomplishOfArchiveFolder(connection.Name, archiveFolder.Name, 30);
        }

        [Test, Description("Sample test cases for historical archive")]
        [Category("WebId=976")]
        [Category("Second Category")]
        public void TestNeil()
        {
            Assert.Fail();
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

// test for git push right -- hugh
