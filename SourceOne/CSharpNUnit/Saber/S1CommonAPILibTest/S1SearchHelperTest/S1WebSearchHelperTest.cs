using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Saber.S1CommonAPILib;
using Saber.S1CommonAPILib.S1SearchHelper;
using Saber.S1CommonAPILib.S1SearchWrapper;

namespace S1CommonAPILibTest.S1SearchHelperTest
{
    [TestFixture]
    public class S1WebSearchHelperTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            
        }

        [Test,Description("Full Text Index Search")]
        public void FullTextIndexSearch()
        {
            String keywords = "hello";
            SearchHelper helper = new SearchHelper();
            SearchQueryBuilder builder = new SearchQueryBuilder();
            string query = builder.CreateXMLQuery(keywords, null, SearchObjectKeys.EMAIL, SearchTypeKeys.Administrator, SearchMailEnv.EXCHANGE);
            string scope = helper.EnumerateArchiveFolders(SearchTypeKeys.Administrator);
            helper.Search(query, scope, string.Empty, SearchTypeKeys.Administrator, 1000, true, false, 0);
            
        }

        [TearDown]
        public void TestCaseTearDown()
        {
           
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
 
        }
    }
}
