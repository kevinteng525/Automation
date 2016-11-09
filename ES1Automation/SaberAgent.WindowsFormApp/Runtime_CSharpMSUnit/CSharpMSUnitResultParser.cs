using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Core.Model;
using Common.FileCommon;


namespace SaberAgent.WindowsFormApp
{
    public class CSharpMSUnitResultParser : ResultParser
    {
        public override void ParseResult(string resultFilePath, int jobId)
        {
            if (FileHelper.IsExistsFile(resultFilePath))
            {
                XmlDocument document = new XmlDocument();
                document.Load(resultFilePath);
                XmlNamespaceManager nsm = new XmlNamespaceManager(document.NameTable);
                nsm.AddNamespace("ns", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
                XmlNodeList testCaseResultList = document.SelectNodes("//ns:Results/ns:UnitTestResult", nsm);
                foreach (XmlNode testCaseResult in testCaseResultList)
                {
                    ResultType result = GetResult(testCaseResult);
                    string sourceId = GetSourceIdByFileName(resultFilePath);
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

        private string GetSourceIdByFileName(string resultFilePath)
        {
            string sourceId = string.Empty;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(resultFilePath);

            sourceId = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(fileInfo.Extension));

            return sourceId;
        }

        protected override Core.Model.ResultType GetResult(System.Xml.XmlNode resultNode)
        {
            string result = resultNode.Attributes["outcome"].Value;
            
            if (string.Compare(result, "Passed", true) == 0)
            {
                return ResultType.Pass;
            }
            else
            {
                return ResultType.Failed;
            }
        }

        protected override string GetSourceId(System.Xml.XmlNode resultNode)
        {
            throw new NotImplementedException();
        }

        protected override string GetFailureInfo(System.Xml.XmlNode resultNode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (XmlNode node in resultNode.ChildNodes)
            {
                if (node.Name.ToLower() == "output")
                {
                    foreach (XmlNode no in node.ChildNodes)
                    {
                        if (no.Name.ToLower() == "errorinfo")
                        {
                            foreach (XmlNode n in no.ChildNodes)
                            {
                                if (n.Name.ToLower() == "message")
                                {
                                    builder.AppendLine(n.InnerText);
                                }
                                else if (n.Name.ToLower() == "stacktrace")
                                {
                                    builder.AppendLine(n.InnerText);
                                }
                            }
                        }
                    }
                }
            }
            return builder.ToString();
        }

    }
}
