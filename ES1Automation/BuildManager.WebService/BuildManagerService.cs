using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

using Core;
using Core.DTO;
using Core.Model;
using ES1Common.Logs;

namespace BuildManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class BuildManagerService
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances

        [WebGet(UriTemplate = "")]
        public List<BuildDTO> GetCollection()
        {
            return Build.GetBuildsByStatus(BuildStatus.Success).ToDTOs();
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public BuildDTO Create(BuildDTO instance)
        {
            return Build.Add(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "{id}")]
        public BuildDTO Get(string id)
        {
            return Build.GetBuildById(Int32.Parse(id)).ToDTO();
        }

        [WebGet(UriTemplate = "latestbuild?productId={productId}&branchId={branchId}&releaseId={releaseId}")]
        public string GetLatestBuild(int productId, int branchId, int releaseId)
        {
            return Core.Management.BuildManager.GetLatestBuildByProductBranchAndRelease(productId, branchId, releaseId).ToDTO().Name;
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public BuildDTO Update(string id, BuildDTO instance)
        {
            int buildId = Int32.Parse(id);
            instance.BuildId = buildId;
            return Build.Update(buildId, instance).ToDTO();  
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void Delete(string id)
        {
            Build.Delete(Int32.Parse(id));
        }

        [WebInvoke(UriTemplate = "refresh", Method = "POST")]
        public bool refreshTheBuildServer()
        {
            try
            {
                Core.Management.BuildManager.UpdateBuildStatus();
                return true;
            }
            catch(Exception ex)  
            {
                ATFEnvironment.Log.logger.Error("Failed to update the build status.", ex);
                return false;
            }
        }

    }
}
