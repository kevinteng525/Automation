using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.DTO;
using Core.Model;

namespace ProjectManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class ProjectManagerService
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances

        [WebGet(UriTemplate = "")]
        public List<ProjectDTO> GetCollection()        {
           
            return Project.GetAllProjects().ToDTOs();
        }

        [WebGet(UriTemplate = "{projectId}/LatestResultSummary")]
        public AutomationRunningStatusDTO GetLatestResultSummary(string projectId)
        {            
            AutomationTask task = Project.GetTaskWithLatestBuildForProject(int.Parse(projectId));
            AutomationRunningStatusDTO progress = AutomationTask.GetTaskProgress(task);
            if (null == task)
            {
                progress.Information = "There's no tasks in this project. No results yet.";
            }
            else
            {
                Build build = Build.GetBuildById(task.BuildId);
                progress.Information = build.Name;
            }
            return progress;
        }

        [WebGet(UriTemplate = "{projectId}/PassRateHistory")]
        public AutomationRunningStatusDTO GetPassRateHistory(string projectId)
        {
            AutomationRunningStatusDTO progress = new AutomationRunningStatusDTO();
            List<AutomationTask> tasks = Project.GetLatestNAutomationTasksForProject(int.Parse(projectId), 12);
            if (tasks.Count == 0)
            {
                progress.TestCasesExecutionStatusList = "No Executions.";
                progress.TestCasesExecutionStatusCountList = "0";
                progress.Information = string.Format("There're no tasks trigged for this project, so there's no trend chart available.");
            }
            else
            {
                tasks.Reverse();

                foreach (AutomationTask task in tasks)
                {
                    AutomationRunningStatusDTO dto = AutomationTask.GetTaskProgress(task);
                    //add build id to status list
                    progress.TestCasesExecutionStatusList += "|Task " + task.TaskId.ToString() + ":" + Build.GetBuildById(task.BuildId).Name;
                    //add passreate to count list
                    int passRate = 0;
                    List<string> resultTypes = dto.TestCasesExecutionStatusList.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> resultCounts = dto.TestCasesExecutionStatusCountList.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    int passedCount = 0;
                    int totalCount = 0;

                    for (int i = 0; i < resultTypes.Count; i++)
                    {
                        if (resultTypes[i] == ResultType.Pass.ToString())
                        {
                            passedCount = int.Parse(resultCounts[i]);
                        }
                        totalCount += int.Parse(resultCounts[i]);
                    }
                    if (totalCount == 0)
                    {
                        passRate = 0;
                    }
                    else
                    {
                        passRate = passedCount * 100 / totalCount;
                    }

                    progress.TestCasesExecutionStatusCountList += " " + passRate.ToString();
                }

                progress.TestCasesExecutionStatusList = progress.TestCasesExecutionStatusList.TrimStart('|');
                progress.TestCasesExecutionStatusCountList = progress.TestCasesExecutionStatusCountList.TrimStart(' ');

                progress.Information = string.Format("TestCasesExecutionStatusList contains the task id list, and TestCasesExecutionStatusCountList contains the passrate list");
            }
            
            return progress;
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public ProjectDTO Create(ProjectDTO instance)
        {
            return Project.AddProject(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "{id}")]
        public ProjectDTO Get(string id)
        {
            return Project.GetProjectById(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public ProjectDTO Update(string id, ProjectDTO instance)
        {
            instance.ProjectId = Int32.Parse(id);
            return Project.UpdateProject(Int32.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void Delete(string id)
        {
            Project.DeleteProjectById(Int32.Parse(id));
        }

        [WebGet(UriTemplate = "{id}/SupportedEnvironments")]
        public List<SupportedEnvironmentDTO> GetSupportedEnvironmentsCollection(string id)
        {
            return Project.GetAllSupportedEnvironmentsForProject(Int32.Parse(id)).ToDTOs();
        }
    }
}
