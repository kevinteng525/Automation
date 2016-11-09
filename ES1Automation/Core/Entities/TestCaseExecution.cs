using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Core.Model
{
    public enum ExecutionStatus
    {
        NotRunning = 0,
        Running = 1,
        Complete = 2,
        Fail = 3,
        Cancelled = 4,
    }

    public partial class TestCaseExecution
    {
        public ExecutionStatus ExecutionStatus
        {
            get { return (ExecutionStatus)Status; }
            set { Status = (int)value; }
        }

        public static List<TestCaseExecution> GetAllExecutions()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                List<TestCaseExecution> exes = context.TestCaseExecutions.ToList<TestCaseExecution>();
                return exes;
            }
        }

        public static IList<TestCaseExecution> GetTestCaseExectionByJob(int jobId)
        {
            using (var context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestCaseExecutions
                       .Where(execution => execution.JobId == jobId)
                       .Include(execution => execution.TestCase)
                       .ToList();
            }
        }

        public static List<TestCaseExecution> GetExecutions(int jobId, int status)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from e in context.TestCaseExecutions
                        where e.JobId == jobId && (status == -1 ? true : e.Status == status)
                        select e).ToList();
            }
        }

        public static IList<TestCaseExecution> GetTestCaseExectionByStatus(ExecutionStatus status)
        {
            using (var context = new Core.Model.ES1AutomationEntities())
            {
                return (from execution in context.TestCaseExecutions
                        where execution.Status == (int)status
                        select execution).ToList();
            }
        }

        public static TestCaseExecution GetTestCaseExecution(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestCaseExecutions.Find(id);
            }
        }

        public static TestCaseExecution GetTestCaseExecutionByJobIdAndTestCaseId(int jobId, int taskcaseId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestCaseExecutions.Where(e => e.JobId == jobId && e.TestCaseId == taskcaseId).FirstOrDefault();
            }
        }

        public void SetStatus(ExecutionStatus status)
        {
            switch (status)
            {
                case Model.ExecutionStatus.Running:
                    this.StartTime = DateTime.UtcNow;
                    break;

                case Model.ExecutionStatus.Complete:
                    this.EndTime = DateTime.UtcNow;
                    break;

                case Model.ExecutionStatus.Fail:
                    this.EndTime = DateTime.UtcNow;
                    break;
                case Model.ExecutionStatus.Cancelled:
                    this.StartTime = DateTime.UtcNow;
                    this.EndTime = DateTime.UtcNow;
                    break;
            }

            this.ExecutionStatus = status;
            this.Update();
        }

        public bool IsFinished()
        {
            return (this.ExecutionStatus != ExecutionStatus.Complete || this.ExecutionStatus != ExecutionStatus.Fail);
        }

        public static TestCaseExecution CreateExecution(TestCaseExecution instance)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                TestCaseExecution execution = context.TestCaseExecutions.Add(instance);
                context.SaveChanges();
                return execution;
            }
        }

        public bool Update()
        {
            try
            {
                // Entity context
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    context.TestCaseExecutions.Attach(this);
                    context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

            return true;
        }

        public static TestCaseExecution Update(int executionId, Object instance)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    TestCaseExecution execution = context.TestCaseExecutions.Find(executionId);

                    context.Entry(execution).CurrentValues.SetValues(instance);
                    context.SaveChanges();

                    return execution;
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return null;
            }
        }

        public static void Delete(int executionId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                TestCaseExecution execution = context.TestCaseExecutions.Find(executionId);

                if (execution != null)
                {
                    context.TestCaseExecutions.Attach(execution);
                    context.TestCaseExecutions.Remove(execution);
                    context.SaveChanges();
                }
            }
        }

        public static List<TestCaseExecution> GetTestCaseExecutionByCase(int caseId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from e in context.TestCaseExecutions
                        where e.TestCaseId == caseId 
                        select e).OrderByDescending(t => t.EndTime).ToList();
            }
        }
        public static Build GetBuildByExecutionId(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from b in context.Builds
                        where (from t in context.AutomationTasks
                                   where(from m in context.TaskJobMaps
                                         where(from e in context.TestCaseExecutions
                                               where e.ExecutionId == id
                                               select e
                                         ).FirstOrDefault().JobId == m.JobId
                                         select m
                                   ).FirstOrDefault().TaskId == t.TaskId
                                   select t).FirstOrDefault().BuildId == b.BuildId
                        select b).FirstOrDefault();
            }
        }
    }
}
