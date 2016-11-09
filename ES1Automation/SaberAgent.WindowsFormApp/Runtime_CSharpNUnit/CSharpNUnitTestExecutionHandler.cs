using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.FileCommon;
using Common.ScriptCommon;
using Core.Model;

namespace SaberAgent.WindowsFormApp
{
    public class CSharpNUnitTestExecutionHandler : SaberTestExecutionHandler
    {
        private ResultParser resultParser = new CSharpNUnitResultParser();
        private string vcsRootFolder = string.Empty;

        public CSharpNUnitTestExecutionHandler(AutomationJob job2Run, string rootResultPath, Project project)
        {
            this.RootResultPath = rootResultPath;
            this.Job2Run = job2Run;
            this.vcsRootFolder = project.VCSRootPath;
        }

        public override void RunTestCase(TestCaseExecution execution)
        {
            FileHelper.EmptyFolder(this.RootResultPath);
            string resultFileFullPath = this.RootResultPath + @"\" + execution.TestCase.SourceId.ToString() + @".xml";

            SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to running", execution.ExecutionId, Job2Run.JobId));
            execution.SetStatus(ExecutionStatus.Running);

            //get the test case source ids to used by the NUnit console
            string testCaseIdList = "WebId=" + execution.TestCase.SourceId;

            string resultLogFilePath = string.Format(@"{0}\{1}_RunCSharpNUnitTest.bat.log", this.RootResultPath, execution.TestCase.SourceId);
            string parameters = string.Format(@"""{0}"" {1}.xml ""{2}"" {3}> ""{4}""",
                testCaseIdList, execution.TestCase.SourceId, this.RootResultPath, this.vcsRootFolder + @"/CSharpNUnit/Saber", resultLogFilePath);
            string runBATFile = @"C:\SaberAgent\RunCSharpNUnitTest.bat";

            string command = string.Format(@"{0} /C {1}", runBATFile, parameters);
            SaberAgent.Log.logger.Info(command);
            string result = CMDScript.RumCmd(runBATFile, parameters);
            SaberAgent.Log.logger.Info(result);

            SaberAgent.Log.logger.Info(string.Format("Execution of test case [{0}] has been finished for job {1}", execution.TestCase.Name, Job2Run.JobId));
            //Parse the test result
            SaberAgent.Log.logger.Info(string.Format("Start to parse the result for test Case [{0}]", execution.TestCase.Name));
            FileHelper.CopyFile(@"C:\SaberAgent\Config\Environment.xml", this.RootResultPath, true);
            try
            {
                resultParser.ParseResult(resultFileFullPath, Job2Run.JobId);
                SaberAgent.Log.logger.Info(string.Format("Result parsing has been completed for test Case [{0}]", execution.TestCase.Name));
            }
            catch (Exception ex)
            {
                execution.SetStatus(ExecutionStatus.Fail);
                SaberAgent.Log.logger.Error(string.Format("Met error when parse the result of test case [{0}]", execution.TestCase.Name), ex);
                return;
            }
            
            //set execution status to complete
            SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of test case {1} to Complete", execution.TestCase.Name, Job2Run.JobId));
            execution.SetStatus(ExecutionStatus.Complete);
        }
    }
}
