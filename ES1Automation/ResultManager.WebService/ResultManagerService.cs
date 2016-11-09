using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.DTO;
using Core.Model;

namespace ResultManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class ResultManagerService
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances
        #region the operations for the test results

        [WebGet(UriTemplate = "Results")]
        public List<TestResultDTO> GetCollection()
        {
            return TestResult.GetAllResults().ToDTOs();
        }

        [WebGet(UriTemplate = "Results?JobId={jobId}&CaseId={caseId}")]
        public TestResultDTO GetResultByJobIdAndTestCaseId(string jobId,string caseId)
        {
            return TestResult.GetTestResultByJobIdAndTestCaseId(int.Parse(jobId),int.Parse(caseId)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Results", Method = "POST")]
        public TestResultDTO CreateResult(TestResultDTO instance)
        {
            return TestResult.CreateRunResult(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Results/{id}")]
        public TestResultDTO GetResult(string id)
        {
            return TestResult.GetTestResult(Int32.Parse(id)).ToDTO();
        }

        [WebGet(UriTemplate = "Results/{id}/TestCase")]
        public TestCaseDTO GetTestCaseOfResult(string id)
        {
            return TestCase.GetTestCaseByResultId(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Results/{id}", Method = "PUT")]
        public TestResultDTO UpdateResult(string id, TestResultDTO instance)
        {
            instance.ResultId = Int32.Parse(id);
            return TestResult.Update(Int32.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "Results/{id}", Method = "DELETE")]
        public void DeleteResult(string id)
        {
            TestResult.Delete(Int32.Parse(id));
        }
        #endregion

        #region the operations for the test executions

        [WebGet(UriTemplate = "Executions")]
        public List<TestCaseExecutionDTO> GetExecutionCollection()
        {
            return TestCaseExecution.GetAllExecutions().ToDTOs();
        }


        [WebGet(UriTemplate = "Executions?JobId={jobId}&CaseId={caseId}")]
        public TestCaseExecutionDTO GetExecutionByJobIdAndTestCaseId(string jobId, string caseId)
        {
            return TestCaseExecution.GetTestCaseExecutionByJobIdAndTestCaseId(int.Parse(jobId), int.Parse(caseId)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Executions", Method = "POST")]
        public TestCaseExecutionDTO CreateExecution(TestCaseExecutionDTO instance)
        {
            return TestCaseExecution.CreateExecution(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Executions/{id}")]
        public TestCaseExecutionDTO GetExecution(string id)
        {
            return TestCaseExecution.GetTestCaseExecution(Int32.Parse(id)).ToDTO();
        }       

        [WebInvoke(UriTemplate = "Executions/{id}", Method = "PUT")]
        public TestCaseExecutionDTO UpdateExecution(string id, TestCaseExecutionDTO instance)
        {
            instance.ExecutionId = Int32.Parse(id);
            return TestCaseExecution.Update(Int32.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "Executions/{id}", Method = "DELETE")]
        public void DeleteExecution(string id)
        {
            TestCaseExecution.Delete(Int32.Parse(id));
        }
        #endregion

        [WebGet(UriTemplate = "Executions/{id}/Result")]
        public TestResultDTO GetResultOfExecution(string id)
        {
            return TestResult.GetTestResultByExecutionId(Int32.Parse(id)).ToDTO();
        }

        [WebGet(UriTemplate = "Executions/{id}/Build")]
        public BuildDTO GetBuildByExecutionId(string id)
        {
            return TestCaseExecution.GetBuildByExecutionId(Int32.Parse(id)).ToDTO();
        }
               

        public class TestExecutionComparer : IEqualityComparer<TestCaseExecution>
        {
            public static TestExecutionComparer Default = new TestExecutionComparer();
            public bool Equals(TestCaseExecution x, TestCaseExecution y)
            {
                return x.TestCaseId.Equals(y.TestCaseId);
            }
            public int GetHashCode(TestCaseExecution obj)
            {
                return obj.GetHashCode();
            }
        }

        #region test result compare between two tasks
        //the executionIdList is a string contains the list of execution ids seperated by ',', such as '1,2'
        [WebGet(UriTemplate = "TaskResultCompare/{taskIdList}")]
        public List<List<TestCaseExecutionDTO>> GetTaskCompareCollection(string taskIdList)
        {
            string[] taskIds = taskIdList.Split(',');
            if (taskIds.Length != 2)
            {
                return null;
            }
            List<List<TestCaseExecutionDTO>> returnList = new List<List<TestCaseExecutionDTO>>();
            List<TestCaseExecution> list1 = AutomationTask.GetTestCaseExecutionForAutomationTask(Int32.Parse(taskIds[0])).OrderBy(tc => tc.TestCaseId).ToList();
            List<TestCaseExecution> list2 = AutomationTask.GetTestCaseExecutionForAutomationTask(Int32.Parse(taskIds[1])).OrderBy(tc => tc.TestCaseId).ToList();
            int i = 0;
            int j = 0;
            while (i != list1.Count && j != list2.Count)
            {
                TestCaseExecution e1 = list1.ElementAt(i);
                TestCaseExecution e2 = list2.ElementAt(j);
                if (e1.TestCaseId < e2.TestCaseId)
                {
                     if(!list2.Contains(e1,TestExecutionComparer.Default))
                     {
                         TestCaseExecution temp = new TestCaseExecution();
                         temp.TestCaseId = e1.TestCaseId;
                         temp.ExecutionId = -1;//set the executionId to -1 to indicate that the test case is not executed on this task.
                         list2.Insert(j, temp);
                     }
                     j++;//one more element is added
                     i++;
                }
                else if (e1.TestCaseId > e2.TestCaseId)
                {
                    if (!list1.Contains(e2, TestExecutionComparer.Default))
                    {
                        TestCaseExecution temp = new TestCaseExecution();
                        temp.TestCaseId = e2.TestCaseId;
                        temp.ExecutionId = -1;//set the executionId to -1 to indicate that the test case is not executed on this task.
                        list1.Insert(i, temp);
                    }
                    i++;
                    j++;
                }
                else
                {
                    i++;
                    j++;
                }
            }
            if (i == list1.Count)
            {
                list1.InsertRange(i, list2.GetRange(j, list2.Count - j));
            }
            else
            {
                list2.InsertRange(j, list1.GetRange(i, list1.Count - i));
            }
            returnList.Add(list1.ToDTOs());
            returnList.Add(list2.ToDTOs());
            return returnList;

        }
        #endregion

        #region test result compare between two builds
        [WebGet(UriTemplate = "BuildResultCompare/{buildIdList}")]
        public List<List<TestCaseExecutionDTO>> GetBuildCompareCollection(string buildIdList)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
