using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;
using System.Configuration;
using Core.Management;
using Common.Windows;
using Common.Network;
using Common.FileCommon;
using Common.SSH;

namespace SaberAgent.WindowsFormApp.CIS
{
    public class CISRole
    {
        public const string DC = "DC";
        public const string RemoteAgent = "RemoteAgent";
        public const string CIS = "CIS";
    }

    public static class CISBuilderInstallerHelper
    {
        private static TestEnvironment environment = null;
        private static TestEnvironmentConfigHelper config = null;
        private static string LocalInstallScriptRootPath = @"C:\SaberAgent\CIS\scripts\";
        private static string DstAgentRootPath = @"/home/administrator/download/saberAgent/";
        private static string InstallCommand = "install_latest_cis_build.sh > " + DstAgentRootPath + "install.log 2>&1";
        private static string CollectLogsCommand = "collect_cis_product_logs.sh";
        private static AutomationJob job = null;
        private static Build build = null;
        private static string BuildNamePattern = "cis-*.tgz";
        private static string AutomationPattern = "automation-*.tgz";
        private static string RemoteLogFolder = @"/home/administrator/download/saberAgent/logs/";

        private static string buildServerUser = ConfigurationManager.AppSettings["BuildServerUser"];
        private static string buildServerPassword = ConfigurationManager.AppSettings["BuildServerPassword"];
        private const int installTimeOut = 1000 * 60 * 30;
        private const int rebootTimeOut = 1000 * 60 * 30;


        public static void Initialize(TestEnvironment environment)
        {
            CISBuilderInstallerHelper.environment = environment;
            CISBuilderInstallerHelper.config = new TestEnvironmentConfigHelper(environment.Config);
            CISBuilderInstallerHelper.job = getAutomationJob();
            CISBuilderInstallerHelper.build = getBuild();
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

        public static Build getBuild()
        {
            AutomationTask task = JobManagement.GetAutomationTaskOfJob(CISBuilderInstallerHelper.job);
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

        private static void AuthenticateRemoteBuildServer()
        {
            SaberAgent.Log.logger.Info(string.Format("Net use the remote build server before copy any build from it."));
            string remoteServer = string.Empty;
            string buildLocation = build.Location;
            remoteServer = buildLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
            NetUseHelper.NetUserMachine(remoteServer, buildServerUser, buildServerPassword);
            SaberAgent.Log.logger.Info(string.Format("The remote server {0} is authenticated.", remoteServer));
        }

        public static bool CopyBuildFileToRemote(Machine machine)
        {
            try
            {
                string message = "";
                string[] buildFiles = System.IO.Directory.GetFiles(CISBuilderInstallerHelper.build.Location, BuildNamePattern);
                string[] automationFiles = System.IO.Directory.GetFiles(CISBuilderInstallerHelper.build.Location, AutomationPattern);
                foreach (string f in buildFiles.Concat(automationFiles))
                {
                    message = string.Format("Start to copy file [{0}] to [{1}].", f, CISBuilderInstallerHelper.DstAgentRootPath);
                    SaberAgent.Log.logger.Info(message);
                    SSHWrapper.CopyFileFromLocalToRemote(machine.ExternalIP, machine.Administrator, machine.Password, f, CISBuilderInstallerHelper.DstAgentRootPath);
                    message = string.Format("File [{0}] is copied to [{1}] successfully", f, CISBuilderInstallerHelper.DstAgentRootPath);
                    SaberAgent.Log.logger.Info(message);
                    SaberAgent.RunningJob.AddJobProgressInformation(message);
                }

                message = string.Format("All build files are coped to the Test Agent Machine [{0}].", machine.Name);
                SaberAgent.Log.logger.Info(message);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
                return true;
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to copy build from {0} to {1}", CISBuilderInstallerHelper.build.Location, machine.Name);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
                SaberAgent.Log.logger.Error(message, e);
                return false;
            }
        }

        public static bool CleanRemoteAgentFolder(Machine machine)
        {
            try
            {
                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.DeleteRemoteFolder({0}, {1}, {2}, {3});",
                    machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.DstAgentRootPath));
                SSHWrapper.DeleteRemoteFolder(machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.DstAgentRootPath);
                SaberAgent.Log.logger.Debug(string.Format("SSHWrapper.CreateRemoteFolder({0}, {1}, {2}, {3});", 
                    machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.DstAgentRootPath));
                SSHWrapper.CreateRemoteFolder(machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.DstAgentRootPath);
                return true;
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                return false;
            }
        }

        public static bool CopyInstallScriptsToRemote(Machine machine)
        {
            if (FileHelper.IsExistsFolder(CISBuilderInstallerHelper.LocalInstallScriptRootPath))
            {
                try
                {
                    string message = string.Format("SSHWrapper.CopyDirectoryToRemote({0}, {1}, {2}, {3}, {4});", machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.LocalInstallScriptRootPath, CISBuilderInstallerHelper.DstAgentRootPath);
                    SSHWrapper.CopyDirectoryToRemote(machine.ExternalIP, machine.Administrator, machine.Password, CISBuilderInstallerHelper.LocalInstallScriptRootPath, CISBuilderInstallerHelper.DstAgentRootPath);
                    message = string.Format("Install scripts is coped to the Test Agent Machine {0}.", machine.Name);
                    SaberAgent.Log.logger.Info(message);
                    SaberAgent.RunningJob.AddJobProgressInformation(message);
                    return true;
                }
                catch (Exception e)
                {
                    string message = string.Format("Failed to dispatch the installation script of CIS to machine [{0}] of test environment [{1}]", machine.Name, CISBuilderInstallerHelper.environment.Name);
                    SaberAgent.RunningJob.AddJobProgressInformation(message);
                    SaberAgent.Log.logger.Error(message, e);
                    return false;
                }
            }
            else
            {
                string message = string.Format("Could not find the local build path {0}", CISBuilderInstallerHelper.LocalInstallScriptRootPath);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
                SaberAgent.Log.logger.Error(message);
                return false;
            }
        }

        public static bool InstallCISComponentsOnEnvironment()
        {
            try
            {
                if (CISBuilderInstallerHelper.job == null)
                {
                    SaberAgent.Log.logger.Error(string.Format("The job {0} for this environment is null", job.JobId));
                    return false;
                }
                else
                {
                    TestEnvironmentConfigHelper configHelper = new TestEnvironmentConfigHelper(environment.Config);
                    foreach (Machine m in configHelper.SUTConfiguration.Machines)
                    {
                        
                        if (DoesCISNeedInstallOnThisMachine(m))
                        {
                            if (string.IsNullOrEmpty(m.Administrator))
                            {
                                m.Administrator = config.SUTConfiguration.SUTDomainConfig.Adminstrator;
                                m.Password = config.SUTConfiguration.SUTDomainConfig.Password;
                            }
                            if (!CleanRemoteAgentFolder(m))
                            {
                                SaberAgent.Log.logger.Error(string.Format("Failed to clean remote agent folder on machine [{0}]", m.Name));
                                return false;
                            }
                            AuthenticateRemoteBuildServer();
                            if (!CopyBuildFileToRemote(m) || !CopyInstallScriptsToRemote(m))
                            {
                                SaberAgent.Log.logger.Error(string.Format("Failed to copy build file and installation scripts to machine [{0}]", m.Name));
                                return false;
                            }
                        }
                    }
                    foreach (Machine m in configHelper.TestAgentConfiguration.Machines)
                    {
                        if (m.Roles.FindAll(r => r.Key == Core.AgentType.RemoteAgent).Count() > 0)
                        {
                            if (string.IsNullOrEmpty(m.Administrator))
                            {
                                m.Administrator = configHelper.TestAgentConfiguration.TestAgentDomainConfig.Adminstrator;
                                m.Password = configHelper.TestAgentConfiguration.TestAgentDomainConfig.Password;
                            }
                            if (!CleanRemoteAgentFolder(m))
                            {
                                SaberAgent.Log.logger.Error(string.Format("Failed to clean remote agent folder on machine [{0}]", m.Name));
                                return false;
                            }
                            AuthenticateRemoteBuildServer();
                            if (!CopyBuildFileToRemote(m) || !CopyInstallScriptsToRemote(m))
                            {
                                SaberAgent.Log.logger.Error(string.Format("Failed to copy build file and installation scripts to machine [{0}]", m.Name));
                                return false;
                            }
                            //RunInstallScript(m);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                return false;
            }
        }

        public static bool CollectProductLogsAndCopyBackToAgent(string localResultRootPath)
        {
            try
            {
                TestEnvironmentConfigHelper configHelper = new TestEnvironmentConfigHelper(environment.Config);
                foreach (Machine machine in configHelper.SUTConfiguration.Machines)
                {
                    if (DoesCISNeedInstallOnThisMachine(machine))
                    {
                        string collectLogsCommand = "bash " + CISBuilderInstallerHelper.DstAgentRootPath + CISBuilderInstallerHelper.CollectLogsCommand;
                        SSHWrapper.RunCommand(machine.ExternalIP, configHelper.SUTConfiguration.SUTDomainConfig.Adminstrator, configHelper.SUTConfiguration.SUTDomainConfig.Password, collectLogsCommand);
                        //TODO, replace the logs to the according folders
                        SSHWrapper.CopyDirectoryFromRemoteToLocal(machine.ExternalIP, configHelper.SUTConfiguration.SUTDomainConfig.Adminstrator, configHelper.SUTConfiguration.SUTDomainConfig.Password, localResultRootPath, RemoteLogFolder);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                string message = string.Format("Failed to collect the logs");
                CISBuilderInstallerHelper.job.AddJobProgressInformation(message);
                SaberAgent.Log.logger.Error(ex);
                return false;
            }
        }

        public static bool DoesCISNeedInstallOnThisMachine(Machine m)
        {
            foreach (var role in m.Roles)
            {
                if (role.Key == CISRole.CIS && role.Value == false )//Need to install CIS on this machine 
                {
                    return true;
                }
            }
            return false;
        }

        public static bool RunInstallScript(Machine machine)
        {
            //try
            //{
            //    string installCommand = "bash " + CISBuilderInstallerHelper.DstAgentRootPath + CISBuilderInstallerHelper.InstallCommand;
            //    SSHWrapper.RunCommand(machine.ExternalIP, CISBuilderInstallerHelper.sutDomainAdmin, CISBuilderInstallerHelper.sutDomainAdminPassword, installCommand);
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    string message = string.Format("Failed to run install command on {0}", machine.Name);
            //    CISBuilderInstallerHelper.job.AddJobProgressInformation(message);
            //    SaberAgent.Log.logger.Error(message + e.Message);
            //    return false;
            //}
            return false;
        }
    }
}
