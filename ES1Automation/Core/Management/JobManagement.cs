using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;
using ES1Common.Logs;
using Core.Providers.EnvrionmentProviders;
using System.Diagnostics;
using System.IO;
using Common.Windows;
using Common.Network;

namespace Core.Management
{
    public class JobManagement
    {
        /// <summary>
        /// Monitor the assigned jobs
        /// </summary>
        public static void MonitorAssignedJobs()
        {
            //AutomationLog Log = new AutomationLog("MicahTestConsole");
            ATFEnvironment.Log.logger.Debug("Monitor Assigned Jobs");
            List<AutomationJob> assignedJobs = AutomationJob.GetJobs((int)JobStatus.Assigned);
            if (assignedJobs == null)
            {
                ATFEnvironment.Log.logger.Debug("Assigned Jobs count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Assigned Jobs count:" + assignedJobs.Count().ToString());

            foreach (AutomationJob job in assignedJobs)
            {
                try
                {
                    // Env
                    if (job.SUTEnvironmentId == null)
                    {
                        // If the Environment is NOT Ready to accept new env request
                        // (Currently, if there is Env with status "New" or "Setup", we will regard the env as NOT ready)
                        // We will hold on requst new environment
                        // thus we can have more flexibility to edit the Jobs in queue
                        int envProviderId = job.GetSupportedEnv().ProviderId;
                        if (EnvironmentManager.IsAcceptNewEnvironment(envProviderId))//TODO, move this check into the function of RequestSUTEnvironmentForJob
                        {
                            string message = string.Format("Request SUT environment for job [{0}:{1}]", job.JobId, job.Name);
                            ATFEnvironment.Log.logger.Info(message);
                            job.AddJobProgressInformation(message);
                            ATFEnvironment.EnvironmentMgr.RequestSUTEnvironmentForJob(job, Provider.GetProviderById(envProviderId).CreateProvider() as IEnvironmentProvider); //Ask Tony
                        }
                    }
                    else if (job.TestAgentEnvironmentId == null)
                    {
                        EnvironmentType type = EnvironmentConfigHelper.GetResidenceType(job.GetSupportedEnv().Config);
                        if (type == EnvironmentType.Residence_Together)
                        {
                            job.SetTestAgentEnvironment(job.SUTEnvironmentId.Value);
                        }
                        else if (type == EnvironmentType.Residence_Seperate)
                        {
                            int envProviderId = job.GetSupportedEnv().ProviderId;
                            if (EnvironmentManager.IsAcceptNewEnvironment(envProviderId))//TODO, remove this check into the function of RequestTestAgentEnvironmentForJob
                            {
                                ATFEnvironment.Log.logger.Info(string.Format("Request Test Agent environment for job [{0}]", job.Name));
                                ATFEnvironment.EnvironmentMgr.RequestTestAgentEnvironmentForJob(job, Provider.GetProviderById(envProviderId).CreateProvider() as IEnvironmentProvider); //Ask Tony
                                string info = string.Format("Test Agent environment is requested for Job [{0}], waiting... ", job.Name.ToString());
                                ATFEnvironment.Log.logger.Info(info);
                                job.AddJobProgressInformation(info);
                            }
                        }
                    }
                    else//SUT and Test Agent is requested
                    {
                        //get SUT and Test Agent Environment
                        TestEnvironment sutEnvironment = TestEnvironment.GetEnvironmentById(job.SUTEnvironmentId.Value);
                        TestEnvironment testAgentEnvironment = null;
                        EnvironmentType type = EnvironmentConfigHelper.GetResidenceType(job.GetSupportedEnv().Config);
                        if (type == EnvironmentType.Residence_Together)
                        {
                            testAgentEnvironment = null;
                        }
                        else if (type == EnvironmentType.Residence_Seperate)
                        {
                            testAgentEnvironment = TestEnvironment.GetEnvironmentById(job.TestAgentEnvironmentId.Value);
                        }
                        //Check the status of SUT and Test Agent, Only when them/one of them are being prepared, set the job as preparing.
                        if (type == EnvironmentType.Residence_Together)
                        {
                            if (DoesBeginToPrepareEnvironment(sutEnvironment))
                            {
                                job.SetJobsStatus(JobStatus.Preparing);
                                ATFEnvironment.Log.logger.Info(string.Format("Set status of job [{0}] Assigned -> Preparing", job.Name));
                            }
                        }
                        else if (type == EnvironmentType.Residence_Seperate)
                        {
                            if (DoesBeginToPrepareEnvironment(sutEnvironment) || DoesBeginToPrepareEnvironment(testAgentEnvironment))
                            {
                                job.SetJobsStatus(JobStatus.Preparing);
                                ATFEnvironment.Log.logger.Info(string.Format("Set status of job [{0}] Assigned -> Preparing", job.Name));
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Error on get Environment for job [{0}]", job.Name), e);
                    continue;
                }
            }
        }

        /// <summary>
        /// Monitor the preparing jobs
        /// </summary>
        public static void MonitorPreparingJobs()
        {
            //AutomationLog Log = new AutomationLog("MicahTestConsole");
            ATFEnvironment.Log.logger.Debug("Monitor Preparing Jobs");
            List<AutomationJob> preparingJob = AutomationJob.GetJobs((int)JobStatus.Preparing);
            if (preparingJob == null)
            {
                ATFEnvironment.Log.logger.Debug("Preparing Jobs count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Preparing Jobs count:" + preparingJob.Count().ToString());

            foreach (AutomationJob job in preparingJob)
            {
                try
                {
                    EnvironmentType type = EnvironmentConfigHelper.GetResidenceType(job.GetSupportedEnv().Config);
                    TestEnvironment sutEnvironment = TestEnvironment.GetEnvironmentById(job.SUTEnvironmentId.Value);
                    //check the status of SUT and TestAgent, Only when them are ready, set the job as ready.
                    if (type == EnvironmentType.Residence_Together)
                    {
                        if (IsEnvironmentReady(sutEnvironment))
                        {
                            job.SetJobsStatus(JobStatus.Ready);
                            ATFEnvironment.Log.logger.Info(string.Format("Set job [{0}] status Assigned -> Ready", job.Name));
                        }
                    }
                    else if (type == EnvironmentType.Residence_Seperate)
                    {
                        TestEnvironment testAgentEnvironment = TestEnvironment.GetEnvironmentById(job.TestAgentEnvironmentId.Value);
                        if (IsEnvironmentReady(sutEnvironment) && IsEnvironmentReady(testAgentEnvironment))
                        {
                            job.SetJobsStatus(JobStatus.Ready);
                            ATFEnvironment.Log.logger.Info(string.Format("Set job [{0}] status Assigned -> Ready", job.Name));
                        }
                    }

                    //check whether the SUT and Test Agent met error when setup environment
                    if (type == EnvironmentType.Residence_Together)
                    {
                        if (IsEnvironmentError(sutEnvironment))
                        {
                            job.SetJobsStatus(JobStatus.Failed);
                            ATFEnvironment.Log.logger.Info(string.Format("Set job [{0}] status Assigned -> Failed", job.Name));
                        }
                    }
                    else if (type == EnvironmentType.Residence_Seperate)
                    {
                        TestEnvironment testAgentEnvironment = TestEnvironment.GetEnvironmentById(job.TestAgentEnvironmentId.Value);
                        if (IsEnvironmentError(sutEnvironment) || IsEnvironmentError(testAgentEnvironment))
                        {
                            //TODO, handle the environment, if the SUT met error, then we need to dispose the TestAgent, vice versa
                            job.SetJobsStatus(JobStatus.Failed);
                            ATFEnvironment.Log.logger.Info(string.Format("Set job [{0}] status Assigned -> Failed", job.Name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Met error when monitor the preparing job [{0}]", job.Name);
                    ATFEnvironment.Log.logger.Error(message, ex);
                }
            }

        }

        /// <summary>
        /// Monitor ready jobs
        /// 1. This function will never call in JobManagerService
        /// 2. Ready cases will be change to Running by saber Agent service in another thread.
        /// </summary>
        public static void MonitorReadyJobs()
        {
            ATFEnvironment.Log.logger.Debug("Monitor Ready Jobs");
            List<AutomationJob> readyJob = AutomationJob.GetJobs((int)JobStatus.Ready);
            if (readyJob == null)
            {
                ATFEnvironment.Log.logger.Debug("Ready Jobs count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Ready Jobs count:" + readyJob.Count().ToString());
            return;
        }

        /// <summary>
        /// Monitor running jobs
        /// </summary>
        public static void MonitorRunningJobs()
        {
            // AutomationLog Log = new AutomationLog("MicahTestConsole");
            ATFEnvironment.Log.logger.Debug("Monitor Running Jobs");
            List<AutomationJob> runningJob = AutomationJob.GetJobs((int)JobStatus.Running);
            if (runningJob == null)
            {
                ATFEnvironment.Log.logger.Debug("Running Jobs count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Running Jobs count:" + runningJob.Count().ToString());

            foreach (AutomationJob job in runningJob)
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    int countNotFinished = (from e in context.TestCaseExecutions
                                            where e.JobId == job.JobId &&
                                            (e.Status == (int)ExecutionStatus.NotRunning || e.Status == (int)ExecutionStatus.Running)
                                            select e).Count();

                    ATFEnvironment.Log.logger.Info("Job#" + job.Name.ToString() + " Executions are not finished or even not started:" + countNotFinished);

                    AutomationTask at = AutomationJob.GetTaskOfJobByJobId(job.JobId);

                    TestSuite suite = TestSuite.GetTestSuite(int.Parse(at.TestContent));

                    // For the NoneExisting test suite, we'll run and bash/bat file to call the automation scripts and parse the result at the end
                    // And the execution records are created at that stage instead of creating them before automation actually start
                    // And for the NoneExisting suite, we'll change the job status to Complete after the result parsing is done, so here we don't need do this.
                    if (countNotFinished == 0 && AutomationJob.GetTestCaseExecutionsOfJob(job.JobId).Count > 0 && suite.Type != (int)SuiteType.NotExisting)
                    {
                        job.SetJobsStatus(JobStatus.Complete);

                        ATFEnvironment.Log.logger.Info("Job#" + job.Name.ToString() + ": all execution finished, change status Running -> Complete");
                    }
                    else
                    {
                        if (System.DateTime.Compare(System.DateTime.UtcNow, job.ModifyDate.AddMilliseconds((double)(job.Timeout))) > 0)
                        {
                            string info = string.Format("The job [{0}:{1}] is timeout.", job.JobId, job.Name);
                            job.AddJobProgressInformation(info);
                            ATFEnvironment.Log.logger.Info(info);
                            job.SetJobsStatus(JobStatus.Timeout);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Monitor complete jobs
        /// 1. If all the tests are passed, release the environment
        /// 2. Else, kept the environment for a period
        /// </summary>
        public static void MonitorCompleteJobs()
        {
            ATFEnvironment.Log.logger.Debug("Monitor Completed Jobs");
            List<AutomationJob> completeJobs = AutomationJob.GetJobs((int)JobStatus.Complete);
            if (null == completeJobs || completeJobs.Count <= 0)
            {
                ATFEnvironment.Log.logger.Debug("No completed job to handle.");
                return;
            }
            else
            {
                ATFEnvironment.Log.logger.Debug("Completed Jobs count:" + completeJobs.Count().ToString());
                foreach (AutomationJob job in completeJobs)
                {
                    if (AreAllTestCasesFinishedSuccessfully(job))
                    {
                        //string info = string.Format("All tests are passed for Job {0}.", job.JobId);
                        //job.AddJobProgressInformation(info);
                        //ATFEnvironment.Log.logger.Info(info);

                        int environmentKeptTime = ATFConfiguration.GetIntValue("DefaultEnvironmentKeptTimeForPassedJob");
                        if (System.DateTime.Compare(System.DateTime.UtcNow, job.ModifyDate.AddMilliseconds((double)environmentKeptTime)) > 0)
                        {
                            string info = string.Format("The environment for job [{0}] has been kept for {1} minutes, discard the environment right now.", job.Name, environmentKeptTime / (1000 * 60));
                            job.AddJobProgressInformation(info);
                            ATFEnvironment.Log.logger.Info(info);
                            DiscardTestEnvironmentsOfJob(job);
                            job.SetJobsStatus(JobStatus.End);
                        }
                    }
                    else
                    {
                        int environmentKeptTime = ATFConfiguration.GetIntValue("DefaultEnvironmentKeptTimeForFailedJob");
                        if (System.DateTime.Compare(System.DateTime.UtcNow, job.ModifyDate.AddMilliseconds((double)environmentKeptTime)) > 0)
                        {
                            string info = string.Format("Some tests failed for Job [{0}], the environment has been kept for {1} minutes, so we discard it.", job.Name, environmentKeptTime / (1000 * 60));
                            job.AddJobProgressInformation(info);
                            ATFEnvironment.Log.logger.Info(info);
                            DiscardTestEnvironmentsOfJob(job);
                            job.SetJobsStatus(JobStatus.End);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Monitor failed jobs
        /// 1. Discard the job when the period to keep the environment passed.
        /// </summary>
        public static void MonitorFailedJobs()
        {
            ATFEnvironment.Log.logger.Debug("Monitor Failed Jobs");
            List<AutomationJob> failedJobs = AutomationJob.GetJobs((int)JobStatus.Failed);
            ATFEnvironment.Log.logger.Debug(string.Format("Failed Jobs count: {0}", failedJobs.Count));
            foreach (AutomationJob job in failedJobs)
            {
                //Handle the environment
                List<TestEnvironment> environments = GetAllTestEnvironmentsOfJob(job);
                int environmentKeptTime = ATFConfiguration.GetIntValue("DefaultEnvironmentKeptTimeForFailedJob");
                if (System.DateTime.Compare(System.DateTime.UtcNow, job.ModifyDate.AddMilliseconds((double)environmentKeptTime)) > 0)
                {
                    string info = string.Format("Job [{0}] was failed, and the environment has been kept for {1} seconds, so we discard it.", job.Name, environmentKeptTime / 1000);
                    job.AddJobProgressInformation(info);
                    ATFEnvironment.Log.logger.Info(info);
                    DiscardTestEnvironmentsOfJob(job);
                    job.SetJobsStatus(JobStatus.End);
                }
                //handle the executions 
                List<TestCaseExecution> executions = GetAllTestCaseExecutionsOfJob(job);
                foreach (TestCaseExecution execution in executions)
                {
                    if (execution.ExecutionStatus == ExecutionStatus.NotRunning)
                    {
                        execution.SetStatus(ExecutionStatus.Cancelled);
                    }
                    else if (execution.ExecutionStatus == ExecutionStatus.Running)
                    {
                        execution.SetStatus(ExecutionStatus.Fail);
                    }
                }
            }
        }

        /// <summary>
        /// Monitor time out jobs
        /// 1. We'll keep the the test environment for a while for users to triage the result
        /// 2. The test environment will be discarded after a while
        /// </summary>
        public static void MonitorTimeoutJobs()
        {
            List<AutomationJob> timeoutJobs = AutomationJob.GetJobs((int)JobStatus.Timeout);
            foreach (AutomationJob job in timeoutJobs)
            {
                int environmentKeptTime = ATFConfiguration.GetIntValue("DefaultEnvironmentKeptTimeForFailedJob");
                if (System.DateTime.Compare(System.DateTime.UtcNow, job.ModifyDate.AddMilliseconds((double)environmentKeptTime)) > 0)
                {
                    string info = string.Format("Job [{0}] was timeout, and the environment has been kept for {1} seconds, so we discard it.", job.Name, environmentKeptTime / 1000);
                    job.AddJobProgressInformation(info);
                    ATFEnvironment.Log.logger.Info(info);
                    DiscardTestEnvironmentsOfJob(job);
                    job.SetJobsStatus(JobStatus.End);
                }
            }
        }


        /// <summary>
        /// Monitor Cancelled Jobs
        /// 1. Only the job that has not been started to run can be cancelled
        /// 2. So there should be no execution record or result record yet.
        /// </summary>
        public static void MonitorCancelledJobs()
        {
            ATFEnvironment.Log.logger.Debug("Monitor Cancelled Jobs");
            List<AutomationJob> cancelledJobs = AutomationJob.GetJobs((int)JobStatus.Cancelled);
            foreach (AutomationJob job in cancelledJobs)
            {
                //handle the environment
                string info = string.Format("Job [{0}] was cancelled, discard the environment of it.", job.Name);
                job.AddJobProgressInformation(info);
                ATFEnvironment.Log.logger.Info(info);
                DiscardTestEnvironmentsOfJob(job);

                //handle the execution record
                IList<TestCaseExecution> executions = TestCaseExecution.GetTestCaseExectionByJob(job.JobId);
                if (null != executions)
                {
                    foreach (TestCaseExecution e in executions)
                    {
                        e.SetStatus(ExecutionStatus.Cancelled);
                    }
                }

                //handle the job itself
                job.SetJobsStatus(JobStatus.End);
            }
        }

        public static AutomationTask GetAutomationTaskOfJob(AutomationJob job)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                int taskId = (from taskjobmap in context.TaskJobMaps
                              where taskjobmap.JobId == job.JobId
                              select taskjobmap.TaskId).FirstOrDefault();
                return AutomationTask.GetAutomationTask(taskId);
            }
        }

        #region private methods

        private static bool DoesBeginToPrepareEnvironment(TestEnvironment environment)
        {
            if (environment.EnvironmentStatus == EnvironmentStatus.New ||
                environment.EnvironmentStatus == EnvironmentStatus.Setup ||
                environment.EnvironmentStatus == EnvironmentStatus.MachinesReady ||
                environment.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalledAndReady ||
                environment.EnvironmentStatus == EnvironmentStatus.BuildInstalled ||
                environment.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalling ||
                environment.EnvironmentStatus == EnvironmentStatus.Error ||
                environment.EnvironmentStatus == EnvironmentStatus.Ocuppied)//we'll handle the error when monitor the preparing job
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsEnvironmentReady(TestEnvironment environment)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            EnvironmentType type = config.Type;
            if (type == EnvironmentType.SUTAlone || type == EnvironmentType.Residence_Together)
            {
                if (environment.EnvironmentStatus == EnvironmentStatus.Ready)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type == EnvironmentType.TestAgentAlone)
            {
                if (environment.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalledAndReady ||
                    environment.EnvironmentStatus == EnvironmentStatus.Ready || environment.EnvironmentStatus == EnvironmentStatus.Ocuppied)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //code should never run to here
                return false;
            }
        }

        private static bool IsEnvironmentError(TestEnvironment environment)
        {
            if (environment.EnvironmentStatus == EnvironmentStatus.Error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool AreAllTestCasesFinishedSuccessfully(AutomationJob job)
        {
            bool r = true;
            List<TestResult> resultsList = JobManagement.GetAllTestResultsOfJob(job);
            foreach (TestResult result in resultsList)
            {
                if (result.ResultType == ResultType.Pass)
                {
                    continue;
                }
                else
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        private static void DiscardTestEnvironmentsOfJob(AutomationJob job)
        {
            List<TestEnvironment> environments = GetAllTestEnvironmentsOfJob(job);
            foreach (TestEnvironment e in environments)
            {
                if (e.Type != EnvironmentCreateType.Static)
                {
                    e.SetEnvironmentStatus(EnvironmentStatus.Discard);
                }
                else
                {
                    TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(e.Config);
                    //remove the job information from the config of the environment because it'll be reused by following new jobs.
                    config.TestAgentConfiguration.Categories.RemoveAll(category => category == "JobId=" + job.JobId.ToString());
                    e.SetEnvironmentConfig(config.ToXML());

                    if (config.Type == EnvironmentType.SUTAlone)
                    {
                        e.SetEnvironmentStatus(EnvironmentStatus.Disposed);
                        job.AddJobProgressInformation(string.Format("Free the SUT environment [{0}]", e.Name));
                    }
                    else if (config.Type == EnvironmentType.TestAgentAlone)
                    {
                        //shutdown the current running saber agent on the test agent
                        foreach (Machine m in config.TestAgentConfiguration.Machines)
                        {
                            if (EnvironmentManager.IsSaberAgentRequiredInThisMachine(m))
                            {
                                string ip = string.IsNullOrEmpty(m.ExternalIP) ? m.IP : m.ExternalIP;
                                string domainName = config.TestAgentConfiguration.TestAgentDomainConfig.Name;
                                string domainAdmin = config.TestAgentConfiguration.TestAgentDomainConfig.Adminstrator;
                                string domainAdminPassword = config.TestAgentConfiguration.TestAgentDomainConfig.Password;
                                Common.ScriptCommon.CMDScript.CloseRunningApplicationRemotely(ip, domainName + @"\" + domainAdmin, domainAdminPassword, Core.AgentName.WindowsFormApp);
                            }
                        }
                        e.SetEnvironmentStatus(EnvironmentStatus.Disposed);
                        job.AddJobProgressInformation(string.Format("Free the Test Agent environment [{0}]", e.Name));
                    }
                    else
                    {
                        //code should not arrive here.
                    }
                }
            }
        }
        #endregion
        public static List<TestResult> GetAllTestResultsOfJob(AutomationJob job)
        {
            List<TestResult> results = null;
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                results = (from tr in context.TestResults
                           where (from e in context.TestCaseExecutions
                                  where e.JobId == job.JobId
                                  select e.ExecutionId).Contains(tr.ExecutionId)
                           select tr).ToList();
            }
            return results;
        }

        public static List<TestCaseExecution> GetAllTestCaseExecutionsOfJob(AutomationJob job)
        {
            List<TestCaseExecution> executions = null;
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                executions = (from te in context.TestCaseExecutions
                              where te.JobId == job.JobId
                              select te).ToList();
            }
            return executions;
        }

        public static List<TestEnvironment> GetAllTestEnvironmentsOfJob(AutomationJob job)
        {
            List<TestEnvironment> environments = null;
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                environments = (from te in context.TestEnvironments
                                where (job.SUTEnvironmentId == te.EnvironmentId ||
                                job.TestAgentEnvironmentId == te.EnvironmentId)
                                select te).ToList();
            }
            return environments;
        }

    }
}
