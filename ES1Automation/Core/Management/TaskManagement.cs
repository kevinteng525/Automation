using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Xml.Linq;

using Core.Model;
using ES1Common.Logs;
using Common.Windows;
using Common.MailCommon;
using Common.AssemblyCommon;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Common.FileCommon;
using Common.Network;



namespace Core.Management
{
    public class TaskManagement
    {
        /// <summary>
        /// Monitor scheduled tasks
        /// </summary>
        public static void MonitorScheduledTasks()
        {
            string message = string.Empty;
            ATFEnvironment.Log.logger.Debug("Monitor scheduled tasks");
            List<AutomationTask> tasksScheduled = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Scheduled);
            if (tasksScheduled == null)
            {
                ATFEnvironment.Log.logger.Debug("Scheduled tasks count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Scheduled tasks count:" + tasksScheduled.Count().ToString());
            foreach (AutomationTask task in tasksScheduled)
            {
                if (task.RecurrencePattern == (int)RecurrencePattern.AtOnce)
                {
                    // Check for Build
                    message = "Checking specific build for Task: " + task.Name.ToString();
                    ATFEnvironment.Log.logger.Info(message);
                    task.AddProgressInformation(message);
                    if (!GetBuildAndCheckStatusOfItForTask(task))
                    {
                        continue;
                    }

                    // Check for Enviroment
                    ATFEnvironment.Log.logger.Info("Checking specific Enviromnet for Task: " + task.Name.ToString());
                    if (!GetSupporttedEnvironmentAndCheckStatusOfItForTask(task))
                    {
                        continue;
                    }

                    // Get all the test cases for this task, update the test suite(id=task.TestContent) to contains only test cases
                    // The test suite's content may be updated in future, so here we convert the sub test suites into test cases
                    // This is to handle the scenario that after the task is finished, when user look the historical test result, the test case of the task should not be modified.
                    TestSuite testSuite = TestSuite.GetTestSuite(int.Parse(task.TestContent));
                    // Only run the active case
                    List<int> allCasesForTask = null;
                    while (true)
                    {
                        if (!ATFConfiguration.IsTestCasesSuitesSyncing())
                        {
                            List<TestCase> testCases = TestSuite.GetAllCases(testSuite.SuiteId, true);
                            allCasesForTask = testCases.Select(tc => tc.TestCaseId).ToList();
                            break;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(30 * 1000);
                            task.AddProgressInformation(string.Format("Test cases or suites are being syncing now. Wait 30 seconds."));
                        }
                    }

                    if (allCasesForTask == null || allCasesForTask.Count() <= 0 && testSuite.Type != (int)SuiteType.NotExisting)
                    {
                        ATFEnvironment.Log.logger.Error("No cases to run in Task [" + task.Name + @"], suiteId= " + task.TestContent.ToString());
                        task.AddProgressInformation("No cases to run.");
                        task.SetTaskStatus(TaskStatus.Complete);
                        continue;
                    }
                    else if (testSuite.Type == (int)SuiteType.NotExisting)
                    {
                        try                         // create one job
                        {
                            AutomationJob job = new AutomationJob()
                            {
                                Name = "JobForTask " + task.TaskId.ToString(),
                                Status = (int)JobStatus.Assigned,
                                Type = (int)JobType.Sequence,
                                Priority = task.Priority,
                                RetryTimes = 1,
                                Timeout = ATFConfiguration.GetIntValue("DefaultAutomationJobTimeout"),//1 hours
                                CreateDate = System.DateTime.UtcNow,
                                ModifyDate = System.DateTime.UtcNow,
                                CreateBy = 0,
                                ModifyBy = 0,
                            };
                            AutomationJob bCreateJob = AutomationJob.CreateJob(job);

                            message = string.Format("Job [{0}:{1}] is created.", bCreateJob.JobId, bCreateJob.Name);
                            task.AddProgressInformation(message);
                            ATFEnvironment.Log.logger.Info(message);

                            TaskJobMap map = new TaskJobMap()
                            {
                                TaskId = task.TaskId,
                                JobId = job.JobId,
                            };
                            TaskJobMap.CreateMap(map);

                            task.SetTaskStatus(TaskStatus.Dispatched);
                            message = string.Format("Change Task [{0}] status Scheduled to Dispatched", task.Name);
                            ATFEnvironment.Log.logger.Info(message);
                            task.AddProgressInformation(message);
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("Failed to dispatch task [{0}] to jobs.", task.Name);
                            ATFEnvironment.Log.logger.Error(message, ex);
                            task.AddProgressInformation(message);
                            foreach (AutomationJob job in task.GetJobs())
                            {
                                job.SetJobsStatus(JobStatus.Cancelled);
                            }
                            task.SetTaskStatus(TaskStatus.Failed);
                        }
                    }
                    else
                    {
                        message = string.Format("Totally {0} cases selected.", allCasesForTask.Count);
                        task.AddProgressInformation(message);
                        ATFEnvironment.Log.logger.Info(message);
                        allCasesForTask = allCasesForTask.Distinct().ToList();
                        message = string.Format("Totally {0} distinct cases selected after dedupe.", allCasesForTask.Count);
                        task.AddProgressInformation(message);
                        ATFEnvironment.Log.logger.Info(message);
                        //take the snapshot of the test cases and save it to the task
                        string testCaseIdList = string.Empty;
                        foreach (int caseId in allCasesForTask)
                        {
                            if (string.IsNullOrEmpty(testCaseIdList))
                            {
                                testCaseIdList = caseId.ToString();
                            }
                            else
                            {
                                testCaseIdList = string.Format("{0},{1}", testCaseIdList, caseId);
                            }
                        }

                        // For temporary suite, we'll modify the suite in place, else we'll create another temporary suite to contain the cases at present in the suite.
                        if (testSuite.Type == (int)SuiteType.Temporary)
                        {
                            testSuite.SubSuites = "";
                            testSuite.TestCases = testCaseIdList;
                            testSuite.Update();
                        }
                        else
                        {
                            TestSuite tempSuite = new TestSuite
                            {
                                ProviderId = testSuite.ProviderId,
                                SourceId = testSuite.SourceId,
                                Name = testSuite.Name,
                                Type = (int)SuiteType.Temporary,
                                SubSuites = "",
                                TestCases = testCaseIdList,
                                IsActive = true,
                                CreateBy = 0,
                                ModityBy = 0,
                                Description = testSuite.Description,
                                CreateTime = System.DateTime.UtcNow,
                                ModifyTime = System.DateTime.UtcNow,
                            };
                            TestSuite newSuite = TestSuite.CreateSuite(tempSuite);
                            task.SetTestContent(newSuite.SuiteId.ToString());
                        }

                        // Remove the test case which is not suitable for the support environment
                        allCasesForTask = allCasesForTask.FindAll(delegate(int caseId)
                        {
                            if (CouldThisTestCaseRunOnSupporttedEnvironment(caseId, task.EnvironmentId))
                            {
                                return true;
                            }
                            else
                            {
                                TestCase testCase = TestCase.GetTestCase(caseId);
                                SupportedEnvironment testEnvironment = SupportedEnvironment.GetSupportedEnvironmentById(task.EnvironmentId);
                                message = string.Format(@"Test case [{0}](Platform={1}) could not run on the environment [{2}](Platform={3})", testCase.Name, testCase.GetPlatform().ToString(), testEnvironment.Name, testEnvironment.GetPlatformOfEnvironment().ToString());
                                ATFEnvironment.Log.logger.Warn(message);
                                task.AddProgressInformation(message);
                                return false;
                            }
                        });

                        // Dispatch to JOBs
                        message = string.Format("Dispatching to Jobs for Task [{0}]", task.Name);
                        ATFEnvironment.Log.logger.Info(message);
                        task.AddProgressInformation(message);

                        try
                        {
                            DispatchTaskToJobs(task, allCasesForTask);
                            task.SetTaskStatus(TaskStatus.Dispatched);
                            message = string.Format("Change Task [{0}] status Scheduled to Dispatched", task.Name);
                            ATFEnvironment.Log.logger.Info(message);
                            task.AddProgressInformation(message);
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("Failed to dispatch task [{0}] to jobs.", task.Name);
                            ATFEnvironment.Log.logger.Error(message, ex);
                            task.AddProgressInformation(message);
                            foreach (AutomationJob job in task.GetJobs())
                            {
                                job.SetJobsStatus(JobStatus.Cancelled);
                            }
                            task.SetTaskStatus(TaskStatus.Failed);
                        }
                    }

                }
                else if (task.RecurrencePattern == (int)RecurrencePattern.OneTime)
                {
                    CreateOneTimeWindowsScheduledTask(task);
                    ATFEnvironment.Log.logger.Info("Change Task [" + task.Name.ToString() + "] status Scheduled to Scheduling");
                    task.AddProgressInformation("Change status Scheduled to Scheduling");
                    task.SetTaskStatus(TaskStatus.Scheduling);

                }
                else if (task.RecurrencePattern == (int)RecurrencePattern.Weekly)
                {
                    CreateWeeklyWindowsScheduledTask(task);
                    ATFEnvironment.Log.logger.Info("Change Task [" + task.Name.ToString() + "] status Scheduled to Scheduling");
                    task.AddProgressInformation("Change status Scheduled to Completed");
                    task.SetTaskStatus(TaskStatus.Scheduling);
                }
            }
        }

        /// <summary>
        /// Monitor dispatched tasks
        /// </summary>
        public static void MonitorDispatchedTasks()
        {
            //AutomationLog Log = new AutomationLog("MicahTestConsole");
            ATFEnvironment.Log.logger.Debug("Monitor dispatched tasks");
            List<AutomationTask> tasksDispatched = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Dispatched);
            if (tasksDispatched == null)
            {
                ATFEnvironment.Log.logger.Debug("Dispatched tasks count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Dispatched tasks count:" + tasksDispatched.Count().ToString());

            foreach (AutomationTask task in tasksDispatched)
            {
                ATFEnvironment.Log.logger.Info("Checking Jobs' status for Task " + task.Name.ToString());

                List<AutomationJob> jobs = task.GetJobs();
                if (jobs == null || jobs.Count() <= 0)
                {
                    ATFEnvironment.Log.logger.Warn("No jobs for Task " + task.Name.ToString());
                    task.SetTaskStatus(TaskStatus.Failed);
                    ATFEnvironment.Log.logger.Info("Change Task " + task.TaskId.ToString() + " status from Dispatched to Failed");
                    task.AddProgressInformation("Change status: Dispatched to " + "Failed");
                    continue;
                }

                if (IsOneJobOfTaskStarted(task))
                {
                    task.SetTaskStatus(TaskStatus.Running);
                    string message = string.Format("Change Task [{0}] status from Dispatched to Running", task.Name);
                    ATFEnvironment.Log.logger.Info(message);
                    task.AddProgressInformation(message);
                }
                else
                {
                    string message = string.Format("None jobs of Task [{0}] has been started, continue monitor.", task.Name);
                    ATFEnvironment.Log.logger.Info(message);
                }
            }
        }

        /// <summary>
        /// Monitor the Running tasks
        /// </summary>
        public static void MonitorRunningTasks()
        {
            //AutomationLog Log = new AutomationLog("MicahTestConsole");
            ATFEnvironment.Log.logger.Debug("Monitor running tasks");
            List<AutomationTask> tasksRunning = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Running);
            if (tasksRunning == null)
            {
                ATFEnvironment.Log.logger.Debug("Running tasks count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Running tasks count:" + tasksRunning.Count().ToString());

            foreach (AutomationTask task in tasksRunning)
            {
                ATFEnvironment.Log.logger.Info("Checking Jobs' status for Task " + task.TaskId.ToString());

                List<AutomationJob> jobs = task.GetJobs();
                if (jobs == null || jobs.Count() <= 0)
                {
                    ATFEnvironment.Log.logger.Warn("No jobs for Task " + task.Name.ToString());
                    task.SetTaskStatus(TaskStatus.Failed);
                    ATFEnvironment.Log.logger.Info("Change Task [" + task.Name.ToString() + "] status from Running to Failed");
                    task.AddProgressInformation("Change status: Running to " + "Failed");
                    continue;
                }
                if (AreAllJobsOfTaskFinished(task))
                {
                    ATFEnvironment.Log.logger.Info("Change Task [" + task.Name.ToString() + "] all jobs complete, change status Running to Complete");

                    AutomationTask parentTask = AutomationTask.GetAutomationTask(task.ParentTaskId.GetValueOrDefault());
                    if (null != parentTask && parentTask.RecurrencePattern == (int)RecurrencePattern.OneTime)
                    {
                        parentTask.SetTaskStatus(TaskStatus.Complete);
                        ATFEnvironment.Log.logger.Info("Change Task [" + parentTask.Name.ToString() + "] all jobs complete, change status Scheduling to Complete");
                    }

                    string message = string.Format("All jobs of Task [{0}] are complete, change status Running to Complete", task.Name);
                    task.SetTaskStatus(TaskStatus.Complete);
                    task.AddProgressInformation(message);
                    ATFEnvironment.Log.logger.Info(message);

                }
                else
                {
                    string message = string.Format("Not all jobs of Task [{0}] are complete, continue monitor", task.Name);
                    ATFEnvironment.Log.logger.Info(message);
                }
            }
        }


        /// <summary>
        /// Monitor the Cancelling tasks
        /// </summary>
        public static void MonitorCancellingTasks()
        {
            ATFEnvironment.Log.logger.Debug("Monitor cancelling tasks");
            List<AutomationTask> tasksCancelling = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Cancelling);
            if (tasksCancelling == null)
            {
                ATFEnvironment.Log.logger.Debug("Cancelling tasks count: 0");
                return;
            }
            ATFEnvironment.Log.logger.Debug("Cancelling tasks count:" + tasksCancelling.Count().ToString());
            List<AutomationTask> needCancelTask = new List<AutomationTask>();
            foreach (AutomationTask task in tasksCancelling)
            {
                if (task.RecurrencePattern.GetValueOrDefault() != (int)RecurrencePattern.AtOnce)
                {
                    List<AutomationTask> childrenTasks = AutomationTask.GetChildrenTask(task.TaskId);
                    if (childrenTasks != null)
                    {
                        foreach (AutomationTask t in childrenTasks)
                        {
                            if (t.Status <= (int)TaskStatus.Running)
                            {
                                needCancelTask.Add(t);
                            }
                        }
                    }
                    task.SetTaskStatus(TaskStatus.Cancelled);
                    ATFEnvironment.Log.logger.Info(string.Format("Delete the windows scheduled tasks [{0}]", string.Format("[Scheduled] {0}", task.Name)));
                    ScheduledTask.DeleteWindowsScheduledTask(string.Format("[Scheduled] {0}", task.Name));
                }
                else
                {
                    needCancelTask.Add(task);
                }
            }

            foreach (AutomationTask task in needCancelTask)
            {
                ATFEnvironment.Log.logger.Info("Checking Jobs' status for Task " + task.Name.ToString());

                List<AutomationJob> jobs = task.GetJobs();
                if (jobs == null || jobs.Count() <= 0)
                {
                    ATFEnvironment.Log.logger.Warn("No jobs for Task " + task.TaskId.ToString());
                    ATFEnvironment.Log.logger.Info("Change Task [" + task.Name.ToString() + "] status from Cancelling to Failed");
                    task.SetTaskStatus(TaskStatus.Failed);
                    task.AddProgressInformation("No Running jobs, change status Cancelling to Failed");
                    continue;
                }

                foreach (AutomationJob job in jobs)
                {
                    ATFEnvironment.Log.logger.Info("Job [" + job.Name.ToString() + "] status: " + (JobStatus)job.Status);

                    switch (job.JobStatus)
                    {
                        case JobStatus.Assigned:
                        case JobStatus.Preparing:
                        case JobStatus.Ready:
                            {
                                // Handle Job
                                job.SetJobsStatus(JobStatus.Cancelled);
                                ATFEnvironment.Log.logger.Info("Job [" + job.Name.ToString() + "] Set job status to Cancelled");
                                break;
                            }
                        case JobStatus.Running:
                            {
                                // Let the running job continue running, 
                                // when finished, the environment will be disposed by environment manager,
                                // and the execution will be modified as "Complete"(or "Failed" if error occures) by Execution Engine.
                                // The job will be marked as "Completed"(or "Failed" if error occures).
                                break;
                            }
                        case JobStatus.Complete:
                        case JobStatus.Failed:
                        case JobStatus.Paused:
                        case JobStatus.Cancelled:
                            break;
                        case JobStatus.End:
                            break;
                        default:
                            break;
                    }
                }

                if (AreAllJobsOfTaskFinished(task))
                {
                    ATFEnvironment.Log.logger.Info("Task [" + task.Name.ToString() + "] no Running jobs, change status Cancelling to Cancelled");
                    task.SetTaskStatus(TaskStatus.Cancelled);
                    task.AddProgressInformation("No Running jobs, change status Cancelling to Cancelled");
                }
            }
        }

       
        /// <summary>
        /// Mointor the completed tasks
        /// 1. currently do nothing here.
        /// </summary>
        public static void MonitorCompleteTasks()
        {
            List<AutomationTask> completeTasks = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Complete);
            if (completeTasks != null)
            {
                foreach (AutomationTask task in completeTasks)
                {
                    try
                    {
                        SendTestReportOfTaskToStakeholders(task);
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error("Failed to send test report to the stakeholders.", ex);
                    }

                    try
                    {
                        WriteTestResultBackToTestCaseProvider(task);
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error("Failed to write back the test result to RQM.", ex);
                    }

                    task.SetTaskStatus(TaskStatus.End);
                }
            }
        }

        /// <summary>
        /// Monitor failed tasks
        /// 1. Currently do nothing here
        /// </summary>
        public static void MonitorFailedTasks()
        {
            List<AutomationTask> failedTasks = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Failed);
            if (failedTasks != null)
            {
                foreach (AutomationTask task in failedTasks)
                {
                    SendTestReportOfTaskToStakeholders(task);
                    WriteTestResultBackToTestCaseProvider(task);
                    task.SetTaskStatus(TaskStatus.End);
                }
            }
        }

        /// <summary>
        /// Monitor cancelled tasks
        /// 1. Currently do nothing here
        /// </summary>
        public static void MonitorCancelledTasks()
        {
            List<AutomationTask> tasksCanceled = AutomationTask.GetActiveAutomationTask((int)TaskStatus.Cancelled);
            if (tasksCanceled != null)
            {
                foreach (AutomationTask task in tasksCanceled)
                {
                    SendTestReportOfTaskToStakeholders(task);
                    WriteTestResultBackToTestCaseProvider(task);
                    task.SetTaskStatus(TaskStatus.End);
                }
            }          
        }

        /// <summary>
        /// Write the test result back to provider of test case
        /// </summary>
        /// <param name="task">the automation task</param>
        public static void WriteTestResultBackToTestCaseProvider(AutomationTask task)
        {
            if (task.WriteTestResultBack.HasValue && task.WriteTestResultBack.Value)
            {
                Product product = Product.GetProductByID(task.ProductId.Value);
                Provider provider = Provider.GetProviderById(product.TestCaseProviderId.Value);
                Providers.TestCaseProviders.ITestCaseProvider testCaseProvider = provider.CreateProvider() as Providers.TestCaseProviders.ITestCaseProvider;
                testCaseProvider.WriteBackTestResult(task);
            }
        }

        #region private methodes

        /// <summary>
        /// Dispatch task to jobs based on the test suites defined for dependance
        /// </summary>
        /// <param name="task"></param>
        /// <param name="allCasesForTask"></param>
        private static void DispatchTaskToJobs(AutomationTask task, List<int> allCasesForTask)
        {
            int i = 0;
            string message = string.Empty;
            //Store whether the test cases have been dispatched to job or not based on dependency test suite
            Dictionary<string, bool> testCaseDispatchIndicator = new Dictionary<string, bool>();
            int testcaseProviderId = Product.GetProductByID(task.ProductId.GetValueOrDefault()).TestCaseProviderId.GetValueOrDefault();

            List<TestSuite> dependenceSuites = TestSuite.GetTestSuitesByProviderIdAndType(testcaseProviderId, SuiteType.Dependence).ToList();
            foreach (TestSuite dependenceSuite in dependenceSuites)
            {
                string testCasesForJob = string.Empty;
                foreach (string tc in dependenceSuite.TestCases.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (allCasesForTask.Contains(int.Parse(tc)) && !testCaseDispatchIndicator.ContainsKey(tc))
                    {
                        testCasesForJob = testCasesForJob == string.Empty ? tc : testCasesForJob + ',' + tc;
                        testCaseDispatchIndicator.Add(tc, true);
                    }
                }
                if (testCasesForJob != string.Empty)
                {
                    // Create AutomationJob
                    ++i;
                    AutomationJob job = new AutomationJob()
                    {
                        Name = "JobForTask " + task.TaskId.ToString() + "_" + i.ToString(),
                        Status = (int)JobStatus.Assigned,
                        Type = (int)JobType.Sequence,
                        Priority = task.Priority,
                        RetryTimes = 1,
                        Timeout = ATFConfiguration.GetIntValue("DefaultAutomationJobTimeout"),//1 hours
                        CreateDate = System.DateTime.UtcNow,
                        ModifyDate = System.DateTime.UtcNow,
                        CreateBy = 0,//automation, pre-defined user
                        ModifyBy = 0,//automation, pre-defined user
                    };
                    AutomationJob bCreateJob = AutomationJob.CreateJob(job);

                    message = string.Format("Job [{0}] is created.", bCreateJob.JobId);
                    task.AddProgressInformation(message);
                    ATFEnvironment.Log.logger.Info(message);

                    TaskJobMap map = new TaskJobMap()
                    {
                        TaskId = task.TaskId,
                        JobId = job.JobId,
                    };
                    TaskJobMap.CreateMap(map);

                    foreach (string id in testCasesForJob.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        TestCaseExecution ex = new TestCaseExecution()
                        {
                            TestCaseId = int.Parse(id),
                            JobId = job.JobId,
                            Status = (int)ExecutionStatus.NotRunning,
                            StartTime = null,
                            EndTime = null,
                            RetryTimes = 0,
                            Timeout = ATFEnvironment.DefaultTestCaseTimeout,
                        };
                        TestCaseExecution exInDB = TestCaseExecution.CreateExecution(ex);

                        message = string.Format("Execution [{0}] is created.", exInDB.ExecutionId);
                        //task.AddProgressInformation(message);
                        ATFEnvironment.Log.logger.Info(message);

                        TestResult result = new TestResult
                        {
                            ExecutionId = exInDB.ExecutionId,
                            Result = (int)ResultType.NotRun,
                            IsTriaged = false,
                            TriagedBy = null,
                            Files = null,
                        };
                        TestResult.CreateRunResult(result);

                        message = string.Format("Test result [{0}] is created.", result.ResultId);
                        //task.AddProgressInformation(message);
                        ATFEnvironment.Log.logger.Info(message);
                    }
                }
            }

            foreach (int testcaseId in allCasesForTask)
            {
                if (!testCaseDispatchIndicator.ContainsKey(testcaseId.ToString()))//not dispatched to jobs yet
                {
                    // Create AutomationJob
                    ++i;
                    AutomationJob job = new AutomationJob()
                    {
                        Name = "JobForTask " + task.TaskId.ToString() + "_" + i.ToString(),
                        Status = (int)JobStatus.Assigned,
                        Type = (int)JobType.Sequence,
                        Priority = task.Priority,
                        RetryTimes = 1,
                        Timeout = ATFConfiguration.GetIntValue("DefaultAutomationJobTimeout"),//1 hours
                        CreateDate = System.DateTime.UtcNow,
                        ModifyDate = System.DateTime.UtcNow,
                        CreateBy = 0,//automation, pre-defined user
                        ModifyBy = 0,//automation, pre-defined user
                    };
                    AutomationJob bCreateJob = AutomationJob.CreateJob(job);

                    message = string.Format("Job [{0}] is created.", bCreateJob.JobId);
                    task.AddProgressInformation(message);
                    ATFEnvironment.Log.logger.Info(message);

                    TaskJobMap map = new TaskJobMap()
                    {
                        TaskId = task.TaskId,
                        JobId = job.JobId,
                    };
                    TaskJobMap.CreateMap(map);

                    TestCaseExecution ex = new TestCaseExecution()
                    {
                        TestCaseId = testcaseId,
                        JobId = job.JobId,
                        Status = (int)ExecutionStatus.NotRunning,
                        StartTime = null,
                        EndTime = null,
                        RetryTimes = 0,
                        Timeout = ATFEnvironment.DefaultTestCaseTimeout,
                    };
                    TestCaseExecution exInDB = TestCaseExecution.CreateExecution(ex);

                    message = string.Format("Execution [{0}] is created.", exInDB.ExecutionId);
                    //task.AddProgressInformation(message);
                    ATFEnvironment.Log.logger.Info(message);

                    TestResult result = new TestResult
                    {
                        ExecutionId = exInDB.ExecutionId,
                        Result = (int)ResultType.NotRun,
                        IsTriaged = false,
                        TriagedBy = null,
                        Files = null,
                    };
                    TestResult.CreateRunResult(result);

                    message = string.Format("Test result [{0}] is created.", result.ResultId);
                    //task.AddProgressInformation(message);
                    ATFEnvironment.Log.logger.Info(message);
                }
            }
        }

        private static bool CouldThisTestCaseRunOnSupporttedEnvironment(int caseId, int supporttedEnvironmentId)
        {
            TestCase testCase = TestCase.GetTestCase(caseId);
            Platform testCasePlatform = testCase.GetPlatform();
            if (testCasePlatform == Platform.All)
            {
                return true;
            }
            else if (testCasePlatform == Platform.Undefined)
            {
                return false;
            }
            
            SupportedEnvironment supporttedEnvironment = SupportedEnvironment.GetSupportedEnvironmentById(supporttedEnvironmentId);
            Platform environmentPlatform = supporttedEnvironment.GetPlatformOfEnvironment();
            if (environmentPlatform == Platform.Undefined)
            {
                return false;
            }
            else if (testCasePlatform == environmentPlatform)
            {
                return true;
            }
            else
            {
                return false;
            }           
        }

        private static bool GetSupporttedEnvironmentAndCheckStatusOfItForTask(AutomationTask task)
        {
            SupportedEnvironment supEnv = SupportedEnvironment.GetSupportedEnvironmentById(task.EnvironmentId);
            if (supEnv == null)
            {
                ATFEnvironment.Log.logger.Error("Task: " + task.TaskId.ToString() + " ,Supported Environment ERROR");
                task.AddProgressInformation("Supported Environment ERROR");
                task.SetTaskStatus(TaskStatus.Failed);
                return false;
            }
            else
            {
                string message = string.Format("The support environment is ready for task [{0}]", task.Name);
                ATFEnvironment.Log.logger.Info(message);
                task.AddProgressInformation(message);
                return true;
            }
        }

        private static bool GetBuildAndCheckStatusOfItForTask(AutomationTask task)
        {
            //if build id = 0, we need to get the latest mainline build for this task
            if (task.BuildId == 0)
            {
                Build latestBuild = BuildManager.GetLatestBuild(task);
                if (null != latestBuild)
                {
                    string message = string.Format("The latest mainline build is {0}", latestBuild.Name);
                    ATFEnvironment.Log.logger.Info(message);
                    task.AddProgressInformation(message);
                    task.SetBuild(latestBuild.BuildId);
                }
                else
                {
                    string message = string.Format("Could not find the latest build for task {0}", task.Name);
                    ATFEnvironment.Log.logger.Error(message);
                    task.AddProgressInformation(message);
                    task.SetTaskStatus(TaskStatus.Failed);
                    return false;
                }
            }

            // Check whether build is ready
            if (!IsBuildOfTaskReady(task))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Check whether one of the job has been started of the task
        /// </summary>
        /// <param name="task">task</param>
        /// <returns></returns>
        private static bool IsOneJobOfTaskStarted(AutomationTask task)
        {
            List<AutomationJob> jobs = task.GetJobs();
            if (jobs == null || jobs.Count() <= 0)
            {
                ATFEnvironment.Log.logger.Warn("No jobs for Task " + task.TaskId.ToString());
                task.SetTaskStatus(TaskStatus.Failed);
                ATFEnvironment.Log.logger.Info("Change Task " + task.TaskId.ToString() + " status from Dispatched to Failed");
                task.AddProgressInformation("Change status: Dispatched to " + "Failed");
                return false;
            }
            else
            {
                foreach (AutomationJob job in jobs)
                {
                    if (job.Status != (int)JobStatus.Assigned)//The initial status of job
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Check whether all the jobs has been finished or not of the task
        /// 1. The 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static bool AreAllJobsOfTaskFinished(AutomationTask task)
        {
            List<AutomationJob> jobs = task.GetJobs();
            if (jobs == null || jobs.Count() <= 0)
            {
                ATFEnvironment.Log.logger.Warn("No jobs for Task " + task.TaskId.ToString());
                task.SetTaskStatus(TaskStatus.Failed);
                ATFEnvironment.Log.logger.Info("Change Task " + task.TaskId.ToString() + " status from Dispatched to Failed");
                task.AddProgressInformation("Change status: Dispatched to " + "Failed");
                return true;
            }
            else
            {
                foreach (AutomationJob job in jobs)
                {
                    if (job.Status != (int)JobStatus.End
                        && job.Status != (int)JobStatus.Failed
                        && job.Status != (int)JobStatus.Complete
                        && job.Status != (int)JobStatus.Cancelled
                        && job.Status != (int)JobStatus.Timeout)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static bool IsBuildOfTaskReady(AutomationTask task)
        {
            Build build = Build.GetBuildById(task.BuildId);
            switch (build.Status)
            {
                case (int)BuildStatus.Success:
                    {
                        string message = string.Format("The build [{0}] is ready.", build.Name);
                        ATFEnvironment.Log.logger.Info(message);
                        task.AddProgressInformation(message);
                        return true;
                    }
                case (int)BuildStatus.Failed:
                    {
                        string message = string.Format("The build [{0}] is failed.", build.Name);
                        ATFEnvironment.Log.logger.Info(message);
                        task.AddProgressInformation(message);
                        task.SetTaskStatus(TaskStatus.Failed);
                        return false;
                    }
                case (int)BuildStatus.NotExist:
                    {
                        string message = string.Format("The build [{0}] doesn't exist.", build.Name);
                        ATFEnvironment.Log.logger.Info(message);
                        task.AddProgressInformation(message);
                        task.SetTaskStatus(TaskStatus.Failed);
                        return false;
                    }
                case (int)BuildStatus.Delete:
                    {
                        string message = string.Format("The build [{0}] has been deleted.", build.Name);
                        ATFEnvironment.Log.logger.Info(message);
                        task.AddProgressInformation(message);
                        task.SetTaskStatus(TaskStatus.Failed);
                        return false;
                    }
                default:
                    {
                        string message = string.Format("The status of build {0} is invalid.", build.Name);
                        ATFEnvironment.Log.logger.Error(message);
                        task.AddProgressInformation(message);
                        task.SetTaskStatus(TaskStatus.Failed);
                        return false;
                    }
            }
        }

        private static void CreateOneTimeWindowsScheduledTask(AutomationTask task)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string administrator = ConfigurationManager.AppSettings["TaskManagerHostAdministrator"];
            string password = ConfigurationManager.AppSettings["TaskManagerHostPassword"];
            string taskName = string.Format("[Scheduled] {0}", task.Name);
            string taskDescription = string.Format("Galaxy-Scheduled-Task-For-Automation-Task-{0}", task.Name);
            int notifyStakeholders = task.NotifyStakeholders.HasValue && task.NotifyStakeholders.Value ? 1 : 0;
            int writeTestResultBack = task.WriteTestResultBack.HasValue && task.WriteTestResultBack.Value ? 1 : 0;
            string programPath = "ruby.exe";
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(@"""{0}""", ConfigurationManager.AppSettings["WindowsScheduleTaskProgramPath"]);
            string format = @" '{{ ""Name"":""{0}"",""Status"":{1},""Type"":{2},""Priority"":{3},""CreateBy"":{4},""ModifyBy"":{5},""BuildId"":{6},""EnvironmentId"":{7},""TestContent"":{8},""Description"":""{9}"",""RecurrencePattern"":{10},""StartDate"":{11},""StartTime"":{12},""WeekDays"":{13},""WeekInterval"":{14}, ""ParentTaskId"":{15}, ""BranchId"":{16}, ""ReleaseId"":{17}, ""ProductId"":{18}, ""ProjectId"":{19}, ""NotifyStakeholders"":{20}, ""WriteTestResultBack"":{21}, ""SetupScript"":""{22}"", ""TeardownScript"":""{23}"" }}'";
            builder.AppendFormat(format, taskName, (int)TaskStatus.Scheduled, task.Type, 0, 0, 0, task.BuildId, task.EnvironmentId, task.TestContent, taskDescription, (int)RecurrencePattern.AtOnce, serializer.Serialize(task.StartDate.GetValueOrDefault()), serializer.Serialize(task.StartTime.GetValueOrDefault()), task.WeekDays.GetValueOrDefault(), task.WeekInterval.GetValueOrDefault(), task.TaskId, task.BranchId.GetValueOrDefault(), task.ReleaseId.GetValueOrDefault(), task.ProductId.GetValueOrDefault(), task.ProjectId, notifyStakeholders, writeTestResultBack, task.SetupScript, task.TeardownScript);
            string programParameters = builder.ToString();
            string programeWorkingDirectory = ConfigurationManager.AppSettings["WindowsScheduleTaskProgramWorkingDirectory"];
            DateTime startDateTime = task.StartDate.Value;            
            ScheduledTask.CreateWindowsScheduledOneTimeTask(administrator, password, taskName, taskDescription, programPath, programParameters, programeWorkingDirectory, startDateTime);
        }

        private static void CreateWeeklyWindowsScheduledTask(AutomationTask task)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string administrator = ConfigurationManager.AppSettings["TaskManagerHostAdministrator"];
            string password = ConfigurationManager.AppSettings["TaskManagerHostPassword"];
            string taskName = string.Format("[Scheduled] {0}", task.Name);
            string taskDescription = string.Format("Galaxy-Scheduled-Task-For-Automation-Task-{0}", task.Name);
            int notifyStakeholders = task.NotifyStakeholders.HasValue && task.NotifyStakeholders.Value ? 1 : 0;
            int writeTestResultBack = task.WriteTestResultBack.HasValue && task.WriteTestResultBack.Value ? 1 : 0;
            string programPath = "ruby.exe";
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(@"""{0}""", ConfigurationManager.AppSettings["WindowsScheduleTaskProgramPath"]);
            string format = @" '{{ ""Name"":""{0}"",""Status"":{1},""Type"":{2},""Priority"":{3},""CreateBy"":{4},""ModifyBy"":{5},""BuildId"":{6},""EnvironmentId"":{7},""TestContent"":{8},""Description"":""{9}"",""RecurrencePattern"":{10},""StartDate"":{11},""StartTime"":{12},""WeekDays"":{13},""WeekInterval"":{14}, ""ParentTaskId"":{15}, ""BranchId"":{16}, ""ReleaseId"":{17}, ""ProductId"":{18}, ""ProjectId"":{19}, ""NotifyStakeholders"":{20}, ""WriteTestResultBack"":{21}, ""SetupScript"":""{22}"", ""TeardownScript"":""{23}"" }}'";
            builder.AppendFormat(format, taskName, (int)TaskStatus.Scheduled, task.Type, 0, 0, 0, task.BuildId, task.EnvironmentId, task.TestContent, taskDescription, (int)RecurrencePattern.AtOnce, serializer.Serialize(task.StartDate.GetValueOrDefault()), serializer.Serialize(task.StartTime.GetValueOrDefault()), task.WeekDays.GetValueOrDefault(), task.WeekInterval.GetValueOrDefault(), task.TaskId, task.BranchId.GetValueOrDefault(), task.ReleaseId.GetValueOrDefault(), task.ProductId.GetValueOrDefault(), task.ProjectId, notifyStakeholders, writeTestResultBack, task.SetupScript.Replace("\\", "\\\\").Replace("\"", "\\\""), task.TeardownScript.Replace("\\", "\\\\").Replace("\"", "\\\""));
            string programParameters = builder.ToString();
            string programeWorkingDirectory = ConfigurationManager.AppSettings["WindowsScheduleTaskProgramWorkingDirectory"];
            DateTime startDateTime = task.StartDate.Value;
            ScheduledTask.CreateWindowsScheduledWeeklyTask(administrator, password, taskName, taskDescription, programPath, programParameters, programeWorkingDirectory, (short)task.WeekDays.Value, (short)task.WeekInterval.Value, task.StartDate.Value);
        }

        public static string CodecoverageLine(int productId, Build build, AutomationTask task)
        {

            if (productId == 3)//Data Protection Search
            {
                return "<a href=\"http://10.98.26.221/api/" + build.Number + "\" style=\"color:#43c1d7; text-decoration:underline;\">Detail</a>";
            }
            else if (productId == 2)//CIS
            {
                return "<a href=\"http://10.98.28.192:8080/" + task.TaskId + "/" + task.GetJobs().First().JobId + "/coverage" + "\" style=\"color:#43c1d7; text-decoration:underline;\">Detail</a>";
            }
            return "TBD";
        }

        public static void SendTestReportOfTaskToStakeholders(AutomationTask task, bool force = false)
        {
            if (!task.NotifyStakeholders.HasValue && !force)
            {
                return;
            }

            Dictionary<string, string> taskDic = new Dictionary<string, string>();
            //the general information for the task
            Product product = Product.GetProductByID(task.ProductId.GetValueOrDefault());
            Build build = Build.GetBuildById(task.BuildId);
            taskDic.Add("name", task.Name);
            taskDic.Add("taskId", task.TaskId.ToString());
            taskDic.Add("product", product.Name);
            taskDic.Add("build", build.Name);
            string coverageLink = "";
            /*
            if(product.ProductId == 3)//Data Protection Search
            {
               coverageLink = "<a href=\"http://10.98.26.221/api/" + build.Number + "\" style=\"color:#43c1d7; text-decoration:underline;\">Detail</a>";
            }
            else
            {
                coverageLink = "TBD";
            }
             */
            coverageLink = CodecoverageLine(product.ProductId, build, task);
            taskDic.Add("coverageLink", coverageLink);
            taskDic.Add("release", Release.GetReleaseById(task.ReleaseId.GetValueOrDefault()).Name);
            taskDic.Add("branch", Branch.GetBranchById(task.BranchId.GetValueOrDefault()).Name);
            taskDic.Add("recurrencePattern", Enum.GetName(typeof(RecurrencePattern), task.RecurrencePattern.GetValueOrDefault()));
            taskDic.Add("test_environment", SupportedEnvironment.GetSupportedEnvironmentById(task.EnvironmentId).Name);
            taskDic.Add("creator", User.GetUserById(task.CreateBy).Username);
            taskDic.Add("status", Enum.GetName(typeof(TaskStatus), task.Status));
            string testResultStatic = "<tr><td width=\"100%\"  colspan=\"3\" align=\"left\" valign=\"middle\" style=\"padding:5px;font-family:Verdana, Geneva, sans-serif;\"><strong>@pass</strong> of <strong>@total</strong> <span style=\"color:#008000\"> </span> are passed. Passrate is <strong>@rate</strong></td></tr>";
          
            //the test case result information
            List<TestCaseExecution> testCaseExcutionCount = AutomationTask.GetTestCaseExecutionForAutomationTask(task.TaskId);
            List<List<TestCaseExecution>> tess = AutomationTask.GetTestCaseExecutionForAutomationTaskByGroup(task.TaskId);

            string testResultStringFailed = "";
            string testResultStringPassed = "";
            int totalPassCount = 0;
            if (null == testCaseExcutionCount || testCaseExcutionCount.Count == 0)
            {
                taskDic.Add("titleColor", "#808080");
                taskDic.Add("passRate", "None");
                taskDic.Add("result", "Execution result not found"); 
                taskDic.Add("total", "0");
                taskDic.Add("pass", "0");
                string fileServer = @"\\10.98.28.192\TestResult-Dev";               
                string filePath = fileServer+@"\"+ task.TaskId.ToString()+@"\"+AutomationTask.GetAutomationJobsOfTask(task.TaskId).FirstOrDefault().JobId.ToString()+@"\control_logs";
                NetUseHelper.NetUserRemoteFolerToLocalPath(filePath, Core.LocalMappedFolder.ResultTempFolder, "administrator", "qampass1!");
                string body = String.Empty;
                if (FileHelper.IsExistsFile(Core.LocalMappedFolder.ResultTempFolder + @"\installation_results"))
                {
                    body = TXTHelper.GetTXT(Core.LocalMappedFolder.ResultTempFolder + @"\installation_results").Trim('"').Replace("\\", "").Replace("\r", "").Replace("\n", "");

                }
                else
                {
                    body = "Installation failed";
                }
                testResultStringFailed = "<tr><td>" + body + "</td></tr>";
                 
               
            }
            else
            {

                
                foreach (List<TestCaseExecution> tes in tess)
                {
                    int passCount = 0;
                    string testTable="<tr><td width=\"100%\"><table width=\"100%\"><tr><td  width=\"100%\" align=\"center\" valign=\"middle\" style=\"padding:20px;font-family:Verdana, Geneva, sans-serif;font-size:24px; color:#6e6e6e;\"><b>@title </b></td></tr><tr><td width=\"100%\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td  width=\"50%\"  bgcolor=\"#FFFFFF\" ><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td height=\"30\" align=\"center\" valign=\"middle\" style=\"background-color:#FFA500;color:#FFFFFF;\">Test Case</td></tr></table></td><td width=\"16%\" align=\"left\" bgcolor=\"#FFFFFF\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td align=\"center\" valign=\"middle\" height=\"30\" style=\"background-color:#FFA500;color:#FFFFFF;\">Result</td ></tr></table></td><td width=\"34%\" align=\"left\" bgcolor=\"#FFFFFF\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td align=\"center\" valign=\"middle\" height=\"30\" style=\"background-color:#FFA500;color:#FFFFFF;\">Log</td ></tr></table>	</td> </tr>@record </td></tr></table></td></tr></table></td></tr>";
                    List<Dictionary<string, string>> tcDicsPassed = new List<Dictionary<string, string>>();
                    List<Dictionary<string, string>> tcDicsFailed = new List<Dictionary<string, string>>();
                    string title = "Test Results";
                    bool suiteContainsFailures = false;
                    string suiteResultString = "";
                    if (tes[0].Info != null) 
                    {
                        title = tes[0].Info;
                    }
                    testTable = testTable.Replace("@title", title);
                    foreach (TestCaseExecution te in tes)
                    {
                        TestResult tr = TestResult.GetTestResultByExecutionId(te.ExecutionId);
                        int resultType = tr.Result;
                       
                            Dictionary<string, string> tcDic = new Dictionary<string, string>();
                            switch (resultType)
                            {
                                case 0:
                                    tcDic.Add("color", "green");
                                    tcDic.Add("testResult", "Pass");                                    
                                    passCount++;
                                    break;
                                case 1:
                                    tcDic.Add("color", "red");
                                    tcDic.Add("testResult", "Failed");
                                    break;
                                case 2:
                                    tcDic.Add("color", "orange");
                                    tcDic.Add("testResult", "Time Out");
                                    break;
                                case 3:
                                    tcDic.Add("color", "yellow");
                                    tcDic.Add("testResult", "Exception");
                                    break;
                                case 4:
                                    tcDic.Add("color", "grey");
                                    tcDic.Add("testResult", "Not Run");
                                    break;
                                case 5:
                                    tcDic.Add("color", "firebrick");
                                    tcDic.Add("testResult", "Know Issue");
                                    break;
                                case 6:
                                    tcDic.Add("color", "tomato");
                                    tcDic.Add("testResult", "New Issue");
                                    break;
                                case 7:
                                    tcDic.Add("color", "lightskyblue");
                                    tcDic.Add("testResult", "Environment Issue");
                                    break;
                                case 8:
                                    tcDic.Add("color", "gold");
                                    tcDic.Add("testResult", "Scripts Issue");
                                    break;
                                case 9:
                                    tcDic.Add("color", "plum");
                                    tcDic.Add("testResult", "Common Library Issue");
                                    break;
                            }
                        //shorten the test case name and description if it's too long
                            int maxLines = 1;
                            int maxLineLength = 70;
                            int i = 0;
                            string caseName = TestCase.GetTestCase(te.TestCaseId).Name;
                            string subName = string.Empty;
                            string trimedLine = string.Empty;
                            while (caseName.Length > maxLineLength)
                            {

                                if (i < maxLines)
                                {
                                    i++;
                                    subName += string.Format(@"{0}<br/>", caseName.Substring(0, maxLineLength));
                                    caseName = caseName.Substring(maxLineLength);
                                }
                                else
                                {
                                    i = 0;
                                    caseName = "...";
                                    break;
                                }

                            }
                            subName += caseName;

                            tcDic.Add("testcaseName", subName);
                            string temp = string.IsNullOrEmpty(tr.Description) ? "" : tr.Description;
                            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                            temp = reg.Replace(temp, "");
                            //remove the test function name from the description
                            if (temp.IndexOf(@") [") > 0)
                            {
                                temp = temp.Substring(temp.IndexOf(@") [") + 2);
                            }
                            string resultLogString = string.Empty;
                            maxLineLength = 100;
                            i = 0;

                            foreach (string line in temp.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                            {


                                trimedLine = line;
                                while (trimedLine.Length > maxLineLength)
                                {

                                    if (i < maxLines)
                                    {
                                        i++;
                                        resultLogString += string.Format(@"{0}<br/>", trimedLine.Substring(0, maxLineLength));
                                        trimedLine = trimedLine.Substring(maxLineLength);
                                    }
                                    else
                                    {
                                        i = 0;
                                        trimedLine = "...";
                                        break;
                                    }

                                }
                            }
                            resultLogString += trimedLine;
                            tcDic.Add("testResultLog", resultLogString);

                            
                            if (resultType != 0)
                            {
                                tcDicsFailed.Add(tcDic);
                                suiteContainsFailures = true;
                            }
                            else
                            {
                                tcDicsPassed.Add(tcDic);
                            }
                        
                   
                    }

                    string testcaseT = "<td style=\"font-family:Verdana, Geneva, sans-serif; padding: 5px; color:#000000;\">@testcaseName</td>";
                    string testResultT = "<td align=\"center\" valign=\"middle\" style=\"font-family:Verdana, Geneva, sans-serif; padding: 5px; color:@color;\">@testResult</td>";
                    string testResultLogT = "<td align=\"left\" valign=\"left\" style=\"font-family:Verdana, Geneva, sans-serif; padding: 5px; color:@color;\">@testResultLog</td>";
                 

                    string tempStringPassed = "";
                    string tempStringFailed = "";
                    foreach (Dictionary<string, string> d in tcDicsPassed)
                    {
                        string testcaseTemp = testcaseT;
                        string testResultTemp = testResultT;
                        string testResultLogTemp = testResultLogT;
                        foreach (KeyValuePair<string, string> p in d)
                        {
                            testcaseTemp = testcaseTemp.Replace("@" + p.Key, p.Value);
                            testResultTemp = testResultTemp.Replace("@" + p.Key, p.Value);
                            testResultLogTemp = testResultLogT.Replace("@" + p.Key, p.Value);
                        }
                        
                        tempStringPassed += @"<tr>" + testcaseTemp + testResultTemp + testResultLogTemp + @"</tr>";
                        
                    }
                    foreach (Dictionary<string, string> d in tcDicsFailed)
                    {
                        string testcaseTemp = testcaseT;
                        string testResultTemp = testResultT;
                        string testResultLogTemp = testResultLogT;
                        foreach (KeyValuePair<string, string> p in d)
                        {
                            testcaseTemp = testcaseTemp.Replace("@" + p.Key, p.Value);
                            testResultTemp = testResultTemp.Replace("@" + p.Key, p.Value);
                            testResultLogTemp = testResultLogT.Replace("@" + p.Key, p.Value);
                        }

                        tempStringFailed += @"<tr>" + testcaseTemp + testResultTemp + testResultLogTemp + @"</tr>";

                    }
                    suiteResultString = testTable.Replace("@record", tempStringFailed + tempStringPassed);

                    
                    string testSubResultStatic = testResultStatic.Replace("@pass", passCount.ToString());
                    testSubResultStatic = testSubResultStatic.Replace("@rate", Math.Round((double)passCount / tes.Count, 3) * 100 + "%");
                    testSubResultStatic = testSubResultStatic.Replace("@total", tes.Count.ToString());
                    suiteResultString += testSubResultStatic;
                    
                    totalPassCount += passCount;

                    if (suiteContainsFailures)
                    {
                        testResultStringFailed += suiteResultString;
                    }
                    else
                    {
                        testResultStringPassed += suiteResultString;
                    }
                   
                   // taskDic.Add("total", tes.Count.ToString());
                  //  taskDic.Add("pass", passCount.ToString());
                }
                if (totalPassCount == 0)
                {
                    taskDic.Add("titleColor", "#d80000");
                }
                else if (testCaseExcutionCount.Count == totalPassCount)
                {
                    taskDic.Add("titleColor", "#008000");
                }
                else
                {
                    taskDic.Add("titleColor", "#FFD700");
                }
                string res = @"<strong>" + totalPassCount + @"</strong> of " + @"<strong>" + testCaseExcutionCount.Count + @"</strong> cases are passed";
                taskDic.Add("passRate", Math.Round((double)totalPassCount / testCaseExcutionCount.Count, 3) * 100 + "%");
                taskDic.Add("result", res); 
            }

                
            string template = string.Empty;
            using (StreamReader sr = new StreamReader(AssemblyHelper.GetAssemblePath() + "/Documents/galaxy_result_report_template.html"))
            {
                template = sr.ReadToEnd();
            }
            foreach (KeyValuePair<string, string> p in taskDic)
            {
                template = template.Replace("@" + p.Key, p.Value);
            }
            string mailContent = "";
            string attachment = "";
            if (testCaseExcutionCount.Count>500)
            {
                template = template.Replace("@remark", "Only the failed test suites are shown in the right. For the detail, please open the attachment.");
                mailContent = template.Replace("@TOBEREPLACED", testResultStringFailed);
                string reportFile = template.Replace("@TOBEREPLACED", testResultStringFailed + testResultStringPassed);
                attachment = "fullreport.html";
                File.WriteAllText(attachment, reportFile);
            }
            else
            {
                template = template.Replace("@remark", "None");
                mailContent = template.Replace("@TOBEREPLACED", testResultStringFailed + testResultStringPassed);
            }

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            to.Add(User.GetUserById(task.CreateBy).Email);
            if (task.NotifyStakeholders.Value)
            {
                cc = Subscriber.GetSubscribersEmailByProjectId(task.ProjectId.GetValueOrDefault());
            }
           
            try
            {
                string smtpServer = ATFConfiguration.GetStringValue("DefaultSMTPServer");
                MailHelper.sendMail(smtpServer, "galaxy@emc.com", to, cc, "[Test report]" + taskDic["name"] + "-Pass rate: " + taskDic["passRate"], mailContent, attachment);
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error("Failed to send the mails.", e);
            }
        }

        #endregion
    }
}
