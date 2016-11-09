using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.FileCommon;
using Common.ScriptCommon;
using Core.Model;

namespace SaberAgent.WindowsFormApp
{
    public class CSharpMSUnitTestExecutionHandler:SaberTestExecutionHandler
    {
        private string vcsRootFolder = string.Empty;
        
        private ResultParser resultParser = new CSharpMSUnitResultParser();

        public CSharpMSUnitTestExecutionHandler(AutomationJob job2Run, string rootResultPath, Project project)
        {
            this.RootResultPath = rootResultPath;
            
            this.Job2Run = job2Run;
            
            this.vcsRootFolder = project.VCSRootPath;
        }

        public override void RunTestCase(Core.Model.TestCaseExecution execution)
        {
            FileHelper.EmptyFolder(this.RootResultPath);

            SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to running", execution.ExecutionId, Job2Run.JobId));
            
            execution.SetStatus(ExecutionStatus.Running);

            //get the test case source ids to used by the MSTest.exe console
            string testCategory = "WebId=" + execution.TestCase.SourceId;

            string resultLogFilePath = string.Format(@"{0}\{1}_RunCSharpMSUnitTest.bat.log", this.RootResultPath, execution.TestCase.SourceId);

            //below parameters are supposed to get from the project object, here just hard code it.

            string solutionFile = @"SupervisorWebAutomation.sln";

            string testContainer = @"SupervisorWebAutomation_API\bin\Debug\SupervisorWebAutomation_API.dll";

            string testSettings = @"TraceAndTestImpact.testsettings";

            string runBATFile = @"C:\SaberAgent\RunCSharpMSUnitTest.bat";

            string parameters = string.Format(@"""{0}"" {1}.trx ""{2}"" {3} {4} {5} {6}> ""{7}""",
                testCategory, execution.TestCase.SourceId, this.RootResultPath, this.vcsRootFolder + @"/CSharpNUnit/SupervisorWebAutomation", solutionFile, testContainer, testSettings, resultLogFilePath);
            

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
                resultParser.ParseResult(string.Format(@"{0}\{1}.trx", this.RootResultPath, execution.TestCase.SourceId), Job2Run.JobId);
                
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
