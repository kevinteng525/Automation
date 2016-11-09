using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace ES1Common.RQM
{
    public class RQMTestResult
    {
        public static readonly string Passed = "passed";
        public static readonly string Failed = "failed";
        public static readonly string Error = "error";
        public static readonly string Incomplete = "incomplete";
        public static readonly string Blocked = "blocked";
    }

    public class RQMServer
    {
        public const string RQMRESTfulUrl = "service/com.ibm.rqm.integration.service.IIntegrationService";

        public static readonly XNamespace XN = "http://www.w3.org/2005/Atom";

        public static readonly XNamespace XN1 = "http://schema.ibm.com/vega/2008/";

        public static readonly XNamespace XN2 = "http://jazz.net/xmlns/alm/qm/v0.1/";

        public static readonly XNamespace XN3 = "http://purl.org/dc/elements/1.1/";

        public static readonly XNamespace XN4 = "http://jazz.net/xmlns/prod/jazz/process/0.6/";

        public static readonly XNamespace XN5 = "http://jazz.net/xmlns/alm/v0.1/";

        public static readonly XNamespace XN6 = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public static readonly XNamespace XN7 = "http://purl.org/dc/terms/";

        public static readonly XNamespace XN8 = "http://jazz.net/xmlns/alm/qm/v0.1/tsl/v0.1/";

        public static readonly XNamespace XN9 = "http://jazz.net/xmlns/alm/qm/v0.1/testscript/v0.1/";

        public static readonly XNamespace XN10 = "http://jazz.net/xmlns/alm/qm/qmadapter/task/v0.1";

        public static readonly XNamespace XN11 = "http://jazz.net/xmlns/alm/qm/qmadapter/v0.1";

        public static readonly XNamespace XN12 = "http://jazz.net/xmlns/alm/qm/v0.1/executionworkitem/v0.1";

        public static readonly XNamespace XN13 = "http://jazz.net/xmlns/alm/qm/v0.1/executionresult/v0.1";

        public static readonly XNamespace XN14 = "http://jazz.net/xmlns/alm/qm/v0.1/catalog/v0.1";

        public static readonly XNamespace XN16 = "http://jazz.net/xmlns/alm/qm/styleinfo/v0.1/";

        public static readonly XNamespace XN17 = "http://www.w3.org/1999/XSL/Transform";

        private CookieContainer cookieContainer = new CookieContainer();

        public string Url { get; private set; }

        public RQMServer(string url)
        {
            Url = url;

            // ssl certificate
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                 (
                     delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                     {
                         return true;
                     }
                 );
        }

        /// <summary>
        /// All RQM RESTful API should in Authentication context
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void UserAuthentication(string username, string password)
        {
            HttpWebRequest request = WebRequest.Create(Url + "/authenticated/identity") as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;
            request.GetResponse().Close();

            request = WebRequest.Create(Url + "/authenticated/j_security_check") as HttpWebRequest;
            request.Method = "POST";
            request.CookieContainer = cookieContainer;
            var postData = Encoding.Default.GetBytes(string.Format("j_username={0}&j_password={1}", username, password));
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(postData, 0, postData.Length);
            dataStream.Close();

            request.GetResponse().Close();
        }


        public void Logout()
        {
            string logoutURL = Url + @"/service/com.ibm.team.repository.service.internal.ILogoutRestService";
            HttpWebRequest request = WebRequest.Create(logoutURL) as HttpWebRequest;
            request.Method = "POST";
            request.CookieContainer = cookieContainer;
            request.GetResponse().Close();
        }

        /// <summary>
        /// Get all projects information from RQM
        /// </summary>
        /// <returns></returns>
        public IList<RQMProject> GetAllProjects()
        {
            string requestUrl = string.Format("{0}/{1}/resources/projects", Url, RQMRESTfulUrl);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            string result = string.Empty;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            XElement root = XElement.Parse(result);

            IList<RQMProject> projects = new List<RQMProject>();

            foreach (var xml in root.Elements(RQMServer.XN + "entry"))
            {
                projects.Add(new RQMProject(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "project")));
            }

            return projects;
        }

        /// <summary>
        /// Get testcase by project and id
        /// </summary>
        /// <param name="project"></param>
        /// <param name="webId"></param>
        /// <returns></returns>
        public RQMTestCase GetTestCase(RQMProject project, string webId)
        {
            string requestUrl = string.Format("{0}/{1}/resources/{2}/testcase/urn:com.ibm.rqm:testcase:{3}", Url, RQMRESTfulUrl, project.Alias, webId);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            string result = string.Empty;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            XElement root = XElement.Parse(result);

            return new RQMTestCase(root);
        }

        /// <summary>
        /// Get all test cases under specific project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMTestCase> GetTestCasesByProject(RQMProject project)
        {
            return GetTestCasesByProject(project.Alias);
        }

        /// <summary>
        /// Get all the test suites under specific project
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <returns></returns>
        public IList<RQMTestSuite> GetTestSuitesByProject(string projectAlias)
        {
            IList<RQMTestSuite> testsuites = new List<RQMTestSuite>();

            string requestUrl = string.Format("{0}/{1}/resources/{2}/testsuite?abbreviate=false", Url, RQMRESTfulUrl, projectAlias);

            do
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                string result = string.Empty;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                XElement root = XElement.Parse(result);

                foreach (var xml in root.Elements(RQMServer.XN + "entry"))
                {
                    testsuites.Add(new RQMTestSuite(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "testsuite")));
                }

                var nextLink = (root.Elements(RQMServer.XN + "link").Where(l => l.Attribute("rel").Value == "next")).SingleOrDefault();

                requestUrl = nextLink == null ? string.Empty : nextLink.Attribute("href").Value;

            } while (!string.IsNullOrWhiteSpace(requestUrl));

            return testsuites;
        }

        /// <summary>
        /// get all the test cases under specific project
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <returns></returns>
        public IList<RQMTestCase> GetTestCasesByProject(string projectAlias)
        {
            IList<RQMTestCase> testCases = new List<RQMTestCase>();

            string requestUrl = string.Format("{0}/{1}/resources/{2}/testcase?abbreviate=false", Url, RQMRESTfulUrl, projectAlias);

            do
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                string result = string.Empty;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                XElement root = XElement.Parse(result);

                foreach (var xml in root.Elements(RQMServer.XN + "entry"))
                {
                    testCases.Add(new RQMTestCase(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "testcase")));
                }

                var nextLink = (root.Elements(RQMServer.XN + "link").Where(l => l.Attribute("rel").Value == "next")).SingleOrDefault();

                requestUrl = nextLink == null ? string.Empty : nextLink.Attribute("href").Value;

            } while (!string.IsNullOrWhiteSpace(requestUrl));

            return testCases;
        }

        /// <summary>
        /// Get all execution work item of specific project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMExecutionWorkItem> GetExecutionWorkItemByProject(RQMProject project)
        {
            string requestUrl = string.Format("{0}/{1}/resources/{2}/executionworkitem?abbreviate=false", Url, RQMRESTfulUrl, project.Alias);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            string result = string.Empty;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            XElement root = XElement.Parse(result);

            IList<RQMExecutionWorkItem> testCases = new List<RQMExecutionWorkItem>();

            foreach (var xml in root.Elements(RQMServer.XN + "entry"))
            {
                testCases.Add(new RQMExecutionWorkItem(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "executionworkitem")));
            }

            return testCases;
        }

        /// <summary>
        /// Get all the categories of project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMCategory> GetCategoriesByProject(RQMProject project)
        {
            IList<RQMCategory> categories = new List<RQMCategory>();

            string requestUrl = string.Format("{0}/{1}/resources/{2}/category?abbreviate=false", Url, RQMRESTfulUrl, project.Alias);

            do
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                string result = string.Empty;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                XElement root = XElement.Parse(result);

                foreach (var xml in root.Elements(RQMServer.XN + "entry"))
                {
                    categories.Add(new RQMCategory(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "category")));
                }

                var nextLink = (root.Elements(RQMServer.XN + "link").Where(l => l.Attribute("rel").Value == "next")).SingleOrDefault();

                requestUrl = nextLink == null ? string.Empty : nextLink.Attribute("href").Value;

            } while (!string.IsNullOrWhiteSpace(requestUrl));

            return categories;
        }

        /// <summary>
        /// Get the detail of one category
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public RQMCategory GetCategoryByLink(string link)
        {
            HttpWebRequest request = WebRequest.Create(link) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            string result = string.Empty;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            XElement root = XElement.Parse(result);
            return new RQMCategory(root);
        }

        /// <summary>
        /// Get all the category types of the project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMCategoryType> GetCategoryTypesByProject(RQMProject project)
        {

            IList<RQMCategoryType> categoryTypes = new List<RQMCategoryType>();

            string requestUrl = string.Format("{0}/{1}/resources/{2}/categoryType?abbreviate=false", Url, RQMRESTfulUrl, project.Alias);

            do
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                string result = string.Empty;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                XElement root = XElement.Parse(result);

                foreach (var xml in root.Elements(RQMServer.XN + "entry"))
                {
                    categoryTypes.Add(new RQMCategoryType(xml.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + "categoryType")));
                }

                var nextLink = (root.Elements(RQMServer.XN + "link").Where(l => l.Attribute("rel").Value == "next")).SingleOrDefault();

                requestUrl = nextLink == null ? string.Empty : nextLink.Attribute("href").Value;

            } while (!string.IsNullOrWhiteSpace(requestUrl));

            return categoryTypes;
        }

        /// <summary>
        /// Get the detail of one category type
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public RQMCategoryType getCategoryTypeByLink(string link)
        {
            HttpWebRequest request = WebRequest.Create(link) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            string result = string.Empty;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            XElement root = XElement.Parse(result);
            return new RQMCategoryType(root);
        }

        /// <summary>
        /// Get the features' hierarchy, here we have an assumption that there're only two level for feature in RQM
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMFeature> GetFeaturesOfProject(RQMProject project)
        {
            List<RQMFeature> features = new List<RQMFeature>();
            IList<RQMCategoryType> categoriyTypes = GetCategoryTypesByProject(project);
            RQMCategoryType featuresType = categoriyTypes.Where(ct => ct.Title == "Features" && ct.Scope == "TestCase").FirstOrDefault();
            if (null != featuresType)
            {
                foreach (KeyValuePair<string, List<string>> keyValuePair in featuresType.ValueSets)
                {
                    RQMFeature feature = new RQMFeature();
                    RQMCategory featureCategory = GetCategoryByLink(keyValuePair.Key);
                    feature.Id = featureCategory.Id;
                    feature.Title = featureCategory.Title;
                    feature.SubFeatures = new List<RQMFeature>();
                    foreach (string link in keyValuePair.Value)
                    {
                        RQMCategory subFeatureCategory = GetCategoryByLink(link);
                        RQMFeature subFeature = new RQMFeature();
                        subFeature.Id = subFeatureCategory.Id;
                        subFeature.Title = subFeatureCategory.Title;
                        feature.SubFeatures.Add(subFeature);
                    }
                    features.Add(feature);
                }
            }
            return features;
        }

        /// <summary>
        /// Get all the modules of one project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMModule> GetModulesOfProject(RQMProject project)
        {
            List<RQMModule> modules = new List<RQMModule>();

            IList<RQMCategory> categories = GetCategoriesByProject(project);
            IList<RQMCategoryType> categoryTypes = GetCategoryTypesByProject(project);

            RQMCategoryType modulesType = categoryTypes.Where(t => t.Title == "Module" && t.Scope == "TestCase").FirstOrDefault();

            if (null != modulesType)
            {
                foreach (RQMCategory category in categories)
                {
                    if (category.CategoryType == modulesType.Id)
                    {
                        RQMModule module = new RQMModule();
                        module.Id = category.Id;
                        module.Title = category.Title;
                        modules.Add(module);
                    }
                }
            }
            return modules;
        }

        /// <summary>
        /// Get the rankings of one project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMRanking> GetRankingsOfProject(RQMProject project)
        {
            List<RQMRanking> rankings = new List<RQMRanking>();

            IList<RQMCategory> categories = GetCategoriesByProject(project);
            IList<RQMCategoryType> categoryTypes = GetCategoryTypesByProject(project);

            RQMCategoryType rankingType = categoryTypes.Where(t => t.Title == "Ranking" && t.Scope == "TestCase").FirstOrDefault();

            if (null != rankingType)
            {
                foreach (RQMCategory category in categories)
                {
                    if (category.CategoryType == rankingType.Id)
                    {
                        RQMRanking ranking = new RQMRanking();
                        ranking.Id = category.Id;
                        ranking.Title = category.Title;
                        rankings.Add(ranking);
                    }
                }
            }

            return rankings;
        }

        /// <summary>
        /// Get the releases of one project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IList<RQMRelease> GetReleasesOfProject(RQMProject project)
        {
            List<RQMRelease> releases = new List<RQMRelease>();

            IList<RQMCategory> categories = GetCategoriesByProject(project);
            IList<RQMCategoryType> categoryTypes = GetCategoryTypesByProject(project);

            RQMCategoryType rankingType = categoryTypes.Where(t => t.Title == "Release" && t.Scope == "TestCase").FirstOrDefault();

            if (null != rankingType)
            {
                foreach (RQMCategory category in categories)
                {
                    if (category.CategoryType == rankingType.Id)
                    {
                        RQMRelease release = new RQMRelease();
                        release.Id = category.Id;
                        release.Title = category.Title;
                        releases.Add(release);
                    }
                }
            }

            return releases;
        }

        /// <summary>
        /// Create the execution result in RQM
        /// </summary>
        /// <param name="projectAlias">The project alias</param>
        /// <param name="executionWorkItemWebId">The web id of the execution record</param>
        /// <param name="title">result title</param>
        /// <param name="result">result, current supports: passed, failed, error, incomplete, blocked</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>the web id of the created test result</returns>
        public string CreateTestResult(string projectAlias, string executionWorkItemWebId, string title, string result, string buildGeneratedId, System.DateTime startTime, System.DateTime endTime, string details = "")
        {
            string[] permittedResults = new[] { "passed", "failed", "error", "incomplete", "blocked" };
            if (!permittedResults.Contains(result.ToLower()))
            {
                throw new System.Exception(string.Format("The result:{0} is not supportted in RQM.", result));
            }
            Resources.executionresult executionResult = new Resources.executionresult();
            executionResult.title = title;
            executionResult.state = new Resources.state();
            //com.ibm.rqm.execution.common.state.failed
            //com.ibm.rqm.execution.common.state.error
            //com.ibm.rqm.execution.common.state.blocked
            //com.ibm.rqm.execution.common.state.incomplete
            executionResult.state.Text = new string[] { string.Format("com.ibm.rqm.execution.common.state.{0}", result.ToLower()) };
            executionResult.starttime = startTime;
            executionResult.endtime = endTime;
            executionResult.description = details;

            executionResult.executionworkitem = new Resources.executionresultExecutionworkitem();
            executionResult.executionworkitem.href = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias, new Resources.executionworkitem(), executionWorkItemWebId);

            executionResult.buildrecord = new Resources.executionresultBuildrecord();
            executionResult.buildrecord.href = GetResourceURLForGettingAndPuttingUsingGeneratedId(projectAlias, new Resources.buildrecord(), buildGeneratedId);
            
            return CreateResourceByPost(projectAlias, executionResult);
        }

        /// <summary>
        /// Create test plan in RQM
        /// </summary>
        /// <param name="projectAlias">project to create test plan on</param>
        /// <param name="title">test plan title</param>
        /// <param name="description">test plan description</param>
        /// <param name="testCaseWebIds">the list of test cases' web ids to be included in the test plan</param>
        /// <returns>the web id of the created test plan</returns>
        public string CreateTestPlan(string projectAlias, string title, string description, string[] testCaseWebIds, string[] testEnvironmentGeneratedIds)
        {
            Resources.testplan testPlan = new Resources.testplan();
            testPlan.title = title;
            testPlan.description = description;

            testPlan.template = new Resources.testplanTemplate();
            testPlan.template.href = string.Format("{0}/{1}/resources/{2}/testplan/{3}", Url, RQMRESTfulUrl, projectAlias, "com.ibm.rqm.planning.templates.testplan.template_1395123161408");

            //test cases
            int count = testCaseWebIds.Count();
            testPlan.testcase = new Resources.testplanTestcase[count];
            for (int i = 0; i < count; i++)
            {
                string webId = testCaseWebIds[i];
                testPlan.testcase[i] = new Resources.testplanTestcase();
                testPlan.testcase[i].href = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias, new Resources.testcase(), webId);
            }

            //test environments
            testPlan.configuration = new Resources.testplanConfiguration[testEnvironmentGeneratedIds.Count()];
            for (int i = 0; i < testEnvironmentGeneratedIds.Count(); i++)
            {
                testPlan.configuration[i] = new Resources.testplanConfiguration();
                testPlan.configuration[i].href = GetResourceURLForGettingAndPuttingUsingGeneratedId(projectAlias, new Resources.configuration(), testEnvironmentGeneratedIds[i]);
            }

            return CreateResourceByPost(projectAlias, testPlan);
        }

        /// <summary>
        /// Get or Create test case execution record
        /// 1. If execution record with the same case id, plan id and configuration id, return its web id
        /// 2. else, create a new execution record and return the web id
        /// </summary>
        /// <param name="projectAlias">the project the execution record will be created on</param>
        /// <param name="title">record title</param>
        /// <param name="description">record description</param>
        /// <param name="testCaseWebId">test case web id related</param>
        /// <param name="testPlanWebId">test plan web id related</param>
        /// <returns>web id of the created execution record</returns>
        public string GetOrCreateTestCaseExecutionRecord(string projectAlias, string title, string description, string testCaseWebId, string testPlanWebId, string testEnvironmentGeneratedId)
        {
            List<Resources.executionworkitem> existingExecutionRecords = GetExistingExecutionRecordsInRQMByTestCaseTestPlanAndEnvironment(projectAlias, testCaseWebId, testPlanWebId, testEnvironmentGeneratedId);
            if (existingExecutionRecords != null && existingExecutionRecords.Count > 0)
            {
                return existingExecutionRecords.FirstOrDefault().webId.ToString();
            }
            else
            {
                Resources.executionworkitem executionRecord = new Resources.executionworkitem();
                executionRecord.title = title;
                executionRecord.testcase = new Resources.executionworkitemTestcase();
                executionRecord.testcase.href = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias, new Resources.testcase(), testCaseWebId);
                executionRecord.testplan = new Resources.executionworkitemTestplan();
                executionRecord.testplan.href = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias, new Resources.testplan(), testPlanWebId);
                executionRecord.description = description;
                executionRecord.weight = 100;
                executionRecord.configuration = new Resources.executionworkitemConfiguration[1];
                executionRecord.configuration[0] = new Resources.executionworkitemConfiguration();
                executionRecord.configuration[0].href = GetResourceURLForGettingAndPuttingUsingGeneratedId(projectAlias, new Resources.configuration(), testEnvironmentGeneratedId);

                return CreateResourceByPost(projectAlias, executionRecord);
            }
        }

        /// <summary>
        /// Check whether there's existing build record with the same title, if not, created one
        /// </summary>
        /// <param name="projectAlias">proejct alias</param>
        /// <param name="title">the title of the build</param>
        /// <param name="description">the description of build</param>
        /// <returns>the generated id of build record</returns>
        public string GetOrCreateBuildRecord(string projectAlias, string title, string description)
        {
            Resources.buildrecord buildRecord = new Resources.buildrecord();
            buildRecord.title = title;
            buildRecord.description = description;
            buildRecord.state = new Resources.state();
            buildRecord.state.Text = new string[] { "com.ibm.rqm.buildintegration.buildstate.complete" };
            buildRecord.status = "com.ibm.rqm.buildintegration.buildstatus.ok";
            buildRecord.providerTypeId = "com.ibm.rqm.buildintegration.common.manualProvider";
            buildRecord.endtime = System.DateTime.UtcNow;

            return GetOrCreateResourceByTitle(projectAlias, buildRecord);

        }

        /// <summary>
        /// Check whether is there an test environment in RQM, if not, create one
        /// </summary>
        /// <param name="projectAlias">project alias</param>
        /// <param name="title">title of environment</param>
        /// <param name="description">description of environment</param>
        /// <param name="xmlConfig">the xml config of the environment</param>
        /// <returns>the generated id of the environment</returns>
        public string GetOrCreateTestEnvironment(string projectAlias, string title, string description, string xmlConfig)
        {
            Resources.configuration config = new Resources.configuration();
            config.name = title;
            config.title = title;
            config.summary = description;
            config.description = ConvertXElementToXMLNode(XElement.Parse(xmlConfig));       

            return GetOrCreateResourceByTitle(projectAlias, config);
        }

        /// <summary>
        /// Get the test plans list by project
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <returns>list of test plans in RQMTestSuite format</returns>
        public IEnumerable<RQMTestSuite> GetTestPlansByProject(string projectAlias)
        {
            List<RQMTestSuite> testPlanSuites = new List<RQMTestSuite>();
            List<object> testPlans = GetResourcesByURL(GetResourcesURLFoGetting(projectAlias, new Resources.testplan()), new Resources.testplan());
            foreach (object o in testPlans)
            {
                Resources.testplan testPlan = o as Resources.testplan;

                RQMTestSuite suite = new RQMTestSuite();

                suite.SourceId = testPlan.webId.ToString();
                suite.Title = testPlan.title;
                suite.Weight = 100;
                suite.HaltOnFailure = false;
                suite.SequentialExecution = true;
                suite.Categories = new Dictionary<string, string>();
                suite.Categories.Add(new KeyValuePair<string, string>("Description", testPlan.description));

                suite.SubTestCaseSourceIds = new List<string>();
                if (testPlan.testcase != null)
                {
                    foreach (Resources.testplanTestcase testCase in testPlan.testcase)
                    {
                        string subCaseWebId = testCase.href.Substring(testCase.href.LastIndexOf(':') + 1);
                        suite.SubTestCaseSourceIds.Add(subCaseWebId);
                    }
                }

                suite.SubTestSuiteSourceIds = new List<string>();
                if (testPlan.testsuite != null)
                {
                    foreach (Resources.testplanTestsuite testSuite in testPlan.testsuite)
                    {
                        string subSuiteWebId = testSuite.href.Substring(testSuite.href.LastIndexOf(':') + 1);
                        suite.SubTestSuiteSourceIds.Add(subSuiteWebId);
                    }
                }

                testPlanSuites.Add(suite);

            }

            return testPlanSuites;
        }


        #region private methodes        

        /// <summary>
        /// Check whether is there one resource in RQM by title, if not, create one
        /// </summary>
        /// <param name="projectAlias">project's alias</param>
        /// <param name="o">the resource to create</param>
        /// <returns>the generated id of resource created</returns>
        private string GetOrCreateResourceByTitle(string projectAlias, object o)
        {
            List<object> resources = GetExistingResourcesInRQMByTitle(projectAlias, o);
            if (resources.Count > 0)
            {
                string id = o.GetType().GetProperty("identifier").GetValue(resources.First(), null).ToString();
                return id.Substring(id.LastIndexOf("slug_"));
            }
            else
            {
                return CreateResourceByPost(projectAlias, o);
            }
        }

        /// <summary>
        /// Convert the XElement into XmlNode
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private System.Xml.XmlNode ConvertXElementToXMLNode(XElement element)
        {
            using (System.Xml.XmlReader xmlReader = element.CreateReader())
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        /// <summary>
        /// check whether the resource exists with the title
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="o">the object represent the resource</param>
        /// <param name="title">the title to search</param>
        /// <returns>exist or not</returns>
        private List<object> GetExistingResourcesInRQMByTitle(string projectAlias, object o)
        {
            string requestUrl = GetResourcesURLForPosting(projectAlias, o);
            string title = o.GetType().GetProperty("title").GetValue(o, null).ToString();
            requestUrl = string.Format("{0}?fields=feed/entry/content/{1}[title='{2}']/*", requestUrl,o.GetType().Name, title);
            List<object> resources = GetResourcesByURL(requestUrl, o);
            return resources;
        }

        /// <summary>
        /// Get the execution record based on the combination of test case, test plan, test environment(configuration)
        /// Note that, typically, the combination of case/plan/config/timeline dertimine one execution record, here we ignored the timeline.
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="caseWebId">test case WebId</param>
        /// <param name="planWebId">test plan WebId</param>
        /// <param name="environmentGeneratedId">environment Generated Id</param>
        /// <returns>list of execution records</returns>
        private List<Resources.executionworkitem> GetExistingExecutionRecordsInRQMByTestCaseTestPlanAndEnvironment(string projectAlias, string caseWebId, string planWebId, string environmentGeneratedId)
        {
            Resources.testcase testCase = new Resources.testcase();
            string testCaseURL = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias,testCase, caseWebId);

            Resources.testplan testPlan = new Resources.testplan();
            string testPlanURL = GetResourceURLForGettingAndPuttingUsingWebId(projectAlias, testPlan, planWebId);

            Resources.configuration config = new Resources.configuration();
            string configURL = GetResourceURLForGettingAndPuttingUsingGeneratedId(projectAlias, config, environmentGeneratedId);

            Resources.executionworkitem item = new Resources.executionworkitem();
            string requestURL = GetResourcesURLForPosting(projectAlias, item);
            requestURL = string.Format("{0}?fields=feed/entry/content/{1}/(*|testcase[@href='{2}']|testplan[@href='{3}']|configuration[@href='{4}'])",requestURL,item.GetType().Name, testCaseURL, testPlanURL, configURL );
            
            List<object> executionRecords = GetResourcesByURL(requestURL, item);
            if (executionRecords != null)
            {
                return executionRecords.Select(e => e as Resources.executionworkitem).ToList();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Get the resources list from RQM
        /// </summary>
        /// <param name="requestUrl">the url of the resource, can contains the filtering</param>
        /// <param name="o">the object represent the resource</param>
        /// <returns>the resources list</returns>
        private List<object> GetResourcesByURL(string requestUrl, object o)
        {
            List<object> resources = new List<object>();

            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    XmlSerializer serializer = new XmlSerializer(o.GetType());
                    string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        XElement root = XElement.Parse(result);
                        foreach (XElement element in root.Elements(RQMServer.XN + "entry"))
                        {
                            object resource = serializer.Deserialize(new StringReader(element.Element(RQMServer.XN + "content").Element(RQMServer.XN2 + o.GetType().Name).ToString()));
                            resources.Add(resource);
                        }
                    }
                }
            }
            catch (System.Exception)
            {

            }
            return resources;
        }

        /// <summary>
        /// Create RQM resource
        /// </summary>
        /// <param name="projectAlias">project the resource will be created on</param>
        /// <param name="o">the object represent the resource</param>
        /// <returns>the web id of the created resource</returns>
        private string CreateResourceByPost(string projectAlias, object o)
        {
            string requestUrl = GetResourcesURLForPosting(projectAlias, o);

            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.ContentType = "application/xml";
            request.CookieContainer = cookieContainer;

            XmlSerializer serializer = new XmlSerializer(o.GetType());
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, o);
            byte[] body = Encoding.UTF8.GetBytes(writer.ToString());
            Stream stream = request.GetRequestStream();
            stream.Write(body, 0, body.Length);
            stream.Close();

            string generatedId = string.Empty;
            string webId = string.Empty;
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    generatedId = response.Headers.GetValues("Content-Location").FirstOrDefault();
                }
            }
            catch (System.Exception)
            {
                return webId;
            }

            string resourceURL = GetResourceURLForGettingAndPuttingUsingGeneratedId(projectAlias, o, generatedId);

            HttpWebRequest request2 = WebRequest.Create(resourceURL) as HttpWebRequest;
            request2.Method = "GET";
            request2.CookieContainer = cookieContainer;

            using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
            {
                XmlSerializer serializer2 = new XmlSerializer(o.GetType());
                object o2 = serializer2.Deserialize(response2.GetResponseStream());
                try
                {
                    webId = o.GetType().GetProperty("webId").GetValue(o2, null).ToString();
                }
                catch (System.Exception)//for configuratin and build record, there's no WebId property, we'll try to get the Id through the identifier
                {
                    try
                    {  //https://jazzapps.otg.com:9443/qm/service/com.ibm.rqm.integration.service.IIntegrationService/resources/SourceOne+%28Quality+Management%29/configuration/TE19
                        string identifier = o.GetType().GetProperty("identifier").GetValue(o2, null).ToString();
                        return identifier.Substring(identifier.LastIndexOf('/') + 1);
                    }
                    catch (System.Exception)//if failed to get, return the generated Id return by posting
                    {
                        return generatedId;
                    }
                }
            }

            return webId;
        }

        /// <summary>
        /// Get the URL of the resources, it's for creating a new resource by posting
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="o">the object represents the resource to be created</param>
        /// <returns>the url of the sourses</returns>
        private string GetResourcesURLForPosting(string projectAlias, object o)
        {
            return string.Format("{0}/{1}/resources/{2}/{3}", Url, RQMRESTfulUrl, projectAlias, o.GetType().Name);
        }

        /// <summary>
        /// Get the URL of the resources, it's for getting the resources list by getting
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="o">the object represents the resource</param>
        /// <returns>the url of the sourses</returns>
        private string GetResourcesURLFoGetting(string projectAlias, object o)
        {
            return string.Format("{0}/{1}/resources/{2}/{3}?abbreviate=false", Url, RQMRESTfulUrl, projectAlias, o.GetType().Name);
        }

        /// <summary>
        /// Get the URL of some kind of resource, using web id
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="o">the object represents the resource to be created</param>
        /// <param name="webId">the web id (internal id) of resource</param>
        /// <returns>the URL of this resource</returns>
        private string GetResourceURLForGettingAndPuttingUsingWebId(string projectAlias, object o, string webId)
        {
            return string.Format("{0}/{1}/resources/{2}/{3}/urn:com.ibm.rqm:{4}:{5}", Url, RQMRESTfulUrl, projectAlias, o.GetType().Name, o.GetType().Name, webId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectAlias"></param>
        /// <param name="o">the object represents the resource to be created</param>
        /// <param name="generatedId">the generated id of created resource by posting</param>
        /// <returns>the URL of the resource</returns>
        private string GetResourceURLForGettingAndPuttingUsingGeneratedId(string projectAlias, object o, string generatedId)
        {
            return string.Format("{0}/{1}/resources/{2}/{3}/{4}", Url, RQMRESTfulUrl, projectAlias, o.GetType().Name, generatedId);
        }

        #endregion
        
    }
}
