using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using EntityFramework.Extensions;

namespace Core.Model
{
    public partial class TestCase
    {
        public static List<TestCase> GetAllTestCases(bool activeOnly = true)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    return (from tc in context.TestCases
                            where (activeOnly ? (tc.IsActive == true) : true)
                            select tc).OrderBy(tc=>tc.SourceId).ToList();

                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
            }
            return null;
        }

        public static TestCase GetTestCase(int caseId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestCases.Find(caseId);
            }
        }

        public static TestCase GetTestCase(string sourceId, int providerId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestCases.Where(tc => tc.SourceId == sourceId && tc.ProviderId == providerId).SingleOrDefault();
            }
        }

        public static TestCase GetTestCaseByResultId(int resultId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                TestResult result = context.TestResults.Find(resultId);
                TestCaseExecution execution = context.TestCaseExecutions.Find(result.ExecutionId);
                TestCase testCase = context.TestCases.Find(execution.TestCaseId);
                return testCase;
            }
        }
        public static TestCase GetTestCaseByName(String name)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {

                return context.TestCases.Where(tc => tc.Name == name).SingleOrDefault();
            }
        }
        public static TestCase CreateCase(TestCase instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    instance.IsActive = true;
                    TestCase tc = context.TestCases.Add(instance);
                    context.SaveChanges();
                    return tc;
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

                    context.TestCases.Attach(this);
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

        public static TestCase Update(int id, Object instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TestCase t = context.TestCases.Find(id);
                    if (t != null)
                    {
                        context.Entry(t).CurrentValues.SetValues(instance);
                        context.SaveChanges();
                        return t;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return null;
            }
        }

        public static TestCase CreateOrUpdateTestCase(TestCase testCase)
        {
            TestCase findTestCase = TestCase.GetTestCase(testCase.SourceId, testCase.ProviderId);

            // update testcase
            if (findTestCase == null)
            {
                testCase.CreateBy = 0;//automation
                testCase.CreateTime = DateTime.UtcNow;
                TestCase.CreateCase(testCase);
            }
            else
            {
                testCase.TestCaseId = findTestCase.TestCaseId;
                testCase.ModifyBy = 0;//automation
                testCase.ModifyTime = DateTime.UtcNow;

                testCase.Update();
            }

            return testCase;
        }

        public static bool Delete(int caseId)
        {
            try
            {
                TestCase testCase = GetTestCase(caseId);
                if (testCase == null)
                    return false;

                return Delete(testCase);
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

        }

        public static bool Delete(TestCase instance)
        {
            try
            {
                List<TestSuite> parentSuites = null;

                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    if (TestCase.GetTestCase(instance.TestCaseId) == null)
                        return false;
                    context.TestCases.Attach(instance);
                    context.Entry(instance).Reload();
                    instance.IsActive = false;
                    context.Entry(instance).State = System.Data.Entity.EntityState.Modified;

                    string sId = instance.TestCaseId.ToString();
                    // Remove CaseId from test suites
                    parentSuites = (from ts in context.TestSuites
                                    where ts.TestCases.Contains(sId)
                                    select ts).ToList();
                    foreach (TestSuite ts in parentSuites)
                    {
                        ts.TestCases = ts.TestCases.Replace(sId, "");
                        context.TestSuites.Attach(ts);
                        context.Entry(ts).State = System.Data.Entity.EntityState.Modified;
                    }
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

        public static void DisableTestCases(int providerId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                var cases = context.TestCases.Where(tc => tc.ProviderId == providerId);
                cases.Update(tc => new TestCase { IsActive = false });
                //context.TestCases.Update(cases, tc => new TestCase { IsActive = false });
            }
        }

        public Platform GetPlatform()
        {
            string platform = this.Description.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().Find(c => c.Contains("platform="));
            if (string.IsNullOrEmpty(platform))
            {
                return Platform.Undefined;
            }

            platform = platform.Substring(platform.IndexOf('=') + 1);
            if (string.IsNullOrEmpty(platform))
            {
                return Platform.Undefined;
            }
            else
            {
                if (platform == "exchange")
                {
                    return Platform.Exchange;
                }
                else if (platform == "domino")
                {
                    return Platform.Domino;
                }
                else if (platform == "all")
                {
                    return Platform.All;
                }
                else
                {
                    return Platform.Undefined;
                }
            }
        }

        public Runtime GetRuntime()
        {
            string runtime = this.Description.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().Find(c => c.Contains("runtime="));
            if (string.IsNullOrEmpty(runtime))
            {
                return Runtime.Undefined;
            }

            runtime = runtime.Substring(runtime.IndexOf('=') + 1);
            if (string.IsNullOrEmpty(runtime))
            {
                return Runtime.Undefined;
            }
            else
            {
                if (runtime == "minitest")
                {
                    return Runtime.RubyMiniTest;
                }
                else if (runtime == "nunit")
                {
                    return Runtime.CSharpNUnit;
                }
                else if (runtime == "msunit")
                {
                    return Runtime.CSharpMSUnit;
                }
                else
                {
                    return Runtime.Undefined;
                }
            }
        }
    }
}
