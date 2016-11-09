using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using ES1Common.Logs;

using Core;
using Core.Model;
using Core.Management;

using Common.FileCommon;
using Common.ScriptCommon;
using Common.Windows;
using Common.Network;
//using SaberAgent.WindowsFormApp.DPSearch;
using Common.SSH;
//using SaberAgent.WindowsFormApp.Runtime_RubyMinitest;

namespace SaberAgent.WindowsFormApp
{
    public class SaberAgent
    {
        private string agentHostName = string.Empty;

        public static readonly AutomationLog Log = ATFEnvironment.Log;
        public static bool debug = false;

        private static System.Timers.Timer timer = new System.Timers.Timer(30000);
        private static string SaberAgentInstallPath = @"C:\SaberAgent";
        private static string LocalResultRootPath = SaberAgentInstallPath + @"\Result";
        private static string LocalScriptRootPath = SaberAgentInstallPath + @"\AutomationScripts";
        private static string SaberAgentConfigFolder = SaberAgentInstallPath + @"\" + @"Config";
        private static string SaberAgentConfigFilePath = SaberAgentConfigFolder + @"\Environment.xml";
        private static string RemoteAgentRootPath = @"/home/administrator/download/saberAgent";
        private static string RemoteAgentScriptsRootPath = RemoteAgentRootPath + @"/AutomationScripts";

        private static string RemoteResultRootPath = System.Configuration.ConfigurationManager.AppSettings["ResultRootPath"];
        private static string RemoteResultFileServerAdministrator = System.Configuration.ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdmin"];
        private static string RemoteResultFileServerPassword = System.Configuration.ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdminPassword"];

        private static AutomationJob job2Run = null;
        private Project project = null;
        private Product product = null;

        private ProductDeploymentHelper productDeploymentHelper = null;

        public SaberAgent()
        {
            //  InitializeConfiguration();
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(HandleSaberAgentActions);

        }

        public void Start()
        {
            Log.logger.Info("Start Agent Service ...");
            timer.Enabled = true;
            Log.logger.Info(string.Format("Saber Agent Started at {0}", System.DateTime.UtcNow.ToString()));
        }

        public void Stop()
        {
            Log.logger.Info("Stop Agent Service ...");
            timer.Stop();
            Log.logger.Info(string.Format("Saber Agent Stopped at {0}", System.DateTime.UtcNow.ToString()));
        }

        public static AutomationJob RunningJob
        {
            get { return job2Run; }
        }
        /// <summary>
        /// take the job to run
        /// 1. If the build is not installed, install the build
        /// 2. After the build is installed, set the environment status to BuildInstalled
        /// 3. Wait the Environment Manager Windows Service to restart all the machines
        /// 4. After restart, set the category of "Restarted" for each machine in DB
        /// 5. Check whether all the machines in SUT are restarted, if yes, start to run the test cases, else, continue to wait.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void HandleSaberAgentActions(object source, ElapsedEventArgs e)
        {
            // Get the latest config
            if (!InitializeConfiguration())
            {
                timer.Start();
                Log.logger.Info("There is no environment config in this agent, do nothing.");
                return;
            }
            //flush DNS first. Sometimes there're some DNS issue after the vAPP is created
            try
            {
                Common.Windows.WindowsServices.RestartService("Dnscache", 60 * 1000);//restart the DNS Client
                Log.logger.Info("Restart the DNS client service.");
                System.Threading.Thread.Sleep(1000);
                Log.logger.Info("Flush DNS.");
                DNSHelper.FlushDNS();
            }
            catch (Exception ex)
            {
                Log.logger.Error("Failed to flush the DNS on the test agent before the product setup.", ex);
            }

            if (!IsCurrentServiceHostTestAgent())
            {
                timer.Stop();
                Log.logger.Error("The saber agent doesn't run on the agent machine, do nothing.");
                return;
            }

            SaberAgent.Log.logger.Info(string.Format("The service is currently running on the test agent machine."));
            Log.logger.Info(string.Format("Start to monitor the jobs to handle by Saber Agent"));
            Log.logger.Info(string.Format("Stop the timer"));
            timer.Stop();

            //for debug purpose, wait the process attachment of VSTS
            while (debug)
            {
                System.Threading.Thread.Sleep(1000 * 10);
                Log.logger.Info(string.Format("The process is waiting to be attached for debuging..."));
            }

            //Take one job to run
            Log.logger.Info(string.Format("Start to take one job to run"));
            try
            {
                job2Run = GetAssignedJob();
                if (null == job2Run)
                {
                    Log.logger.Info(string.Format("Could not find a job whose status is Ready, start another loop."));
                    timer.Start();
                    return;
                }

            }
            catch (Exception ex)
            {
                Log.logger.Error(string.Format("Met exception when try to take one job in the saber agent to run"));
                Log.logger.Error(string.Format("Error detail: {0}", ex.Message + ex.StackTrace));
                timer.Start();
                return;
            }

            Log.logger.Info(string.Format("The id of the job taken by this agent is {0}", job2Run.JobId));

            //Initialize all parameters that may used below


            //Handle the job
            try
            {
                TestEnvironment sut = TestEnvironment.GetEnvironmentById(job2Run.SUTEnvironmentId.Value);
                TestEnvironment testAgent = TestEnvironment.GetEnvironmentById(job2Run.TestAgentEnvironmentId.Value);
                this.product = AutomationJob.GetProductOfJobByJobId(job2Run.JobId);
                this.project = AutomationJob.GetProjectOfJobByJobId(job2Run.JobId);

                //determine which deployment helper used to deploy the product
                switch (product.Name.ToLower())
                {
                    case "sourceone":
                    case "supervisor web"://The supervisor installer in integrated into the S1 installation.
                        productDeploymentHelper = new SourceOneDeploymentHelper();
                        break;
                    case "data protection search":
                        productDeploymentHelper = new DPSearch.DPSearchDeploymentHelper();
                        break;
                    case "common index search":
                        productDeploymentHelper = new CISDeploymentHelper();
                        break;
                    case "reveal 1.0":
                        productDeploymentHelper = new DPSearch.DPSearchDeploymentHelper();
                        break;
                    case "zinc":
                        productDeploymentHelper = new DPSearch.DPSearchDeploymentHelper();
                        break;
                }

                SaberAgent.Log.logger.Debug(string.Format("The status of the SUT [{0}] is {1}", sut.Name, sut.EnvironmentStatus));
                SaberAgent.Log.logger.Debug(string.Format("The status of the TestAgent [{0}] is {1}", testAgent.Name, testAgent.EnvironmentStatus));
                SaberAgent.Log.logger.Debug(string.Format("The status of the job [{0}] is {1}", job2Run.Name, job2Run.JobStatus));

                if (sut.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalledAndReady)
                {
                    try
                    {
                        MakesureTestAgentCanAccessSUTMachines();
                        job2Run.AddJobProgressInformation(string.Format("Start to install Build for Job [{0}:{1}]", job2Run.JobId, job2Run.Name));
                        productDeploymentHelper.InstallProduct(sut);
                        job2Run.AddJobProgressInformation(string.Format("Build is installed successfully for Job [{0}:{1}]", job2Run.JobId, job2Run.Name));
                        sut.SetEnvironmentStatus(EnvironmentStatus.BuildInstalled);
                    }
                    catch (Exception ex)
                    {
                        Log.logger.Error(string.Format("Error occured during install build on the environment {0}", sut.EnvironmentId));
                        Log.logger.Error(string.Format("Exception detail: {0} {1}", ex.Message, ex.StackTrace));
                        job2Run.AddJobProgressInformation(string.Format("Error occurred during the installation of build for Job [{0}:{1}]", job2Run.JobId, job2Run.Name));
                        sut.SetEnvironmentStatus(EnvironmentStatus.Error);
                        job2Run.SetJobsStatus(JobStatus.Failed);
                        timer.Start();
                        return;
                    }
                }
                else if (sut.EnvironmentStatus == EnvironmentStatus.BuildInstalled)
                {
                    //The status will be handled by the environment management service
                    //The environment management service will restart all the servers, and udate the environment status to Ready
                    job2Run.AddJobProgressInformation(string.Format("Going to reboot the machines with build installed for Job [{0}:{1}]", job2Run.JobId, job2Run.Name));
                    SaberAgent.Log.logger.Info(string.Format("Change Environment Status of {0} from BuildInstalled-->Ready", sut.Name));
                    sut.SetEnvironmentStatus(EnvironmentStatus.Ready);
                    productDeploymentHelper.RebootMachinesAfterProductInstalled(sut);
                    job2Run.AddJobProgressInformation(string.Format("All machines with build installed are rebooted for Job [{0}:{1}]", job2Run.JobId, job2Run.Name));
                }
                else if (sut.EnvironmentStatus == EnvironmentStatus.Ready)
                {
                    //Run test case
                    if (job2Run.JobStatus == JobStatus.Ready)
                    {
                        string message = string.Format("Wait until all the machines are rebooted before run the test case for Job [{0}:{1}]", job2Run.JobId, job2Run.Name);
                        Log.logger.Info(message);
                        job2Run.AddJobProgressInformation(message);
                        if (productDeploymentHelper.WaitAllMachinesStartedAfterReboot(sut))
                        {
                            string info = string.Format("All machines have been restarted.");
                            Log.logger.Info(info);
                            job2Run.AddJobProgressInformation(info);

                            productDeploymentHelper.CheckPreconditionForEachTestExecution(sut);
                            info = string.Format("All services are running. Ready to run the test cases for Job [{0}:{1}]", job2Run.JobId, job2Run.Name);
                            Log.logger.Info(info);
                            job2Run.AddJobProgressInformation(info);

                            info = string.Format("Start to run the test cases for Job [{0}:{1}]", job2Run.JobId, job2Run.Name);
                            Log.logger.Info(info);
                            job2Run.AddJobProgressInformation(info);

                            //get the saber module
                            string temp_message = "Start to install the Saber.";
                            /*
                            Log.logger.Info(temp_message);
                            job2Run.AddJobProgressInformation(temp_message);
                            if (GetAndBuildSaber())
                            {
                                temp_message = "Succeed to install the Saber.";
                                Log.logger.Info(temp_message);
                                job2Run.AddJobProgressInformation(temp_message);
                            }
                            else
                            {
                                temp_message = "Failed to install the Saber.";
                                Log.logger.Info(temp_message);
                                job2Run.AddJobProgressInformation(temp_message);
                                job2Run.SetJobsStatus(JobStatus.Failed);
                                timer.Start();
                                return;
                            }
                            */

                            //sync the test code
                            if (!GetTestScriptsOfJob(project))
                            {
                                temp_message = string.Format("Failed to sync the automation scripts of project [{0}].", project.Name);
                                Log.logger.Error(temp_message);
                                job2Run.AddJobProgressInformation(temp_message);
                                job2Run.SetJobsStatus(JobStatus.Failed);
                                timer.Start();
                                return;
                            }

                            //Copy the scripts to the remote saberAgent if has any such agent
                            DeployAutomationScriptsOfJob(testAgent);

                            //run the setup script of task
                            AutomationTask task = AutomationJob.GetTaskOfJobByJobId(job2Run.JobId);
                            string setupScript = task.SetupScript;

                            string scriptRootFolder = @"C:\SaberAgent\AutomationScripts";
                            string currentDirectoryFolder = string.Format(@"{0}\{1}\", scriptRootFolder, project.VCSRootPath);
                            message = @"Wait 1 minutes before run the test cases";
                            job2Run.AddJobProgressInformation(message);
                            System.Threading.Thread.Sleep(1000 * 60 * 1);//wait one minutes before run the test cases.

                            if (!string.IsNullOrEmpty(setupScript))
                            {
                                job2Run.AddJobProgressInformation(string.Format("Start to run the setup script of this task[ {0}:{1}]", task.Name, setupScript));
                                string temp = System.IO.Path.GetTempFileName() + ".bat";
                                TXTHelper.ClearTXTContent(temp);

                                TXTHelper.WriteNewLine(temp, "C:", Encoding.Default);
                                TXTHelper.WriteNewLine(temp, string.Format("cd {0}", currentDirectoryFolder), Encoding.Default);
                                TXTHelper.WriteNewLine(temp, setupScript, Encoding.Default);

                                string result = CMDScript.RumCmdWithWindowsVisible(temp, string.Empty);
                                job2Run.AddJobProgressInformation(result);

                                FileHelper.DeleteFile(temp);

                                job2Run.AddJobProgressInformation(string.Format("Setup script is executed for task {0}.", task.Name));
                            }

                            //Set job status to running
                            job2Run.SetJobsStatus(JobStatus.Running);
                            try
                            {
                                message = string.Format("Start to run the test cases of job [{0}:{1}].", job2Run.JobId, job2Run.Name);
                                job2Run.AddJobProgressInformation(message);
                                Log.logger.Info(message);

                                //System.Threading.Thread.Sleep(1000 * 60 * 5);//Neil, sleep 5 minute before run the test cases, maybe the SQL service is not started yet.

                                RunAllTestCases();

                                info = string.Format("All the test cases are finished for Job [{0}:{1}]", job2Run.JobId, job2Run.Name);
                                Log.logger.Info(info);
                                job2Run.AddJobProgressInformation(info);
                            }
                            catch (Exception ex)
                            {
                                info = string.Format("Error occured during running test cases on the environment {0}", sut.Name);
                                Log.logger.Error(info);
                                Log.logger.Error(string.Format("Exception detail: {0} {1}", ex.Message, ex.StackTrace));
                                job2Run.AddJobProgressInformation(info);
                                job2Run.SetJobsStatus(JobStatus.Failed);
                                timer.Start();
                                return;
                            }

                            //run the teardown scripts of task
                            string teardownScript = task.TeardownScript;
                            if (!string.IsNullOrEmpty(teardownScript))
                            {
                                string temp = System.IO.Path.GetTempFileName() + ".bat";
                                TXTHelper.ClearTXTContent(temp);

                                TXTHelper.WriteNewLine(temp, "C:", Encoding.Default);
                                TXTHelper.WriteNewLine(temp, string.Format("cd {0}", currentDirectoryFolder), Encoding.Default);
                                TXTHelper.WriteNewLine(temp, teardownScript, Encoding.Default);

                                string result = CMDScript.RumCmd(temp, string.Empty);
                                job2Run.AddJobProgressInformation(result);

                                FileHelper.DeleteFile(temp);
                            }

                            //collect the product logs from SUT
                            string localResultFolderForJob = GetLocalResultFolderForJob();
                            if (!string.IsNullOrEmpty(localResultFolderForJob))
                            {
                                string localProductionLogsFolder = System.IO.Path.Combine(localResultFolderForJob, "production_logs");
                                FileHelper.CreateFolder(localProductionLogsFolder);
                                productDeploymentHelper.CollectProductionLogsAfterExecutionToLocal(sut, localProductionLogsFolder);
                                CopyProductionLogsToCentralResultFileServer(localProductionLogsFolder);
                            }
                            //set job status to complelte
                            job2Run.SetJobsStatus(JobStatus.Complete);
                        }
                        else
                        {
                            string info = string.Format("Timeout to wait all the machines to be restarted and ready for Job [{0}:{1}].", job2Run.JobId, job2Run.Name);
                            Log.logger.Info(info);
                            job2Run.AddJobProgressInformation(info);
                            job2Run.SetJobsStatus(JobStatus.Timeout);
                            timer.Start();
                            return;
                        }
                    }
                    else
                    {
                        //deleting the scripts folder
                        string scriptRootFolder = SaberAgentInstallPath + @"\AutomationScripts";
                        string message = string.Format("Start to delete the scrpt folder {0}", scriptRootFolder);
                        if (FileHelper.IsExistsFolder(scriptRootFolder))
                        {
                            FileHelper.EmptyFolder(scriptRootFolder);
                            message = string.Format("Delete the scrpt folder {0}", scriptRootFolder);
                            SaberAgent.Log.logger.Info(message);
                        }

                        Log.logger.Info(string.Format("The Job {0} is running or completed, we'll not to rerun it anymore.", job2Run.JobId));
                        timer.Start();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex.Message, ex);
                job2Run.SetJobsStatus(JobStatus.Failed);
                timer.Start();
                return;
            }
            finally
            {
                timer.Start();
            }
        }

        #region private methods

        private void DeployAutomationScriptsOfJob(TestEnvironment environment)
        {
            TestEnvironmentConfigHelper helper = new TestEnvironmentConfigHelper(environment.Config);

            foreach (Machine m in helper.TestAgentConfiguration.Machines)
            {
                if (m.Roles.FindAll(r => r.Key == AgentType.RemoteAgent).Count() > 0)//For the remote Agent
                {
                    string message = string.Format("Start to copy automation scripts to remote agent [{0}]", m.Name);

                    job2Run.AddJobProgressInformation(message);

                    SaberAgent.Log.logger.Info(message);

                    string administrator = string.IsNullOrEmpty(m.Administrator)? helper.DomainConfiguration.Adminstrator:m.Administrator;

                    string password = string.IsNullOrEmpty(m.Password)?helper.DomainConfiguration.Password:m.Password;

                    //Clear the existing code
                    message = string.Format("SSHWrapper.DeleteRemoteFolder({0}, {1}, {2}, {3})",m.ExternalIP, administrator, password, RemoteAgentScriptsRootPath);
                    SaberAgent.Log.logger.Debug(message);
                    SSHWrapper.DeleteRemoteFolder(m.ExternalIP, administrator, password, RemoteAgentScriptsRootPath);
                    
                    //copy the automation source code to it
                    message = string.Format("SSHWrapper.CopyDirectoryToRemote({0}, {1}, {2}, {3}, {4})", m.ExternalIP, administrator, password, LocalScriptRootPath, RemoteAgentScriptsRootPath);
                    SaberAgent.Log.logger.Debug(message);
                    SSHWrapper.CopyDirectoryToRemote(m.ExternalIP, administrator, password, LocalScriptRootPath, RemoteAgentScriptsRootPath);

                    message = string.Format("Folder [{0}] is copied to [{1}] on machine [{2}]", LocalScriptRootPath, RemoteAgentScriptsRootPath, m.Name);
                    SaberAgent.Log.logger.Info(message);

                    //Clear the config file
                    string remoteAgentConfigPath = string.Format("{0}/Config", RemoteAgentRootPath);

                    message = string.Format(" SSHWrapper.DeleteRemoteFolder({0}, {1}, {2}, {3})", m.ExternalIP, administrator, password, remoteAgentConfigPath);
                    SaberAgent.Log.logger.Debug(message);
                    SSHWrapper.DeleteRemoteFolder(m.ExternalIP, administrator, password, remoteAgentConfigPath);
                    
                    //Copy the config file to remote
                    message = string.Format("SSHWrapper.CopyDirectoryToRemote({0}, {1}, {2}, {3}, {4})", m.ExternalIP, administrator, password, SaberAgentConfigFolder, remoteAgentConfigPath);
                    SaberAgent.Log.logger.Debug(message);
                    SSHWrapper.CopyDirectoryToRemote(m.ExternalIP, administrator, password, SaberAgentConfigFolder, remoteAgentConfigPath);

                    message = string.Format("Folder [{0}] is copied to [{1}] on machine [{2}]", SaberAgentConfigFolder, remoteAgentConfigPath, m.Name);

                    SaberAgent.Log.logger.Info(message);
                }
            }
        }

        private string GetLocalResultFolderForJob()
        {
            string jobResultPath = string.Empty;
            try
            {
                int taskId = JobManagement.GetAutomationTaskOfJob(job2Run).TaskId;
                string taskResultPath = System.IO.Path.Combine(SaberAgent.LocalResultRootPath, taskId.ToString());
                FileHelper.CreateFolder(taskResultPath);
                jobResultPath = System.IO.Path.Combine(taskResultPath, job2Run.JobId.ToString());
                FileHelper.CreateFolder(jobResultPath);
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                return string.Empty;
            }
            if (System.IO.Directory.Exists(jobResultPath))
            {
                return jobResultPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetRemoteResultFolderForJob()
        {
            string jobResultPath = string.Empty;
            try
            {
                int taskId = JobManagement.GetAutomationTaskOfJob(job2Run).TaskId;
                NetUseHelper.NetUserRemoteFolerToLocalPath(SaberAgent.RemoteResultRootPath, Core.LocalMappedFolder.ResultTempFolder2, SaberAgent.RemoteResultFileServerAdministrator, SaberAgent.RemoteResultFileServerPassword);
                string taskResultPath = Core.LocalMappedFolder.ResultTempFolder2 + @"\" + taskId.ToString();
                FileHelper.CreateFolder(taskResultPath);
                jobResultPath = System.IO.Path.Combine(taskResultPath, job2Run.JobId.ToString());
                FileHelper.CreateFolder(jobResultPath);
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                return string.Empty;
            }
            if (System.IO.Directory.Exists(jobResultPath))
            {
                return jobResultPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private bool GetTestScriptsOfJob(Project project)
        {
            if (!FileHelper.IsExistsFolder(LocalScriptRootPath))
            {
                FileHelper.CreateFolder(LocalScriptRootPath);
            }
            String batName = null;
            switch (project.VCSType)
            {
                case (int)VCSType.TFS:
                    batName = "SyncTFSCode.bat";
                    break;
                case (int)VCSType.ClearCase:
                    batName = "SyncClearCaseCode.bat";
                    break;
                case (int)VCSType.Git:
                    batName = "SyncGITCode.bat";
                    break;
                case (int)VCSType.NotSync:
                    return true;                   
            }

            int i = 0;
            string output = string.Empty;
            do
            {
                string bat = @"C:\SaberAgent\" + batName;
                string cspFileName = @"C:\SaberAgent\CSPs\" + project.Name + ".csp";//currently, we get the csp by the project name
                if (project.VCSType == (int)VCSType.Git)
                {
                    string log = string.Format(@">{0}\synclog.log", SaberAgentInstallPath);
                    SaberAgent.Log.logger.Info(string.Format(@"Start to run command [{0} /C {1}] to syn the code.", bat, project.VCSServer + " " + LocalScriptRootPath + " " + log));
                    output = CMDScript.RumCmd(bat, project.VCSServer + " " + LocalScriptRootPath + " " + log);
                }
                else
                {
                    output = CMDScript.RumCmd(bat, project.VCSRootPath + " " + project.VCSServer + " " + project.VCSUser + " \"" + project.VCSPassword + "\" " + cspFileName);
                }
                SaberAgent.Log.logger.Info(string.Format("The output of the code syncing is [{0}].", output));

                i++;
            } while (System.IO.Directory.EnumerateDirectories(LocalScriptRootPath).Count() == 0 && i < 5);

            if (System.IO.Directory.EnumerateDirectories(LocalScriptRootPath).Count() == 0)
            {
                SaberAgent.Log.logger.Error(output);
                return false;
            }
            else
            {
                SaberAgent.Log.logger.Info(output);
                return true;
            }
        }

        private bool CopyProductionLogsToCentralResultFileServer(string localLogFolder)
        {
            string jobResultPath = GetRemoteResultFolderForJob();
            if (!string.IsNullOrEmpty(jobResultPath))
            {
                FileHelper.CopyDirectory(localLogFolder, jobResultPath);
                return true;
            }
            else
            {
                string message = string.Format("Failed to get the folder to copy the production logs to");
                SaberAgent.Log.logger.Error(message);
                job2Run.AddJobProgressInformation(message);
                return false;
            }            
        }

        private void RunAllTestCases()
        {
            string message = string.Empty;

            AutomationTask at = AutomationJob.GetTaskOfJobByJobId(job2Run.JobId);

            string rootResultPath = GetLocalResultFolderForJob();

            //Currently, the NotExisting suite type is used by CIS and DPSearch, thus the scripts will run on remote linux machine.
            if (TestSuite.GetTestSuite(int.Parse(at.TestContent)).Type == (int)SuiteType.NotExisting)
            {
                //Neil, TO DO NEXT WEEK!!!!!
                //TODO, refine the logic to determine whether the handler should be remote minitest execution handler
                TestCaseExecution useless = new TestCaseExecution();
                SaberTestExecutionHandler handler = new Runtime_RubyMinitest.RemoteRubyMiniTestExecutionHandler(job2Run, rootResultPath);
                handler.RunTestCase(useless);
            }
            else
            {
                List<TestCaseExecution> executions = TestCaseExecution.GetTestCaseExectionByJob(job2Run.JobId).ToList();

                //This is the folder where the test result will copy to. And at last, the result files will be copied to central file server.

                foreach (TestCaseExecution ex in executions)
                {
                    // check the precondition for each execution
                    SaberAgent.Log.logger.Info(string.Format("Check precondition for execution {0} of job {1}", ex.ExecutionId, job2Run.JobId));
                    productDeploymentHelper.CheckPreconditionForEachTestExecution(TestEnvironment.GetEnvironmentById(job2Run.SUTEnvironmentId.Value));

                    //update the execution status to be running
                    SaberAgent.Log.logger.Info(string.Format("Set status of execution {0} of job {1} to running", ex.ExecutionId, job2Run.JobId));
                    ex.SetStatus(ExecutionStatus.Running);

                    TestCase testCase = TestCase.GetTestCase(ex.TestCaseId);

                    message = string.Format("Start Execution of test case [{0}:{1}]", testCase.SourceId, testCase.Name);

                    job2Run.AddJobProgressInformation(message);

                    Runtime runtime = testCase.GetRuntime();

                    if (runtime == Runtime.CSharpNUnit)
                    {
                        SaberTestExecutionHandler handler = new CSharpNUnitTestExecutionHandler(job2Run, rootResultPath, this.project);
                        handler.RunTestCase(ex);
                    }
                    else if (runtime == Runtime.RubyMiniTest)
                    {
                        SaberTestExecutionHandler handler = new RubyMiniTestExecutionHandler(job2Run, rootResultPath, this.project);
                        handler.RunTestCase(ex);
                    }
                    else if (runtime == Runtime.CSharpMSUnit)
                    {
                        SaberTestExecutionHandler handler = new CSharpMSUnitTestExecutionHandler(job2Run, rootResultPath, this.project);
                        handler.RunTestCase(ex);
                    }
                    else
                    {
                        message = string.Format("The runtime of test case [{0}] is not specified.", ex.TestCase.Name);
                        SaberAgent.Log.logger.Error(message);
                        job2Run.AddJobProgressInformation(message);
                        ex.SetStatus(ExecutionStatus.Fail);
                    }
                    message = string.Format("End execution of test case [{0}:{1}]", testCase.SourceId, testCase.Name);
                    job2Run.AddJobProgressInformation(message);
                }
            }//end of else
            productDeploymentHelper.CollectProductionLogsAfterExecutionToLocal(TestEnvironment.GetEnvironmentById(job2Run.SUTEnvironmentId.Value), rootResultPath);

        }


        /// <summary>
        /// Check whether the Saber Agent service running on the Test Agent machine
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentServiceHostTestAgent()
        {
            string configXML = TXTHelper.GetTXT(SaberAgentConfigFilePath);
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(configXML);
            foreach (Machine m in config.TestAgentConfiguration.Machines)
            {
                if (m.Name.ToLower() == agentHostName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }


        private bool MakesureTestAgentCanAccessSUTMachines()
        {
            string configXML = TXTHelper.GetTXT(SaberAgentConfigFilePath);
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(configXML);

            string sutDomain = config.SUTConfiguration.SUTDomainConfig.Name;
            int sutMachinesCount = config.SUTConfiguration.Machines.Count;

            System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[sutMachinesCount];

            for (int i = 0; i < config.TestAgentConfiguration.Machines.Count; i++)
            {
                Machine agent = config.TestAgentConfiguration.Machines[i];
                if (string.Compare(agent.Name, agentHostName, true) == 0)//current running on the test agent machine
                {
                    for (int j = 0; j < sutMachinesCount; j++)
                    {
                        Machine sut = config.SUTConfiguration.Machines[j];
                        tasks[j] = System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {

                            if (config.Type != EnvironmentType.Residence_Together)
                            {
                                string ip = sut.ExternalIP == string.Empty ? sut.IP : sut.ExternalIP;
                                PingHelper.WaitUntillPingable(ip, 300);
                            }
                            else
                            {
                                PingHelper.WaitUntillPingable(sut.IP, 300);
                                PingHelper.WaitUntillPingable(sut.Name, 300);
                                if (!sut.Name.ToLower().Contains(sutDomain.ToLower()))
                                {
                                    PingHelper.WaitUntillPingable(string.Format("{0}.{1}", sut.Name, sutDomain), 300);
                                }
                            }
                        });
                    }
                }
            }
            try
            {
                if (System.Threading.Tasks.Task.WaitAll(tasks, 60 * 1000))
                {
                    SaberAgent.Log.logger.Info(string.Format("All the SUT machines can be access from test agent!"));
                    return true;
                }
                else
                {
                    string message = string.Format("Not all the SUT machines can be access from test agent after 10 minutes.");
                    SaberAgent.Log.logger.Error(message);
                    throw new Exception(message);
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    SaberAgent.Log.logger.Error(string.Format("all the SUT machines can be access from test agent after 10 minutes."));
                    SaberAgent.Log.logger.Error(e);
                }
                throw ex;
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get the test agent runtime from the configuration file.
        /// If not defined, return CSharpNUnit by default
        /// </summary>
        /// <returns></returns>
        private string GetRunTimeOfTestAgent()
        {
            string configXML = TXTHelper.GetTXT(SaberAgentConfigFilePath);
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(configXML);
            foreach (string c in config.TestAgentConfiguration.Categories)
            {
                string cName = c.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (cName.ToLower() == "runtime")
                {
                    return c.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }
            //if not defined, return CSharpNUnit by default.
            return "CSharpNUnit";
        }

        /// <summary>
        /// Take one job for this agent to run
        /// </summary>
        /// <returns></returns>
        private AutomationJob GetAssignedJob()
        {
            string jobId = string.Empty;
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(TXTHelper.GetTXT(SaberAgentConfigFilePath));
            foreach (string c in config.TestAgentConfiguration.Categories)
            {
                if (c.ToLower().Contains("jobid"))
                {
                    jobId = c.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    break;
                }
            }
            if (string.IsNullOrEmpty(jobId))
            {
                string message = string.Format("The jobId in local config file is null or empty.");
                Log.logger.Info(message);
                return null;
            }
            else
            {
                return AutomationJob.GetAutomationJob(int.Parse(jobId));
            }
        }

        private bool InitializeConfiguration()
        {

            if (!System.IO.File.Exists(SaberAgentConfigFilePath))
            {
                string message = string.Format("Could not find the test agent config file on the folder {0}", SaberAgentConfigFilePath);
                Log.logger.Info(message);
                return false;
            }
            else
            {
                this.agentHostName = Environment.MachineName;
                return true;
            }
        }

        private bool DoesCurrentMachineHostAnyOfSourceComponentRoles(string role)
        {
            string configXML = TXTHelper.GetTXT(SaberAgentConfigFilePath);
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(configXML);
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                if (m.Roles.FindAll(r => r.Key == role).Count > 0 && m.Name.ToLower() == agentHostName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private bool GetAndBuildSaber()
        {
            string result = string.Empty;
            string saber_vcs_server = "http://10.37.11.121:8080/tfs/defaultcollection";
            string saber_vcs_user = @"es1\wangn6";
            string saber_vcs_password = @"wangn6";
            string saber_vcs_root_path = @"AutomationFramework/ES1Automation/Main/SourceOne/CSharpNUnit/Saber";

            string parameters = string.Format(@"""{0}"" {1} {2} {3}",
                saber_vcs_server, saber_vcs_user, saber_vcs_password, saber_vcs_root_path);
            string runBATFile = @"C:\SaberAgent\GetAndBuildSaber.bat";
            while (!System.IO.Directory.Exists(@"C:\SaberAgent\Saber\AutomationFramework\ES1Automation\Main\SourceOne\CSharpNUnit\Saber"))
            {
                result = CMDScript.RumCmd(runBATFile, parameters);
                if (!result.Contains("Build succeeded."))//build saber solution failed.
                {
                    SaberAgent.Log.logger.Info(result);
                }
                else
                {
                    SaberAgent.Log.logger.Info(string.Format("Succeeded to install the Saber module."));
                }
            }
            return true;
        }

        #endregion
    }
}
        
