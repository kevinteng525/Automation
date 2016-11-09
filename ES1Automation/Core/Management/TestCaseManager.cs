using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Core.Model;
using Core.Providers.TestCaseProviders;

namespace Core.Management
{
    public class TestCaseManager
    {
        public static void UpdateTestSuite()
        {
            ATFEnvironment.Log.logger.Info(string.Format("Start to sync test suits and test cases from test suite server."));
            foreach (Provider provider in Provider.GetProvidersByCategory(ProviderCategory.TestCase))
            {
                ITestCaseProvider testcaseProvider = null;
                try
                {
                    testcaseProvider = provider.CreateProvider() as ITestCaseProvider;
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Failed to initialize the test case provider {0}", provider.Name), ex);
                    //return;
                }
                testcaseProvider.SyncAllTestCase();
                testcaseProvider.SyncAllTestSuite();
                testcaseProvider.SyncAllTestPlan();
            }
            ATFEnvironment.Log.logger.Info(string.Format("Finished the syncing of test suites and testcases from test case server."));
        }
    }
}
