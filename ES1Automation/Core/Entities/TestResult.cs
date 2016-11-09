using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core;
using ES1Common.Logs;

namespace Core.Model
{
    public enum ResultType
    {
        Pass = 0,
        Failed = 1,
        TimeOut = 2,
        Exception = 3,
        NotRun = 4,
        KnownIssue = 5,
        NewIssue = 6,
        EnvironmentIssue = 7,
        ScriptsIssue = 8,
        CommonLibIssue = 9,
    }

    public partial class TestResult
    {
        public ResultType ResultType
        {
            get { return (ResultType)Result; }
            set { Result = (int)value; }
        }

        public void SetResult(ResultType result)
        {
            this.ResultType = result;
            this.Update();
        }

        public static List<TestResult> GetAllResults()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                List<TestResult> allResults = context.TestResults.ToList<TestResult>();
                return allResults;
            }
        }

        public static TestResult GetTestResult(int resultId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestResults.Find(resultId);
            }
        }

        public static TestResult GetTestResultByJobIdAndTestCaseId(int jobId, int testcaseId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from result in context.TestResults
                        where (from excution in context.TestCaseExecutions
                               where excution.TestCaseId == testcaseId && excution.JobId == jobId
                               select excution.ExecutionId).Contains(result.ExecutionId)
                        select result).FirstOrDefault();
            }
        }

        public static TestResult GetTestResultByExecutionId(int executionId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from tr in context.TestResults
                            where tr.ExecutionId == executionId
                            select tr).ToList<TestResult>().FirstOrDefault();
            }
        }

        public static TestResult CreateRunResult(TestResult instance)
        {
            try
            {

                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TestResult result = context.TestResults.Add(instance);
                    context.SaveChanges();
                    return result;
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return null;
            }
        }

        public bool Update()
        {
            try
            {
                // Entity context
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                    //ATFEnvironment.Log.logger.Info("Save changes to result" + this.ResultType);
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

        public static TestResult Update(int resultId, Object instance)
        {
            try 
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    TestResult result = context.TestResults.Find(resultId);
                    if (result != null)
                    {
                        context.Entry(result).CurrentValues.SetValues(instance);
                        context.SaveChanges();
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return null;
            }
        }

        public static bool Delete(int resultId)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TestResult testResult = context.TestResults.Find(resultId);
                    if (testResult == null)
                        return false;
                    context.TestResults.Attach(testResult);
                    context.TestResults.Remove(testResult);
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

        public static bool Delete(TestResult instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    if (!context.TestResults.Contains(instance))
                        return false;

                    context.TestResults.Remove(instance);
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

        public static TestResult GetTestResultByJobAndSourceID(int jobId, string sourceId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from r in context.TestResults
                        where r.ExecutionId == (
                                                from e in context.TestCaseExecutions
                                                where e.JobId == jobId &&
                                                e.TestCaseId == (
                                                                    from c in context.TestCases
                                                                    where c.SourceId == sourceId
                                                                    select c
                                                                    ).FirstOrDefault().TestCaseId
                                                select e
                                                ).FirstOrDefault().ExecutionId
                        select r).FirstOrDefault();
            }
        }
  
    }
}
