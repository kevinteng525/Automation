using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saber.S1CommonAPILib;
using NUnit.Framework;

namespace S1CommonAPILibTest.S1ObjectWrapperTest
{
    [TestFixture]
    class S1ActivityTest
    {
        private string archiveConnection = "CONN";
        private string organizationalPolicy = "Test Policy";
        private string archiveFolder = "NA Folder";
        private string mappedFolder = "Mapped Folder";
        private string dataSource = "ES1Service";
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //SetUpNAConnection();
            //ConfigWorkers();
            
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            //CreateArchiveFolder();
            //CreateMappedFolder(); 
        }

        [Test()]
        public void CreateNewActivity()
        {
            //CreateHistoricalArchive();
            //Assert.True(true);
        }

        [TearDown]
        public void TestCaseTearDown()
        {
            //DeleteMappedFolder();
            //DeleteArchiveFolder();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
 
        }
    }
}
