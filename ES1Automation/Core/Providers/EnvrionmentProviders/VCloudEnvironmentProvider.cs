using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using com.vmware.vcloud.sdk;
using Core.Model;
using ES1Common.Exceptions;
using ES1Common.Virtualization;
using Common.Windows;

namespace Core.Providers.EnvrionmentProviders
{
    public class VCloudEnvironmentProvider : IEnvironmentProvider
    {
        protected static string ModuleName;

        protected VCloud VCloud { get; private set; }

        protected string Organization { get; private set; }

        protected string VDC { get; private set; }

        protected int Timeout { get; private set; }

        public Provider Provider { get; set; }

        protected IList<string> GetAllVApps()
        {
            try
            {
                return VCloud.GetVappsInOrginazation(Organization);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Met error when get all vApps from vCloud.", ex);
                return new List<string>();
            }
        }

        protected void DeployVCloudEnvironmentAsync(TestEnvironment environment, int timeout)
        {
            System.Threading.Tasks.Task.Factory.StartNew
            (
                () =>
                {
                    try
                    {
                        var vApp = VCloud.GetVappByName(Organization, VDC, environment.Name);

                        ATFEnvironment.Log.logger.Debug("start deploy vApp: " + environment.Name);
                        VCloud.DeployVapp(vApp, true, timeout, false);
                        ATFEnvironment.Log.logger.Debug("finish deploy vApp: " + environment.Name);
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = string.Format("Failed to Deploy vApp by template: {0}", environment.Description);
                        ATFEnvironment.Log.logger.Error(errorMsg, ex);
                        environment.SetEnvironmentStatus(EnvironmentStatus.Error);//here we set the environment to be error. Because this is a async operation, it's not reasonable to throw an exception.
                    }
                }
            );
        }

        protected void CreateVCloudEnvironmentAsync(TestEnvironment environment, int timeout)
        {
            environment.SetEnvironmentStatus(EnvironmentStatus.Setup);
            System.Threading.Tasks.Task.Factory.StartNew
            (
                () =>
                {
                    try
                    {
                        ATFEnvironment.Log.logger.Debug(string.Format("Creating Envrionment Id = {0}, Name= {1}", environment.EnvironmentId, environment.Name));
                        
                        ATFEnvironment.Log.logger.Info("Try to find vApp Template: " + environment.Description);
                        VappTemplate vAppTemplate = null;
                        vAppTemplate = VCloud.GetVappTemplateByName(Organization, VDC, environment.Description);

                        if (vAppTemplate == null)
                        {
                            string errorMsg = string.Format("vApp template: {0} not find", environment.Description);
                            environment.SetEnvironmentStatus(EnvironmentStatus.Error);
                            ATFEnvironment.Log.logger.Error(errorMsg);
                            throw new EnvironmentException(ModuleName, errorMsg);
                        }

                        ATFEnvironment.Log.logger.Debug("start create vApp: " + environment.Name);
                        var vApp = VCloud.CreateVappFromTemplate(Organization, VDC, environment.Description, environment.Name, timeout);
                        ATFEnvironment.Log.logger.Debug("finish create vApp: " + environment.Name);

                        ATFEnvironment.Log.logger.Debug("start deploy vApp: " + environment.Name);
                        VCloud.DeployVapp(vApp, true, timeout, false);
                        ATFEnvironment.Log.logger.Debug("start deploy vApp: " + environment.Name);
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = string.Format("Failed to create vApp by template: {0}", environment.Description);
                        ATFEnvironment.Log.logger.Error(errorMsg, ex);
                        environment.SetEnvironmentStatus(EnvironmentStatus.Error);//here we set the environment to be error. Because this is a async operation, it's not reasonable to throw an exception.
                    }
                }
            );
        }

        protected void DeleteVCloudEnvironmentAsync(TestEnvironment environment)
        {
            environment.SetEnvironmentStatus(EnvironmentStatus.Disposing);
            System.Threading.Tasks.Task.Factory.StartNew
            (
               () =>
               {
                   try
                   {
                       ATFEnvironment.Log.logger.Debug(string.Format("Disposing Envrionment Id = {0}, Name= {1}", environment.EnvironmentId, environment.Name));
                       
                       Vapp vApp = VCloud.GetVappByName(Organization, VDC, environment.Name);

                       if (vApp != null)
                       {
                           var status = VCloud.GetVAppStatus(vApp);

                           if (!(status == vAppStatus.Undeploy || status == vAppStatus.Error))
                           {
                               ATFEnvironment.Log.logger.Debug("start undeploy vApp: " + environment.Name);
                               VCloud.UndeployVapp(vApp);
                               ATFEnvironment.Log.logger.Debug("finish undeploy vApp: " + environment.Name);
                           }

                           ATFEnvironment.Log.logger.Debug("start delete vApp: " + environment.Name);
                           VCloud.DeleteVapp(vApp);
                           ATFEnvironment.Log.logger.Debug("finish delete vApp: " + environment.Name);
                       }
                   }
                   catch (Exception ex)
                   {
                       ATFEnvironment.Log.logger.Error(string.Format("Failed to dispose envrionment: {0}. Exception: {1}", environment.Name, ex.Message), ex);
                       environment.SetEnvironmentStatus(EnvironmentStatus.Discard);//here we set the environment to be discard. Because this is a async operation, it's not reasonable to throw an exception.
                   }
               }
           );
        }

        protected virtual double GetMemoryUsage()
        {
            Vdc vdc = null;
            try
            {
                vdc = VCloud.GetVdc(Organization, VDC);

                long total = VCloud.GetAllocatedMemory(vdc);
                long used = VCloud.GetUsedMemory(vdc);

                return (double)used / total;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to get the memory usage of the VDC {0}", vdc.Resource.name), ex);
                return (double)1;
            }
        }

        protected virtual double GetCPUUsage()
        {
            Vdc vdc = null;
            try
            {
                vdc = VCloud.GetVdc(Organization, VDC);

                long total = VCloud.GetAllocatedCPU(vdc);
                long used = VCloud.GetUsedCPU(vdc);

                return (double)used / total;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to get the CPU usage of the VDC {0}", vdc.Resource.name), ex);
                return (double)1;
            }
        }

        protected virtual bool IsAllowNewVappDeploy()
        {
            bool allowed = false;
            try
            {
                allowed = GetMemoryUsage() <= 0.85 && GetCPUUsage() <= 0.85;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
            return allowed;
        }

        public void ApplyConfig(string config)
        {
            XElement root = XElement.Parse(config);

            string vCloudUrl = root.Element("vCloudUrl").Value;
            string vCloudUsername = root.Element("username").Value;
            string vCloudPassword = root.Element("password").Value;

            Organization = root.Element("organization").Value;
            VDC = root.Element("vdc").Value;
            Timeout = int.Parse(root.Element("timeout").Value);

            ModuleName = string.Format("vCloud({0})", vCloudUrl);

            try
            {
                VCloud = new VCloud(vCloudUrl, string.Format("{0}@{1}", vCloudUsername, Organization), vCloudPassword);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Failed to init vCloud.", ex);
                throw new EnvironmentException(ModuleName, "Failed to init vCloud");
            }
        }

        public IList<string> GetAllEnvironment()
        {
            return GetAllVApps();
        }

        public virtual bool IsEnvironmentExist(string environmentName)
        {
            try
            {
                // find vApp template
                return VCloud.IsVappTemplateExist(Organization, VDC, environmentName);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Met error when check whether the environment is existing in vCloud.", ex);
                return false;
            }
        }

        public virtual void RequestEnvironment(TestEnvironment environment)
        {
            // create vApp in backgroud, timeout 30 minutes
            CreateVCloudEnvironmentAsync(environment, 1000 * 60 * 30);
        }

        /// <summary>
        /// Get test environment detail config
        /// </summary>
        /// <returns>
        /// A list of machines in the 
        /// Format of each item is a key-value pair.
        ///
        /// Example:
        ///  List item 1:
        ///     name = DC01
        ///     ip   = 192.168.8.1
        ///     role = DC
        ///     os   = windows server 2008R2
        ///  List item 2:
        ///     name = Exchange01
        ///     ip   = 192.168.8.2
        ///     role = exchange server
        ///     os   = windows server 2008R2
        /// 
        /// </returns>
        public virtual IList<IDictionary<string, string>> GetEnvironmentConfig(TestEnvironment environment)
        {
            var configs = new List<IDictionary<string, string>>();

            XElement config = XElement.Parse(environment.Config);

            foreach (var machineElement in config.Elements("machine"))
            {
                IDictionary<string, string> machineDic = new Dictionary<string, string>();

                foreach (var machineAttr in machineElement.Attributes())
                {
                    machineDic.Add(machineAttr.Name.ToString(), machineAttr.Value.ToString());
                }

                configs.Add(machineDic);
            }

            return configs;
        }

        /// <summary>
        /// Refresh environment status
        /// </summary>
        /// <param name="environment"></param>
        public virtual void RefreshEnvironmentStatus(TestEnvironment environment)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(environment.Config);
            EnvironmentType type = config.Type;
            EnvironmentDeploymentType deployType = EnvironmentDeploymentType.Undefined;
            vAppStatus status = vAppStatus.Unknow;

            switch (type)
            {
                case EnvironmentType.Residence_Together:
                    deployType = config.SUTConfiguration.DeploymentType;
                    break;
                case EnvironmentType.SUTAlone:
                    deployType = config.SUTConfiguration.DeploymentType;
                    break;
                case EnvironmentType.TestAgentAlone:
                    deployType = config.TestAgentConfiguration.DeploymentType;
                    break;
            }

            switch (environment.EnvironmentStatus)
            {
                case EnvironmentStatus.New:
                    if (deployType == EnvironmentDeploymentType.ToBeCreated)
                    {
                        try
                        {
                            if (IsAllowNewVappDeploy())
                            {
                                RequestEnvironment(environment);
                            }
                        }
                        catch (Exception ex)
                        {
                            ATFEnvironment.Log.logger.Error(ex);
                        }
                    }
                    else
                    {
                        environment.SetEnvironmentStatus(EnvironmentStatus.AgentServiceInstalledAndReady);
                    }
                    break;

                case EnvironmentStatus.Setup:
                    status = vAppStatus.Unknow;
                    try
                    {
                        status = VCloud.GetVAppStatus(Organization, VDC, environment.Name);
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error("Failed to get the status of vAPP.", ex);
                    }
                    switch (status)
                    {
                        case vAppStatus.Error:
                            environment.SetEnvironmentStatus(EnvironmentStatus.Error);
                            break;

                        case vAppStatus.Ready:
                            UpdateEnvironmentConfig(environment);
                            environment.SetEnvironmentStatus(EnvironmentStatus.MachinesReady);
                            break;

                        case vAppStatus.NotExist://Neil, sometimes the vAPP is not created yet, and request another environment with the same name will cause exception.
                            //if (IsAllowNewVappDeploy())
                            //{
                            //    RequestEnvironment(environment);
                            //}
                            break;

                        // redeploy
                        case vAppStatus.Unknow:
                        case vAppStatus.Undeploy://Neil, The deployment will be done when request the environment, here we did nothing. 

                            //if (IsAllowNewVappDeploy())
                            //{
                            //    DeployVCloudEnvironmentAsync(environment, 1000 * 60 * 30);
                            //}
                            break;
                    }

                    break;

                case EnvironmentStatus.MachinesReady:

                    break;

                case EnvironmentStatus.AgentServiceInstalledAndReady:

                    break;

                case EnvironmentStatus.BuildInstalled:

                    break;

                case EnvironmentStatus.Ready:

                    break;

                case EnvironmentStatus.Error:

                    break;

                case EnvironmentStatus.Discard:
                    DisposeEnvironment(environment);
                    break;

                case EnvironmentStatus.Disposing:
                    status = vAppStatus.Unknow;
                    try
                    {
                        status = VCloud.GetVAppStatus(Organization, VDC, environment.Name);
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error("Failed to get the status of vAPP.", ex);
                    }
                    switch (status)
                    {
                        case vAppStatus.NotExist:
                            environment.SetEnvironmentStatus(EnvironmentStatus.Disposed);
                            break;

                        case vAppStatus.Ready:
                            DisposeEnvironment(environment);
                            break;

                        case vAppStatus.Undeploy:
                            DisposeEnvironment(environment);
                            break;

                        case vAppStatus.Error:
                            DisposeEnvironment(environment);
                            break;
                    }

                    break;
            }
        }

        public virtual void UpdateEnvironmentConfig(TestEnvironment environment)
        {
            IList<VM> vms = null;
            try
            {
                vms = VCloud.GetVMsByVapp(Organization, VDC, environment.Name);
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to get the VMs of the vAPP {0}", environment.Name), ex);
            }

            if (vms == null)
            {
                ATFEnvironment.Log.logger.Error(string.Format("No vm is found in environment {0}", environment.Name));
                return;
            }

            string xmlConfig = environment.Config;
            EnvironmentType type = EnvironmentConfigHelper.GetResidenceType(xmlConfig);
            foreach (var vm in vms)
            {
                string ip = string.Empty;
                try
                {
                    ip = vm.GetIpAddressesById().Count > 0 ? vm.GetIpAddressesById().First().Value : string.Empty;
                }
                catch(Exception ex)
                {
                    ATFEnvironment.Log.logger.Error("Could not get the IP address of the VM.", ex);
                    environment.SetEnvironmentStatus(EnvironmentStatus.Error);
                    return;
                }
                if (ip != string.Empty)
                {
                    string externalIP = string.Empty;
                    xmlConfig = EnvironmentConfigHelper.SetMachineIP(xmlConfig, type, vm.Resource.name, ip);
                    try
                    {
                        externalIP = vm.GetNetworkConnections().Count > 0 ? vm.GetNetworkConnections()[0].ExternalIpAddress : string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ATFEnvironment.Log.logger.Error(string.Format("Failed to get the external IP of the VM {0}.", vm.Resource.name), ex);
                        environment.SetEnvironmentStatus(EnvironmentStatus.Error);
                        return;
                    }
                    xmlConfig = EnvironmentConfigHelper.SetMachineExternalIP(xmlConfig, type, vm.Resource.name, externalIP);
                }
                else
                {
                    string message = string.Format("Could not get the IP address for the machine {0}", vm.Resource.name);
                    ATFEnvironment.Log.logger.Error(message);
                    environment.SetEnvironmentStatus(EnvironmentStatus.Error);
                    return;
                }
            }

            environment.SetEnvironmentConfig(xmlConfig);
        }

        public virtual bool IsAllEnvironmentReady()
        {
            return TestEnvironment.GetEnvironmentByProvider(this.Provider.ProviderId)
                   .Where(e => e.Status == (int)EnvironmentStatus.New || e.Status == (int)EnvironmentStatus.Setup)
                   .Any();
        }

        public virtual bool DisposeEnvironment(TestEnvironment environment)
        {
            DeleteVCloudEnvironmentAsync(environment);

            return true;
        }
    }
}
