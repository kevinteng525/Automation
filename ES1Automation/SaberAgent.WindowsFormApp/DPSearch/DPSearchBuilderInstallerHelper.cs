using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;
using Core.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Common.Windows;
using Common.ScriptCommon;
using Common.FileCommon;
using Common.Network;
using Common.SQL;
using Common.ActiveDirecotry;
using Microsoft.Win32;
using Common.SSH;

namespace SaberAgent.WindowsFormApp
{
    public class DPSearchRole
    {
        public const string Worker = "Worker";
        public const string AvamarServer = "AvamarServer";
        public const string AvamarClient = "AvamarClient";
        public const string CIS = "CIS";
        public const string DC = "DC";
        public const string Dpnode = "Dpnode";
        public const string ElasticSearch = "ElasticSearch";
        public const string RemoteAgent = "RemoteAgent";
        public const string NetworkerClient = "NetworkerClient";
        public const string NetworkerServer = "NetworkerServer";

    }

    public static class DPSearchBuilderInstallerHelper
    {
        private static TestEnvironment environment = null;
        private static TestEnvironmentConfigHelper config = null;
        private static string sutDomainName = string.Empty;
        private static string sutDomainAdmin = string.Empty;
        private static string sutDomainAdminPassword = string.Empty;
        private static string remoteAgent = string.Empty;
        private static string sourceScriptPath = @"C:\SaberAgent\DPSearch\scripts\";
        private static string dstScriptPath = @"/home/administrator/download/saberAgent/";
        private static string installCommand = "install_latest_build.sh cis api > " + dstScriptPath + "install.log 2>&1";
        private static AutomationJob job = null;
        private static Build build = null;
        private static string buildName = "*.t*z";

        private static List<KeyValuePair<string, string>> MSICodeMappings = new List<KeyValuePair<string, string>>();

        private static string buildServerUser = ConfigurationManager.AppSettings["BuildServerUser"];
        private static string buildServerPassword = ConfigurationManager.AppSettings["BuildServerPassword"];
        private const int installTimeOut = 1000 * 60 * 30;
        private const int rebootTimeOut = 1000 * 60 * 30;


        public static void Initialize(TestEnvironment environment)
        {
            DPSearchBuilderInstallerHelper.environment = environment;
            DPSearchBuilderInstallerHelper.config = new TestEnvironmentConfigHelper(environment.Config);
            DPSearchBuilderInstallerHelper.sutDomainName = config.SUTConfiguration.SUTDomainConfig.Name;
            DPSearchBuilderInstallerHelper.sutDomainAdmin = config.SUTConfiguration.SUTDomainConfig.Adminstrator;
            DPSearchBuilderInstallerHelper.sutDomainAdminPassword = config.SUTConfiguration.SUTDomainConfig.Password;
            DPSearchBuilderInstallerHelper.remoteAgent = getRemoteAgent();
            DPSearchBuilderInstallerHelper.job = getAutomationJob();
            DPSearchBuilderInstallerHelper.build = getBuild();
        }

        public static AutomationJob getAutomationJob()
        {

            AutomationJob job = EnvironmentManager.GetAutomationJobOfTestEnvironment(environment);
            if (job == null)
            {
                SaberAgent.Log.logger.Error(string.Format("The job  for this environment {0} is null", environment.EnvironmentId));

            }
            return job;

        }
        public static string getRemoteAgent() //Get machine's ip which is remote agent
        {
            List<Machine> machines = DPSearchBuilderInstallerHelper.config.SUTConfiguration.Machines;
            foreach (Machine machine in machines)
            {
                foreach (KeyValuePair<string, bool> r in machine.Roles)
                {
                    if (r.Key == DPSearchRole.RemoteAgent && r.Value == true)
                    {
                        return machine.ExternalIP;
                    }
                }
            }
            string message = string.Format("Failed to get the remote agent to test environment {0}", environment.EnvironmentId);
            SaberAgent.Log.logger.Error(message);
            return string.Empty;
        }
        public static Build getBuild()
        {
            AutomationTask task = JobManagement.GetAutomationTaskOfJob(DPSearchBuilderInstallerHelper.job);
            if (task != null)
            {
                return Build.GetBuildById(task.BuildId);
            }
            else
            {
                SaberAgent.Log.logger.Error(string.Format("The task of the job {0} should not be null.", job.JobId));
                return null;
            }

        }
        private static void AuthenticateRemoteServer()
        {
            SaberAgent.Log.logger.Info(string.Format("Net use the remote build server before copy any build from it."));

            string remoteServer = string.Empty;
            string buildLocation = build.Location;
            remoteServer = buildLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
            NetUseHelper.NetUserMachine(remoteServer, buildServerUser, buildServerPassword);
            SaberAgent.Log.logger.Info(string.Format("The remote server {0} is authenticated.", remoteServer));
        }
        public static bool copyBuildFileToRemote()
        {
            if (FileHelper.IsExistsFolder(DPSearchBuilderInstallerHelper.sourceScriptPath))
            {
                try
                {
                    string[] files = System.IO.Directory.GetFiles(DPSearchBuilderInstallerHelper.build.Location, buildName);
                    SSHWrapper.DeleteRemoteFolder(DPSearchBuilderInstallerHelper.remoteAgent, DPSearchBuilderInstallerHelper.sutDomainAdmin, DPSearchBuilderInstallerHelper.sutDomainAdminPassword, DPSearchBuilderInstallerHelper.dstScriptPath);
                    SSHWrapper.CreateRemoteFolder(DPSearchBuilderInstallerHelper.remoteAgent, DPSearchBuilderInstallerHelper.sutDomainAdmin, DPSearchBuilderInstallerHelper.sutDomainAdminPassword, DPSearchBuilderInstallerHelper.dstScriptPath);
                    foreach (string f in files)
                    {
                        SSHWrapper.CopyFileFromLocalToRemote(DPSearchBuilderInstallerHelper.remoteAgent, DPSearchBuilderInstallerHelper.sutDomainAdmin, DPSearchBuilderInstallerHelper.sutDomainAdminPassword, f, DPSearchBuilderInstallerHelper.dstScriptPath);
                    }

                    string message = string.Format("Build is coped to the Test Agent Machine.");
                    SaberAgent.Log.logger.Info(message);
                    SaberAgent.RunningJob.AddJobProgressInformation(message);
                    return true;
                }
                catch (Exception e)
                {
                    string message = string.Format("Failed to dispatch the installation script of DPSearch to test environment {0}", DPSearchBuilderInstallerHelper.remoteAgent);
                    SaberAgent.Log.logger.Error(message + e.Message);
                    return false;
                }
            }
            return false;


        }
        public static bool copyScriptFileToRemote()
        {
            if (FileHelper.IsExistsFolder(DPSearchBuilderInstallerHelper.sourceScriptPath))
            {
                try
                {

                    SSHWrapper.CopyDirectoryToRemote(DPSearchBuilderInstallerHelper.remoteAgent, DPSearchBuilderInstallerHelper.sutDomainAdmin, DPSearchBuilderInstallerHelper.sutDomainAdminPassword, DPSearchBuilderInstallerHelper.sourceScriptPath, DPSearchBuilderInstallerHelper.dstScriptPath);
                    string message = string.Format("Install scripts is coped to the Test Agent Machine.");
                    SaberAgent.Log.logger.Info(message);
                    SaberAgent.RunningJob.AddJobProgressInformation(message);
                    return true;
                }
                catch (Exception e)
                {
                    string message = string.Format("Failed to dispatch the installation script of DPSearch to test environment {0}", DPSearchBuilderInstallerHelper.remoteAgent);
                    SaberAgent.Log.logger.Error(message + e.Message);
                    return false;
                }
            }
            return false;

        }
        public static bool InstallDPSearchOnSUTEnvironment()
        {

            try
            {
                AutomationJob job = EnvironmentManager.GetAutomationJobOfTestEnvironment(environment);
                if (job == null)
                {
                    SaberAgent.Log.logger.Error(string.Format("The job {0} for this environment is null", job.JobId));
                    return false;
                }
                else
                {

                    AuthenticateRemoteServer();
                    copyBuildFileToRemote();
                    copyScriptFileToRemote();
          //          runInstallScript();
                    return true;
                }
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex.Message);
                job.JobStatus = JobStatus.Failed;
                throw ex;
            }
        }
        public static bool runInstallScript()
        {
            try
            {
                string installCommand = "bash " + DPSearchBuilderInstallerHelper.dstScriptPath + DPSearchBuilderInstallerHelper.installCommand;
                SSHWrapper.RunCommand(DPSearchBuilderInstallerHelper.remoteAgent, DPSearchBuilderInstallerHelper.sutDomainAdmin, DPSearchBuilderInstallerHelper.sutDomainAdminPassword, installCommand);
                return true;
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to run install command on {0}", DPSearchBuilderInstallerHelper.remoteAgent);
                SaberAgent.Log.logger.Error(message + e.Message);
                return false;
            }

        }

    }
}
