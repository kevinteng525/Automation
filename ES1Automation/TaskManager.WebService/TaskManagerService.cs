using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

using Core;
using Core.Model;
using Core.Management;
using Core.DTO;
using ES1Common.Logs;

namespace TaskManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class TaskManagerService
    {
        // TODO: Implement the collection resource that will contain the AutomationTask instances

        //get the collection of all the Automation Tasks
        [WebGet(UriTemplate = "")]
        public List<AutomationTaskDTO> GetCollection()
        {
            return AutomationTask.GetAllTasks().ToDTOs();
        }

        //get the automationTask with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}")]
        public AutomationTaskDTO GetTaskByTaskId(string taskId)
        {
            return AutomationTask.GetAutomationTask(Int32.Parse(taskId)).ToDTO();
        }

        //create an automation task
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public AutomationTaskDTO Create(AutomationTaskDTO instance)
        {
            instance.CreateDate = DateTime.UtcNow;
            instance.ModifyDate = DateTime.UtcNow;
            AutomationTask task = AutomationTask.CreateTask(instance.ToEntity());
            task.AddProgressInformation(string.Format("{0} submitted an automation task.", User.GetUserById(task.CreateBy).Username));
            return task.ToDTO();
        }

        //update the task with taskId
        //This will implement the below oprations against an automation task
        //1. Update the task common information, such as the name, description, etc.
        //2. Cancel a task not started yet
        //3. Cancel a task execution
        //4. Change the task priority
        //5. Pending a task
        //6. Maybe other operations...
        [WebInvoke(UriTemplate = "{taskId}", Method = "PUT")]
        public AutomationTaskDTO Update(string taskId, AutomationTaskDTO instance)
        {
            instance.ModifyDate = DateTime.UtcNow;
            instance.TaskId = Int32.Parse(taskId);            
            string message = string.Empty;
            string user =  User.GetUserById(instance.ModifyBy).Username;
            if (instance.Status == (int)TaskStatus.Cancelling)
            {
                message = string.Format("Task is cancelled by {0}.", user);
            }
            else
            {
                message = string.Format("Task is updated by {0}.", user);
            }

            instance.Information = AutomationTask.GetAutomationTask(int.Parse(taskId)).Information;

            AutomationTask task = AutomationTask.UpdateAutomationTask(Int32.Parse(taskId), instance);
            task.AddProgressInformation(message);
            return task.ToDTO();
        }

        //delete the task with taskId
        [WebInvoke(UriTemplate = "{taskId}", Method = "DELETE")]
        public void Delete(string taskId)
        {
            AutomationTask.Delete(Int32.Parse(taskId));
        }

        [WebGet(UriTemplate = "{taskId}/jobs")]
        public List<AutomationJobDTO> GetAutomationJobsOfTaskByTaskId(string taskId)
        {
            return AutomationTask.GetAutomationJobsOfTask(int.Parse(taskId)).ToDTOs();
        }

        [WebGet(UriTemplate = "{taskId}/reports")]
        public string SendTheTestReportToStakeholders(string taskId)
        {
            string errorMessage = "Failed to send the email to the stakeholders of the project.";
            try
            {
                AutomationTask task = AutomationTask.GetAutomationTask(int.Parse(taskId));
                TaskManagement.SendTestReportOfTaskToStakeholders(task, true);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(errorMessage, ex);
                return errorMessage;
            }
            return "succeed";
        }

        //get the accomplish percentage of the task execution with taskId "taskId"
        //To be detailed based on other more accurate factors, here just get the accomplete
        [WebGet(UriTemplate = "{taskId}/progress")]
        public AutomationRunningStatusDTO GetTestExecutionProgressByTaskId(string taskId)
        {
            AutomationTask task = AutomationTask.GetAutomationTask(Int32.Parse(taskId));
            return AutomationTask.GetTaskProgress(task);
        }

        public int GetTaskExecutionPercentageByTaskId(AutomationTask task)
        {
            
            switch (task.Status)
            {
                case (int)TaskStatus.Scheduled:
                    return 0;
                case (int)TaskStatus.Dispatched:
                    return 10;
                case (int)TaskStatus.Running:
                    return 20 + AutomationTask.GetTestCasesExecutionStatus(task.TaskId, 80);
                case (int) TaskStatus.Complete:
                    return 100;
                default:
                    return 100;
            }
        }

        //get the realtime log of the task execution with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}/log")]
        public string GetTaskExecutionLogByTaskId(string taskId)
        {
            return AutomationTask.GetAutomationTask(Int32.Parse(taskId)).ToDTO().Information;
        }

        //get all the child test cases of the task execution with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}/testcases")]
        public List<TestCaseDTO> GetTaskTestCasesByTaskId(string taskId)
        {
            return AutomationTask.GetTestCasesForAutomationTask(Int32.Parse(taskId)).ToDTOs();
        }

        //get all the test executions of the task with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}/testexecutions")]
        public List<TestCaseExecutionDTO> GetTaskTestExecutionsByTaskId(string taskId)
        {
            return AutomationTask.GetTestCaseExecutionForAutomationTask(Int32.Parse(taskId)).ToDTOs();
        }

        //get the test suites contain all the test cases failed of the task
        [WebGet(UriTemplate = "{taskId}/suitesfailed")]
        public TestSuiteDTO GetOrCreateTestSuiteContainingFailedCasesByTaskId(string taskId)
        {
            return AutomationTask.GetTestSuiteContainsAllFailedTestCasesOfTask(Int32.Parse(taskId)).ToDTO();
        }

        //get all the child test cases of the task execution with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}/testcases/{testcaseId}/result")]
        public TestResultDTO GetTestResultByTaskAndTestCaseId(string taskId, string testcaseId)
        {
            return AutomationTask.GetTestCaseResultForTestCaseInTask(Int32.Parse(taskId), Int32.Parse(testcaseId)).ToDTO();
        }

        //get all the child test cases of the task execution with taskId "taskId"
        [WebGet(UriTemplate = "{taskId}/results/{resultType}/count")]
        public int GetTaskTestResultCount(string taskId, string resultType)
        {
            return AutomationTask.GetResultTypeCountForTask(Int32.Parse(taskId), (ResultType)Int32.Parse(resultType));   
        }

    }
}
