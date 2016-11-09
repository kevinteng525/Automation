using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;
using Common.Windows;
using Common.SSH;
using System.Configuration;
using Common.FileCommon;


namespace SaberAgent.WindowsFormApp.Runtime_RubyMinitest
{

    class RemoteRubyMiniTestExecutionHandler : SaberTestExecutionHandler
    {

        public class RemoteAgent
        {
            public string Admin;
            public string Password;
            public string Server;
        }

        private string vcsRootPath = string.Empty;
        private TestEnvironment environment = null;
        private RemoteAgent remote = null;
        private string scriptRootFolder = @"C:\SaberAgent\AutomationScripts\";
        private string remoteLinuxAgentFolder = @"/home/administrator/download/saberAgent/";
        private string executionCommand = string.Empty;
        private string remoteLogFolder = @"/home/administrator/download/saberAgent/logs/";
        private string remoteReportFolder = "/home/administrator/download/saberAgent/report/";

        private ResultParser resultParser = new RemoteRubyMiniTestResultParseForNotExistingTestCase();

        public RemoteRubyMiniTestExecutionHandler(AutomationJob job2Run, string rootResultPath)
        {
            Project project = AutomationJob.GetProjectOfJobByJobId(job2Run.JobId);
            this.RootResultPath = rootResultPath;
            this.Job2Run = job2Run;
            this.environment = TestEnvironment.GetEnvironmentById(job2Run.TestAgentEnvironmentId.Value);
            this.vcsRootPath = project.VCSRootPath;
            this.remote = GetRemoteTestAgent(this.environment);
            this.executionCommand = GetExecutionCommand(job2Run);
        }

        public string GetExecutionCommand(AutomationJob job)
        {
            AutomationTask at = AutomationJob.GetTaskOfJobByJobId(job.JobId);
            return TestSuite.GetTestSuite(int.Parse(at.TestContent)).ExecutionCommand;
        }

        public RemoteAgent GetRemoteTestAgent(TestEnvironment environment)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            string domainAdmin = config.SUTConfiguration.SUTDomainConfig.Adminstrator;
            string domainAdminPassword = config.SUTConfiguration.SUTDomainConfig.Password;
            RemoteAgent r = new RemoteAgent();
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                if (m.Roles.FindAll(role => role.Key == Core.AgentType.RemoteAgent).Count() > 0)
                {
                    r.Admin = string.IsNullOrEmpty(m.Administrator) ? domainAdmin : m.Administrator;
                    r.Password = string.IsNullOrEmpty(m.Password) ? domainAdminPassword : m.Password;
                    r.Server = m.ExternalIP;
                    return r;
                }
            }
            return null;
        }

        public override void RunTestCase(TestCaseExecution execution)
        {
            try
            {
                AutomationTask at = AutomationJob.GetTaskOfJobByJobId(Job2Run.JobId);
                Project p = Project.GetProjectById((int)at.ProjectId);
                string codePath = scriptRootFolder + p.VCSRootPath + @"\";
                string remoteCodePath = remoteLinuxAgentFolder + @"automation/";
                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.CreateRemoteFolder({0}, {1}, {2}, {3})", remote.Server, remote.Admin, remote.Password, remoteCodePath));
                SSHWrapper.CreateRemoteFolder(remote.Server, remote.Admin, remote.Password, remoteCodePath);
                if (p.VCSType != (int)Core.VCSType.NotSync)
                {
                    SSHWrapper.CopyDirectoryToRemote(remote.Server, remote.Admin, remote.Password, codePath, remoteCodePath);
                }
                string command = "cd '" + remoteLinuxAgentFolder + "';" + this.executionCommand;
                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.RunCommand({0}, {1}, {2}, {3})", remote.Server, remote.Admin, remote.Password, command));
                string res = SSHWrapper.RunCommand(remote.Server, remote.Admin, remote.Password, command);
                SaberAgent.Log.logger.Info(string.Format("Test Cases have been executed, result is {0}", res));

                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.CopyDirectoryFromRemoteToLocal({0}, {1}, {2}, {3}, {4});", remote.Server, remote.Admin, remote.Password, RootResultPath, remoteLogFolder));
                SSHWrapper.CopyDirectoryFromRemoteToLocal(remote.Server, remote.Admin, remote.Password, RootResultPath, remoteLogFolder);
                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.CopyDirectoryFromRemoteToLocal({0}, {1}, {2}, {3}, {4});", remote.Server, remote.Admin, remote.Password, RootResultPath, remoteReportFolder));
                SSHWrapper.CopyDirectoryFromRemoteToLocal(remote.Server, remote.Admin, remote.Password, RootResultPath, remoteReportFolder);
                
                //Parse the test result
                resultParser.ParseResult(RootResultPath, Job2Run.JobId);
                SaberAgent.Log.logger.Info(string.Format("Result parsing has been completed for execution {0} of job {1}", execution.ExecutionId, Job2Run.JobId));

                //Clean the remote results and logs
                SSHWrapper.DeleteRemoteFolder(remote.Server, remote.Admin, remote.Password, remoteLogFolder);
                SSHWrapper.DeleteRemoteFolder(remote.Server, remote.Admin, remote.Password, remoteReportFolder);
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to result parsing {0}, result Path {1}", this.remote.Server, RootResultPath);
                SaberAgent.Log.logger.Error(message, e);
            }
        }
    }
}
