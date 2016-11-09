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
    public class CSharpNUnitResultParser: ResultParser
    {
        public override void ParseResult(string resultFilePath, int jobId)
        {
            if (FileHelper.IsExistsFile(resultFilePath))
            {
                XmlDocument document = new XmlDocument();
                document.Load(resultFilePath);
                XmlNodeList testCaseResultList = document.SelectNodes("//results/test-case");
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
                        throw new Exception(message);
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
                throw new Exception(message);
            }
        }

        protected override Core.Model.ResultType GetResult(System.Xml.XmlNode resultNode)
        {
            string resultString = resultNode.Attributes["result"].Value;
            ResultType result = ResultType.NotRun;
            switch (resultString.ToLower())
            {
                case "failure":
                    result = ResultType.Failed;
                    break;
                case "success":
                    result = ResultType.Pass;
                    break;
                default:
                    result = ResultType.NotRun;
                    break;
            }
            return result;
        }

        protected override string GetSourceId(System.Xml.XmlNode resultNode)
        {
            foreach (XmlNode node in resultNode.ChildNodes)
            {
                if (node.Name.ToLower() == "categories")
                {
                    string sourceId = string.Empty;
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        string category = n.Attributes["name"].Value;
                        if (category.ToLower().StartsWith("webid"))
                        {
                            sourceId = category.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).ToList()[1];
                            break;
                        }
                    }
                    return sourceId;
                }
            }
            return string.Empty;
        }

        protected override string GetFailureInfo(System.Xml.XmlNode resultNode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (XmlNode node in resultNode.ChildNodes)
            {
                if (node.Name.ToLower() == "failure")
                {
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if (n.Name.ToLower() == "message")
                        {
                            builder.AppendLine(n.InnerText);
                        }
                        else if (n.Name.ToLower() == "stack-trace")
                        {
                            builder.AppendLine(n.InnerText);
                        }

                    }
                }
            }
            return builder.ToString();
        }
    }
}
