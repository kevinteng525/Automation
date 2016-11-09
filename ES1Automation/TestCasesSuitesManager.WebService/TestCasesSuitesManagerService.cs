using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

using Core;
using Core.DTO;
using Core.Model;
using Core.Providers.TestCaseProviders;
using ES1Common.Logs;


namespace TestCasesSuitesManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class TestCasesSuitesManagerService
    {
        #region the virtual root for the test library

        [WebGet(UriTemplate = "Root")]
        public TestSuiteDTO GetRootVirtualTestSuite()
        {
            return TestSuite.GetRootNodeOfAllTestCases().ToDTO();
        }

        [WebGet(UriTemplate = "UserCreatedSuitesRoot")]
        public TestSuiteDTO GetUserCreatedTestSuiteRoot()
        {
            return TestSuite.GetRootNodeOfUserCustomizedTestSuites().ToDTO();
        }

        [WebGet(UriTemplate = "RQMSuitesRoot")]
        public TestSuiteDTO GetRQMTestSuiteRoot()
        {
            return TestSuite.GetRootNodeOfNormalTestSuitesFromExternalProvider().ToDTO();
        }

        [WebGet(UriTemplate = "RQMTestPlansRoot")]
        public TestSuiteDTO GetRQMTestPlanRoot()
        {
            return TestSuite.GetRootNodeOfNormalTestPlansFromExternalProvider().ToDTO();
        }

        [WebInvoke(UriTemplate = "TestCases/Refresh", Method = "POST")]
        public bool RefreshTestCasesFromExternalTestCaseManagementSystem()
        {
            try
            {
                if (ATFConfiguration.IsTestCasesSuitesSyncing())
                {
                    return true;
                }
                ATFConfiguration.SetTestCasesSuitesSyncingStartIndicator();
                foreach (Provider provider in Provider.GetProvidersByCategory(ProviderCategory.TestCase))
                {
                    ITestCaseProvider p = null;
                    try
                    {
                        p = provider.CreateProvider() as ITestCaseProvider;
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error(string.Format("Failed to initialize the provider {0} for RQM server.", provider.Name), ex);
                        continue;
                    }
                    p.SyncAllTestCase();
                }
                return true;
            }
            finally
            {
                ATFConfiguration.SetTestCasesSuitesSyncingEndIndicator();
            }
        }

        [WebInvoke(UriTemplate = "TestSuites/Refresh", Method = "POST")]
        public bool RefreshTestSuitesFromExternalTestCaseManagementSystem()
        {
            try
            {
                if (ATFConfiguration.IsTestCasesSuitesSyncing())
                {
                    return true; 
                }
                ATFConfiguration.SetTestCasesSuitesSyncingStartIndicator();
                foreach (Provider provider in Provider.GetProvidersByCategory(ProviderCategory.TestCase))
                {
                    ITestCaseProvider p = null;
                    try
                    {
                        p = provider.CreateProvider() as ITestCaseProvider;
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error(string.Format("Failed to initialize the provider {0} for RQM server.", provider.Name), ex);
                        continue;
                    }
                    p.SyncAllTestSuite();
                    p.SyncAllTestPlan();
                }
                return true;
            }
            finally
            {
                ATFConfiguration.SetTestCasesSuitesSyncingEndIndicator();
            }
        }

        [WebGet(UriTemplate = "Rankings")]
        public List<string> GetRankingsCollection()
        {
            List<string> rankings = new List<String>();
            foreach (Ranking r in Ranking.GetAllRankings())
                rankings.Add(r.Name);
            return rankings;
        }

        [WebGet(UriTemplate = "Releases")]
        public List<string> GetReleasesCollection()
        {
            List<string> releases = new List<String>();
            foreach (Release r in Release.GetAllReleasesByType(1))
                releases.Add(r.Name);         
            return releases;

        }

        #endregion

        #region test case operations

        [WebGet(UriTemplate = "TestCases")]
        public List<TestCaseDTO> GetTestCasesCollection()
        {
            return TestCase.GetAllTestCases().ToDTOs();
        }

        [WebInvoke(UriTemplate = "TestCases", Method = "POST")]
        public TestCaseDTO CreateTestCase(TestCaseDTO instance)
        {
            return TestCase.CreateCase(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "TestCases/{id}")]
        public TestCaseDTO GetTestCase(string id)
        {
            return TestCase.GetTestCase(Int32.Parse(id)).ToDTO();
        }

        [WebGet(UriTemplate = "TestCases/{testcaseId}/HistoricalTestResults?size={size}")]
        public List<HistoricalTestResultsDTO> GetHistoricalTestResultsOfTestCase(string testcaseId, string size)
        {
            int caseId = Int32.Parse(testcaseId);
            
            int sizeInt = 5;
            try { sizeInt = Int32.Parse(size); }
            catch { }
            List<HistoricalTestResultsDTO> historicalResults = new List<HistoricalTestResultsDTO>();
            List<AutomationTask> tasks = AutomationTask.GetLatestAutomationTasksContainTestCase(caseId, sizeInt);
            foreach (AutomationTask task in tasks)
            {
                HistoricalTestResultsDTO resultDTO = new HistoricalTestResultsDTO();
                TestResult result = AutomationTask.GetTestCaseResultForTestCaseInTask(task.TaskId, caseId);
                resultDTO.BuildId = task.BuildId;
                resultDTO.BuildName = Build.GetBuildById(task.BuildId).Name;
                resultDTO.TaskId = task.TaskId;
                resultDTO.TaskName = task.Name;
                resultDTO.TestCaseId = caseId;
                resultDTO.TestCaseName = TestCase.GetTestCase(caseId).Name;
                resultDTO.ResultId = result.ResultId;
                resultDTO.Result = result.Result;
                resultDTO.ResultComments = result.Description;
                historicalResults.Add(resultDTO);
            }
            return historicalResults;
        }

        [WebInvoke(UriTemplate = "TestCases/{id}", Method = "PUT")]
        public TestCaseDTO UpdateTestCase(string id, TestCaseDTO instance)
        {
            int testCaseId = Int32.Parse(id);
            instance.TestCaseId = testCaseId;
            return TestCase.Update(testCaseId, instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "TestCases/{id}", Method = "DELETE")]
        public void DeleteTestCase(string id)
        {
            TestCase.Delete(Int32.Parse(id));
        }

        #endregion

        #region test suite operations

        [WebGet(UriTemplate = "TestSuites")]
        public List<TestSuiteDTO> GetTestSuitesCollection()
        {
            return TestSuite.GetAllTestSuites().ToDTOs();
        }

        [WebInvoke(UriTemplate = "TestSuites", Method = "POST")]
        public TestSuiteDTO CreateTestSuite(TestSuiteDTO instance)
        {
            return TestSuite.CreateSuite(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "TestSuites/{id}")]
        public TestSuiteDTO GetTestSuite(string id)
        {
            return TestSuite.GetTestSuite(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "TestSuites/{id}", Method = "PUT")]
        public TestSuiteDTO UpdateTestSuite(string id, TestSuiteDTO instance)
        {
            int testSuiteId = Int32.Parse(id);
            instance.SuiteId = testSuiteId;
            return TestSuite.Update(testSuiteId, instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "TestSuites/{id}", Method = "DELETE")]
        public void DeleteTestSuite(string id)
        {
            TestSuite.Delete(Int32.Parse(id));
        }

        #endregion

        #region sub test suites/cases for a test suite

        [WebGet(UriTemplate = "TestSuites/{id}/SubTestCases")]
        public List<TestCaseDTO> GetSubTestCasesCollection(string id)
        {
            List<TestCase> testCaseList = TestSuite.GetAllSubCases(Int32.Parse(id));
            if (testCaseList == null || testCaseList.Count == 0)
            {
                return null;
            }
            else
            {
                List<TestCase> activeTestCasesList = testCaseList.FindAll(tc => tc.IsActive == true);
                if (activeTestCasesList.Count > 0)
                {
                    return activeTestCasesList.OrderBy(tc => tc.SourceId).ToList().ToDTOs();
                }
                else
                {
                    return null;
                }
            }
        }

        [WebInvoke(UriTemplate = "TestSuites/{testSuiteId}/SubTestCases", Method = "POST")]
        public TestSuiteDTO AddSubTestCaseToTestSuite(string testSuiteId, TestCaseDTO instance)
        {
            TestSuite ts = TestSuite.GetTestSuite(Int32.Parse(testSuiteId));

            if (ts != null)
            {
                ts.AddSubTestCase(instance.TestCaseId);
                return ts.ToDTO();
            }
            else
            {
                return null;
            }
            
        }


        [WebInvoke(UriTemplate = "TestSuites/{testSuiteId}/SubTestCases/{subTestCaseId}", Method = "DELETE")]
        public TestSuiteDTO DeleteSubTestCaseFromTestSuite(string testSuiteId, string subTestCaseId)
        {
            TestSuite ts = TestSuite.GetTestSuite(Int32.Parse(testSuiteId));

            if (ts != null)
            {
                ts.DeleteSubTestCase(Int32.Parse(subTestCaseId));
                return ts.ToDTO();
            }
            else
            {
                return null;
            }
        }

        [WebGet(UriTemplate = "TestSuites/{id}/SubTestSuites")]
        public List<TestSuiteDTO> GetSubTestSuiteCollection(string id)
        {
            TestSuite suite = TestSuite.GetTestSuite(int.Parse(id));
            if(suite == null)
            {
                return null;
            }
            else
            {
                List<TestSuite> testSuites = TestSuite.GetAllSubSuites(suite);
                if (testSuites != null && testSuites.Count() > 0)
                {
                    return testSuites.FindAll(t => t.IsActive == true).ToList().ToDTOs();
                }
                else
                {
                    return null;
                }
            }
        }

        [WebInvoke(UriTemplate = "TestSuites/{testSuiteId}/SubTestSuites", Method = "POST")]
        public TestSuiteDTO AddSubTestSuiteToTestSuite(string testSuiteId, TestSuiteDTO instance)
        {
            TestSuite ts = TestSuite.GetTestSuite(Int32.Parse(testSuiteId));

            if (ts != null)
            {
                ts.AddSubTestSuite(instance.SuiteId);
                return ts.ToDTO();
            }
            else
            {
                return null;
            }

        }


        [WebInvoke(UriTemplate = "TestSuites/{testSuiteId}/SubTestSuites/{subTestSuiteId}", Method = "DELETE")]
        public TestSuiteDTO DeleteSubTestSuiteFromTestSuite(string testSuiteId, string subTestSuiteId)
        {
            TestSuite ts = TestSuite.GetTestSuite(Int32.Parse(testSuiteId));
            if (ts != null)
            {
                ts.DeleteSubTestSuite(Int32.Parse(subTestSuiteId));
                return ts.ToDTO();
            }
            else
            {
                return null;
            }
        }


        [WebGet(UriTemplate = "TestSuites/{id}/TestCases")]
        public List<TestCaseDTO> GetAllTestCasesCollection(string id)
        {
            List<int> allSubTestCaseIdList = TestSuite.GetAllCasesIds(Int32.Parse(id), true);

            if (allSubTestCaseIdList.Count > 0)
            {
                List<TestCase> testCaseList = new List<TestCase>();
                foreach (int testCaseId in allSubTestCaseIdList)
                {
                    TestCase tc = TestCase.GetTestCase(testCaseId);
                    testCaseList.Add(tc);
                }
                return testCaseList.ToDTOs();
            }
            else
            {
                return null;
            }
        }


        [WebGet(UriTemplate = "TestSuites/{id}/TestCases/Count")]
        public int GetAllTestCasesCollectionCount(string id)
        {
            List<TestCaseDTO> tsList = this.GetAllTestCasesCollection(id);
            if(tsList != null)
            {
                return tsList.Count;
            }
            else
            {
                return 0;
            }

        }

        #endregion
        
    }
}
