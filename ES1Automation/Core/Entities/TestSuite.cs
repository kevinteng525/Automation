using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Extensions;

namespace Core.Model
{
    public enum SuiteType
    {
        Static = 0,//the suite is synced from outer test case management system
        Temporary = 1,//the test suite is created for a task, it's internal and only used to get the test cases for a task
        Dependence = 2,//only defines the test cases dependence, such as whether several test cases can run on the same environment, it's internal too
        Dynamic = 3,//the test suites created by galaxy users, such as a smoke/sanity test suite
        TestPlan = 4,//stands for the test plan
        NotExisting = 5,
    }

    public partial class TestSuite
    {
        public static TestSuite GetRootNodeOfAllTestCases()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                //The test suite with the SuiteId=0 is the root suite by default.
                return context.TestSuites.Find(0);
            }
        }

        public static TestSuite GetRootNodeOfUserCustomizedTestSuites()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                //The test suite with the SuiteId=1000 is the root suite by default.
                return context.TestSuites.Find(1000);
            }
        }

        public static TestSuite GetRootNodeOfNormalTestSuitesFromExternalProvider()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                //The test suite with the SuiteId=2000 is the root suite by default.
                return context.TestSuites.Find(2000);
            }
        }

        public static TestSuite GetRootNodeOfNormalTestPlansFromExternalProvider()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                //The test suite with the SuiteId=2000 is the root suite by default.
                return context.TestSuites.Find(3000);
            }
        }

        public static List<TestSuite> GetAllTestSuites()
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return (from ts in context.TestSuites
                        where ts.Type == (int)SuiteType.Static
                        select ts).OrderBy(ts=>ts.SourceId).ToList();
            }                   
        }

        public static TestSuite GetTestSuite(int id)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestSuites.Find(id);
            }
        }

        public static TestSuite CreateSuite(TestSuite instance)
        {
            try
            {
                TestSuite ts = null;
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    instance.IsActive = true;
                    ts = context.TestSuites.Add(instance);                  
                    context.SaveChanges();
                }
                if (ts.Type == (int)SuiteType.Dynamic || ts.Type == (int) SuiteType.NotExisting)//user created test suite
                {
                    TestSuite root = TestSuite.GetRootNodeOfUserCustomizedTestSuites();
                    root.AddSubTestSuite(ts.SuiteId);
                }

                return ts;
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
                    context.TestSuites.Attach(this);
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

        public static TestSuite Update(int id, Object instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TestSuite t = context.TestSuites.Find(id);
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

        public static bool Delete(int suiteId)
        {
            try
            {
                TestSuite testSuite = TestSuite.GetTestSuite(suiteId);
                if (testSuite == null)
                {
                    return false;
                }
                else
                {
                    return Delete(testSuite); 
                }
            }
            catch (Exception )
            {
                return false;
            }
        }

        public static bool Delete(TestSuite instance)
        {
            try
            {
                List<TestSuite> parentSuites = null;

                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    if (TestSuite.GetTestSuite(instance.SuiteId) == null)
                        return false;
                    context.TestSuites.Attach(instance);
                    instance.IsActive = false;
                    context.SaveChanges(); 

                    string sId = instance.SuiteId.ToString();
                    parentSuites = (from ts in context.TestSuites
                                    where ts.SubSuites.Contains(sId)
                                    select ts).ToList();

                }
                
                foreach (TestSuite ts in parentSuites)
                {
                    ts.DeleteSubTestSuite(instance.SuiteId);
                }
            }

                  
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get TestSuites by provider id
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public static IList<TestSuite> GetTestSuitesByProviderId(int providerId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestSuites.Where(ts => ts.ProviderId == providerId).OrderBy(ts=>ts.SourceId).ToList();
            }
        }

        /// <summary>
        /// get the test suites by ProviderId and suite type
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="suiteType"></param>
        /// <returns></returns>
        public static IList<TestSuite> GetTestSuitesByProviderIdAndType(int providerId, SuiteType suiteType)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestSuites.Where(ts => ts.ProviderId == providerId && ts.Type == (int)suiteType && ts.IsActive == true).OrderBy(ts=>ts.SourceId).ToList();
            }
        }

        /// <summary>
        /// Get test suite by provider id and source id
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static TestSuite GetTestSuiteByProviderIdSourceIdAndType(int providerId, string sourceId, int suiteType)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TestSuites.Where(ts => ts.ProviderId == providerId && ts.SourceId == sourceId && ts.Type == (int)suiteType).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get direct cases, Or all cases, including those in sub-suites.
        /// </summary>
        /// <param name="suiteId"></param>
        /// <param name="bRecursive">whether include cases of sub-suites</param>
        /// <returns></returns>
        public static List<int> GetAllCasesIds(int suiteId, bool bRecursive = true)
        {
            List<int> allcases = new List<int>();

            // Get TestSuite and check existance here
            TestSuite testSuite = GetTestSuite(suiteId);
            if (testSuite == null)
            {
                return allcases;
            }
            // Get direct testcases
            string cases = testSuite.TestCases;
            List<int> ints = SplitStringToIntList(cases);
            if (ints != null && ints.Count() > 0)
            {
                allcases.AddRange(ints);
            }

            // Get sub TestSuites(including nested TestSuites)
            // then get all direct case of all those TestSuites
            if (bRecursive) 
            {
                List<int> subSuites = GetAllSubSuiteIds(suiteId, true);
                foreach (int subSuite in subSuites)
                {
                    List<int> strs = GetAllCasesIds(subSuite, false);
                    if (strs != null && strs.Count() > 0)
                    {
                        allcases.AddRange(strs);
                    }
                }            
            }

            return (allcases.Count() > 0) ? allcases.Distinct().ToList() : allcases;                  
        }

        /// <summary>
        /// Get all the test cases, including the sub suites
        /// </summary>
        /// <param name="suiteId"></param>
        /// <returns></returns>
        public static List<TestCase> GetAllCases(int suiteId, bool activeOnly = true)
        {
            List<int> allcases = TestSuite.GetAllCasesIds(suiteId, true);
            List<TestCase> testCasesList = new List<TestCase>();
            foreach (int caseId in allcases)
            {
                TestCase tc = TestCase.GetTestCase(caseId);
                if (tc != null && (!activeOnly || tc.IsActive))
                {
                    testCasesList.Add(tc);
                }
            }
            return testCasesList.OrderBy(tc=>tc.SourceId).ToList();
        }

        /// <summary>
        /// Get all cases, includeing those in sub-suites.
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="recursionDeepth">limit the recursion deepth, avoiding endless loop</param>
        /// <returns></returns>
        public static List<int> GetAllSubSuiteIds(int sID, bool bRecursive = true, int recursionDeepth = 0)
        {
            if (recursionDeepth > 10) // Adjust the deep limitation here, avoid endless loop
            {
                return null; 
            }

            ++recursionDeepth;

            List<int> suites = new List<int>();
            TestSuite testSuite = GetTestSuite(sID);
            if (testSuite == null)
            {
                return suites;
            }
            List<int> subIDs = SplitStringToIntList(testSuite.SubSuites);

            // Breadth fisrt
            foreach (int subID in subIDs)
            {
                TestSuite suite = TestSuite.GetTestSuite(subID);
                suites.Add(subID);
            }
            if (bRecursive)
            {
                foreach (int subID in subIDs)
                {
                    List<int> ints = GetAllSubSuiteIds(subID, bRecursive, recursionDeepth);
                    if(ints != null && ints.Count() > 0)
                    {
                        suites.AddRange(GetAllSubSuiteIds(subID, bRecursive, recursionDeepth));
                    }
                }
            }

            return (suites.Count() > 0) ? suites.Distinct().ToList() : suites;                
            
        }

        public static List<TestSuite> GetAllSubSuites(TestSuite suite, bool recursive = false)
        {
            List<TestSuite> subSuites = new List<TestSuite>();
            List<int> subSuiteIds = GetAllSubSuiteIds(suite.SuiteId, recursive, 0);
            foreach (int id in subSuiteIds)
            {
                subSuites.Add(TestSuite.GetTestSuite(id));
            }
            return subSuites.OrderBy(ts=>ts.SourceId).ToList();
        }

        public static List<TestCase> GetAllSubCases(int suiteId)
        {
            List<TestCase> subCases = new List<TestCase>();
            List<int> subCaseIds = GetAllCasesIds(suiteId, false);
            foreach (int id in subCaseIds)
            {
                subCases.Add(TestCase.GetTestCase(id));
            }
            return subCases.OrderBy(tc => tc.SourceId).ToList();
        }

        public bool AddSubTestCase(int testCaseId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                // Check the test cases
                TestCase testCase = context.TestCases.Find(testCaseId);
                if (testCase == null)
                    return false;
            }
                
                // Get all test cases
            List<int> allcases = SplitStringToIntList(TestCases);
            if (!allcases.Contains(testCaseId))
                allcases.Add(testCaseId);
            else
                return false;

            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                TestCases = CreateStringFromIntList(allcases);
                context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public bool AddSubTestSuite(int testSuiteId)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    // Check the test suites
                    TestSuite testSuite = context.TestSuites.Find(testSuiteId);
                    if (testSuite == null)
                    {
                        return false;
                    }
                }

                // Get all test cases
                List<int> allSubSuite = SplitStringToIntList(SubSuites);
                if (!allSubSuite.Contains(testSuiteId))
                {
                    allSubSuite.Add(testSuiteId);
                }
                else
                {
                    return false;
                }

                SubSuites = CreateStringFromIntList(allSubSuite);
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception )
            {
                return false;
            }
            
            return true;
        }

        public void DisableSubTestSuites()
        {
            foreach (TestSuite s in TestSuite.GetAllSubSuites(this))
            {
                s.IsActive = false;
                s.Description = string.Format("Disabled at {0}", System.DateTime.UtcNow);
                s.Update();
            }
        }

        public void DisableSubTestSuitesOfProvider( int providerId)
        {
            foreach (TestSuite s in TestSuite.GetAllSubSuites(this).Where(s=>s.ProviderId == providerId))
            {
                s.IsActive = false;
                s.Description = string.Format("Disabled at {0}", System.DateTime.UtcNow);
                s.Update();
            }
        }

        public bool DeleteSubTestCase(int testCaseId)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    // Check the test cases
                    TestCase testCase = context.TestCases.Find(testCaseId);
                    if (testCase == null)
                        return false;
                }

                // Get all test cases
                List<int> allcases = SplitStringToIntList(TestCases);
                if (allcases.Contains(testCaseId))
                    allcases.Remove(testCaseId);
                else
                    return false;

                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TestCases = CreateStringFromIntList(allcases);
                    context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

            }
            catch (Exception )
            { 
            }
            
            return true;
        }

        public bool DeleteInActiveSubTestSuites()
        {
            foreach (TestSuite suite in TestSuite.GetAllSubSuites(this))
            {
                if (!suite.IsActive)
                {
                    this.DeleteSubTestSuite(suite.SuiteId);
                }
            }
            return true;
        }

        public bool DeleteSubTestSuite(int testSuiteId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                // Check the test suite
                TestSuite testSuite = context.TestSuites.Find(testSuiteId);
                if (testSuite == null)
                {
                    return false;
                }
            }

            // Get all test suite
            List<int> allSubSuite = SplitStringToIntList(SubSuites);
            if (allSubSuite.Contains(testSuiteId))
            {
                allSubSuite.Remove(testSuiteId);
            }
            else
            {
                return false;
            }

            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                SubSuites = CreateStringFromIntList(allSubSuite);
                context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public static TestSuite CreateOrUpdateTestSuite(TestSuite testSuite)
        {
            TestSuite suite = TestSuite.GetTestSuiteByProviderIdSourceIdAndType(testSuite.ProviderId.Value, testSuite.SourceId, testSuite.Type);
            if (suite == null)
            {
                return TestSuite.CreateSuite(testSuite);
            }
            else
            {
                testSuite.SuiteId = suite.SuiteId;
                testSuite.Update();
                return testSuite;
            }
        }

        public static void CreateUpdateAllTestSuites(IList<TestSuite> testSuites)
        {
            foreach (var testSuite in testSuites)
            {
                if (TestSuite.GetTestSuite(testSuite.SuiteId) != null)
                {
                    testSuite.ModifyTime = DateTime.UtcNow;
                    testSuite.Update();
                }
                else
                {
                    testSuite.CreateTime = DateTime.UtcNow;
                    TestSuite.CreateSuite(testSuite);
                }
            }
        }

        /// <summary>
        /// Split the string of "SubSuites" or "TestCases" with Seperators{',', ' '},
        /// put them into List<int>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<int> SplitStringToIntList(string str)
        {
            List<int> lInt = new List<int>();
            if(!string.IsNullOrEmpty(str))
            {
                char[] separators = {',', ' '};
                string[] strings = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strings.Length; ++i)
                {
                    int id = 0;
                    string s = strings[i];
                    if (int.TryParse(s, out id))
                    {
                        lInt.Add(id);
                    }
                }
            }
            return lInt;
        }

        private static string CreateStringFromIntList(List<int> items)
        {
            string allitems = "";
            foreach (int item in items)
            {
                allitems += item.ToString();
                allitems += ",";
            }
            char[] separators = {',', ' '};
            allitems.TrimEnd(separators);
            return allitems;
        }

        public static void DisableTestSutiesByProviderAndType(int providerId, SuiteType suiteType)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                var suites = context.TestSuites.Where(ts => ts.ProviderId == providerId && ts.Type == (int)suiteType).Update(ts => new TestSuite { IsActive = false });
            }
        }
        
    }
}
