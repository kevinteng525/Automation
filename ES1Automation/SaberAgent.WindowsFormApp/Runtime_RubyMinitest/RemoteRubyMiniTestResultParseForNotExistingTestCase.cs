using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.FileCommon;
using System.Xml;
using Core.Model;
using Core;
using System.IO;
using System.Configuration;
using Common.Network;

namespace SaberAgent.WindowsFormApp.Runtime_RubyMinitest
{
    public class RemoteRubyMiniTestResultParseForNotExistingTestCase : ResultParser
    {
        public override void ParseResult(string resultFilePath, int jobId)
        {
            string[] files = System.IO.Directory.GetFiles(resultFilePath);
            try
            {
                foreach (string resultPath in files)
                {
                    string fi = Path.GetFileNameWithoutExtension(resultPath);
                    XmlDocument document = new XmlDocument();
                    document.Load(resultPath);
                    XmlNodeList testCaseResultList = document.SelectNodes("//testsuite/testcase");
                    foreach (XmlNode testCaseResult in testCaseResultList)
                    {
                        try
                        {
                            string caseName = GetTestCase(testCaseResult);  //get name from xml 
                            string failureInfo = GetFailureInfo(testCaseResult);

                            Product product = AutomationJob.GetProductOfJobByJobId(jobId);
                            TestCase testcase = TestCase.GetTestCaseByName(caseName);
                            if (testcase == null)
                            {
                                TestCase tc = new TestCase()
                                {
                                    ProviderId = 2,//ATFTestCaseProvider
                                    Name = caseName,
                                    ProductId = product.ProductId,
                                    SourceId = "-1",  // set a default value to sign as a not existing case
                                    Feature = "Galaxy automation",
                                    Weight = 100,
                                    IsAutomated = true,
                                    IsActive = false,
                                };
                                testcase = TestCase.CreateCase(tc);
                            }

                            TestCaseExecution ex = new TestCaseExecution()
                            {
                                TestCaseId = testcase.TestCaseId,
                                JobId = jobId,
                                Status = (int)ExecutionStatus.NotRunning,
                                StartTime = null,
                                EndTime = null,
                                Info = fi,
                                RetryTimes = 0,
                                Timeout = ATFEnvironment.DefaultTestCaseTimeout,
                            };
                            TestCaseExecution exInDB = TestCaseExecution.CreateExecution(ex);
                            SaberAgent.Log.logger.Info(string.Format("Start to parse the result for execution {0} of job {1}", exInDB.ExecutionId, jobId));

                            ResultType result = GetResult(testCaseResult);
                            TestResult re = new TestResult
                            {
                                ExecutionId = exInDB.ExecutionId,
                                Result = (int)result,
                                IsTriaged = false,
                                TriagedBy = null,
                                Description = string.IsNullOrEmpty(failureInfo) ? "" : failureInfo,
                                Files = resultPath,
                            };

                            TestResult resultInDB = TestResult.CreateRunResult(re);

                            SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to Compleate", exInDB.ExecutionId, jobId));
                            exInDB.SetStatus(ExecutionStatus.Complete);
                        }
                        catch (Exception e)
                        {
                            SaberAgent.Log.logger.Error(e);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
            }
            try
            {
                CopyResultFilesToServer(resultFilePath, jobId);
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
            }
        }

        protected override Core.Model.ResultType GetResult(System.Xml.XmlNode resultNode)
        {
            foreach (XmlNode node in resultNode.ChildNodes)
            {
                if (node.Name.ToLower() == "failure" || node.Name.ToLower() == "error")
                {
                    return ResultType.Failed;
                }
            }
            return ResultType.Pass;
        }

        protected string GetTestCase(System.Xml.XmlNode resultNode)
        {
            string name = String.Empty;
            if (resultNode.Attributes["name"] != null)
            {
                name = resultNode.Attributes["name"].Value;
            }
            return name;
        }

        protected override string GetFailureInfo(System.Xml.XmlNode resultNode)
        {
            if (resultNode.ChildNodes.Count > 0)
            {
                XmlNode failure = resultNode.ChildNodes[0];
                return failure.InnerText;
            }
            else
            {
                return string.Empty;
            }
        }

        protected override string GetSourceId(System.Xml.XmlNode resultNode)
        {
            if (resultNode.Attributes["name"] != null)
            {
                string name = resultNode.Attributes["name"].Value;
                if (name.ToLower().Contains("_webid_"))
                {
                    return name.Split(new[] { "_webid_" }, StringSplitOptions.RemoveEmptyEntries).Last();
                }
            }
            return string.Empty;
        }
    }
}
