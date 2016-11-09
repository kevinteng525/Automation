using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.Model;
using Core.DTO;

namespace JobManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class JobManagerService
    {        
        //get the collection of all the automation jobs
        [WebGet(UriTemplate = "")]
        public List<AutomationJobDTO> GetJobCollection()
        {
            return AutomationJob.GetAllJobs().ToDTOs();
        }

        //get the collection of all the Automation Jobs with status "jobStatus"
        [WebGet(UriTemplate = "?searchBy=jobStatus&jobStatus={jobStatus}")]
        public List<AutomationJobDTO> GetJobCollectionByJobStatus(string jobStatus)
        {
            JobStatus status; 
            bool success = Enum.TryParse<JobStatus>(jobStatus, true,out status);
            if (success)
            {
                return AutomationJob.GetJobsByStatus(status).ToDTOs();
            }
            else
            {
                return new List<AutomationJobDTO>();
            }
        }

        
        //get the AutomationJob with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}")]
        public AutomationJobDTO GetJobByJobId(string jobId)
        {
            return AutomationJob.GetAutomationJob(int.Parse(jobId)).ToDTO();
        }

        //get the build of job with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/build")]
        public BuildDTO GetBuildOfJobByJobId(string jobId)
        {
            return AutomationJob.GetBuildOfJobByJobId(int.Parse(jobId)).ToDTO();
        }

        //get the build of task with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/task")]
        public AutomationTaskDTO GetTaskOfJobByJobId(string jobId)
        {
            return AutomationJob.GetTaskOfJobByJobId(int.Parse(jobId)).ToDTO();
        }

        //get the build of task with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/product")]
        public ProductDTO GetProductOfJobByJobId(string jobId)
        {
            return AutomationJob.GetProductOfJobByJobId(int.Parse(jobId)).ToDTO();
        }

        //get the accomplish percentage of the job execution with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/progress")]
        public string GetJobExecutionProgressByJobId(string jobId)
        {
            return AutomationJob.GetAutomationJob(int.Parse(jobId)).Description;
        }

        //get the realtime log of the job execution with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/log")]
        public string GetJobExecutionLogByJobId(string jobId)
        {
            return AutomationJob.GetAutomationJob(int.Parse(jobId)).Description;
        }

        //get all the child test cases of the job with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/testcases")]
        public List<TestCaseDTO> GetTestCasesByJobId(string jobId)
        {
            return AutomationJob.GetTestCasesOfJob(int.Parse(jobId)).ToDTOs();
        }

        //get all the child test suites of the job with jobId "jobId"
        [WebGet(UriTemplate = "{jobId}/testsuites")]
        public AutomationTask GetTestSuitesByJobId(string jobId)
        {
            // TODO: Replace the current implementation to return the test suites of the job with jobId "jobId"
            throw new NotImplementedException();
        }

        //create an automation job
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public AutomationJobDTO Create(AutomationJobDTO instance)
        {
            return AutomationJob.CreateJob(instance.ToEntity()).ToDTO();
        }    

        //update the job with jobId
        //This will implement the below oprations against an automation job
        //1. Update the job common information, such as the name, description, etc.
        //2. Cancel a job not started yet
        //3. Cancel a job execution
        //4. Change the job priority??
        //5. Pending a job
        //6. Maybe other operations...
        [WebInvoke(UriTemplate = "{jobId}", Method = "PUT")]
        public AutomationJobDTO Update(string jobId, AutomationJobDTO instance)
        {
            instance.JobId = int.Parse(jobId);
            instance.ModifyDate = System.DateTime.UtcNow;
            return AutomationJob.UpdateAutomationJob(int.Parse(jobId), instance).ToDTO();            
        }

        //delete a job with jobId "jobId"
        [WebInvoke(UriTemplate = "{jobId}", Method = "DELETE")]
        public void Delete(string jobId)
        {
            AutomationJob.Delete(int.Parse(jobId));
        }

    }
}
