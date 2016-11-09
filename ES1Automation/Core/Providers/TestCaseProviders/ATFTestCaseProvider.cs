using System;
using Core.Model;

namespace Core.Providers.TestCaseProviders
{
    public class ATFTestCaseProvider : ITestCaseProvider
    {
        public Provider Provider { get; set; }

        public void ApplyConfig(string config)
        {
        }

        public void SyncAllTestCase()
        {
            //var testCase01 = TestCase.CreateOrUpdateTestCase
            //(
            //    new TestCase
            //    {
            //        ProviderId = this.Provider.ProviderId,
            //        SourceId = "1",
            //        Name = "ATF Test 01: Automation Work Flow Test",
            //        Feature = "ATF Test",
            //        ProductId = 0,
            //        Weight = 100,
            //        IsAutomated = true,
            //        ModifyBy = 0,
            //        ModifyTime = DateTime.UtcNow,
            //        IsActive = true,
            //    }
            //);

            //testCase01 = TestCase.CreateOrUpdateTestCase(testCase01);

            //var testCase02 = TestCase.CreateOrUpdateTestCase
            //(
            //    new TestCase
            //    {
            //        ProviderId = this.Provider.ProviderId,
            //        SourceId = "2",
            //        Name = "ATF Test 02: Automation Work Flow Test",
            //        Feature = "ATF Test",
            //        ProductId = 0,
            //        Weight = 100,
            //        IsAutomated = true,
            //        ModifyBy = 0,
            //        ModifyTime = DateTime.UtcNow,
            //        IsActive = true,
            //    }
            //);

            //TestSuite suite = TestSuite.GetTestSuite(100000);
            //suite.TestCases = string.Format("{0},{1}", testCase01.TestCaseId, testCase02.TestCaseId);
            //suite.Update();
        }

        public void SyncTestCaseById(int caseId)
        {
            throw new NotImplementedException();
        }

        public void SyncTestCaseBySourceId(string sourceId)
        {
            throw new NotImplementedException();
        }

        public TestCase GetTestCaseBySourceId(string sourceId)
        {
            throw new NotImplementedException();
        }


        public System.Collections.Generic.List<string> GetAllTestCaseRankings()
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<string> GetAllTestCaseReleases()
        {
            throw new NotImplementedException();
        }


        public void SyncAllTestSuite()
        {
           // throw new NotImplementedException();
        }


        public void WriteBackTestResult(AutomationTask task)
        {
            //throw new NotImplementedException();
        }


        public void SyncAllTestPlan()
        {
            //throw new NotImplementedException();
        }
    }
}
