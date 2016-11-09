using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saber.S1CommonAPILib;

using NUnit.Framework;


namespace S1CommonAPILibTest.S1ObjectHelperTest
{
    [TestFixture]
    class S1MappedFolderHelperTest
    {
        [SetUp]
        public void Setup()
        {
 
        }

        [Test]
        public void CreateMappedFolder()
        {
            S1MappedFolderHelper.IsMappedFolderExist("MDAF1");
        }

        [TearDown]
        public void TearDown()
        {
 
        }
    }
}
