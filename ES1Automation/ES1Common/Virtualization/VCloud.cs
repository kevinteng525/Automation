using com.vmware.vcloud.sdk;
using com.vmware.vcloud.sdk.constants;
using com.vmware.vcloud.sdk.admin;
using com.vmware.vcloud.sdk.utility;
using com.vmware.vcloud.api.rest.schema;
using ES1Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ES1Common.Logs;

namespace ES1Common.Virtualization
{
    /// <summary>
    /// A warpper for vCloud API
    /// </summary>
    public class VCloud
    {
        private const string VCLOUD = "VCLOUD";
        private static System.Threading.Mutex mutex = new System.Threading.Mutex();
        private vCloudClient client;

        private AutomationLog Log = new AutomationLog("vCloudAppender");

        public VCloud(string vCloudUrl, string username, string password)
        {
            Log.logger.Debug("START: Init vCloud client");

            CreateVCloudClient(vCloudUrl, username, password);

            Log.logger.Debug("COMPLETE: Init vCloud client ");
        }

        #region vCloud client

        /// <summary>
        /// Accept any certificate...
        /// </summary>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Defined a fake certificate policy for accepting the certificate temporarily
        /// </summary>
        private static void FakeCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.
            Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
        }

        /// <summary>
        /// Create a vCloudClient instance
        /// </summary>
        /// <param name="vCloudUrl">the vCloud Director server URL.</param>
        /// <param name="username">the name of a user account (format:user@vcloud-organization)</param>
        /// <param name="password">the user's password</param>
        private void CreateVCloudClient(string vCloudUrl, string username, string password)
        {
            try
            {
                FakeCertificatePolicy();

                client = new vCloudClient(vCloudUrl, com.vmware.vcloud.sdk.constants.Version.V5_1);

                client.Login(username, password);
            }
            catch (TimeoutException e)
            {
                Log.logger.Error("login vCloud client timeout", e);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
            catch (VCloudException e)
            {
                Log.logger.Error(e.Message, e);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message, e);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        #endregion

        #region Organization

        /// <summary>
        /// Get all vCloud organizations' name list
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAllOrganizationsList()
        {
            try
            {
                return client.GetOrgRefsByName().Keys.ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get vCloud organization by name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public Organization GetOrgnizationByName(string organizationName)
        {
            try
            {
                ReferenceType orgRef = client.GetOrgRefByName(organizationName);
                return Organization.GetOrganizationByReference(client, orgRef);
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        #endregion

        #region VDC

        /// <summary>
        /// Get all vdc's name list by organization name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public IList<string> GetAllVdcByOrganization(string organizationName)
        {
            var org = GetOrgnizationByName(organizationName);

            if (org == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            try
            {
                return org.GetVdcRefsByName().Keys.ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get vdc by its orgnization name and vdc name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <returns></returns>
        public Vdc GetVdc(string organizationName, string vdcName)
        {

            var organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            try
            {
                return Vdc.GetVdcByReference(client, organization.GetVdcRefByName(vdcName));
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get allocated memory in vDC
        /// </summary>
        /// <param name="vdc"></param>
        /// <returns></returns>
        public long GetAllocatedMemory(Vdc vdc)
        {
            return vdc.Resource.ComputeCapacity.Memory.Allocated;
        }

        /// <summary>
        /// Get allocated memory in vDC
        /// </summary>
        /// <param name="vdc"></param>
        /// <returns></returns>
        public long GetUsedMemory(Vdc vdc)
        {
            return vdc.Resource.ComputeCapacity.Memory.Used;
        }

        /// <summary>
        /// Get allocated Cpu in vDC
        /// </summary>
        /// <param name="vdc"></param>
        /// <returns></returns>
        public long GetAllocatedCPU(Vdc vdc)
        {
            return vdc.Resource.ComputeCapacity.Cpu.Allocated;
        }

        /// <summary>
        /// Get allocated Cpu in vDC
        /// </summary>
        /// <param name="vdc"></param>
        /// <returns></returns>
        public long GetUsedCPU(Vdc vdc)
        {
            return vdc.Resource.ComputeCapacity.Cpu.Used;
        }

        #endregion

        #region Catalog

        /// <summary>
        /// Get catalogs in specific orgnization
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public IList<string> GetCatalogs(string organizationName)
        {
            Organization organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            try
            {
                return organization.GetCatalogRefs().Select(c => c.name).ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get cagalog by name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        public Catalog GetCatalogByName(string organizationName, string catalogName)
        {
            Organization organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            try
            {
                foreach (ReferenceType Ref in organization.GetCatalogRefs())
                {
                    if (Ref.name.Equals(catalogName))
                    {
                        return Catalog.GetCatalogByReference(client, Ref);
                    }
                }
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }

            return null;
        }

        #endregion

        #region Template

        /// <summary>
        /// Get all vApp templates' name list in specific vdc
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVappTemplatesInVdc(string organizationName, string vdcName)
        {
            Vdc vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return null;
            }

            try
            {
                return vdc.GetVappTemplateRefs().Select(t => t.name).ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get vApp template by name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vappTemplateName"></param>
        /// <returns></returns>
        public VappTemplate GetVappTemplateByName(string organizationName, string vdcName, string vappTemplateName)
        {
            Organization organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            Vdc vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return null;
            }

            try
            {
                foreach (ReferenceType vappTemplateRef in vdc.GetVappTemplateRefs())
                {
                    if (vappTemplateRef.name.Equals(vappTemplateName))
                    {
                        return GetVappTemplateByRef(vappTemplateRef);
                    }
                }
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }

            return null;
        }

        /// <summary>
        /// Is vApp template exist
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vappTemplateName"></param>
        /// <returns></returns>
        public bool IsVappTemplateExist(string organizationName, string vdcName, string vappTemplateName)
        {
            Organization organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return false;
            }

            Vdc vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return false;
            }

            try
            {
                foreach (ReferenceType vappTemplateRef in vdc.GetVappTemplateRefs())
                {
                    if (vappTemplateRef.name.Equals(vappTemplateName))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }

            return false;
        }

        /// <summary>
        /// Get vApp by vAppTemplateRef
        /// </summary>
        /// <param name="vAppTemplateRef"></param>
        /// <returns></returns>
        public VappTemplate GetVappTemplateByRef(ReferenceType vAppTemplateRef)
        {
            try
            {
                return VappTemplate.GetVappTemplateByReference(client, vAppTemplateRef);
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        #endregion

        #region vApp

        /// <summary>
        /// Get vApp by name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <returns></returns>
        public Vapp GetVappByName(string organizationName, string vdcName, string vAppName)
        {
            Vdc vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return null;
            }

            try
            {
                return Vapp.GetVappByReference(client, vdc.GetVappRefByName(vAppName));
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get all vApp name list in specific vdc
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVappsInVdc(Vdc vdc)
        {
            try
            {
                return vdc.GetVappRefsByName().Keys.ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get all vApp name list in specific vdc
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVappsInVdc(string organizationName, string vdcName)
        {
            var vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return null;
            }

            try
            {
                return vdc.GetVappRefsByName().Keys.ToList();
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Get vApps' name list in specific organization
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public IList<string> GetVappsInOrginazation(string organizationName)
        {
            Organization organization = GetOrgnizationByName(organizationName);

            if (organization == null)
            {
                Log.logger.Warn(string.Format("ORGANIZATION:{0} not found", organizationName));
                return null;
            }

            List<string> vapps = new List<string>();

            try
            {
                foreach (var vdc in GetAllVdcByOrganization(organizationName))
                {
                    vapps.AddRange(GetVappsInVdc(organizationName, vdc));
                }
            }
            catch (Exception e)
            {
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }

            return vapps;
        }

        /// <summary>
        /// Wait for all task in vApp finished
        /// </summary>
        /// <param name="vapp"></param>
        /// <param name="timeout"></param>
        public void WaitAppTaskFinish(Vapp vapp, int timeout)
        {
            foreach (var task in vapp.Tasks)
            {
                task.WaitForTask(timeout);
            }
        }

        /// <summary>
        /// Create vApp from template async
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppTempateName"></param>
        /// <param name="vAppName"></param>
        /// <returns></returns>
        public Vapp CreateVappFromTemplateeAsync(string organizationName, string vdcName, string vAppTempateName, string vAppName)
        {
            Log.logger.Debug(string.Format("create vApp: {0} from template {1}", vAppName, vAppTempateName));

            Vdc vdc = GetVdc(organizationName, vdcName);

            if (vdc == null)
            {
                Log.logger.Warn(string.Format("VDC:{0} not found", vdcName));
                return null;
            }

            VappTemplate vAppTemplate = GetVappTemplateByName(organizationName, vdcName, vAppTempateName);

            if (vAppTemplate == null)
            {
                Log.logger.Warn(string.Format("TEMPLATE:{0} not found", vAppTempateName));
                return null;
            }

            try
            {
                // create vApp
                InstantiationParamsType instantiationParams = new InstantiationParamsType();
                List<Section_Type> sections = new List<Section_Type>();
                instantiationParams.Items = sections.ToArray();

                // create the request body (InstantiateVAppTemplateParams)
                InstantiateVAppTemplateParamsType instVappTemplParams = new InstantiateVAppTemplateParamsType();
                instVappTemplParams.name = vAppName;
                instVappTemplParams.Source = vAppTemplate.Reference;
                instVappTemplParams.InstantiationParams = instantiationParams;
                mutex.WaitOne();
                Vapp app = vdc.InstantiateVappTemplate(instVappTemplParams);
                System.Threading.Thread.Sleep(1000 * 5);
                mutex.ReleaseMutex();
                return app;
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message + e.StackTrace + e.Data);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Create vApp from template
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppTempateName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Vapp CreateVappFromTemplate(string organizationName, string vdcName, string vAppTempateName, string vAppName, int timeout)
        {
            Vapp vApp = CreateVappFromTemplateeAsync(organizationName, vdcName, vAppTempateName, vAppName);

            WaitAppTaskFinish(vApp, timeout);

            return vApp;
        }

        /// <summary>
        /// Get vApp status
        /// </summary>
        /// <param name="vApp"></param>
        /// <returns></returns>
        public vAppStatus GetVAppStatus(Vapp vApp)
        {
            if (vApp == null)
            {
                return vAppStatus.NotExist;
            }

            // get vApp status by vCloud SDK
            var status = vApp.GetVappStatus().Value();

            if (vApp.IsDeployed())
            {
                if (status == VappStatus.POWERED_ON.Value())
                {
                    // todo: check vm is ready for use, more than just power on
                    return vAppStatus.Ready;
                }
                else if (status == VappStatus.FAILED_CREATION.Value())
                {
                    return vAppStatus.Error;
                }
            }
            else
            {
                if (status == VappStatus.FAILED_CREATION.Value())
                {
                    return vAppStatus.Error;
                }
                else if (status == VappStatus.MIXED.Value())
                {
                    return vAppStatus.Error;
                }
                else if (status == VappStatus.POWERED_OFF.Value())
                {
                    return vAppStatus.Undeploy;
                }
                else if (status == VappStatus.UNKNOWN.Value())
                {
                    return vAppStatus.Unknow;
                }
            }

            return vAppStatus.Unknow;
        }

        /// <summary>
        /// Get vApp status by its name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <returns></returns>
        public vAppStatus GetVAppStatus(string organizationName, string vdcName, string vAppName)
        {
            return GetVAppStatus(GetVappByName(organizationName, vdcName, vAppName));
        }

        #region Deploy

        /// <summary>
        /// Deploy vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="isPowerOn"></param>
        /// <param name="lease"></param>
        /// <param name="forceCustomization"></param>
        public void DeployVappAsync(Vapp vApp, bool isPowerOn, int lease, bool forceCustomization)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Deploy vApp: {0}", vApp.Reference.name));
                vApp.Deploy(isPowerOn, lease, forceCustomization);
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Deploy vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="isPowerOn"></param>
        /// <param name="lease"></param>
        /// <param name="forceCustomization"></param>
        /// <param name="timeout"></param>
        public void DeployVapp(Vapp vApp, bool isPowerOn, int lease, bool forceCustomization, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Deploy vApp Start: {0}", vApp.Reference.name));
                vApp.Deploy(isPowerOn, lease, forceCustomization).WaitForTask(timeout);
                Log.logger.Debug(string.Format("Deploy vApp Finish: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Deploy vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="isPowerOn"></param>
        /// <param name="lease"></param>
        /// <param name="forceCustomization"></param>
        public void DeployVapp(Vapp vApp, bool isPowerOn, int lease, bool forceCustomization)
        {
            DeployVapp(vApp, isPowerOn, lease, forceCustomization, 0);
        }

        /// <summary>
        /// Deploy vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void DeployVapp(string organizationName, string vdcName, string vAppName, bool isPowerOn, int lease, bool forceCustomization, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            DeployVapp(vApp, isPowerOn, lease,forceCustomization, timeout);
        }

        /// <summary>
        /// Deploy vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="isPowerOn"></param>
        /// <param name="lease"></param>
        /// <param name="forceCustomization"></param>
        public void DeployVapp(string organizationName, string vdcName, string vAppName, bool isPowerOn, int lease, bool forceCustomization)
        {
            DeployVapp(organizationName, vdcName, vAppName, isPowerOn, lease, forceCustomization, 0);
        }

        #endregion

        #region Undeploy

        /// <summary>
        /// Undeploy vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void UndeployVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Undeploy vApp: {0}", vApp.Reference.name));
                vApp.Undeploy(UndeployPowerActionType.DEFAULT);
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Undeploy vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void UndeployVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Undeploy vApp Start: {0}", vApp.Reference.name));
                vApp.Undeploy(UndeployPowerActionType.DEFAULT).WaitForTask(timeout);
                Log.logger.Debug(string.Format("Undeploy vApp Finish: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Undeploy vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void UndeployVapp(Vapp vApp)
        {
            UndeployVapp(vApp, 0);
        }

        /// <summary>
        /// Undeploy vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void UndeployVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            UndeployVapp(vApp, timeout);
        }

        /// <summary>
        /// Undeploy vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void UndeployVapp(string organizationName, string vdcName, string vAppName)
        {
            UndeployVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        #region Power on

        /// <summary>
        /// Power on vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void PowerOnVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("PowerOn vApp: {0}", vApp.Reference.name));
                vApp.PowerOn();
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Power on vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void PowerOnVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("PowerOn vApp Start: {0}", vApp.Reference.name));
                vApp.PowerOn().WaitForTask(timeout);
                Log.logger.Debug(string.Format("PowerOn vApp Finish: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Power on vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void PowerOnVapp(Vapp vApp)
        {
            PowerOnVapp(vApp, 0);
        }

        /// <summary>
        /// Power on vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void PowerOnVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            PowerOnVapp(vApp, timeout);
        }

        /// <summary>
        /// Power on vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void PowerOnVapp(string organizationName, string vdcName, string vAppName)
        {
            PowerOnVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        #region Power off

        /// <summary>
        /// Power off vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void PowerOffVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("PowerOff vApp: {0}", vApp.Reference.name));
                vApp.PowerOff();
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Power off vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void PowerOffVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("PowerOff vApp: {0}", vApp.Reference.name));
                vApp.PowerOff().WaitForTask(timeout);
                Log.logger.Debug(string.Format("PowerOff vApp: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Power off vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void PowerOffVapp(Vapp vApp)
        {
            PowerOffVapp(vApp, 0);
        }

        /// <summary>
        /// Power off vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void PowerOffVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            PowerOffVapp(vApp, timeout);
        }

        /// <summary>
        /// Power off vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void PowerOffVapp(string organizationName, string vdcName, string vAppName)
        {
            PowerOffVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        #region Shutdown

        /// <summary>
        /// Shutdown vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void ShutdownVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Shutdown vApp: {0}", vApp.Reference.name));
                vApp.Shutdown();
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Shutdown vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void ShutdownVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Shutdown vApp: {0}", vApp.Reference.name));
                vApp.Shutdown().WaitForTask(timeout);
                Log.logger.Debug(string.Format("Shutdown vApp: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Shutdown vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void ShutdownVapp(Vapp vApp)
        {
            ShutdownVapp(vApp, 0);
        }

        /// <summary>
        /// Shutdown vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void ShutdownVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            ShutdownVapp(vApp, timeout);
        }

        /// <summary>
        /// Shutdown vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void ShutdownVapp(string organizationName, string vdcName, string vAppName)
        {
            ShutdownVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        #region Suspend

        /// <summary>
        /// Suspend vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void SuspendVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Suspend vApp: {0}", vApp.Reference.name));
                vApp.Suspend();
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Suspend vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void SuspendVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Suspend vApp: {0}", vApp.Reference.name));
                vApp.Suspend().WaitForTask(timeout);
                Log.logger.Debug(string.Format("Suspend vApp: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Suspend vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void SuspendVapp(Vapp vApp)
        {
            SuspendVapp(vApp, 0);
        }

        /// <summary>
        /// Suspend vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void SuspendVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            SuspendVapp(vApp, timeout);
        }

        /// <summary>
        /// Suspend vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void SuspendVapp(string organizationName, string vdcName, string vAppName)
        {
            SuspendVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete vApp async
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void DeleteVappAsync(Vapp vApp)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Delete vApp: {0}", vApp.Reference.name));
                vApp.Delete();
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Delete vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="timeout"></param>
        public void DeleteVapp(Vapp vApp, int timeout)
        {
            if (vApp == null)
            {
                string errorMsg = "vApp is null reference";
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            try
            {
                Log.logger.Debug(string.Format("Delete vApp Start: {0}", vApp.Reference.name));
                vApp.Delete().WaitForTask(timeout);
                Log.logger.Debug(string.Format("Delete vApp Finish: {0}", vApp.Reference.name));
            }
            catch (Exception e)
            {
                Log.logger.Error(e.Message);
                throw new EnvironmentException(VCLOUD, e.Message, e);
            }
        }

        /// <summary>
        /// Delete vApp
        /// </summary>
        /// <param name="vApp"></param>
        public void DeleteVapp(Vapp vApp)
        {
            DeleteVapp(vApp, 0);
        }

        /// <summary>
        /// Delete vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <param name="timeout"></param>
        public void DeleteVapp(string organizationName, string vdcName, string vAppName, int timeout)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                string errorMsg = string.Format("vApp:{0} not found", vAppName);
                Log.logger.Error(errorMsg);
                throw new EnvironmentException(VCLOUD, errorMsg);
            }

            DeleteVapp(vApp, timeout);
        }

        /// <summary>
        /// Delete vApp
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        public void DeleteVapp(string organizationName, string vdcName, string vAppName)
        {
            DeleteVapp(organizationName, vdcName, vAppName, 0);
        }

        #endregion

        /// <summary>
        /// Enable of disable vApp firewall
        /// </summary>
        /// <param name="vApp"></param>
        /// <param name="networkName"></param>
        /// <param name="isEnabled"></param>
        public void ConfigVAppNetworkFirework(Vapp vApp, string networkName, bool isEnabled)
        {
            var networkConfig = vApp.GetNetworkConfigSection();

            var configType = networkConfig.NetworkConfig.SingleOrDefault(n => n.networkName == networkName);

            if (configType != null)
            {
                var config = configType.Configuration;

                var firewallService = config.Features.SingleOrDefault(f => (f as FirewallServiceType) != null);

                firewallService.IsEnabled = true;

                vApp.UpdateSection(networkConfig).WaitForTask(1000 * 60);
            }
        }

        #endregion

        #region VM

        /// <summary>
        /// Get VM in specific vApp
        /// </summary>
        /// <param name="vApp"></param>
        /// <returns></returns>
        public IList<VM> GetVMsByVapp(Vapp vApp)
        {
            return vApp.GetChildrenVms();
        }

        /// <summary>
        /// Get VM in specific vApp by name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="vdcName"></param>
        /// <param name="vAppName"></param>
        /// <returns></returns>
        public IList<VM> GetVMsByVapp(string organizationName, string vdcName, string vAppName)
        {
            Vapp vApp = GetVappByName(organizationName, vdcName, vAppName);

            if (vApp == null)
            {
                Log.logger.Warn(string.Format("vApp {0} not found", vAppName));
                return null;
            }
            else
            {
                return GetVMsByVapp(vApp);
            }
        }

        #endregion
    }

    public enum vAppStatus
    {
        NotExist = 0,
        Created  = 1,
        Deployed = 2,
        Ready    = 3,
        Error    = 4,
        Undeploy = 5,
        Unknow   = 6,
    }
}
