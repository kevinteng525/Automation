using Core.Model;
using System.Collections.Generic;

namespace Core.Providers.TestCaseProviders
{
    public interface ITestCaseProvider : IProvider
    {
        void SyncAllTestCase();

        void SyncAllTestSuite();

        void SyncAllTestPlan();

        void SyncTestCaseById(int caseId);

        void SyncTestCaseBySourceId(string sourceId);

        TestCase GetTestCaseBySourceId(string sourceId);

        List<string> GetAllTestCaseRankings();

        List<string> GetAllTestCaseReleases();

        void WriteBackTestResult(AutomationTask task);
    }
}
