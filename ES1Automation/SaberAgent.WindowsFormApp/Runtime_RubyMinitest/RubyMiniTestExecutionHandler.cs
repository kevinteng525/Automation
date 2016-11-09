using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Model;
using Common.ScriptCommon;
using Common.FileCommon;

namespace SaberAgent.WindowsFormApp
{
    public class RubyMiniTestExecutionHandler : SaberTestExecutionHandler
    {
        private string scriptRootFolder = @"C:\SaberAgent\AutomationScripts";

        private string vcsRootPath = string.Empty;

        private ResultParser resultParser = new RubyMiniTestResultParser();

        public RubyMiniTestExecutionHandler(AutomationJob job2Run, string rootResultPath, Project project)
        {
            this.RootResultPath = rootResultPath;
            this.Job2Run = job2Run;
            this.vcsRootPath = project.VCSRootPath;
        }

        public override void RunTestCase(TestCaseExecution execution)
        {
            //get the test case source ids to be used by the Minitest
            string webId = execution.TestCase.SourceId;
            //update the execution status to be running
            SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to running", execution.ExecutionId, Job2Run.JobId));
            execution.SetStatus(ExecutionStatus.Running);
            string scriptRootFolderForProject = scriptRootFolder + @"\" + vcsRootPath + @"\RubyMinitest";

            string scriptFilePath = GetTestScriptFilesnFolderContainTestCaseWithWebId(scriptRootFolderForProject, webId);
            if (string.IsNullOrEmpty(scriptFilePath))
            {
                SaberAgent.Log.logger.Info(string.Format("Could not find the test script for the test execution {0} of job {1}", execution.ExecutionId, Job2Run.JobId));
                execution.SetStatus(ExecutionStatus.Fail);
            }
            else
            {
                try
                {
                    string logfilePath = string.Format(@"{0}\webid_{1}.log", this.RootResultPath, execution.TestCase.SourceId);
                    string testcasePattern = string.Format(@"/_webid_{0}/", webId);
                    string parameters = string.Format(@"{0} ""{1}"" ""{2}"" ""{3}""", testcasePattern, scriptFilePath, scriptRootFolderForProject, logfilePath);
                    string runBATFile = @"C:\SaberAgent\RunRubyMiniTest.bat";
                    string command = string.Format(@"{0} /C {1}", runBATFile, parameters);
                    SaberAgent.Log.logger.Info(command);
                    string result = CMDScript.RumCmdWithWindowsVisible(runBATFile, parameters);
                    SaberAgent.Log.logger.Info(result);
                    
                    SaberAgent.Log.logger.Info(string.Format("Execution of test case has been finished for job {0}", Job2Run.JobId));
                    string defaultMiniTestResultFile = string.Format(@"{0}\test\reports\_webid_{1}.xml", scriptRootFolderForProject, webId);
                    //copy related files to Result folder
                    FileHelper.CopyFile(@"C:\SaberAgent\Config\Environment.xml", this.RootResultPath, true);
                    if (FileHelper.IsExistsFile(defaultMiniTestResultFile))
                    {
                        FileHelper.CopyFile(defaultMiniTestResultFile, this.RootResultPath, true);
                    }
                    else
                    {
                        string message = string.Format("Could not find the result file for case [{0}] at location [{1}]", execution.TestCase.Name, defaultMiniTestResultFile);
                        SaberAgent.Log.logger.Error(message);
                        Job2Run.AddJobProgressInformation(message);
                        execution.SetStatus(ExecutionStatus.Fail);
                        return;
                    }
                    //Parse the test result
                    SaberAgent.Log.logger.Info(string.Format("Start to parse the result for execution {0} of job {1}", execution.ExecutionId, Job2Run.JobId));
                    resultParser.ParseResult(string.Format(@"{0}\_webid_{1}.xml", this.RootResultPath, webId), Job2Run.JobId);
                    SaberAgent.Log.logger.Info(string.Format("Result parsing has been completed for execution {0} of job {1}", execution.ExecutionId, Job2Run.JobId));
                    //set execution status to complete
                    SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to Compleate", execution.ExecutionId, Job2Run.JobId));
                    execution.SetStatus(ExecutionStatus.Complete);
                }
                catch (Exception ex)
                {
                    execution.SetStatus(ExecutionStatus.Fail);
                    string error = string.Format("Failed to run the execution {0}", execution.ExecutionId);
                    SaberAgent.Log.logger.Error(error, ex);
                }
            }

        }

       
        /// <summary>
        /// Check whether the rb file contains the test
        /// 1. If one line of the file ends with the _webid_{webid}, the file is considered to caontain the test
        /// 2. This should be able to handle the scenario that there are several functions in the file with the same prefix for the web id, such as _webid_12 and _webid_123
        /// </summary>
        /// <param name="scriptFolder"></param>
        /// <param name="webId"></param>
        /// <returns></returns>
        private string GetTestScriptFilesnFolderContainTestCaseWithWebId(string scriptFolder, string webId)
        {
            if (!System.IO.Directory.Exists(scriptFolder))
            {
                return string.Empty;
            }
            foreach (string file in System.IO.Directory.GetFiles(scriptFolder, "*.rb", System.IO.SearchOption.AllDirectories))
            {
                string content = System.IO.File.ReadAllText(file);
                if (!string.IsNullOrEmpty(content))
                {
                    foreach (string line in content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimedLine = line.Trim(new []{'\t',' '}).Trim().ToLower();
                        if (!trimedLine.StartsWith(@"#") && trimedLine.EndsWith(string.Format(@"_webid_{0}", webId)))
                        {
                            return file;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
