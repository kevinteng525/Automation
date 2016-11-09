using System;
using System.Configuration;
using Core.Model;
using Core.Providers.EnvrionmentProviders;
using ES1Common.Exceptions;
using Common.Windows;
using Common.FileCommon;
using Common.ScriptCommon;
using Common.Network;
using System.Linq;
using System.Collections.Generic;

namespace Core.Management
{
    public class EnvironmentManager
    {
        /// <summary>
        /// Update the test environment based on their state.
        /// </summary>
        public void UpdateEnvironmentStatus()
        {
            var testEnvironments = TestEnvironment.GetEnvironmentInUsage();
            //update the environment status
            foreach (TestEnvironment environment in testEnvironments)
            {
                ATFEnvironment.Log.logger.Debug(string.Format("Update the machine states for TestEnvironment: Id = {0}, Name = {1}, status = {2}", environment.EnvironmentId, environment.Name, environment.Status));
                //below will mainly focus on the machines
                environment.RefreshEnvironmentStatus();
            }
            //Install the Test Saber Agent Windows Application on the Test Environment
            foreach (TestEnvironment environment in testEnvironments)
            {
                TestEnvironment temp = environment;
                if (temp.EnvironmentStatus == EnvironmentStatus.MachinesReady)
                {
                    HandleTestEnvironmentWithStatusMachinesReady(temp);
                }
                else if (temp.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalledAndReady)
                {
                    // here we do nothing, this kind of environment will be handled by the Saber Agent service.
                    // the environment status will be changed to BuildInstalled after SUT in installed
                }
                else if (temp.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalling)
                {
                    //wait the agent service to be installed on the environment, the environment status will be changed to AgentServiceInstalledAndReady at tge end
                    continue;
                }
                else if (temp.EnvironmentStatus == EnvironmentStatus.BuildInstalled)
                {
                    //Here we do nothing, the Saber Agent will dertermine whether to restart the machine or not
                }
                else if (temp.EnvironmentStatus == EnvironmentStatus.Ready)
                {
                    //The test case will be executed by the Saber agent when the machine is restarted.
                    //After all test cases is finished, the environment status should be changed to Discard to dispose
                }
            }
        }

        /// <summary>
        /// Handle the test environment with machines ready, work include:
        /// 1. Install the Saber Agent services on the machines and start the services on remote machine
        /// 2. Update the environment information for the TestAgent and SUT, then the TestAgent and SUT know the details of each other(mainly IPs here)
        /// 3. Specify what kinds of works the Saber Agent services will take after it restarted. such as 
        ///   1). to tell services hosted on test agent to install the S1 build
        ///   2). to tell the services hosted on test agent to run the test case for which job
        /// 4. After that, the test environment status is AgentServiceInstalling or AgentServiceInstalledAndReady
        /// Below actions are taken by other components
        /// 5. The saber agent service on the test agent will install the S1 build, before that it'll wait the SUT to be AgentServiceInstalledAndReady
        /// 6. The saber agent service will set the environment status to be BuildInstalled
        /// 7. Then the environment manager will restart all the machines in SUT
        /// 8. After restarted, the saber agent in test agent will check all the machines in SUT are started and start to run the test cases.
        /// </summary>
        /// <param name="environment"></param>
        public void HandleTestEnvironmentWithStatusMachinesReady(TestEnvironment environment)
        {
            EnvironmentType type = EnvironmentConfigHelper.GetResidenceType(environment.Config);
            if (type == EnvironmentType.Residence_Together)
            {
                //add the jobId into the configuration file to let the Test Agent know which job the test agent is for.
                TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
                config.TestAgentConfiguration.Categories.Add(string.Format("JobId={0}", EnvironmentManager.GetAutomationJobOfTestEnvironment(environment).JobId.ToString()));
                //config.TestAgentConfiguration.Categories.Add(string.Format("mode={0}", SaberAgentMode.InstallS1Build));
                environment.SetEnvironmentConfig(config.ToXML());
                environment.SetEnvironmentStatus(EnvironmentStatus.AgentServiceInstalling);
                try
                {
                    InstallTestAgentWindowsServiceOnEnvironmentAsynchronous(environment);
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Execption captured when install test agent windows service on environment asynchronously for environment {0}",environment.EnvironmentId), ex);
                }

                ATFEnvironment.Log.logger.Info(string.Format("Test Agents started to be installed on environment {0}", environment.Name));
                ATFEnvironment.Log.logger.Info(string.Format("Environment status changes from MachinesReady -> AgentServiceInstalling"));
            }
            else if (type == EnvironmentType.TestAgentAlone)
            {
                //to make sure in the test agent, we have the information about the SUT, 
                //we'll first check whether the SUT is MachinesReady, 
                //if yes, we'll copy the SUT config to the TestAgent config, then setup the Saber Agent
                //else, do nothing and wait another loop
                TestEnvironment sutEnvironment = EnvironmentManager.GetSUTEnvironmentOfTestAgentEnvironment(environment);
                if (null != sutEnvironment)
                {
                    //if the sut is machine ready or other status after machine ready
                    if (sutEnvironment.EnvironmentStatus == EnvironmentStatus.MachinesReady ||
                        sutEnvironment.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalling ||
                        sutEnvironment.EnvironmentStatus == EnvironmentStatus.AgentServiceInstalledAndReady ||
                        sutEnvironment.EnvironmentStatus == EnvironmentStatus.BuildInstalled ||
                        sutEnvironment.EnvironmentStatus == EnvironmentStatus.Ready
                    )
                    {
                        //update the SUT part of the configuration of the test agent configuration.
                        //then the test agent know the detail of the SUT.
                        TestEnvironmentConfigHelper sutConfig = new TestEnvironmentConfigHelper(sutEnvironment.Config);
                        TestEnvironmentConfigHelper testAgentConfig = new TestEnvironmentConfigHelper(environment.Config);
                        testAgentConfig.SUTConfiguration = sutConfig.SUTConfiguration;
                        environment.SetEnvironmentConfig(testAgentConfig.ToXML());

                        //add the jobId into the configuration file to let the Test Agent know which job the test agent is for.
                        TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
                        config.TestAgentConfiguration.Categories.Add("JobId=" + EnvironmentManager.GetAutomationJobOfTestEnvironment(environment).JobId.ToString());
                        environment.SetEnvironmentConfig(config.ToXML());

                        //Install the Saber Agent service into the environment
                        environment.SetEnvironmentStatus(EnvironmentStatus.AgentServiceInstalling);
                        InstallTestAgentWindowsServiceOnEnvironmentAsynchronous(environment);

                        ATFEnvironment.Log.logger.Info(string.Format("Start to install Saber Agent on environment {0}", environment.Name));
                        ATFEnvironment.Log.logger.Info(string.Format("Environment status changes from MachinesReady -> AgentServiceInstalling"));
                    }
                }
            }
            else if (type == EnvironmentType.SUTAlone)//TODO, do we need to install the build on the SUT? currently we do nothing and assume that the environment is ready
            {
                TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
                config.TestAgentConfiguration.Categories.Add(string.Format("JobId={0}", EnvironmentManager.GetAutomationJobOfTestEnvironment(environment).JobId.ToString()));
                environment.SetEnvironmentConfig(config.ToXML());
                
                ATFEnvironment.Log.logger.Info(string.Format("Test Agents started to be installed on environment {0}", environment.Name));
                //ATFEnvironment.Log.logger.Info(string.Format("Environment status changes from {0} -> AgentServiceInstalling", environment.EnvironmentStatus));
                environment.SetEnvironmentStatus(EnvironmentStatus.AgentServiceInstalling);
                InstallTestAgentWindowsServiceOnEnvironmentAsynchronous(environment);
                ATFEnvironment.Log.logger.Info(string.Format("Test Agents have been installed on environment {0}", environment.Name));
                
                
            }
        }

        /// <summary>
        /// Restart all the machines that with S1 component installed
        /// </summary>
        /// <param name="environment"></param>
        public void RestartAllMachinesWithS1ComponentInstalled(TestEnvironment environment)
        {
            string restartCMD = string.Format("shutdown /r");
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            string domainName = config.DomainConfiguration.Name;
            string domainAdmin = config.DomainConfiguration.Adminstrator;
            string domainAdminPassword = config.DomainConfiguration.Password;
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                CMDScript.PsExecCMD(m.IP, domainName + @"\" + domainAdmin, domainAdminPassword, restartCMD);
            }
        }

        /// <summary>
        /// Get the job which is the test environment for.
        /// </summary>
        /// <param name="environment">Test Environment</param>
        /// <returns></returns>
        public static AutomationJob GetAutomationJobOfTestEnvironment(TestEnvironment environment)
        {
            if (environment.EnvironmentStatus == EnvironmentStatus.New || environment.EnvironmentStatus == EnvironmentStatus.Setup)
            {
                return null;
            }
            else
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    AutomationJob job = (from j in context.AutomationJobs
                                         where (j.TestAgentEnvironmentId == environment.EnvironmentId || j.SUTEnvironmentId == environment.EnvironmentId) 
                                                && j.Status != (int)JobStatus.End//Currently one environment may be re-used by several jobs, we only return the active one
                                         select j).FirstOrDefault();
                    return job;
                }
            }
        }
       
        /// <summary>
        /// Request the test agent environment for the job
        /// </summary>
        /// <param name="job">job</param>
        /// <param name="provider">environment provider</param>
        public void RequestTestAgentEnvironmentForJob(AutomationJob job, IEnvironmentProvider provider)
        {           
            SupportedEnvironment supportEnvironment = job.GetSupportedEnv();
            AutomationTask task = JobManagement.GetAutomationTaskOfJob(job);
            string templateName = new TestEnvironmentConfigHelper(supportEnvironment.Config).TestAgentConfiguration.Name;
            string environmentName = string.Format("{0}_{1}_{2}_{3}", task.Name, job.JobId, "TestAgent", Guid.NewGuid());

            TestEnvironmentConfigHelper sutConfig = new TestEnvironmentConfigHelper(supportEnvironment.Config);

            // Get the avaliable permenent agent, typically we maintain a pool of machines act as the test agent, no need to deploy new vApps
            // If no available agents in the pool now, we'll create a new vApp from the template with name of templateName
            TestEnvironment availableReadyStaticAgent = TestEnvironment.GetAvalibleStaticTestAgent4SupportedEnvironment(supportEnvironment);
            EnvironmentDeploymentType deployType = new TestEnvironmentConfigHelper(supportEnvironment.Config).TestAgentConfiguration.DeploymentType;
            if (deployType == EnvironmentDeploymentType.Existing )
            {
                if (availableReadyStaticAgent != null)
                {
                    string info = string.Format("Find one avaliable permanent test agent [{0}:{1}] for job [{2}]", availableReadyStaticAgent.EnvironmentId, availableReadyStaticAgent.Name, job.Name);
                    availableReadyStaticAgent.SetEnvironmentStatus(EnvironmentStatus.MachinesReady);
                    ATFEnvironment.Log.logger.Info(info);
                    job.AddJobProgressInformation(info);
                    // set SUT information to test agent's config
                    // set test agent type to TestAgentAlone
                    /*
                    TestEnvironmentConfigHelper testAgentConfig = new TestEnvironmentConfigHelper(availableReadyStaticAgent.Config);
                    testAgentConfig.SUTConfiguration = sutConfig.SUTConfiguration;
                    testAgentConfig.Type = EnvironmentType.TestAgentAlone;
                    availableReadyStaticAgent.Config = testAgentConfig.ToXML();

                    info = string.Format("Change the permanent agent's status, AgentServiceInstalledAndReady -> Ocuppied");
                    ATFEnvironment.Log.logger.Info(info);
                    job.AddJobProgressInformation(info);
                    // Set this permanent agent to occuppied 
                    //availableReadyStaticAgent.Status = (int)EnvironmentStatus.Ocuppied;
                    //availableReadyStaticAgent.Update();
                
                    TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(availableReadyStaticAgent.Config);
                    //clear the finished job information
                    config.TestAgentConfiguration.Categories.Clear();
                    //add the jobId into the configuration file to let the Test Agent know which job the test agent is for.
                    config.TestAgentConfiguration.Categories.Add("JobId=" + job.JobId.ToString());
                    availableReadyStaticAgent.Config = config.ToXML();
                    availableReadyStaticAgent.Update();


                    //copy the config file to the test agent
                    List<Machine> testAgents = GetMachinesNeedTestAgentInstalledOn(config);
                    string ip = testAgents.Count() > 0 ? testAgents[0].ExternalIP : string.Empty;
                    string domain = config.TestAgentConfiguration.TestAgentDomainConfig.Name;
                    string administrator = config.TestAgentConfiguration.TestAgentDomainConfig.Adminstrator;
                    string password = config.TestAgentConfiguration.TestAgentDomainConfig.Password;

                    string targetPath = @"\\" + ip + @"\C$\SaberAgent";              
                    string targetEnvironmentConfigFolder = targetPath + @"\Config";
                    string targetEnvironmentConfigFile = targetEnvironmentConfigFolder + @"\Environment.xml";

                    if (!NetUseHelper.NetUserMachine(ip, domain + @"\" + administrator, password))
                    {
                        ATFEnvironment.Log.logger.Error(string.Format("Net use the machine [{0}] failed.", ip));
                    }
                  
                    Common.ScriptCommon.CMDScript.FlushDNSRemotely(ip, domain + @"\" + administrator, password);

                    if (!FileHelper.IsExistsFolder(targetEnvironmentConfigFolder))
                    {
                        FileHelper.CreateFolder(targetEnvironmentConfigFolder);
                    }

                    TXTHelper.ClearTXTContent(targetEnvironmentConfigFile);
                    TXTHelper.WriteNewLine(targetEnvironmentConfigFile, config.ToXML(), System.Text.Encoding.Default);
                    info = string.Format("Copy the file[{0}] to permanent agent", targetEnvironmentConfigFile);
                    ATFEnvironment.Log.logger.Info(info);
                    job.AddJobProgressInformation(info);
                    */
                     job.SetTestAgentEnvironment(availableReadyStaticAgent.EnvironmentId);
                }
                else
                {
                    string info = string.Format("There's no available test agents right now, please wait other tasks to free some environments.");
                    ATFEnvironment.Log.logger.Info(info);
                    job.AddJobProgressInformation(info);
                }
            }
            else
            {
                //create a new record in DB for Test Agent, and Galaxy will handle the environment later(install, config and so on)
                sutConfig.TestAgentConfiguration.DeploymentType = EnvironmentDeploymentType.ToBeCreated;
                sutConfig.Type = EnvironmentType.TestAgentAlone;
                string config = sutConfig.ToXML();
                try
                {
                    var testEnvironment = new TestEnvironment
                    {
                        ProviderId = provider.Provider.ProviderId,
                        Name = environmentName,
                        Type = provider.Provider.Name,
                        Status = (int)EnvironmentStatus.New,
                        CreateDate = DateTime.UtcNow,
                        ModifyDate = DateTime.UtcNow,
                        //Config = EnvironmentConfigHelper.SetResidenceType(supportEnvironment.Config, EnvironmentType.TestAgentAlone),
                        Config = config,
                        Description = templateName,
                    };

                    if (job.JobStatus == JobStatus.Cancelled || job.JobStatus == JobStatus.End)
                    {
                        testEnvironment.SetEnvironmentStatus(EnvironmentStatus.Discard);
                    }

                    TestEnvironment.Add(testEnvironment);

                    job.SetTestAgentEnvironment(testEnvironment.EnvironmentId);
                }
                catch (Exception ex)
                {
                    job.SetJobsStatus(JobStatus.Failed);
                    string info = string.Format("Failed to request Test Agent environment {0}, Exception: {1}", environmentName, ex.Message);
                    job.AddJobProgressInformation(info);
                    ATFEnvironment.Log.logger.Error(info, ex);
                }
            }

            
        }

        /// <summary>
        /// Request the sut environment for job
        /// </summary>
        /// <param name="job">job</param>
        /// <param name="provider">environment provider</param>
        public void RequestSUTEnvironmentForJob(AutomationJob job, IEnvironmentProvider provider)
        {
            SupportedEnvironment supportEnvironment = job.GetSupportedEnv();
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(supportEnvironment.Config);
            string templateName = config.SUTConfiguration.Name;

            string sutConfig = string.Empty;
            if (config.Type == EnvironmentType.Residence_Seperate)
            {
                sutConfig = EnvironmentConfigHelper.SetResidenceType(supportEnvironment.Config, EnvironmentType.SUTAlone);
            }
            else if (config.Type == EnvironmentType.Residence_Together)
            {
                sutConfig = EnvironmentConfigHelper.SetResidenceType(supportEnvironment.Config, EnvironmentType.Residence_Together);
            }
            AutomationTask task = JobManagement.GetAutomationTaskOfJob(job);

            if (config.SUTConfiguration.DeploymentType == EnvironmentDeploymentType.Existing)
            {
                //Note: the existing SUT environments are distinguished by it's name, two environments with same name are considered as the same one
                var sutEnvironment = TestEnvironment.GetAvalibleStaticSUT4SupportedEnvironment(supportEnvironment);
                if (sutEnvironment == null)//wait untill another environment is freed
                {
                    string info = string.Format("There's no SUT environment in the pool available now, please wait for other tasks to free any environment.");
                    ATFEnvironment.Log.logger.Info(info);
                    job.AddJobProgressInformation(info);
                }
                else//reuse the record
                {
                    string message = string.Format("Get an available SUT environment [{0}] for the job [{1}:{2}]", sutEnvironment.Name, job.JobId, job.Name);
                    job.AddJobProgressInformation(message);
                    message = string.Format("Change environment [{0}:{1}] status from {2} to {3}", sutEnvironment.EnvironmentId, sutEnvironment.Name, sutEnvironment.Status, "MachinesReady");
                    job.AddJobProgressInformation(message);
                    sutEnvironment.SetEnvironmentStatus(EnvironmentStatus.MachinesReady);
                    job.SetSUTEnvironment(sutEnvironment.EnvironmentId);
                }
            }
            else
            {
                string environmentName = string.Format("{0}_{1}_{2}_{3}", task.Name, job.JobId, "SUT", Guid.NewGuid());
                try
                {
                    var testEnvironment = new TestEnvironment
                    {
                        ProviderId = provider.Provider.ProviderId,
                        Name = environmentName,
                        Type = provider.Provider.Name,
                        CreateDate = DateTime.UtcNow,
                        ModifyDate = DateTime.UtcNow,
                        Config = sutConfig,
                        Description = templateName,
                    };
                    if (job.JobStatus == JobStatus.Cancelled || job.JobStatus == JobStatus.End)
                        testEnvironment.SetEnvironmentStatus(EnvironmentStatus.Discard);
                    TestEnvironment.Add(testEnvironment);
                    string message = string.Format("Environment [{0}:{1} is created for job [{2}:{3}]]", testEnvironment.EnvironmentId, testEnvironment.Name, job.JobId, job.Name);
                    job.AddJobProgressInformation(message);
                    job.SetSUTEnvironment(testEnvironment.EnvironmentId);
                }
                catch (Exception ex)
                {
                    string info = string.Format("Failed to assign {0}, Exception: {1}", environmentName, ex.Message);
                    job.SetJobsStatus(JobStatus.Failed);
                    job.AddJobProgressInformation(info);
                    ATFEnvironment.Log.logger.Error(info, ex);
                }
            }
        }

        /// <summary>
        /// Check whether the environment provider is able to deploy new environment for the new jobs
        /// 1. Currently it just return true, and new environment request record is inserted into DB
        /// 2. The environment manager service then check whether it's able to create new vAPP
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public static bool IsAcceptNewEnvironment(int providerId)
        {
            IEnvironmentProvider provider = Provider.GetProviderById(providerId).CreateProvider() as IEnvironmentProvider;

            if (provider != null)
            {
                return true;
            }
            else
            {
                throw new FrameworkException("EnvironmentMgr", string.Format("Provider id = {0} is found", providerId));
            }
        }


        #region private methods


        #region install the Saber Agent Windows Service

        //get the machine that needs to install the test agent on
        //There may be multiple machines on the configuration xml for test agent, such as there may be the remote Linux test agent which needs no agent to be installed on it
        private static List<Machine> GetMachinesNeedTestAgentInstalledOn(TestEnvironmentConfigHelper config)
        {
            List<Machine> machines = new List<Machine>();
            foreach (Machine m in config.TestAgentConfiguration.Machines)
            {
                var anySaberAgentRoles = m.Roles.Select(r => r.Key.ToLower() == AgentType.SaberAgent.ToString().ToLower());
                if (anySaberAgentRoles.Count() > 0)
                {
                    machines.Add(m);
                }
            }
            return machines;
        }

        private static void RemoteInstallTestAgentWindowsServiceConcurrentlyAndWaitToFinish(TestEnvironment environment)
        {
            string domain = string.Empty;
            string administrator = string.Empty;
            string password = string.Empty;
            string serviceBinaryName = "SaberAgent.WinService.exe";
            string serviceName = "SaberAgent";
            string serviceDisplayName = "Galaxy Saber Agent";
            string sourceHostName = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachine"];
            string sourceHostAdmin = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdmin"];
            string sourceHostAdminPassword = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdminPassword"];
            string sourceInstallPath = ConfigurationManager.AppSettings["SaberAgentInstallerPath"];
            string targetSharePath = @"\C$\SaberAgent";
            string targetIntallPath = @"C:\SaberAgent";
            string testAgentConfigFileName = "Environment.xml";
            string configFilePath = sourceInstallPath + @"\" + "Config";
            NetUseHelper.NetUserMachine(sourceHostName, sourceHostAdmin, sourceHostAdminPassword);

            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            EnvironmentType type = config.Type;
            if (type == EnvironmentType.Residence_Together)
            {
                domain = config.SUTConfiguration.SUTDomainConfig.Name;
                administrator = config.SUTConfiguration.SUTDomainConfig.Adminstrator;
                password = config.SUTConfiguration.SUTDomainConfig.Password;
                System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[config.SUTConfiguration.Machines.Count];
                int i = 0;
                foreach (Machine m in config.SUTConfiguration.Machines)
                {
                    Machine temp = m;//it's critical
                    tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                        () =>
                        {
                            RemoteInstallTestAgentWindowsServiceOnMachine(temp, targetSharePath, domain, administrator, password, environment.Config, testAgentConfigFileName, sourceInstallPath, targetIntallPath, serviceName, serviceBinaryName, serviceDisplayName);
                        });
                    i++;
                }
                System.Threading.Tasks.Task.WaitAll(tasks);
            }
            else if (type == EnvironmentType.SUTAlone)
            {
                //do nothing
            }
            else if (type == EnvironmentType.TestAgentAlone)
            {
                domain = config.TestAgentConfiguration.TestAgentDomainConfig.Name;
                administrator = config.TestAgentConfiguration.TestAgentDomainConfig.Adminstrator;
                password = config.TestAgentConfiguration.TestAgentDomainConfig.Password;
                System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[config.TestAgentConfiguration.Machines.Count];
                int i = 0;
                foreach (Machine m in config.TestAgentConfiguration.Machines)
                {
                    Machine temp = m;
                    tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                        () =>
                        {
                            RemoteInstallTestAgentWindowsServiceOnMachine(temp, targetSharePath, domain, administrator, password, environment.Config, testAgentConfigFileName, sourceInstallPath, targetIntallPath, serviceName, serviceBinaryName, serviceDisplayName);
                        });
                    i++;
                }
                System.Threading.Tasks.Task.WaitAll(tasks);
                ATFEnvironment.Log.logger.Info(string.Format("Saber Agents are installed on all the machines in Environment {0} successfully.", environment.Name));

            }
        }

        private static void RemoteInstallTestAgentWindowsServiceOnMachine(Machine m, string targetSharePath, string domain, string administrator, string password, string config, string testAgentConfigFileName, string sourceInstallPath, string targetIntallPath, string serviceName, string serviceBinaryName, string serviceDisplayName)
        {
            string ip = string.IsNullOrEmpty(m.ExternalIP) ? m.IP : m.ExternalIP;
            while (!PingHelper.IsPingable(ip))
            {
                ATFEnvironment.Log.logger.Error(string.Format("The machine [{0}:{1}] cannot be accessed by ping.", m.Name, m.IP));
                System.Threading.Thread.Sleep(1000 * 10);
            }
            string targetPath = @"\\" + ip + targetSharePath;

            if (!NetUseHelper.NetUserMachine(ip, domain + @"\" + administrator, password))
            {
                ATFEnvironment.Log.logger.Error(string.Format("Net use the machine [{0}:{1}] failed.", m.Name, m.IP));                
            }

            FileHelper.EmptyFolder(targetPath);
            FileHelper.CreateFolder(targetPath + @"\" + "Config");
            TXTHelper.ClearTXTContent(targetPath + @"\" + "Config" + @"\" + testAgentConfigFileName);
            TXTHelper.WriteNewLine(targetPath + @"\" + "Config" + @"\" + testAgentConfigFileName, config, System.Text.Encoding.Default);

            WindowsServices.RemoteInstallWinService(ip, domain + @"\" + administrator, password, serviceBinaryName, serviceName, sourceInstallPath, targetPath, targetIntallPath, serviceDisplayName);
            ATFEnvironment.Log.logger.Info(string.Format("Saber Agent is installed on machine {0} successfully.", ip));
        }

        #endregion

        #region Install the Saber Agent Windows Form Application

        private static bool RemoteInstallTestAgentWindowsFormAppConcurrentlyAndWaitToFinsh(TestEnvironment environment)
        {
            int timeoutMunites = 30;
            string domain = string.Empty;
            string administrator = string.Empty;
            string password = string.Empty;

            string sourceHostName = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachine"];
            string sourceHostAdmin = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdmin"];
            string sourceHostAdminPassword = ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdminPassword"];
            string sourceInstallPath = ConfigurationManager.AppSettings["SaberAgentInstallerPath"];

            string targetSharePath = @"\C$\SaberAgent";
            string targetIntallPath = @"C:\SaberAgent";
            string testAgentConfigFileName = "Environment.xml";
            string configFilePath = sourceInstallPath + @"\" + "Config";

            AutomationJob job = EnvironmentManager.GetAutomationJobOfTestEnvironment(environment);

            int i = 0;
            while (!NetUseHelper.NetUserMachine(sourceHostName, sourceHostAdmin, sourceHostAdminPassword) && i < 10)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Cannot net use the remote machine [{0}]", sourceHostName));
                System.Threading.Thread.Sleep(1000 * 10);
                i++;
            }
            if (i >= 10)
            {
                return false;
            }

            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            EnvironmentType type = config.Type;
            if (type == EnvironmentType.Residence_Together)//For Residence_Together, currently there're all Windows Machine
            {
                domain = config.SUTConfiguration.SUTDomainConfig.Name;
                administrator = config.SUTConfiguration.SUTDomainConfig.Adminstrator;
                password = config.SUTConfiguration.SUTDomainConfig.Password;
                System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[config.SUTConfiguration.Machines.Count];
                i = 0;
                foreach (Machine m in config.SUTConfiguration.Machines)//TODO, here seems only need to install on the test agent machine. Neil
                {
                    Machine temp = m;//it's critical
                    if (IsSaberAgentRequiredInThisMachine(temp))//
                    {
                        tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                            () =>
                            {
                                string message = string.Format("Start to install Saber Agent on SUT machine [{0}].", temp.Name);
                                job.AddJobProgressInformation(message);
                                bool ret = RemoteInstallTestAgentWindowsFormAppOnMachine(temp, targetSharePath, domain, administrator, password, environment.Config, testAgentConfigFileName, sourceInstallPath, targetIntallPath);
                                if (ret == true)
                                {
                                    message = string.Format("Saber Agent on Agent machine [{0}] installed.", temp.Name);
                                }
                                else
                                {
                                    message = string.Format("Saber Agent was failed to install on Agent machine [{0}].", temp.Name);
                                }
                                job.AddJobProgressInformation(message);
                            });

                    }
                    else
                    {
                        tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                            () =>
                            { }
                            );
                    }
                    i++;
                }

                try
                {
                    if (System.Threading.Tasks.Task.WaitAll(tasks, 1000 * 60 * timeoutMunites))
                    {
                        ATFEnvironment.Log.logger.Info(string.Format("The Saber Agent was installed on all the machine on environment [{0}] successfully.", environment.Name));
                        return true;
                    }
                    else
                    {
                        ATFEnvironment.Log.logger.Info(string.Format("It's timeout to install Saber Agent on all the machine on environment [{0}] within {1} minutes.", environment.Name, timeoutMunites));
                        return false;
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (Exception e in ex.InnerExceptions)
                    {
                        ATFEnvironment.Log.logger.Error(e);
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(ex);
                    return false;
                }
            }
            else if (type == EnvironmentType.SUTAlone)
            {
                //do nothing
                return true;
            }
            else if (type == EnvironmentType.TestAgentAlone)
            {
                domain = config.TestAgentConfiguration.TestAgentDomainConfig.Name;
                administrator = config.TestAgentConfiguration.TestAgentDomainConfig.Adminstrator;
                password = config.TestAgentConfiguration.TestAgentDomainConfig.Password;
                System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[config.TestAgentConfiguration.Machines.Count];
                i = 0;
                //Not all the test agents configured in the xml need the test agent, some agent is the remote agent on linux system
                foreach (Machine m in GetMachinesNeedTestAgentInstalledOn(config))
                {
                    Machine temp = m;
                    if (IsSaberAgentRequiredInThisMachine(temp))
                    {
                        tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                            () =>
                            {
                                string message = string.Format("Start to install Saber Agent on Agent machine [{0}].", temp.Name);
                                job.AddJobProgressInformation(message);
                                bool ret = RemoteInstallTestAgentWindowsFormAppOnMachine(temp, targetSharePath, domain, administrator, password, environment.Config, testAgentConfigFileName, sourceInstallPath, targetIntallPath);
                                if (ret == true)
                                {
                                    message = string.Format("Saber Agent on Agent machine [{0}] installed.", temp.Name);
                                }
                                else
                                {
                                    message = string.Format("Saber Agent was failed to install on Agent machine [{0}].", temp.Name);
                                }
                                job.AddJobProgressInformation(message);
                            });
                    }
                    else
                    {
                        tasks[i] = System.Threading.Tasks.Task.Factory.StartNew(
                                                  () => { });
                    }
                    i++;
                }
                try
                {
                    if (System.Threading.Tasks.Task.WaitAll(tasks, 1000*60*timeoutMunites))
                    {
                        ATFEnvironment.Log.logger.Info(string.Format("The Saber Agent was installed on all the machine on environment [{0}] successfully.", environment.Name));
                        return true;
                    }
                    else
                    {
                        ATFEnvironment.Log.logger.Info(string.Format("It's timeout to install Saber Agent on all the machine on environment [{0}] within {1} minutes.", environment.Name, timeoutMunites));
                        return false;
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (Exception e in ex.InnerExceptions)
                    {
                        ATFEnvironment.Log.logger.Error(e);
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsSaberAgentRequiredInThisMachine(Machine m)
        {
            bool hasSaberAgentRole = false;
            bool hasRemoteAgentRole = false;
            //Check wheter need to install the test agent on one of the machine. We'll check whether there's a role of "SaberAgent"/ without role "RemoteAgent" and "SaberAgent"
            foreach (var role in m.Roles)
            {
                if (role.Key == AgentType.SaberAgent && role.Value == false)//work as Saber Agent and the agent has not been installed
                {
                    hasSaberAgentRole = true;
                    break;
                }
                if (role.Key == AgentType.RemoteAgent)//The remote agent currently is remote Linux machine, we'll not install any agent here 
                {
                    hasRemoteAgentRole = true;
                    break;
                }
            }
            if (hasRemoteAgentRole)
            {
                return false;
            }
            else if (hasSaberAgentRole)
            {
                return true;
            }
            else//no SaberAgent Role or RemoteAgent role defined. In order to support existing config(No AgentType specified), we will install the Agent on it
            {
                return true;
            }
        }

        private static bool RemoteInstallTestAgentWindowsFormAppOnMachine(Machine machine, string targetSharePath, string domainName, string domainAdmin, string domainAdminPassword, string config, string testAgentConfigFileName, string sourceInstallPath, string targetIntallPath)
        {
            //make sure the target machine is ready.
            string ip = string.IsNullOrEmpty(machine.ExternalIP) ? machine.IP : machine.ExternalIP;
            int i = 0;
            while (!PingHelper.IsPingable(ip) && i<10)
            {
                ATFEnvironment.Log.logger.Info(string.Format("The machine [{0}:{1}] cannot be accessed by ping after {2} seconds.", machine.Name, ip, i * 30));
                System.Threading.Thread.Sleep(1000 * 30);
                i++;
            }

            if (i == 10)
            {
                ATFEnvironment.Log.logger.Error(string.Format("The machine [{0}:{1}] cannot be accessed by ping after 5 minutes.", machine.Name, machine.IP));
                return false;
            }

            if (!NetUseHelper.NetUserMachine(ip, domainName + @"\" + domainAdmin, domainAdminPassword))
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to net use the machine [{0}:{1}].", machine.Name, machine.IP));
                return false;
            }

            Common.ScriptCommon.CMDScript.FlushDNSRemotely(ip, domainName + @"\" + domainAdmin, domainAdminPassword);

            //Close the SaberAgent app if there's any running, else will failed to remove the existing folder
            Common.ScriptCommon.CMDScript.CloseRunningApplicationRemotely(ip, domainName + @"\" + domainAdmin, domainAdminPassword, Core.AgentName.WindowsFormApp);

            string targetPath = @"\\" + ip + targetSharePath;            

            FileHelper.EmptyFolder(targetPath);
            FileHelper.CreateFolder(targetPath + @"\" + "Config");
            TXTHelper.ClearTXTContent(targetPath + @"\" + "Config" + @"\" + testAgentConfigFileName);
            TXTHelper.WriteNewLine(targetPath + @"\" + "Config" + @"\" + testAgentConfigFileName, config, System.Text.Encoding.Default);

            FileHelper.CopyDirectory(sourceInstallPath, targetPath);

            string tempCMDFile = System.IO.Path.GetTempFileName() + "_set_schedule_task_temp.bat";
            string taskConfigTempXMLFilePath = System.IO.Path.GetTempFileName() + "_" + machine.Name + "_Task_Config.xml";

            bool ret = false;
            try
            {
                string taskName = "Saber Agent Windows Form App Starter";

                Common.Windows.ScheduledTask.DeleteWindowsScheduleTaskRemotely(taskName, ip, domainName + @"\" + domainAdmin, domainAdminPassword);

                string taskConfigXMLTemplate = Common.AssemblyCommon.AssemblyHelper.GetAssemblePath() + "/Documents/SaberAgentStarterTaskTemplate.xml";

                CreateTaskConfigFromTemplate(taskConfigXMLTemplate, taskConfigTempXMLFilePath, domainName, domainAdmin);
               
                ATFEnvironment.Log.logger.Info("Delete the task remotely on machine: " + machine.Name);
               
                TXTHelper.ClearTXTContent(tempCMDFile);

                string cmd = string.Empty;

                ATFEnvironment.Log.logger.Info("Create the task remotely on machine: " + machine.Name);
                //cmd = string.Format(@"schtasks /Create /S {0} /U {1}\{2} /P {3} /XML ""{4}"" /TN ""{5}"" /RU {6}\{7} /RP {8}",
                //   machine.ExternalIP, domainName, domainAdmin, domainAdminPassword, taskConfigTempXMLFilePath, taskName, domainName, domainAdmin, domainAdminPassword);
                //If the RU and RP switch specified, the task is created as "Run whether user is logged or not", which makes the execution invisible ( without Window )


                //Modified by Neil on 2015/04/03. It seems we do not need to specify the /U and /P, because we have do the net use before.
                cmd = string.Format(@"schtasks /Create /S {0} /U {1}\{2} /P {3} /XML ""{4}"" /TN ""{5}""",
                  machine.ExternalIP, domainName, domainAdmin, domainAdminPassword, taskConfigTempXMLFilePath, taskName);

                //cmd = string.Format(@"schtasks /Create /S {0} /XML ""{1}"" /TN ""{2}""",
                //  machine.ExternalIP, taskConfigTempXMLFilePath, taskName);

                TXTHelper.WriteNewLine(tempCMDFile, cmd, System.Text.Encoding.Default);

                //Modified by Neil on 2015/04/03. It seems we do not need to specify the /U and /P, because we have do the net use before.
                cmd = string.Format(@"schtasks /Run /S {0} /U {1}\{2} /P {3} /TN ""{4}""", machine.ExternalIP, domainName, domainAdmin, domainAdminPassword,  taskName);
                TXTHelper.WriteNewLine(tempCMDFile, cmd, System.Text.Encoding.Default);

                //cmd = string.Format(@"schtasks /Run /S {0} /TN ""{1}""", machine.ExternalIP, taskName);
                //TXTHelper.WriteNewLine(tempCMDFile, cmd, System.Text.Encoding.Default);
                i = 0;
                while (!ret && i < 10)//wait 5 minutes totally
                {
                    string msg = CMDScript.RumCmd(tempCMDFile, string.Empty);
                    if (msg.Contains("SUCCESS"))
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                        ATFEnvironment.Log.logger.Error(msg);
                    }
                    System.Threading.Thread.Sleep(1000 * 30);
                    i++;
                }

            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e.Message, e);
                ret = false;
            }
            finally
            {
                FileHelper.DeleteFile(tempCMDFile);
                FileHelper.DeleteFile(taskConfigTempXMLFilePath);
            }
            return ret;
        }


        private static string CreateTaskConfigFromTemplate(string templateFile, string configFilePath, string domainName, string domainAdmin)
        {
            try
            {
                string author = domainName + @"\" + domainAdmin;
                string userId = domainName + @"\" + domainAdmin;
                TXTHelper.ClearTXTContent(configFilePath);
                string content = TXTHelper.GetTXT(templateFile);
                content = content.Replace("USER_ID_TO_BE_REPLACED", userId);
                content = content.Replace("AUTHOR_TO_BE_REPLACED", author);
                TXTHelper.WriteNewLine(configFilePath, content, System.Text.Encoding.Default);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }
            return configFilePath;
        }

        #endregion


        /// <summary>
        /// Get the correspording SUT of the Test Agent environment for the same job
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        private static TestEnvironment GetSUTEnvironmentOfTestAgentEnvironment(TestEnvironment environment)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                AutomationJob job = GetAutomationJobOfTestEnvironment(environment);
                if (job.SUTEnvironmentId == null)
                {
                    return null;
                }
                else
                {
                    return (from e in context.TestEnvironments
                            where e.EnvironmentId == job.SUTEnvironmentId.Value
                            select e).FirstOrDefault();
                }

            }

        }

        /// <summary>
        /// Install the test agent windows service on the test agent
        /// </summary>
        private static void InstallTestAgentWindowsServiceOnEnvironmentAsynchronous(TestEnvironment testEnvironment)
        {
            System.Threading.Tasks.Task.Factory.StartNew
            (
                () =>
                {
                    AutomationJob job = GetAutomationJobOfTestEnvironment(testEnvironment);
                    try
                    {
                        job.AddJobProgressInformation(string.Format("Start to install the Saber Agent Service on environment [{0}] for Job [{1}]", testEnvironment.Name, job.Name));

                        //RemoteInstallTestAgentWindowsServiceConcurrentlyAndWaitToFinish(testEnvironment);
                        if (RemoteInstallTestAgentWindowsFormAppConcurrentlyAndWaitToFinsh(testEnvironment))
                        {
                            job.AddJobProgressInformation(string.Format("The Saber Agent service has been installed on environment [{0}] for Job [{1}]", testEnvironment.Name, job.Name));
                            testEnvironment.SetEnvironmentStatus(EnvironmentStatus.AgentServiceInstalledAndReady);
                        }
                        else
                        {
                            job.AddJobProgressInformation(string.Format("The Saber Agent service failed to be installed on environment [{0}] for Job [{1}]", testEnvironment.Name, job.Name));
                            job.SetJobsStatus(JobStatus.Failed);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO, exception handling, we may need to revert the record in DB -> reinstall the windows services.
                        ATFEnvironment.Log.logger.Error(string.Format("Error happened when install the Saber Agent service on machine [{0}]", testEnvironment.Name), ex);
                        job.SetJobsStatus(JobStatus.Failed);
                    }
                }
            );
        }

        
        #endregion

    }
}
