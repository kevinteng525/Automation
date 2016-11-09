using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core.Model;
using ES1Common.Exceptions;
using ES1Common.RQM;

namespace Core.Providers.TestCaseProviders
{
    public class RQMTestCaseProvider : ITestCaseProvider
    {
        private readonly string RQMTESTPLANPREFIX = "[Galaxy]";

        protected static string ModuleName;

        protected string Url { get; private set; }

        public RQMServer RQMServer { get; protected set; }

        public string Username { get; protected set; }

        public string Password { get; protected set; }

        public string ProjectAlias { get; protected set; }

        public Provider Provider { get; set; }

        public void ApplyConfig(string config)
        {
            XElement root = XElement.Parse(config);

            Url = root.Element("url").Value;
            Username = root.Element("username").Value;
            Password = root.Element("password").Value;
            ProjectAlias = root.Element("projectAlias").Value;

            ModuleName = string.Format("RQM_{0}({1})", ProjectAlias, Url);

            try
            {
                RQMServer = new RQMServer(Url);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Init RQM Failed.",  ex);
                throw new FrameworkException(ModuleName, "Init RQM Failed");
            }        

        }


        public void SyncAllTestPlan()
        {
            try
            {
                ATFEnvironment.Log.logger.Info("RQM Login");

                RQMServer.UserAuthentication(Username, Password);

                ATFEnvironment.Log.logger.Info("RQM Sync Test Plans Start");

                //disable all the test suites from the external provider
                TestSuite rootSuiteOfTestPlans = TestSuite.GetRootNodeOfNormalTestPlansFromExternalProvider();

                rootSuiteOfTestPlans.DisableSubTestSuitesOfProvider(Provider.ProviderId);

                Product product = GetProductOfTestCaseProvider();//Product.GetProductByName(RQMProjectProductMapping["SourceOne+%28Quality+Management%29"]);

                if (product != null)
                {
                    IEnumerable<RQMTestSuite> rqmTestPlanSuites = RQMServer.GetTestPlansByProject(ProjectAlias);
                    foreach (RQMTestSuite rqmTestSuite in rqmTestPlanSuites)
                    {
                        //skip the test plan created by Galaxy
                        if (rqmTestSuite.Title.StartsWith(RQMTESTPLANPREFIX))
                        {
                            continue;
                        }

                        string testCaseIdString = string.Empty;
                        foreach (string testCaseWebId in rqmTestSuite.SubTestCaseSourceIds)
                        {
                            TestCase testCase = TestCase.GetTestCase(testCaseWebId, this.Provider.ProviderId);
                            //only sync the automated test cases
                            if (testCase != null && testCase.IsAutomated)
                            {
                                if (testCaseIdString == string.Empty)
                                {
                                    testCaseIdString = (testCase.TestCaseId).ToString();
                                }
                                else
                                {
                                    testCaseIdString = testCaseIdString + "," + (testCase.TestCaseId).ToString();
                                }
                            }
                        }

                        string testSuiteIdString = string.Empty;
                        foreach (string suiteWebId in rqmTestSuite.SubTestSuiteSourceIds)
                        {
                            TestSuite suite = TestSuite.GetTestSuiteByProviderIdSourceIdAndType(this.Provider.ProviderId, suiteWebId, (int)SuiteType.Static);
                            if (suite != null)
                            {
                                if (string.IsNullOrEmpty(testSuiteIdString))
                                {
                                    testSuiteIdString = suite.SuiteId.ToString();
                                }
                                else
                                {
                                    testSuiteIdString = string.Format("{0},{1}", testSuiteIdString, suite.SuiteId);
                                }
                            }
                        }

                        string description = string.Format("Title={0}, Type={1}", rqmTestSuite.Title, SuiteType.TestPlan.ToString());
                        if (rqmTestSuite.Categories != null)
                        {
                            foreach (KeyValuePair<string, string> category in rqmTestSuite.Categories)
                            {
                                description = string.Format("{0}, {1}={2}", description, category.Key, category.Value);
                            }
                        }

                        TestSuite newSuite = new TestSuite
                        {
                            ProviderId = this.Provider.ProviderId,
                            SourceId = rqmTestSuite.SourceId,
                            Name = string.Format(@"[{0,6}]:{1}", rqmTestSuite.SourceId, rqmTestSuite.Title),
                            Type = (int)SuiteType.TestPlan,
                            SubSuites = testSuiteIdString,
                            TestCases = testCaseIdString,
                            IsActive = true,
                            CreateBy = 0,
                            ModityBy = 0,
                            Description = description,
                        };

                        TestSuite testSuite = TestSuite.CreateOrUpdateTestSuite(newSuite);

                        rootSuiteOfTestPlans.AddSubTestSuite(testSuite.SuiteId);
                    }
                }

                rootSuiteOfTestPlans.DeleteInActiveSubTestSuites();

                RQMServer.Logout();

                ATFEnvironment.Log.logger.Info("Logout RQM server");

                ATFEnvironment.Log.logger.Info("RQM Sync Test Plans Finish");
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Error happened while sync the test plans from RQM.", ex);
            }
        }

        /// <summary>
        /// Sync all the test suite, here we only use the test suite to get test cases dependence.
        /// </summary>
        public void SyncAllTestSuite()
        {
            try
            {

                ATFEnvironment.Log.logger.Info("RQM Login");

                RQMServer.UserAuthentication(Username, Password);

                ATFEnvironment.Log.logger.Info("RQM Sync Test Suites Start");

                // disable all test suites of this provider
                //TODO, there's an assumption that only the test suite from RQM with ForAutomation=True can be with SuiteType.Dependence.
                //So here we disable all the dependence suite and will enable them based on whether it exists in RQM any more
                TestSuite.DisableTestSutiesByProviderAndType(this.Provider.ProviderId, SuiteType.Dependence);

                //disable all the test suites from the external provider, typically the normal test suite defined in RQM, ForAutomation=Falses
                TestSuite rootSuiteOfExternalProviders = TestSuite.GetRootNodeOfNormalTestSuitesFromExternalProvider();

                rootSuiteOfExternalProviders.DisableSubTestSuitesOfProvider(Provider.ProviderId);

                Product product = GetProductOfTestCaseProvider();//Product.GetProductByName(RQMProjectProductMapping["SourceOne+%28Quality+Management%29"]);

                if (product != null)
                {

                    foreach (RQMTestSuite rqmTestSuite in RQMServer.GetTestSuitesByProject(ProjectAlias))
                    {
                        bool isForAutomation = false;
                        try
                        {
                            isForAutomation = rqmTestSuite.Categories.ContainsKey("ForAutomation") ? bool.Parse(rqmTestSuite.Categories["ForAutomation"]) : false;
                        }
                        catch (Exception)
                        {

                        }

                        string testCaseIdString = string.Empty;
                        foreach (string webId in rqmTestSuite.SubTestCaseSourceIds)
                        {
                            TestCase testCase = TestCase.GetTestCase(webId, this.Provider.ProviderId);
                            //only sync the automated test cases
                            if (testCase != null && testCase.IsAutomated)
                            {
                                if (testCaseIdString == string.Empty)
                                {
                                    testCaseIdString = (testCase.TestCaseId).ToString();
                                }
                                else
                                {
                                    testCaseIdString = testCaseIdString + "," + (testCase.TestCaseId).ToString();
                                }
                            }
                        }

                        TestSuite newSuite = new TestSuite
                                 {
                                     ProviderId = this.Provider.ProviderId,
                                     SourceId = rqmTestSuite.SourceId,
                                     Name = string.Format(@"[{0,6}]:{1}", rqmTestSuite.SourceId, rqmTestSuite.Title),
                                     Type = (int)SuiteType.Static,
                                     SubSuites = null,
                                     TestCases = testCaseIdString,
                                     IsActive = true,
                                     CreateBy = 0,
                                     ModityBy = 0,
                                 };

                        if (isForAutomation)
                        {
                            newSuite.Type = (int)SuiteType.Dependence;
                            newSuite.Description = string.Format("Title={0}, Weight={1}, SequentialExecution={2}, HaltOnFailure={3}, Type={4}, ForAutomation={5}", rqmTestSuite.Title, rqmTestSuite.Weight, rqmTestSuite.SequentialExecution, rqmTestSuite.HaltOnFailure, SuiteType.Dependence.ToString(), isForAutomation);
                            TestSuite.CreateOrUpdateTestSuite(newSuite);
                        }

                        newSuite.Type = (int)SuiteType.Static;
                        newSuite.Description = string.Format("Title={0}, Weight={1}, SequentialExecution={2}, HaltOnFailure={3}, Type={4}, ForAutomation={5}", rqmTestSuite.Title, rqmTestSuite.Weight, rqmTestSuite.SequentialExecution, rqmTestSuite.HaltOnFailure, SuiteType.Static.ToString(), isForAutomation);
                        TestSuite.CreateOrUpdateTestSuite(newSuite);

                        rootSuiteOfExternalProviders.AddSubTestSuite(newSuite.SuiteId);

                    }
                }

                rootSuiteOfExternalProviders.DeleteInActiveSubTestSuites();

                RQMServer.Logout();

                ATFEnvironment.Log.logger.Info("Logout RQM server");

                ATFEnvironment.Log.logger.Info("RQM Sync Test Suite Finish");
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Error met while sync the test suites from RQM.", ex);
            }
        }

        public void SyncAllTestCase()
        {
            try
            {

                ATFEnvironment.Log.logger.Info("RQM Login");

                RQMServer.UserAuthentication(Username, Password);

                ATFEnvironment.Log.logger.Info("RQM Sync Test Cases Start");

                // disable all test cases of this provider
                TestCase.DisableTestCases(this.Provider.ProviderId);

                Product product = GetProductOfTestCaseProvider();//Product.GetProductByName(RQMProjectProductMapping["SourceOne+%28Quality+Management%29"]);

                if (product != null)
                {
                    //update the test suite hierarchy
                    var root = TestSuite.GetRootNodeOfAllTestCases();
                    CreateOrUpdateProjectLevelSuite(root);

                    //the leaf node of test suite
                    //var testSuites = TestSuite.GetTestSuitesByProviderId(Provider.ProviderId).Where(ts => ts.SubSuites == null && ts.IsActive == true && ts.Type == (int)SuiteType.Static).ToList();
                    var testSuites = TestSuite.GetAllSubSuites(root, true).Where(
                        ts => ts.SubSuites == null &&
                            ts.IsActive == true &&
                            ts.Type == (int)SuiteType.Static &&
                            ts.ProviderId == this.Provider.ProviderId
                            ).ToList();
                    //remove all the test case children of the leaf node of test suite
                    foreach (var testSuite in testSuites)
                    {
                        testSuite.CreateBy = 0;
                        testSuite.ModityBy = 0;
                        testSuite.TestCases = string.Empty;
                    }

                    foreach (var rqmTestCase in RQMServer.GetTestCasesByProject(ProjectAlias))
                    {
                        string ranking = rqmTestCase.Categories.ContainsKey("Ranking") ? rqmTestCase.Categories["Ranking"] : null;
                        string release = rqmTestCase.Categories.ContainsKey("Release") ? rqmTestCase.Categories["Release"] : null;
                        string features = rqmTestCase.Categories.ContainsKey("Features") ? rqmTestCase.Categories["Features"] : string.Empty;
                        string module = rqmTestCase.Categories.ContainsKey("Module") ? rqmTestCase.Categories["Module"] : null;
                        string platform = rqmTestCase.Categories.ContainsKey("Platform") ? rqmTestCase.Categories["Platform"] : null;
                        string runtime = rqmTestCase.Categories.ContainsKey("RunTime") ? rqmTestCase.Categories["RunTime"] : null;
                        if (string.IsNullOrEmpty(runtime))
                        {
                            //This is to fix the issue that the category of "RunTime" in SourceOne project is called "RunTIme" but in Supervisor Project, it's called "Runtime"
                            runtime = rqmTestCase.Categories.ContainsKey("Runtime") ? rqmTestCase.Categories["Runtime"] : null;                            
                        }
                        bool isAutomated = rqmTestCase.Categories.ContainsKey("IsAutomated") ? bool.Parse(rqmTestCase.Categories["IsAutomated"]) : false;

                        if (ranking != null)
                        {
                            Ranking rankingtemp = new Ranking
                            {
                                Name = ranking,
                                Description = ranking,

                            };
                            Ranking.AddOrUpdateRanking(rankingtemp);
                        }

                        if (release != null)
                        {
                            Release tempRelease = new Release
                            {
                                Name = release,
                                Description = release,
                                Path = string.Empty,
                                Type = (int)SyncSourceType.FromRQM,
                                ProductId = product.ProductId,
                            };

                            Release.AddOrUpdateRelease(tempRelease);
                        }

                        if (isAutomated)
                        {
                            var testCase = TestCase.CreateOrUpdateTestCase
                                (
                                    new TestCase
                                    {
                                        ProviderId = this.Provider.ProviderId,
                                        SourceId = rqmTestCase.WebId,
                                        Name = string.Format("[{0, 6}]:{1}", rqmTestCase.WebId, rqmTestCase.Title),
                                        Feature = features,
                                        Description = string.Format("WebId={0}, Release={1}, Ranking={2}, Module={3}, Feature={4}, RunTime={5}, Platform={6}, IsAutomated={7}", rqmTestCase.WebId, release, ranking, module, features, runtime, platform, isAutomated),
                                        ProductId = product.ProductId,
                                        Weight = rqmTestCase.Weight,
                                        Ranking = ranking,
                                        Release = release,
                                        IsAutomated = isAutomated,
                                        IsActive = true,
                                    }
                                );

                            // update test suite
                            // here we have a assumption that the feature names under different modules of projects should be unique. 
                            foreach (var testSuite in testSuites)
                            {
                                if (rqmTestCase.Categories.Keys.Contains("Features") && rqmTestCase.Categories["Features"] == testSuite.Name)
                                {
                                    if (string.IsNullOrWhiteSpace(testSuite.TestCases))
                                    {
                                        testSuite.TestCases = testCase.TestCaseId.ToString();
                                    }
                                    else
                                    {
                                        testSuite.TestCases = testSuite.TestCases + "," + testCase.TestCaseId.ToString();
                                    }
                                }
                            }
                        }
                    }

                    TestSuite.CreateUpdateAllTestSuites(testSuites);
                }

                RQMServer.Logout();

                ATFEnvironment.Log.logger.Info("Logout RQM server");

                ATFEnvironment.Log.logger.Info("RQM Sync Test Cases Finish");
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Met error while sync the test cases from RQM server.", ex);
            }
        }

        public void SyncTestCaseById(int caseId)
        {
            throw new NotImplementedException();
        }

        public void SyncTestCaseBySourceId(string sourceId)
        {
            throw new NotImplementedException();
        }

        public TestCase GetTestCaseBySourceId(string sourceId)
        {
            throw new NotImplementedException();
        }

        #region private function

        private Product GetProductOfTestCaseProvider()
        {
            using (var context = new ES1AutomationEntities())
            {
                return context.Products.Where(p => p.TestCaseProviderId == this.Provider.ProviderId &&
                    p.RQMProjectAlias == this.ProjectAlias).FirstOrDefault();
            }
        }

        private void CreateOrUpdateProjectLevelSuite(TestSuite suite)
        {
            IList<RQMProject> projects = RQMServer.GetAllProjects();
            List<TestSuite> suites = TestSuite.GetAllSubSuites(suite);
            RQMProject project = projects.Where(p => p.Alias == this.ProjectAlias).FirstOrDefault();
            if (null != project)
            {
                TestSuite projectSuite = suites.Where(s => s.Name == project.Title).FirstOrDefault();
                if (null == projectSuite)
                {
                    TestSuite temp = new TestSuite
                    {
                        Name = project.Title,
                        ProviderId = Provider.ProviderId,
                        IsActive = true,
                        Description = string.Format("Title=Project {0} in RQM, Type={1}", project.Title, SuiteType.Static.ToString()),
                        CreateBy = 0,
                        Type = 0,
                        SubSuites = null,
                        TestCases = null,
                    };
                    projectSuite = TestSuite.CreateSuite(temp);
                    suite.AddSubTestSuite(projectSuite.SuiteId);
                    CreateOrUpdateModuleLevelSuite(projectSuite, project);
                }
                else
                {
                    //we don't remove the project from the hierarchy even it doesn't exist on RQM
                    //because not all the project are on RQM and we may have some customized project
                    CreateOrUpdateModuleLevelSuite(projectSuite, project);
                }
            }
            
        }

        private void CreateOrUpdateModuleLevelSuite(TestSuite projectSuite, RQMProject project)
        {
            IList<RQMModule> modules = RQMServer.GetModulesOfProject(project);
            IList<TestSuite> moduleSuites = TestSuite.GetAllSubSuites(projectSuite);
            //disable all the module level suites firstly
            projectSuite.DisableSubTestSuites();
            //iterate all the modules in RQM server, enable the existing one and add new ones
            foreach (RQMModule m in modules)
            {
                TestSuite moduleSuite = moduleSuites.Where(s => s.Name == m.Title).FirstOrDefault();
                if (null == moduleSuite)
                {
                    //add module suite
                    TestSuite temp = new TestSuite
                    {
                        Name = m.Title,
                        ProviderId = Provider.ProviderId,
                        IsActive = true,
                        Description = string.Format("Title=Module {0} in RQM, Type={1}", m.Title, SuiteType.Static.ToString()),
                        TestCases = null,
                        SubSuites = null,
                        CreateBy = 0,
                        ModityBy = 0,
                    };
                    moduleSuite = TestSuite.CreateSuite(temp);
                    projectSuite.AddSubTestSuite(moduleSuite.SuiteId);
                    CreateOrUpdateFeatureLevelSuite(moduleSuite, project);
                }
                else
                {
                    moduleSuite.IsActive = true;
                    moduleSuite.ProviderId = Provider.ProviderId;
                    moduleSuite.Update();
                    CreateOrUpdateFeatureLevelSuite(moduleSuite, project);
                }
            }
        }

        private void CreateOrUpdateFeatureLevelSuite(TestSuite moduleSuite, RQMProject project)
        {
            IList<RQMFeature> modules = RQMServer.GetFeaturesOfProject(project);
            IList<TestSuite> featureSuites = TestSuite.GetAllSubSuites(moduleSuite);
            moduleSuite.DisableSubTestSuites();
            RQMFeature module = modules.Where(m => m.Title == moduleSuite.Name).FirstOrDefault();
            if (null != module)
            {
                IList<RQMFeature> features = module.SubFeatures;
                foreach (RQMFeature feature in features)
                {
                    TestSuite featureSuite = featureSuites.Where(s => s.Name == feature.Title).FirstOrDefault();
                    if (null == featureSuite)
                    {
                        TestSuite temp = new TestSuite
                        {
                            Name = feature.Title,
                            ProviderId = Provider.ProviderId,
                            IsActive = true,
                            TestCases = null,
                            SubSuites = null,
                            Description = string.Format("Title=Feature {0} in RQM, Type={1}", feature.Title, SuiteType.Static.ToString()),
                            CreateBy = 0,
                            ModityBy = 0,
                        };
                        featureSuite = TestSuite.CreateSuite(temp);
                        moduleSuite.AddSubTestSuite(featureSuite.SuiteId);
                    }
                    else
                    {
                        featureSuite.IsActive = true;
                        featureSuite.ProviderId = Provider.ProviderId;
                        featureSuite.Update();
                    }
                }
            }
            else
            {

            }

        }

        /// <summary>
        /// mapping between the test result in Galaxy and RQM
        /// </summary>
        /// <param name="result">The result type defined in Galaxy</param>
        /// <returns>The test result in RQM</returns>
        private string ConvertGalaxyResultToRQMResult(ResultType result)
        {
            switch (result)
            {
                case ResultType.Pass:
                    return RQMTestResult.Passed;
                case ResultType.Failed:
                case ResultType.KnownIssue:
                case ResultType.NewIssue:
                    return RQMTestResult.Failed;
                case ResultType.NotRun:
                case ResultType.TimeOut:
                    return RQMTestResult.Incomplete;
                case ResultType.Exception:
                case ResultType.CommonLibIssue:
                case ResultType.EnvironmentIssue:
                case ResultType.ScriptsIssue:
                    return RQMTestResult.Error;
                default:
                    return RQMTestResult.Error;
            }
        }
        #endregion


        public List<string> GetAllTestCaseRankings()
        {
            try
            {
                ATFEnvironment.Log.logger.Info("RQM Login");

                RQMServer.UserAuthentication(Username, Password);

                ATFEnvironment.Log.logger.Info("RQM Get Rankins Start");

                RQMProject project = new RQMProject();

                project.Alias = ProjectAlias;

                List<string> rankings = new List<string>();

                foreach (RQMRanking rank in RQMServer.GetRankingsOfProject(project))
                {
                    rankings.Add(rank.Title);
                }

                ATFEnvironment.Log.logger.Info("RQM Get Rankins Finished");

                return rankings;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to get all the rankings from RQM server.", ex);
                return new List<string>();
            }
        }

        public List<string> GetAllTestCaseReleases()
        {
            try
            {
                ATFEnvironment.Log.logger.Info("RQM Login");

                RQMServer.UserAuthentication(Username, Password);

                ATFEnvironment.Log.logger.Info("RQM Get Rankings Start");

                RQMProject project = new RQMProject();

                project.Alias = ProjectAlias;

                List<string> releases = new List<string>();

                foreach (RQMRelease release in RQMServer.GetReleasesOfProject(project))
                {
                    releases.Add(release.Title);
                }

                ATFEnvironment.Log.logger.Info("RQM Get releases Finished");

                return releases;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to get the releases of projects from RQM", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// Write the test result back to RQM
        /// </summary>
        /// <param name="task">the automation task</param>
        public void WriteBackTestResult(AutomationTask task)
        {
            ATFEnvironment.Log.logger.Info("RQM Login");
            try
            {
                RQMServer.UserAuthentication(Username, Password);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to login RQM", ex);
                return;
            }

            ATFEnvironment.Log.logger.Info(string.Format("Start to write back the test result for task [{0}]", task.Name));

            string testPlanTitle = string.Format("{0} : {1}", RQMTESTPLANPREFIX, task.Name);
            string projectAlias = ProjectAlias;

            SupportedEnvironment environment = SupportedEnvironment.GetSupportedEnvironmentById(task.EnvironmentId);
            string environmentId = string.Empty;
            try
            {
                environmentId = RQMServer.GetOrCreateTestEnvironment(projectAlias, environment.Name, environment.Description, environment.Config);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to get or create the test environment in RQM", ex);
            }
            Build build = Build.GetBuildById(task.BuildId);
            string buildId = string.Empty;
            try
            {
                buildId = RQMServer.GetOrCreateBuildRecord(projectAlias, build.Name, string.Format("Build created by Galaxy system. Name:{0}; Location:{1}; Description: {2}", build.Name, build.Location, build.Description));
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to get or create the build record in RQM", ex);
            }

            List<TestCase> testCases = AutomationTask.GetTestCasesForAutomationTask(task.TaskId).ToList();

            string testPlanWebId = string.Empty;
            if (task.Type == (int)TaskType.ByTestPlan)
            {
                try
                {
                    TestSuite rootSuite = TestSuite.GetTestSuite(int.Parse(task.TestContent));
                    TestSuite testPlanSuite = TestSuite.GetTestSuite(int.Parse(rootSuite.SubSuites.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().Trim()));
                    testPlanWebId = testPlanSuite.SourceId;
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Failed to get the test plan through the task."), ex);
                }
            }
            else
            {
                try
                {
                    testPlanWebId = RQMServer.CreateTestPlan(projectAlias, testPlanTitle, task.Description, testCases.Select(t => t.SourceId).ToArray(), new string[] { environmentId });
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error("Failed to create test plan in RQM", ex);
                }
            }
                
            if (string.IsNullOrWhiteSpace(testPlanWebId))
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to create or get the existing test plan [{0}] on RQM.", testPlanTitle));
                return;
            }
            else
            {
                ATFEnvironment.Log.logger.Info(string.Format("Test plan [{0}:{1}] has been created in RQM.", testPlanWebId, testPlanTitle));
            }

            foreach (TestCase testCase in testCases)
            {
                string executionRecordWebId = string.Empty;

                TestCaseExecution execution = AutomationTask.GetTestCaseExecutionForTestCaseInTask(task.TaskId, testCase.TestCaseId);
                System.DateTime startTime = (execution != null && execution.StartTime.HasValue) ? execution.StartTime.Value : System.DateTime.UtcNow;
                System.DateTime endTime = (execution != null && execution.EndTime.HasValue) ? execution.EndTime.Value : System.DateTime.UtcNow;
                try
                {
                    executionRecordWebId = RQMServer.GetOrCreateTestCaseExecutionRecord(projectAlias, testCase.Name, string.Format("Created by Galaxy. Test environment : {0}, Build : {1}", environment.Name, build.Name), testCase.SourceId, testPlanWebId, environmentId);
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error("Failed to get or create test case execution record in RQM", ex);
                }
                if (string.IsNullOrWhiteSpace(executionRecordWebId))
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Failed to create the test execution record for case [{0}] on RQM.", testCase.Name));
                    continue;
                }
                else
                {
                    ATFEnvironment.Log.logger.Info(string.Format("Test execution record [{0}:{1}] has been created in RQM.", executionRecordWebId, testCase.Name));
                }

                string result = string.Empty;
                if (execution != null)
                {
                    TestResult testResult = TestResult.GetTestResultByExecutionId(execution.ExecutionId);
                    result = ConvertGalaxyResultToRQMResult(testResult.ResultType);
                }
                else
                {
                    result = RQMTestResult.Incomplete;
                }

                string resultWebId = string.Empty;
                try
                {
                    resultWebId = RQMServer.CreateTestResult(projectAlias, executionRecordWebId, testCase.Name, result, buildId, startTime, endTime);
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error("Failed to create test result in RQM", ex);
                }
                if (string.IsNullOrWhiteSpace(resultWebId))
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Failed to create the execution result [{0}] in RQM.", testCase.Name));
                    continue;
                }
                else
                {
                    ATFEnvironment.Log.logger.Info(string.Format("Execution result [{0}:{1}] has been created in RQM.", resultWebId, testCase.Name));
                }
            }

            RQMServer.Logout();
        }
       
    }
}
