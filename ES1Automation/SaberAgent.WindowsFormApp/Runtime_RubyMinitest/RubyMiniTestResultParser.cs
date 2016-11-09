using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Common.FileCommon;
using Common.ScriptCommon;

using Core.Model;


namespace SaberAgent.WindowsFormApp
{
    public class RubyMiniTestResultParser: ResultParser
    {
        public override void ParseResult(string resultFilePath, int jobId)
        {
            if (FileHelper.IsExistsFile(resultFilePath))
            {
                XmlDocument document = new XmlDocument();
                document.Load(resultFilePath);
                XmlNodeList testCaseResultList = document.SelectNodes("//testsuite/testcase");
                foreach (XmlNode testCaseResult in testCaseResultList)
                {
                    ResultType result = GetResult(testCaseResult);
                    string sourceId = GetSourceId(testCaseResult);
                    string failureInfo = GetFailureInfo(testCaseResult);
                    string resultFileRemotePath = CopyResultFilesToServer(System.IO.Path.GetDirectoryName(resultFilePath), jobId);
                    if (string.IsNullOrEmpty(sourceId))
                    {
                        string message = string.Format("Could not get the sourceId in the result file for job {0} in the result file {1}", jobId, resultFilePath);
                        SaberAgent.Log.logger.Error(message);
                    }
                    TestResult resultInDB = TestResult.GetTestResultByJobAndSourceID(jobId, sourceId);
                    if (resultInDB == null)
                    {
                        string message = string.Format("Could not get the TestResult Record in DB for job {0} with the sourceId {1}", jobId, sourceId);
                        SaberAgent.Log.logger.Error(message);
                        throw new Exception(message);
                    }
                    resultInDB.ResultType = result;
                    resultInDB.Description = string.IsNullOrEmpty(failureInfo) ? "" : failureInfo;
                    resultInDB.Files = resultFileRemotePath;
                    resultInDB.Update();
                }
            }
            else
            {
                string message = string.Format("The result file {0} can not be found.", resultFilePath);
                SaberAgent.Log.logger.Error(message);
                AutomationJob job = AutomationJob.GetAutomationJob(jobId);
                job.AddJobProgressInformation(message);
                job.SetJobsStatus(JobStatus.Failed);
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
    }
}
