using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.SqlClient;
using ES1Common.Logs;
using Core.DTO;


namespace Core.Model
{
    public enum TaskType
    {
        Normal = 0,
        ByTestPlan = 1,
    }

    public enum RecurrencePattern
    {
        AtOnce = 0,//run at once
        OneTime = 1,//run on a future time
        Weekly = 2,//run weekly
    }

    public enum TaskStatus
    {
        Scheduled = 0,
        Dispatched = 1,
        Running = 2,
        Complete = 3,
        Failed = 4,
        Paused = 5,
        Cancelling = 6,
        Cancelled = 7,
        Scheduling = 8,
        End = 9,
    }

    public partial class AutomationTask
    {
        private static volatile System.Threading.Mutex mutex = new System.Threading.Mutex(false);

        public TaskStatus TaskStatus
        {
            get { return (TaskStatus)Status; }
            set { Status = (int)value; }
        }

        public TaskType TaskType
        {
            get { return (TaskType)Type; }
            set { Type = (int)value; }
        }

        public void SetTaskStatus(TaskStatus status)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationTasks.Attach(this);
                    context.Entry(this).Reload();
                    this.TaskStatus = status;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.Status).IsModified = true;
                    context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
        }

        private AutomationLog Log = new AutomationLog("AutomationTask");


        public static AutomationRunningStatusDTO GetTaskProgress(AutomationTask task)
        {
            AutomationRunningStatusDTO progress = new AutomationRunningStatusDTO();
            var testCasesExecutionStatusList = "";
            var testCasesExecutionStatusCountList = "";
            if (null == task)
            {
                progress.ExecutionPercentage = -1;
                progress.Status = (int)TaskStatus.Cancelled;
                progress.TaskId = -1;
                progress.Information = "There's no tasks executed yet for this project.";
                var resultType = typeof(ResultType);
                foreach (string result in Enum.GetNames(resultType))
                {
                    ResultType r = (ResultType)Enum.Parse(resultType, result);
                    int count = 0;
                    if (r == ResultType.NotRun)
                    { 
                        count = 1; 
                    }
                    else
                    {
                        count = 0;
                    }                    
                    testCasesExecutionStatusList += " " + r.ToString();
                    testCasesExecutionStatusCountList += " " + count.ToString();
                }
                
            }
            else
            {
                progress.ExecutionPercentage = GetTaskExecutionPercentageByTaskId(task);
                progress.Status = task.Status;
                progress.TaskId = task.TaskId;
                progress.Information = task.Information == null ? "Not Started Yet!" : task.Information;
        
                var resultType = typeof(ResultType);
                foreach (string result in Enum.GetNames(resultType))
                {
                    ResultType r = (ResultType)Enum.Parse(resultType, result);
                    int count = AutomationTask.GetResultTypeCountForTask(task.TaskId, r);
                    testCasesExecutionStatusList += " " + r.ToString();
                    testCasesExecutionStatusCountList += " " + count.ToString();
                }
                
            }
            progress.TestCasesExecutionStatusList = testCasesExecutionStatusList.TrimStart(' ');
            progress.TestCasesExecutionStatusCountList = testCasesExecutionStatusCountList.TrimStart(' ');
            return progress;
        }

        public static List<AutomationTask> GetAllTasks()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                List<AutomationTask> testcases = context.AutomationTasks.OrderByDescending(t => t.CreateDate).Take(50).ToList<AutomationTask>();
                return testcases;
            }
        }

        public static List<AutomationJob> GetAutomationJobsOfTask(int taskId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from j in context.AutomationJobs
                        where ((from tjm in context.TaskJobMaps
                                where tjm.TaskId == taskId
                                select tjm.JobId).Contains(j.JobId))
                        select j).ToList();
            }
        }

        public static List<AutomationTask> GetLatestAutomationTasksContainTestCase(int testCaseId, int size = 5)
        {
            List<AutomationTask> tasks = new List<AutomationTask>();
            int i = 0;
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {

                List<TestCaseExecution> ex = TestCaseExecution.GetTestCaseExecutionByCase(testCaseId);
                foreach (TestCaseExecution e in ex)
                {
                    if (i < size)
                    {
                        tasks.Add(AutomationJob.GetTaskOfJobByJobId(e.JobId));
                        i++;
                    }
                    else 
                    {
                        break;
                    }
                }
              /*  
                foreach (AutomationTask task in context.AutomationTasks.Where(t => (t.Status == (int)TaskStatus.Complete || t.Status == (int)TaskStatus.End) && t.RecurrencePattern == 0).OrderByDescending(t => t.CreateDate))
                {
                    if (i < size)
                    {
                        if (TestSuite.GetAllCases(Int32.Parse(task.TestContent), false).Where(c => c.TestCaseId == testCaseId).Count() > 0)
                        {
                            tasks.Add(task);
                            i++;
                        }
                    }
                    else//Only get the latest 5 tasks containing the test cases.
                    {
                        break;
                    }

                }
               */
            }
            return tasks.OrderByDescending(t => t.CreateDate).ToList();
        }

        public static AutomationTask GetAutomationTask(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.AutomationTasks.Find(id);
            }
        }

        public static TestSuite GetTestSuiteContainsAllFailedTestCasesOfTask(int taskId)
        {
            AutomationTask task = AutomationTask.GetAutomationTask(taskId);
            if (null == task || !task.IsFinished())
            {
                return null;
            }
            else
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    List<int> ids = (from tc in context.TestCases
                                     where (from ex in context.TestCaseExecutions
                                            where (from tjm in context.TaskJobMaps
                                                   where tjm.TaskId == taskId
                                                   select tjm.JobId
                                                 ).Contains(ex.JobId) &&
                                                 (from tr in context.TestResults
                                                  where tr.Result != (int)ResultType.Pass && tr.ExecutionId == ex.ExecutionId
                                                  select tr).Count() > 0
                                            select ex.TestCaseId
                                     ).Contains(tc.TestCaseId)
                                     select tc.TestCaseId).ToList();
                    if (ids.Count() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        string testcases = string.Empty;
                        foreach (int id in ids)
                        {
                            if (testcases == string.Empty)
                            {
                                testcases = id.ToString();
                            }
                            else
                            {
                                testcases = testcases + "," + id.ToString();
                            }
                        }
                        TestSuite failedSuite = TestSuite.CreateSuite(new TestSuite
                        {
                            Name = "Suite contains Failed Test Cases of task:" + taskId.ToString(),
                            TestCases = testcases,
                            Type = (int)SuiteType.Temporary,
                            IsActive = true,
                            Description = "",
                            CreateBy = 0,
                            ModityBy = 0,
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                        });

                        return failedSuite;
                    }
                }
            }
        }

        public static List<AutomationTask> GetActiveAutomationTask(int status = -1)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {                
                List<AutomationTask> tasks;
                if (status == -1)
                {
                    tasks = (from t in context.AutomationTasks
                             where t.Status != (int)TaskStatus.Complete &&
                                   t.Status != (int)TaskStatus.Failed &&
                                   t.Status != (int)TaskStatus.Paused &&
                                   t.Status != (int)TaskStatus.Cancelled &&
                                   t.Status != (int)TaskStatus.End
                             select t).ToList();
                    //tasks = context.AutomationTasks.Where(t => t.Status != (int)TaskStatus.Complete &&
                    //                                           t.Status != (int)TaskStatus.Failed &&
                    //                                           t.Status != (int)TaskStatus.Paused &&
                    //                                           t.Status != (int)TaskStatus.Cancelled &&
                    //                                           t.Status != (int)TaskStatus.End).ToList();
                }
                else
                {
                    //tasks = context.AutomationTasks.Where(t => t.Status == status).ToList();
                    tasks = (from t in context.AutomationTasks
                             where t.Status == status
                             select t).ToList();
                }
                return tasks;
            }
        }

        public static AutomationTask CreateTask(AutomationTask instance)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                AutomationTask task = context.AutomationTasks.Add(instance);
                context.SaveChanges();
                return task;
            }
        }

        public bool Update()
        {
            // Entity context
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationTasks.Attach(this);
                context.Entry(this).Reload();
                context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

        public bool SetBuild(int buildId)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationTasks.Attach(this);
                    context.Entry(this).Reload();
                    this.BuildId = buildId;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.BuildId).IsModified = true;
                    context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
                return false;
            }
        }

        public bool SetTestContent(string suiteId)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationTasks.Attach(this);
                    context.Entry(this).Reload();
                    this.TestContent = suiteId;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.TestContent).IsModified = true;
                    context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
                return false;
            }
        }

        public void AddProgressInformation(string information)
        {
            if (mutex.WaitOne(1000 * 60))
            {
                try
                {
                    using (ES1AutomationEntities context = new ES1AutomationEntities())
                    {
                       context.AutomationTasks.Attach(this);
                       context.Entry(this).Reload();
                        this.Information = string.Format(@"<span class='date_time'>{0}</span>: {1}|{2}", System.DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"), information, this.Information);

                        context.Entry(this).Property(p => p.Information).IsModified = true;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(ex);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                ATFEnvironment.Log.logger.Error(string.Format("Timeout to wait to write the info [{0}] to task [{1}]", information, this.Name));
            }
        }

        public static List<AutomationTask> GetChildrenTask(int parentId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                List<AutomationTask> tasks;
                tasks = context.AutomationTasks.Where(t => t.ParentTaskId == parentId).ToList();
                return tasks;
            }
        }

        public static AutomationTask UpdateAutomationTask(int taskId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                AutomationTask task = context.AutomationTasks.Find(taskId);
                if (task != null)
                {
                    context.Entry(task).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                }
                return task;
            }
        }

        public List<TaskJobMap> GetTaskJobMaps()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationTasks.Attach(this);
                context.Entry(this).Collection(t => t.TaskJobMaps).Load();
                return this.TaskJobMaps.ToList();
            }
        }

        public static List<TestCase> GetTestCasesForAutomationTask(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                AutomationTask task = context.AutomationTasks.Find(id);
                if (task != null)
                {
                    int testSuiteId = Int32.Parse(task.TestContent);
                    return TestSuite.GetAllCases(testSuiteId, false);
                }
                return null;
            }
        }
        public static List<List<TestCaseExecution>> GetTestCaseExecutionForAutomationTaskByGroup(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                var groups = (from tce in context.TestCaseExecutions
                              where (from tjm in context.TaskJobMaps
                                     where tjm.TaskId == id
                                     select tjm.JobId).Contains(tce.JobId)
                              select tce
                    ).GroupBy(x => x.Info).Select(x => x).ToList().ToList();
                List<List<TestCaseExecution>> tces = new List<List<TestCaseExecution>>();
                foreach (var group in groups)
                {
                    List<TestCaseExecution> tce = new List<TestCaseExecution>();
                    foreach (var g in group)
                    {
                        tce.Add(g);
                    }
                    tces.Add(tce);
                }
                return tces;
            }
        }
        public static List<TestCaseExecution> GetTestCaseExecutionForAutomationTask(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from tce in context.TestCaseExecutions
                        where (from tjm in context.TaskJobMaps
                               where tjm.TaskId == id
                               select tjm.JobId).Contains(tce.JobId)
                        select tce
                    ).OrderBy(o => o.TestCaseId).ToList<TestCaseExecution>();
            }
        }

        public static TestCaseExecution GetTestCaseExecutionForTestCaseInTask(int taskId, int testCaseId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                var testCaseExecution = (from tce in context.TestCaseExecutions
                                         where (from tjm in context.TaskJobMaps
                                                where tjm.TaskId == taskId
                                                select tjm.JobId).Contains(tce.JobId)
                                                    && tce.TestCaseId == testCaseId
                                         select tce);

                return testCaseExecution.FirstOrDefault();
            }
        }

        public static TestResult GetTestCaseResultForTestCaseInTask(int taskId, int testCaseId)
        {
            var testCaseExecution = AutomationTask.GetTestCaseExecutionForTestCaseInTask(taskId, testCaseId);
            if (testCaseExecution != null)//the execute has been started.
            {
                return TestResult.GetTestResultByExecutionId(testCaseExecution.ExecutionId);
            }
            else//the execute has not been started.
            {
                TestResult result = new TestResult();
                result.Result = (int)ResultType.NotRun;
                return result;
            }
        }

        public static int GetResultTypeCountForTask(int taskId, ResultType resultType)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                var testCaseExecution = (from tce in context.TestCaseExecutions
                                         where (from tjm in context.TaskJobMaps
                                                where tjm.TaskId == taskId
                                                select tjm.JobId).Contains(tce.JobId)
                                         select tce.ExecutionId
                            );

                if (testCaseExecution.Count() > 0)
                {
                    int count = (from tr in context.TestResults
                                 where testCaseExecution.Contains(tr.ExecutionId) &&
                                     tr.Result == (int)resultType
                                 select tr).Count();
                    if (resultType == ResultType.NotRun)
                    {
                        if (TestSuite.GetTestSuite(int.Parse(AutomationTask.GetAutomationTask(taskId).TestContent)).Type == (int)SuiteType.NotExisting)
                        {
                            foreach (var j in AutomationTask.GetAutomationJobsOfTask(taskId))
                            {
                                count = count + AutomationJob.GetTestCaseExecutionsOfJob(j.JobId).Count();
                            }
                            return count - testCaseExecution.Count();
                        }
                        else return count + AutomationTask.GetTestCasesForAutomationTask(taskId).Count - testCaseExecution.Count();
                       
                    }
                    else
                    {
                        return count;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int GetTestCasesExecutionStatus(int taskId, int maxFactor)
        {
            int total = AutomationTask.GetTestCaseExecutionForAutomationTask(taskId).Count;
            int notRunCount = AutomationTask.GetResultTypeCountForTask(taskId, ResultType.NotRun);
            return total == 0 ? 0 : maxFactor * (total - notRunCount) / total;
        }

        public List<AutomationJob> GetJobs()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                var query = (from j in context.TaskJobMaps//.Include(automationJob => automationJob.AutomationJob)
                             where j.TaskId == this.TaskId
                             select j.AutomationJob).ToList();
                return query;
            }
        }

        public static bool Delete(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                AutomationTask task = context.AutomationTasks.Find(id);
                if (task == null)
                {
                    return false;
                }
                context.AutomationTasks.Remove(task);
                context.SaveChanges();
            }
            return true;
        }

        public static bool Delete(AutomationTask instance)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationTasks.Attach(instance);
                context.AutomationTasks.Remove(instance);
                context.SaveChanges();
            }
            return true;
        }

        public bool CanBeCancel()
        {
            switch (TaskStatus)
            {
                case TaskStatus.Scheduled:
                case TaskStatus.Dispatched:
                case TaskStatus.Running:
                    return true;
                case TaskStatus.Complete:
                case TaskStatus.Failed:
                case TaskStatus.Paused:
                case TaskStatus.Cancelling:
                case TaskStatus.Cancelled:
                    return false;
                default:
                    return false;
            }
        }

        public bool Cancel()
        {
            switch (TaskStatus)
            {
                case TaskStatus.Scheduled:
                case TaskStatus.Dispatched:
                case TaskStatus.Running:
                    {
                        SetTaskStatus(TaskStatus.Cancelling);
                        return true;
                    }
                case TaskStatus.Complete:
                case TaskStatus.Failed:
                case TaskStatus.Paused:
                case TaskStatus.Cancelling:
                case TaskStatus.Cancelled:
                    return false;
                default:
                    return false;
            }
        }

        public bool IsFinished()
        {
            switch (this.TaskStatus)
            {
                case Model.TaskStatus.Complete:
                case Model.TaskStatus.Cancelled:
                case Model.TaskStatus.Failed:
                case Model.TaskStatus.End:
                    return true;
                case Model.TaskStatus.Cancelling:
                case Model.TaskStatus.Dispatched:
                case Model.TaskStatus.Paused:
                case Model.TaskStatus.Running:
                case Model.TaskStatus.Scheduled:
                    return false;
                default:
                    return false;
            }
        }


        public static int GetTaskExecutionPercentageByTaskId(AutomationTask task)
        {

            switch (task.Status)
            {
                case (int)TaskStatus.Scheduled:
                    return 0;
                case (int)TaskStatus.Dispatched:
                    return 10;
                case (int)TaskStatus.Running:
                    return 20 + AutomationTask.GetTestCasesExecutionStatus(task.TaskId, 80);
                case (int)TaskStatus.Complete:
                    return 100;
                default:
                    return 100;
            }
        }
    }
}
