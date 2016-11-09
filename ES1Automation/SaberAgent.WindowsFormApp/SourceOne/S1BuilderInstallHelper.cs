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

namespace SaberAgent.WindowsFormApp
{
    /// <summary>
    /// this is the list of all the possible S1 roles.
    /// 1. We'll use this list to check the roles of a Machine installed or to install
    /// 2. The list will be used by Saber too, Saber will parse the environment config file to get the environment information.
    /// 3. When change any of this values(Not recommeded), please update the according values in Saber and the Support Environment config
    /// </summary>
    public class S1ComponentRole
    {
        public const string Worker = "Worker";
        public const string Master = "Master";
        public const string DomainController = "DomainController";
        public const string NativeArchive = "NativeArchive";
        public const string Console = "Console";
        public const string WebService = "WebService";
        public const string Mobile = "Mobile";
        public const string DMServer = "DMServer";
        public const string DMWeb = "DMWeb";
        public const string DMClient = "DMClient";
        public const string SQLServer = "SQLServer";
        public const string Search = "Search";
        public const string ExchangeServer = "ExchangeServer";
        public const string SharePointServer = "SharePointServer";
        public const string SharePointBCE = "SharePointBCE";
        public const string FileArchiveBCE = "FileArchiveBCE";
        public const string Supervisor = "Supervisor";
    }
    /// <summary>
    /// This the files genereated when the components are installed successfully.
    /// They are generated in the silent batch. 
    /// So here please do not modify them and update them when you modified the bat files.
    /// </summary>
    public class S1ComponentRoleInstalledIndicatorFileName
    {
        public const string INS_DB_INSTALLED_INDICATOR = "DB_Installed_Successfully.txt";
        public const string INS_NA_INSTALLED_INDICATOR = "NA_Installed_Successfully.txt";
        public const string INS_WORKER_INSTALLED_INDICATOR = "Worker_Installed_Successfully.txt";
        public const string INS_MASTER_INSTALLED_INDICATOR = "Master_Installed_Successfully.txt";
        public const string INS_WEBSERVICE_INSTALLED_INDICATOR = "WebService_Installed_Successfully.txt";
        public const string INS_SEARCH_INSTALLED_INDICATOR = "Search_Installed_Successfully.txt";
        public const string INS_CONSOLE_INSTALLED_INDICATOR = "Console_Installed_Successfully.txt";
        public const string INS_MOBILE_INSTALLED_INDICATOR = "Mobile_Installed_Successfully.txt";
        public const string INS_DMSERVER_INSTALLED_INDICATOR = "DMServer_Installed_Successfully.txt";
        public const string INS_DMCLIENT_INSTALLED_INDICATOR = "DMClient_Installed_Successfully.txt";
        public const string INS_DMWEB_INSTALLED_INDICATOR = "DMWeb_Installed_Successfully.txt";
        public const string INS_SUPERVISOR_INSTALLED_INDICATOR = "Supervisor_Installed_Successfully.txt";
    }

    public class S1ComponentInstallLogFileName
    {
        public const string Worker = "EMC_Worker_Install.log";
        public const string Master = "EMC_Master_Install.log";
        public const string NativeArchive = "EMC_Archive_Install.log";
        public const string Console = "EMC_Console_Install.log";
        public const string WebService = "EMC_WebSvcs_Install.log";
        public const string Mobile = "EMC_Mobile_Install.log";
        public const string DMServer = "EMC_DMServer_Install.log";
        public const string DMWeb = "EMC_DMWeb_Install.log";
        public const string DMClient = "EMC_DMClient_Install.log";
        public const string SQLServer = "EMC_Database_Install.log";
        public const string DMDatabase = "EMC_DMDB_Install.log";
        public const string Search = "EMC_Search_Install.log";
        public const string SharePointBCE = "EMC_SPBCE_Install.log";
        public const string FileArchiveBCE = "EMC_FABCE_Install.log";
        public const string Supervisor = "EMC_Supervisor_Install.log";
    }

    public static class S1BuilderInstallHelper
    {
        private static TestEnvironment environment = null;
        private static TestEnvironmentConfigHelper config = null;
        private static string domainName = string.Empty;
        private static string domainAdmin = string.Empty;
        private static string domainAdminPassword = string.Empty;

        private static string buildPathOnTestAgent = @"C:\SaberAgent\Installations";
        private static string s1InstallBatchPath = @"C:\SaberAgent\ES1_SilentBatch\install.bat";
        private static string s1InstallBatchFolder = @"C:\SaberAgent\ES1_SilentBatch";
        private static string ES1CDRoomRaletivePath = @"Install\ES1_CDRom";
        private static string SupvisorCDRoomRaletivePath = @"Install\CDRom";
        private static string DISCOCDRoomRaletivePath = @"Install\Disco_CDRom";
        
        private static string S1MSICodeMappingFilePath = @"C:\SaberAgent\Installations\ES1_CDRom\Setup\MSICode.txt";
        private static string MSICodePattern = ".MSICode=";
        private static List<KeyValuePair<string, string>> MSICodeMappings = new List<KeyValuePair<string,string>>();

        private static string buildServerUser = ConfigurationManager.AppSettings["BuildServerUser"];
        private static string buildServerPassword = ConfigurationManager.AppSettings["BuildServerPassword"];
        private const int installTimeOut = 1000 * 60 * 30;
        private const int rebootTimeOut = 1000 * 60 * 30;

        public static void Initialize(TestEnvironment environment)
        {
            S1BuilderInstallHelper.environment = environment;
            S1BuilderInstallHelper.config = new TestEnvironmentConfigHelper(environment.Config);
            S1BuilderInstallHelper.domainName = config.DomainConfiguration.Name;
            S1BuilderInstallHelper.domainAdmin = config.DomainConfiguration.Adminstrator;
            S1BuilderInstallHelper.domainAdminPassword = config.DomainConfiguration.Password;
        }


        /// <summary>
        /// Make sure the machine is ready before install S1 components
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool CheckThePreconditionForS1ComponentsInstallation(int timeout = S1BuilderInstallHelper.rebootTimeOut)
        {
            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                SaberAgent.Log.logger.Info("The environment is existing and assumed as ready.");
                return true;
            }

            int count = config.SUTConfiguration.Machines.Count;
            SaberAgent.Log.logger.Info("Wait untill machines are accessable before the installation of S1 components.");
            Task[] tasks = new Task[count];
            int i = 0;
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                Machine temp = m;
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    MakeSureMachineIsReadyAndTheSupporttedServicesAreRunningForS1(temp);
                });
                i++;
            }
            try
            {
                if (Task.WaitAll(tasks, rebootTimeOut))
                {
                    SaberAgent.Log.logger.Info("All the machines are accessable and necessary services are running for S1.");
                    return true;
                }
                else
                {
                    SaberAgent.Log.logger.Error("Not all the machines are accessable and necessary services are running for S1. Waiting timeout.");
                    return false;
                }
            }
            catch (AggregateException e)
            {
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    SaberAgent.Log.logger.Error(e.InnerExceptions[j]);
                }
                return false;
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Initialize the mapping list of the S1 roles and the MSI code.
        /// We'll check whether the S1 component is installed successfully by checking whether the MSI code is registered in system. 
        /// </summary>
        private static void InitialzeMSICodeMappings()
        {
            if (System.IO.File.Exists(S1MSICodeMappingFilePath))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(S1MSICodeMappingFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.Contains(MSICodePattern))
                        {
                            string s1Role1 = line.Substring(0, line.IndexOf(MSICodePattern)).Replace("Upgrade", "");
                            string s1Role = string.Empty;
                            switch (s1Role1)
                            {
                                case "Master":
                                    s1Role = S1ComponentRole.Master;
                                    break;
                                case "Worker":
                                    s1Role = S1ComponentRole.Worker;
                                    break;
                                case "Console":
                                    s1Role = s1Role1;
                                    break;
                                case "Archive":
                                    s1Role = S1ComponentRole.NativeArchive;
                                    break;
                                case "WebServices":
                                    s1Role = S1ComponentRole.WebService;
                                    break;
                                case "Search":
                                    s1Role = S1ComponentRole.Search;
                                    break;
                                case "Mobile":
                                    s1Role = S1ComponentRole.Mobile;
                                    break;
                                case "SharePointBCE":
                                    s1Role = S1ComponentRole.SharePointBCE;
                                    break;
                                case "FileArchiveBCE":
                                    s1Role = S1ComponentRole.FileArchiveBCE;
                                    break;
                                case "DiscoveryManagerServer":
                                    s1Role = S1ComponentRole.DMServer;
                                    break;
                                default://for others, there's no msi code for us to check whether the components are installed or not
                                    s1Role = s1Role1;
                                    break;
                            }
                            string msiCode = line.Substring(line.IndexOf(MSICodePattern) + MSICodePattern.Length);
                            S1BuilderInstallHelper.MSICodeMappings.Add(new KeyValuePair<string, string>(s1Role, msiCode));
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Install the S1 Components on the Test Environment
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>true if success, else false</returns>
        public static bool InstallS1OnSUTEnvironment()
        {
            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                SaberAgent.Log.logger.Info("The environment is existing and assumed as ready.");
                return true;
            }

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
                    SaberAgent.Log.logger.Info(string.Format("Start to get the build for the job {0}", job.Name));
                    Build build = GetBuildForTheJob(job);
                    SaberAgent.Log.logger.Info(string.Format("The build for the job is {0}", build.Name));
                    SaberAgent.Log.logger.Info(string.Format("Start to authenticate the remote server."));
                    AuthenticateRemoteServer(build);
                    SaberAgent.Log.logger.Info("Start to copy the build to saber agent.");
                    CopyBuildToTestAgentMachine(build);
                    SaberAgent.Log.logger.Info("Start to dispatch the installation between hosts based on the roles to be installed.");
                    DispatchInstallScriptWithConfiguration();
                    SaberAgent.Log.logger.Info("Start to run the installation scripts on the remote machines concurrently and wait all finished.");
                    RunInstallScriptsOnRemoteMachinesConcurrentlyAndWaitAllFinish();
                    SaberAgent.Log.logger.Info("SourceOne has been installed on the environment successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                throw ex;
            }
        }


        /// <summary>
        /// Reboot the machines with S1 components installed
        /// 1. Firstly reboot other machines
        /// 2. Secondly, update the environment status to Ready
        /// 3. At last, reboot the current machine.
        /// </summary>
        public static void RestartAllMachinesWithS1ComponentInstalled()
        {

            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                SaberAgent.Log.logger.Info("The environment is existing and assumed as ready.");
                return;
            }

            SaberAgent.Log.logger.Info(string.Format("Start to restart all the machines with S1 Components installed for environment {0}", environment.EnvironmentId));
            Machine currentHostMachine = null;
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                if (IsAnyS1ComponentInstalledOnMachineThisTime(m))
                {
                    if (IsTheMachineCurrentHost(m))
                    {
                        currentHostMachine = m;
                    }
                    else
                    {
                        SaberAgent.Log.logger.Info(string.Format("Reboot remote machine {0}", m.Name));
                        RebootRemoteMachine(m);
                    }
                }
            }
            if (null != currentHostMachine && IsAnyS1ComponentInstalledOnMachineThisTime(currentHostMachine))
            {
                SaberAgent.Log.logger.Info(string.Format("Reboot current host {0} at last.", currentHostMachine.Name));
                RebootCurrentMachine(currentHostMachine);
            }
        }

        /// <summary>
        /// Check and start the S1 service if it's not started.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool MakeSureAllS1ServicesAreStarted(int timeout = S1BuilderInstallHelper.rebootTimeOut)
        {
            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                SaberAgent.Log.logger.Info("The environment is existing and assumed as ready.");
                return true;
            }

            int count = config.SUTConfiguration.Machines.Count;
            SaberAgent.Log.logger.Info("Check and start all s1 components services.");
            Task[] tasks = new Task[count];
            int i = 0;
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                Machine temp = m;

                tasks[i] = Task.Factory.StartNew(() =>
                {

                    MakeSureS1ComponentsServicesAreStarted(temp);
                    //SaberAgent.RunningJob.AddJobProgressInformation(string.Format("Services in Machine [{0}] are started.", temp.Name));

                });

                i++;
            }

            try
            {
                if (Task.WaitAll(tasks, rebootTimeOut))
                {
                    SaberAgent.Log.logger.Info("All the S1 services are running.");
                    return true;
                }
                else
                {
                    SaberAgent.Log.logger.Info("Not all the S1 services are running. Waiting timeout.");
                    return false;
                }
            }
            catch (AggregateException e)
            {
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    SaberAgent.Log.logger.Error(e.InnerExceptions[j]);
                }
                return false;
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Wait untill the S1 component host machine are rebooted and all the services needed for S1 are ready(S1 services, DC services, Exchange services and so on.)
        /// </summary>
        /// <returns>true if all rebooted within timeout, else false</returns>
        public static bool WaitMachinesRebootedAfterS1ComponentsInstallation(int timeout = S1BuilderInstallHelper.rebootTimeOut)
        {
            int count = config.SUTConfiguration.Machines.Count;

            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                SaberAgent.Log.logger.Info("The environment is existing and assumed as ready.");
                return true;
            }

            SaberAgent.Log.logger.Info("Wait all the machines are rebooted and accessable after installation s1 components.");
            Task[] tasks = new Task[count];
            int i = 0;
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                Machine temp = m;
                if (IsAnyS1ComponentInstalledOnMachineThisTime(temp))
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {

                        WaitUntillRebootedOfMachineWithS1ComponentInstalled(temp);
                        MakeSureMachineIsReadyAndTheSupporttedServicesAreRunningForS1(temp);
                        SaberAgent.RunningJob.AddJobProgressInformation(string.Format("Machine [{0}] is rebooted and ready.", temp.Name));

                    });
                }
                else
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        MakeSureMachineIsReadyAndTheSupporttedServicesAreRunningForS1(temp);

                    });
                }
                i++;
            }
            // Exceptions thrown by tasks will be propagated to the main thread
            // while it waits for the tasks. The actual exceptions will be wrapped in AggregateException
            try
            {
                if (Task.WaitAll(tasks, rebootTimeOut))
                {
                    SaberAgent.Log.logger.Info("All the machines are rebooted and accessable after installation s1 components.");
                    return true;
                }
                else
                {
                    SaberAgent.Log.logger.Info("Not all the machines are rebooted and accessable after installation s1 components. Waiting timeout.");
                    return false;
                }
            }
            catch (AggregateException e)
            {
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    SaberAgent.Log.logger.Error(e.InnerExceptions[j]);
                }
                return false;
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e);
                return false;
            }
        }



        #region private functions

        private static bool IsTheMachineCurrentHost(Machine m)
        {
            string hostMachineName = Environment.MachineName;
            return (hostMachineName.ToLower() == m.Name.ToLower());
        }

        private static void RebootRemoteMachine(Machine m)
        {
            int timeout = 10;//10 second
            NetUseHelper.NetUserMachine(m.IP, domainName + @"\" + domainAdmin, domainAdminPassword);
            string cmd = string.Format(@"shutdown /m \\{0} /t {1} /r /f", m.IP, timeout);
            string temp = System.IO.Path.GetTempFileName() + ".bat";
            TXTHelper.ClearTXTContent(temp);
            TXTHelper.WriteNewLine(temp, cmd, Encoding.Default);
            CMDScript.RumCmd(temp, string.Empty);
            FileHelper.DeleteFile(temp);
            SaberAgent.RunningJob.AddJobProgressInformation(string.Format("Machine [{0}] is rebooted.", m.Name));
        }

        private static void RebootCurrentMachine(Machine m)
        {
            int timeout = 10;//1 minute
            string cmd = string.Format(@"shutdown /t {0} /r /f", timeout);
            string temp = System.IO.Path.GetTempFileName() + ".bat";
            TXTHelper.ClearTXTContent(temp);
            TXTHelper.WriteNewLine(temp, cmd, Encoding.Default);
            CMDScript.RumCmd(temp, string.Empty);
            FileHelper.DeleteFile(temp);
            SaberAgent.RunningJob.AddJobProgressInformation(string.Format("Machine [{0}] is rebooted.", m.Name));
        }

        /// <summary>
        /// start a service remotely
        /// </summary>
        /// <param name="m"></param>
        /// <param name="service"></param>
        /// <param name="timeout"></param>
        private static void TryToStartServiceRemotely(Machine m, string service, int timeout)
        {
            if (!WindowsServices.IsServiceRunningRemotely(service, m.IP))
            {
                SaberAgent.Log.logger.Warn(string.Format("The service [{0}] is not started on machine [{1}]", service, m.Name));
                try
                {
                    WindowsServices.StartServiceRemotely(service, m.IP, timeout);
                }
                catch (Exception ex)
                {
                    SaberAgent.Log.logger.Warn(ex);
                }
            }
            if (WindowsServices.IsServiceRunningRemotely(service, m.IP))
            {
                SaberAgent.Log.logger.Info(string.Format("The service [{0}] is running on machine [{1}]", service, m.Name));
            }
            else
            {
                string msg = string.Format("The service [{0}] can not be started on machine [{1}]", service, m.Name);
                SaberAgent.Log.logger.Error(msg);
                throw new Exception(msg);
            }
        }

        // Make sure the S1 service is started.
        private static void MakeSureS1ComponentsServicesAreStarted(Machine m, int timeout = S1BuilderInstallHelper.rebootTimeOut)
        {
            SaberAgent.Log.logger.Info(string.Format("Make sure the SouceOne services on machine [{0}] is running.", m.Name));

            foreach (KeyValuePair<string, bool> role in m.Roles)
            {
                if (role.Key == S1ComponentRole.NativeArchive)
                {
                    //check all the services for NA are started and running
                    string[] naServices = { "ExAsAdmin", "ExAsArchive", "ExAsIndex", "ExAsQuery" };
                    foreach (string service in naServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }

                if (role.Key == S1ComponentRole.Master)
                {
                    string[] masterServices = { "ExAddressCacheService", "ExJobScheduler" /*,"ES1MoverAgent"*/ };
                    foreach (string service in masterServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }

                if (role.Key == S1ComponentRole.Worker)
                {
                    string[] workerServices = { "ES1AddressResolutionService", "ExJobDispatcher" };
                    foreach (string service in workerServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }

                if (role.Key == S1ComponentRole.WebService)
                {
                    string[] wsServices = { "ExDocMgmtSvc", "ExSearchService", "ExDocMgmtSvcOA" };
                    foreach (string service in wsServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }

                if (role.Key == S1ComponentRole.DMServer)
                {
                    string[] dmServices = { "DCWorkerService" };
                    foreach (string service in dmServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }

                if (role.Key == S1ComponentRole.Supervisor)
                {
                    string[] supervisorRelatedServices = { "ExmABSyncSvc", "EXMCmdMsgSvc", "EXMEmailCacheSvc", "EXMFilterSvc", "EXMIndexerSvc", "EXMSamplingSvc", "EXMSignalSvc", "GenPerfSvc", "Generic Trace" };
                    foreach (string service in supervisorRelatedServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }
            }
        }

        // wait all the machine is pingable by ip and machine name, and the Exchagne/SQLServer/Exchange/AD works well
        private static void MakeSureMachineIsReadyAndTheSupporttedServicesAreRunningForS1(Machine m, int timeout = S1BuilderInstallHelper.installTimeOut)
        {
            SaberAgent.Log.logger.Info(string.Format("Make sure machine [{0}] is accessable and necessary services are running.", m.Name));
            if (PingHelper.WaitUntillPingable(m.IP, 60 * 5) && PingHelper.WaitUntillPingable(m.Name, 60 * 5))
            {
                SaberAgent.Log.logger.Info(string.Format("Machine [{0}] is accessable.", m.Name));
            }
            else
            {
                throw new Exception("The machine [{0}] can not be pinged within 5 minutes.");
            }

            foreach (KeyValuePair<string, bool> role in m.Roles)
            {
                if (role.Key == S1ComponentRole.SQLServer)
                {
                    string[] sqlRelatedServices = { "MSSQLSERVER" };
                    foreach (string service in sqlRelatedServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }

                    //if (SQLServer.IsSQLServerConnected(m.Name, S1BuilderInstallHelper.domainAdmin, S1BuilderInstallHelper.domainAdminPassword))
                    //{
                    //    SaberAgent.Log.logger.Info("SQLServer can be connected.");
                    //}
                    //else
                    //{
                    //    SaberAgent.Log.logger.Error("SQLServer can not be connected.");
                    //    throw new Exception("SQL Server can not be connected.");
                    //}
                }
                if (role.Key == S1ComponentRole.DomainController)
                {
                    string[] domainControllerRelatedServices = { "kdc", "IsmServ", "DNS", "DFSR", "NTDS" };
                    foreach (string service in domainControllerRelatedServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }


                    SaberAgent.Log.logger.Info("Start to verify the AD service by authenticating the domain administrator.");
                    int i = 0;
                    while (!ADUser.Authenticate(config.SUTConfiguration.SUTDomainConfig.Adminstrator, config.SUTConfiguration.SUTDomainConfig.Password, config.SUTConfiguration.SUTDomainConfig.Name)
                        && i < 40)//wait 20 minutes
                    {
                        SaberAgent.Log.logger.Info("Can not authenticate the Domain Administrator("
                           + config.SUTConfiguration.SUTDomainConfig.Name
                           + @"\" + config.SUTConfiguration.SUTDomainConfig.Adminstrator
                           + " " + config.SUTConfiguration.SUTDomainConfig.Password
                           + ") through the AD servie. Will retry after 30 seconds.");

                        System.Threading.Thread.Sleep(1000 * 30);
                        i++;
                    }
                    if (i >= 40)
                    {
                        string msg = "After waiting 20 minutes, we still can not authenticate the domain admin through the AD service.";
                        SaberAgent.Log.logger.Error(msg);
                        throw new Exception(msg);
                    }
                    else
                    {
                        SaberAgent.Log.logger.Info("The AD service works well and we can authenticate the domain admin throught it.");
                    }
                }
                if (role.Key == S1ComponentRole.ExchangeServer)
                {
                    string[] exchangeRelatedServices = { "MSExchangeADTopology", "MSExchangeAB", "MSExchangeServiceHost", "MSExchangeRPC", "MSExchangeIS" };
                    foreach (string service in exchangeRelatedServices)
                    {
                        TryToStartServiceRemotely(m, service, timeout);
                    }
                }
            }
        
        }

        private static void WaitUntillRebootedOfMachineWithS1ComponentInstalled(Machine m, int timeout = S1BuilderInstallHelper.rebootTimeOut)
        {
            if (!IsAnyS1ComponentInstalledOnMachineThisTime(m))//no new s1 component insatlled on the machine, no need to be restarted.
            {
                return;
            }
            else
            {
                while (true)
                {
                    if (PingHelper.IsPingable(m.IP))
                    {
                        NetUseHelper.NetUserMachine(m.IP, domainName + @"\" + domainAdmin, domainAdminPassword);
                        string indicatorFileName = System.IO.Path.Combine(string.Format(@"\\{0}\C$\SaberAgent\ES1_SilentBatch", m.IP), m.Name + "_Restarted.txt");
                        if (System.IO.File.Exists(indicatorFileName))
                        {
                            return;
                        }
                    }
                    System.Threading.Thread.Sleep(1000 * 10);
                }
            }
        }

        private static bool IsTheRoleOneOfSourceOneComponents(string role)
        {
            switch (role)
            {
                case S1ComponentRole.Console:
                case S1ComponentRole.DMClient:
                case S1ComponentRole.DMServer:
                case S1ComponentRole.DMWeb:
                case S1ComponentRole.FileArchiveBCE:
                case S1ComponentRole.Master:
                case S1ComponentRole.Mobile:
                case S1ComponentRole.NativeArchive:
                case S1ComponentRole.Search:
                case S1ComponentRole.SharePointBCE:
                case S1ComponentRole.WebService:
                case S1ComponentRole.Worker:
                case S1ComponentRole.Supervisor:
                    //case S1ComponentRole.SQLServer:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsAnyS1ComponentInstalledOnMachineThisTime(Machine m)
        {
            bool ret = false;
            foreach (KeyValuePair<string, bool> role in m.Roles)
            {
                if (IsTheRoleOneOfSourceOneComponents(role.Key))
                {
                    if (role.Value)//role has been installed in the template already?
                    {

                    }
                    else//The comoponent is installed this time.
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        private static void DispatchInstallScriptWithConfiguration()
        {
            try
            {
                SaberAgent.Log.logger.Info(string.Format("Start to update the configuration for S1 installation, such as which components to be installed on each machine"));
                foreach (Machine m in config.SUTConfiguration.Machines)
                {
                    NetUseHelper.NetUserMachine(m.IP, domainName + @"\" + domainAdmin, domainAdminPassword);
                    //FileHelper.CopyDirectory(@"C:\SaberAgent\ES1_SilentBatch", string.Format(@"\\{0}\C$\SaberAgent\ES1_SilentBatch", m.IP));
                    string remoteDefaultValuesFilePath = string.Format(@"\\{0}\C$\SaberAgent\ES1_SilentBatch\DefaultValues.bat", m.IP);
                    //write the domain info
                    TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_DOMAIN={0}", domainName), Encoding.Default);
                    //here we have the assumption that the db group are marked as "QAES1\ES1 Security Group" and "QAES1\ES1 Admins Group"
                    TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_DB_DOMAIN={0}", domainName.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper()), Encoding.Default);
                    TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_USER={0}", domainAdmin), Encoding.Default);
                    TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_PASS={0}", domainAdminPassword), Encoding.Default);
                    ConfigCommonSettingsOnMachine(remoteDefaultValuesFilePath);
                    ConfigWhichComponentToBeInstallOnMachine(m, remoteDefaultValuesFilePath);
                    GenerateTheIndicatorFileForAlreadyInstallComponents(m);
                }
                SaberAgent.Log.logger.Info(string.Format("Configurations for installation are all updated."));
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to dispatch the installatio script of S1 to test environment {0}", environment.EnvironmentId);
                SaberAgent.Log.logger.Error(message + ex.Message);
                throw new Exception(message);
            }
        }

        //Generate the indicator to tell the install batch that the role(S1 components) have been installed
        private static void GenerateTheIndicatorFileForAlreadyInstallComponents(Machine m)
        {
            foreach (KeyValuePair<string, bool> role in m.Roles)
            {
                if (role.Value == true)//already installed in template
                {
                    switch (role.Key)
                    {
                        case S1ComponentRole.Console:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_CONSOLE_INSTALLED_INDICATOR), "Console has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.DMClient:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_DMCLIENT_INSTALLED_INDICATOR), "DMClient has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.DMServer:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_DMSERVER_INSTALLED_INDICATOR), "DMServer has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.DMWeb:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_DMWEB_INSTALLED_INDICATOR), "DMWeb has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.DomainController:
                        case S1ComponentRole.ExchangeServer:
                        case S1ComponentRole.FileArchiveBCE:
                        case S1ComponentRole.SharePointBCE:
                        case S1ComponentRole.SharePointServer:
                            break;
                        case S1ComponentRole.Master:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_MASTER_INSTALLED_INDICATOR), "Master has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.Mobile:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_MOBILE_INSTALLED_INDICATOR), "Mobile has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.NativeArchive:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_NA_INSTALLED_INDICATOR), "Native Archive has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.Search:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_SEARCH_INSTALLED_INDICATOR), "Search has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.SQLServer:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_DB_INSTALLED_INDICATOR), "Database has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.WebService:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_WEBSERVICE_INSTALLED_INDICATOR), "Web Service has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.Worker:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_WORKER_INSTALLED_INDICATOR), "Worker has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        case S1ComponentRole.Supervisor:
                            TXTHelper.WriteNewLine(System.IO.Path.Combine(s1InstallBatchFolder, S1ComponentRoleInstalledIndicatorFileName.INS_SUPERVISOR_INSTALLED_INDICATOR), "Supervisor has been installed in template.", System.Text.Encoding.UTF8);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void ConfigWhichComponentToBeInstallOnMachine(Machine m, string remoteDefaultValuesFilePath)
        {
            //write the roles to install
            foreach (KeyValuePair<string, bool> role in m.Roles)
            {
                if (role.Value == false)//the role has not been installed yet
                {
                    switch (role.Key)
                    {
                        case S1ComponentRole.SQLServer:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_DB=y", Encoding.Default);
                            break;
                        case S1ComponentRole.NativeArchive:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_NA=y", Encoding.Default);
                            break;
                        case S1ComponentRole.Worker:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_WORKER=y", Encoding.Default);
                            break;
                        case S1ComponentRole.Console:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_CONSOLE=y", Encoding.Default);
                            break;
                        case S1ComponentRole.WebService:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_WEBSERVICE=y", Encoding.Default);
                            break;
                        case S1ComponentRole.Search:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_SEARCH=y", Encoding.Default);
                            break;
                        case S1ComponentRole.Master:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_MASTER=y", Encoding.Default);
                            break;
                        case S1ComponentRole.Mobile:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_MOBILE=y", Encoding.Default);
                            break;
                        case S1ComponentRole.DMClient:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_DMCLIENT=y", Encoding.Default);
                            break;
                        case S1ComponentRole.DMServer:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_DMSERVER=y", Encoding.Default);
                            break;
                        case S1ComponentRole.DMWeb:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET INS_DMWEB=y", Encoding.Default);
                            break;
                        case S1ComponentRole.FileArchiveBCE:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET DEFAULT_FABCE_INSTALL=y", Encoding.Default);
                            break;
                        case S1ComponentRole.SharePointBCE:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET DEFAULT_SPBCE_INSTALL=y", Encoding.Default);
                            break;
                        case S1ComponentRole.ExchangeServer:
                            break;
                        case S1ComponentRole.DomainController:
                            break;
                        case S1ComponentRole.Supervisor:
                            TXTHelper.WriteNewLine(remoteDefaultValuesFilePath, "SET DEFAULT_SUPERVISOR_INSTALL=y", Encoding.Default);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //Do nothing
                }
            }
        }

        private static void ConfigCommonSettingsOnMachine(string remoteDefaultValuesFilePath)
        {
            //go through all the roles to determine which server is SQL/DC/Mail/NA etc.
            string sqlHost = string.Empty;
            string naHost = string.Empty;
            string dcHost = string.Empty;
            string workHost = string.Empty;
            string mailHost = string.Empty;
            string masterHost = string.Empty;
            string supervisorHost = string.Empty;

            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                foreach (KeyValuePair<string, bool> role in m.Roles)
                {
                    switch (role.Key)
                    {
                        case S1ComponentRole.SQLServer:
                            sqlHost = m.Name;
                            break;
                        case S1ComponentRole.NativeArchive:
                            naHost = m.Name;
                            break;
                        case S1ComponentRole.Worker:
                            workHost = m.Name;
                            break;
                        case S1ComponentRole.Console:
                            break;
                        case S1ComponentRole.WebService:
                            break;
                        case S1ComponentRole.Search:
                            break;
                        case S1ComponentRole.Master:
                            masterHost = m.Name;
                            break;
                        case S1ComponentRole.Mobile:
                            break;
                        case S1ComponentRole.DMClient:
                        case S1ComponentRole.DMServer:
                            break;
                        case S1ComponentRole.FileArchiveBCE:
                            break;
                        case S1ComponentRole.SharePointBCE:
                            break;
                        case S1ComponentRole.ExchangeServer:
                            mailHost = m.Name;
                            break;
                        case S1ComponentRole.DomainController:
                            dcHost = m.Name;
                            break;
                        case S1ComponentRole.Supervisor:
                            supervisorHost = m.Name;
                            break;
                    }
                }
            }
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_SQL={0}", sqlHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_MAIL={0}", mailHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_DC={0}", dcHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_MASTER={0}", masterHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_WORKER={0}", workHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_NATIVEARCHIVE={0}", naHost), Encoding.Default);
            TXTHelper.WriterNewLineAtBeginning(remoteDefaultValuesFilePath, string.Format("SET DEFAULT_EX_SERVER_SUPERVISOR={0}", supervisorHost), Encoding.Default);
        }

        private static void RunInstallScriptsOnRemoteMachinesConcurrentlyAndWaitAllFinish()
        {
            SaberAgent.Log.logger.Info(string.Format("Start to invoke the installation in all machines."));

            var cts = new CancellationTokenSource();

            InitialzeMSICodeMappings();

            int count = config.SUTConfiguration.Machines.Count;

            Task[] tasks = new Task[count];

            for (int i = 0; i < count; i++)
            {
                Machine m = config.SUTConfiguration.Machines[i];
                tasks[i] = Task.Factory.StartNew(() => { 
                    RunInstallScriptOnRemoteMachine(m, cts.Token);
                    CheckWhetherTheS1ComponentsAreInstalledSuccessfully(m);
                });
            }

            SaberAgent.Log.logger.Info(string.Format("The installations on all machines are started, wait all to be finished."));
            try
            {
                if (Task.WaitAll(tasks, installTimeOut))
                {
                    SaberAgent.Log.logger.Info(string.Format("The installation on all the machines are finished!"));
                }
                else
                {
                    cts.Cancel();
                    string message = string.Format("Not all the installations on all the machines are finished within {0} miliseconds", installTimeOut);
                    SaberAgent.Log.logger.Error(message);
                    throw new Exception(message);
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    SaberAgent.Log.logger.Error(string.Format("Not all the components are installed successfully in all machines."));
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

        private static void CheckWhetherTheS1ComponentsAreInstalledSuccessfully(Machine m)
        {
            string message = string.Empty;
            if (!System.IO.File.Exists(S1MSICodeMappingFilePath))
            {
                message = string.Format("There's no MSI mapping file at [{0}], skip the validation on machine {1}.", S1MSICodeMappingFilePath, m.Name);
                SaberAgent.Log.logger.Info(message);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
                return;
            }
            else
            {
                foreach (KeyValuePair<string, bool> role in m.Roles)
                {
                    List<KeyValuePair<string, string>> mappings = MSICodeMappings.FindAll(map => map.Key == role.Key);
                    if (mappings.Count > 0)
                    {
                        string registerKeyName = mappings[0].Value;
                        RegistryKey registerKey = null;
                        if (IsTheMachineCurrentHost(m))
                        {
                            registerKey = RegistryHelper.GetLocalMSICodeRegistryKey(registerKeyName);
                        }
                        else
                        {
                            registerKey = RegistryHelper.GetRemoteMSICodeRegistryKey(m.Name, registerKeyName);
                        }

                        if (registerKey != null)
                        {
                            message = string.Format("The validation of {0} installation on machine {1} succeeded.", role.Key, m.Name);
                            SaberAgent.Log.logger.Info(message);
                            SaberAgent.RunningJob.AddJobProgressInformation(message);
                            continue;
                        }
                        else
                        {
                            message = string.Format("The validation of {0} installation on machine {1} failed.", role.Key, m.Name);
                            SaberAgent.RunningJob.AddJobProgressInformation(message);
                            throw new Exception(message);
                        }
                    }
                    else//not all the s1 components has the MSI code defined in the mapping file
                    {
                        SaberAgent.Log.logger.Info(string.Format("Could not find the MSI code in mapping file for component {0} of SourceOne, skip the validation on machine {1}.", role.Key, m.Name));
                        SaberAgent.RunningJob.AddJobProgressInformation(message);
                        continue;
                    }
                }
            }
        }

        private static bool InstallWindowsTaskToGenerateRebootedIndicator(Machine machine)
        {
            bool ret = false;
            try
            {
                string taskConfigXMLTemplate = @"C:\SaberAgent\ES1_SilentBatch\RestartedChecker.xml";
                string taskConfigTempXMLFilePath = System.IO.Path.GetTempFileName() + "_" + machine.Name + "_Task_Config.xml";
                CreateTaskConfigFromTemplate(taskConfigXMLTemplate, taskConfigTempXMLFilePath);
                string taskName = "Saber Restart Checker";

                string tempCMDFile = System.IO.Path.GetTempFileName() + "_set_schedule_task_temp.bat";
                TXTHelper.ClearTXTContent(tempCMDFile);
                string cmd = string.Empty;
                if (!IsTheMachineCurrentHost(machine))//run remotely
                {
                    SaberAgent.Log.logger.Info("Create the task remotely on machine: " + machine.Name);
                    cmd = string.Format(@"schtasks /Create /S {0} /U {1}\{2} /P {3} /XML ""{4}"" /TN ""{5}"" /RU {6}\{7} /RP {8}",
                       machine.IP, domainName, domainAdmin, domainAdminPassword, taskConfigTempXMLFilePath, taskName, domainName, domainAdmin, domainAdminPassword);
                }
                else//run locally
                {
                    SaberAgent.Log.logger.Info("Create the task locally on machine: " + machine.Name);
                    cmd = string.Format(@"schtasks /Create /XML ""{0}"" /TN ""{1}"" /RU {2}\{3} /RP {4}",
                                     taskConfigTempXMLFilePath, taskName, domainName, domainAdmin, domainAdminPassword);
                }

                TXTHelper.WriteNewLine(tempCMDFile, cmd, Encoding.Default);
                string msg = CMDScript.RumCmd(tempCMDFile, string.Empty);
                if (msg.Contains("SUCCESS"))
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                    SaberAgent.Log.logger.Error(msg);
                }
                FileHelper.DeleteFile(tempCMDFile);
                FileHelper.DeleteFile(taskConfigTempXMLFilePath);
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
                ret = false;
            }
            return ret;
        }

        private static void RunInstallScriptOnRemoteMachine(Machine machine, CancellationToken token)
        {
            string message = string.Empty;
            //add the scheduled windows task to write the file into the shared folder when the machine is rebooted.
            //the bat to run in the task are specified in the XML file.
            int i = 0;
            while (i < 30 && !InstallWindowsTaskToGenerateRebootedIndicator(machine))//waitting 30 minutes at most
            {
                System.Threading.Thread.Sleep(60 * 1000);
                i++;
            }

            if (i == 30)
            {
                message = string.Format("The task to generate the rebooted indicator failed to be set on machine {0} after 20 tries", machine.Name);
                SaberAgent.Log.logger.Error(message);
                throw new Exception(message);
            }
            else
            {
                SaberAgent.Log.logger.Info(string.Format("The task to generate the rebooted indicator is set on machine {0}", machine.Name));
                //call the install script
                message = string.Format("The installation on machine {0} is started.", machine.Name);
                SaberAgent.Log.logger.Info(message);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
                SaberAgent.Log.logger.Info(string.Format("Start the installation using psExec on remote machine {0}, user:{1}, password:{2}, install script {3}", machine.IP, domainName + @"\" + domainAdmin, domainAdminPassword, s1InstallBatchPath));
                message = CMDScript.PsExec(machine.IP, domainName + @"\" + domainAdmin, domainAdminPassword, s1InstallBatchPath);
                SaberAgent.Log.logger.Info(message);
                message = string.Format("The installation on machine {0} is done.", machine.Name);
                SaberAgent.Log.logger.Info(message);
                SaberAgent.RunningJob.AddJobProgressInformation(message);
            }

            if (token.IsCancellationRequested)
            {
                message = string.Format("The installation of S1 component on machine {0} is cancelled. ", machine.Name);
                message += string.Format("This mainly caused by the time out of one S1 component installtion on one machine.");
                SaberAgent.Log.logger.Error(message);
                throw new Exception(message);
            }
        }

        private static string CreateTaskConfigFromTemplate(string templateFile, string configFilePath)
        {
            try
            {
                string author = domainName + @"\" + domainAdmin;
                string userId = domainName + @"\" + domainAdmin;
                TXTHelper.ClearTXTContent(configFilePath);
                string content = TXTHelper.GetTXT(templateFile);
                content = content.Replace("USER_ID_TO_BE_REPLACED", userId);
                content = content.Replace("AUTHOR_TO_BE_REPLACED", author);
                TXTHelper.WriteNewLine(configFilePath, content, Encoding.Default);
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }
            return configFilePath;
        }

        private static Build GetBuildForTheJob(AutomationJob job)
        {
            AutomationTask task = JobManagement.GetAutomationTaskOfJob(job);
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

        private static void CopyBuildToTestAgentMachine(Build build)
        {
            string message = string.Format("Start to copy build [{0}] to local test agent machine..", build.Name);
            SaberAgent.Log.logger.Info(message);
            SaberAgent.RunningJob.AddJobProgressInformation(message);
            FileHelper.EmptyFolder(buildPathOnTestAgent);
            string buildRootPath = build.Location;
            Product product = Product.GetProductByID(build.ProductId);
            if (product.Name == "SourceOne")//hard coded, TODO to change it configurable?
            {
                string cdRoomPath = System.IO.Path.Combine(buildRootPath, ES1CDRoomRaletivePath);
                SaberAgent.Log.logger.Info(string.Format("Start to copy build installation files from {0} to {1}", cdRoomPath, buildPathOnTestAgent));
                FileHelper.CopyDirectoryWithParentFolder(cdRoomPath, buildPathOnTestAgent);
                cdRoomPath = System.IO.Path.Combine(buildRootPath, DISCOCDRoomRaletivePath);
                SaberAgent.Log.logger.Info(string.Format("Start to copy build installation files from {0} to {1}", cdRoomPath, buildPathOnTestAgent));
                FileHelper.CopyDirectoryWithParentFolder(cdRoomPath, buildPathOnTestAgent);
            }
            else if (product.Name == "Supervisor Web")//hard coded, TODO to change it configurable?
            {
                string cdRoomPath = System.IO.Path.Combine(buildRootPath, SupvisorCDRoomRaletivePath);
                SaberAgent.Log.logger.Info(string.Format("Start to copy build installation files from {0} to {1}", cdRoomPath, buildPathOnTestAgent));
                FileHelper.CopyDirectoryWithParentFolder(cdRoomPath, buildPathOnTestAgent);
            }
            message = string.Format("Build is coped to the Test Agent Machine.");
            SaberAgent.Log.logger.Info(message);
            SaberAgent.RunningJob.AddJobProgressInformation(message);
        }

        private static void AuthenticateRemoteServer(Build build)
        {
            SaberAgent.Log.logger.Info(string.Format("Net use the remote build server before copy any build from it."));

            string remoteServer = string.Empty;
            string buildLocation = build.Location;
            remoteServer = buildLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];

            SaberAgent.Log.logger.Info(string.Format("Remote Server is {0}", remoteServer));
            SaberAgent.Log.logger.Info(string.Format("Net use machine {0} as user {1} with paswword {2}", remoteServer, buildServerUser, buildServerPassword));
            NetUseHelper.NetUserMachine(remoteServer, buildServerUser, buildServerPassword);
            SaberAgent.Log.logger.Info(string.Format("The remote server {0} is authenticated.", remoteServer));
        }

        #endregion
    }
}
