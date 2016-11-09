using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Core.Management;

namespace Core.Model
{
    public enum JobStatus
    {
        Assigned  = 0,
        Preparing = 1,
        Ready     = 2,
        Running   = 3,
        Complete  = 4,
        Failed    = 5,
        Paused    = 6,
        Cancelled = 7,
        LockedBySaberAgent = 8,
        Timeout = 9,
        End = 10,
    }

    public enum JobType
    {
        NoDefinded  = 0,
        Sequence    = 1,
        Concurrency = 2,
    }

    public partial class AutomationJob
    {

        private static volatile System.Threading.Mutex mutex = new System.Threading.Mutex(false);

        public JobStatus JobStatus
        {
            get { return (JobStatus)Status; }
            set { if (Status != (int)JobStatus.Cancelled && Status != (int)JobStatus.End || value == JobStatus.End ) Status = (int)value; }
        }

        public JobType JobType
        {
            get { return (JobType)Type; }
            set { Type = (int)value; }
        }

        //only update the status field of the job
        public void SetJobsStatus(JobStatus status)
        {
            if (this.JobStatus != JobStatus.Cancelled && this.JobStatus != JobStatus.End || status == JobStatus.End)
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationJobs.Attach(this);
                    context.Entry(this).Reload();
                    this.JobStatus = status;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.Status).IsModified = true;
                    context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                    context.SaveChanges();
                }
            }
        }

        public static List<AutomationJob> GetAllJobs()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                List<AutomationJob> jobs = context.AutomationJobs.ToList<AutomationJob>();
                return jobs;
            }
        }

        public static List<AutomationJob> GetJobs(int status)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                var query = (from j in context.AutomationJobs
                             where ((status == -1) ? true : j.Status == status)
                             select j).ToList();
                return query;
            }
        }

        public static IList<AutomationJob> GetJobsByStatus(JobStatus status)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from job in context.AutomationJobs
                        where job.Status == (int)status
                        select job).ToList();
            }
        }

        public static AutomationJob GetAutomationJob(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.AutomationJobs.Find(id);
            }
        }

        public static AutomationJob CreateJob(AutomationJob instance)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                AutomationJob job = context.AutomationJobs.Add(instance);
                context.SaveChanges();
                return job;
            }
        }


        public static AutomationJob UpdateAutomationJob(int jobId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                AutomationJob job = context.AutomationJobs.Find(jobId);
                if (job != null)
                {
                    context.Entry(job).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                }
                return job;
            }
        }

        public void AddJobProgressInformation(string information)
        {
            try
            {
                if (mutex.WaitOne(1000 * 60))
                {
                    using (ES1AutomationEntities context = new ES1AutomationEntities())
                    {
                        context.AutomationJobs.Attach(this);
                        context.Entry(this).Reload();
                        this.Description = string.Format(@"<span class='date_time'>{0}</span>: {1}|{2}", System.DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"), information, this.Description);
                        context.Entry(this).Property(p => p.Description).IsModified = true;
                        context.SaveChanges();
                    }
                }             
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            AutomationTask task = JobManagement.GetAutomationTaskOfJob(this);
            task.AddProgressInformation(information);
        }

        public bool Update()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationJobs.Attach(this);
                context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public bool SetTestAgentEnvironment(int environmentId)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationJobs.Attach(this);
                    context.Entry(this).Reload();
                    this.TestAgentEnvironmentId = environmentId;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.TestAgentEnvironmentId).IsModified = true;
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

        public bool SetSUTEnvironment(int environmentId)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.AutomationJobs.Attach(this);
                    context.Entry(this).Reload();
                    this.SUTEnvironmentId = environmentId;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.SUTEnvironmentId).IsModified = true;
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

        public static AutomationTask GetTaskOfJobByJobId(int jobId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from task in context.AutomationTasks
                        where (from map in context.TaskJobMaps
                               where map.JobId == jobId
                               select map.TaskId).Contains(task.TaskId)
                        select task).FirstOrDefault();
            }
        }

        public static Build GetBuildOfJobByJobId(int jobId)
        {
            AutomationTask task = GetTaskOfJobByJobId(jobId);
            if (task != null)
            {
                return Build.GetBuildById(task.BuildId);
            }
            else
            {
                return null;
            }
        }

        public static Project GetProjectOfJobByJobId(int jobId)
        {
            AutomationTask task = GetTaskOfJobByJobId(jobId);
            if (task != null)
            {
                return Project.GetProjectById(task.ProjectId.Value);
            }
            else
            {
                return null;
            }
        }

        public static Product GetProductOfJobByJobId(int jobId)
        {
            AutomationTask task = GetTaskOfJobByJobId(jobId);
            if (task != null)
            {
                return Product.GetProductByID(task.ProductId.Value);
            }
            else
            {
                return null;
            }
        }

        public List<TaskJobMap> GetTaskJobMaps()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationJobs.Attach(this);
                context.Entry(this).Collection(t => t.TaskJobMaps).Load();
                return this.TaskJobMaps.ToList();
            }
        }

        public static List<TestCase> GetTestCasesOfJob(int jobId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from tc in context.TestCases
                        where (from tce in context.TestCaseExecutions
                               where tce.JobId == jobId
                               select tce.TestCaseId).Contains(tc.TestCaseId)
                        select tc).ToList();
            }
        }

        public static List<TestCaseExecution> GetTestCaseExecutionsOfJob(int jobId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from tce in context.TestCaseExecutions
                               where tce.JobId == jobId
                               select tce).ToList();
            }
        }

        public SupportedEnvironment GetSupportedEnv()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (
                            from m in context.TaskJobMaps
                            from t in context.AutomationTasks
                            where m.TaskId == t.TaskId && m.JobId == this.JobId
                            select t.SupportedEnvironment
                        ).FirstOrDefault();
            }
        }

        public static bool Delete(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                AutomationJob task = context.AutomationJobs.Find(id);
                if (task == null)
                {
                    return false;
                }
                context.AutomationJobs.Remove(task);
                context.SaveChanges();
            }
            return true;
        }

        public static bool Delete(AutomationJob instance)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                context.AutomationJobs.Attach(instance);
                context.AutomationJobs.Remove(instance);
                context.SaveChanges();
            }
            return true;
        }

        public bool Cancel()
        {
            switch (JobStatus)
            {
                case JobStatus.Assigned:
                case JobStatus.Preparing:
                case JobStatus.Ready:
                    {
                        SetJobsStatus(JobStatus.Cancelled);
                        return true;
                    }
                case JobStatus.Running:
                case JobStatus.Complete:
                case JobStatus.Failed:
                case JobStatus.Paused:
                case JobStatus.Cancelled:
                    return false;
                default:
                    return false;
            }
        }
    }
}
