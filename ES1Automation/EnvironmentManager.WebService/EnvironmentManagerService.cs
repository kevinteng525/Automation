using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.DTO;
using Core.Model;

namespace EnvironmentManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class EnvironmentManagerService
    {
        #region for the supported environment
        [WebGet(UriTemplate = "Templates")]
        public List<SupportedEnvironmentDTO> GetTemplatesCollection()
        {
            return SupportedEnvironment.GetSupportedEnvironments().ToDTOs();
        }

        [WebInvoke(UriTemplate = "Templates", Method = "POST")]
        public SupportedEnvironmentDTO CreateTemplate(SupportedEnvironmentDTO instance)
        {            
            return SupportedEnvironment.Add(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Templates/{id}")]
        public SupportedEnvironmentDTO GetTemplate(string id)
        {
            return SupportedEnvironment.GetSupportedEnvironmentById(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Templates/{id}", Method = "PUT")]
        public SupportedEnvironmentDTO UpdateTemplate(string id, SupportedEnvironmentDTO instance)
        {
            int environmentId =  Int32.Parse(id);
            instance.EnvironmentId = environmentId;
            return SupportedEnvironment.Update(environmentId, instance).ToDTO();
            
        }

        [WebInvoke(UriTemplate = "Templates/{id}", Method = "DELETE")]
        public void DeleteTemplate(string id)
        {
            SupportedEnvironment.Delete(Int32.Parse(id));
        }
        #endregion

        #region for the map between project and supported environment
        [WebGet(UriTemplate = "ProjectsTemplatesMap")]
        public List<ProjectEnvironmentMapDTO> GetMapCollection()
        {
            return ProjectEnvironmentMap.GetAllProductEnvironmentMap().ToDTOs();
        }

        [WebInvoke(UriTemplate = "ProjectsTemplatesMap", Method = "POST")]
        public ProjectEnvironmentMapDTO CreateMap(ProjectEnvironmentMapDTO instance)
        {
            return ProjectEnvironmentMap.Add(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "ProjectsTemplatesMap/{id}")]
        public ProjectEnvironmentMapDTO GetMap(string id)
        {
            return ProjectEnvironmentMap.GetProductEnvironmentMapById(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "ProjectsTemplatesMap/{id}", Method = "PUT")]
        public ProjectEnvironmentMapDTO UpdateMap(string id, ProjectEnvironmentMapDTO instance)
        {
            int mapId = Int32.Parse(id);
            instance.MapId = mapId;
            return ProjectEnvironmentMap.Update(mapId, instance).ToDTO();

        }

        [WebInvoke(UriTemplate = "ProjectsTemplatesMap/{id}", Method = "DELETE")]
        public void DeleteMap(string id)
        {
            ProjectEnvironmentMap.Delete(Int32.Parse(id));
        }

        #endregion

        #region for the actural test environment

        [WebGet(UriTemplate = "Environments")]
        public List<TestEnvironmentDTO> GetEnvironmentsCollection()
        {
            return TestEnvironment.GetAllEnvironments().ToDTOs();
        }


        [WebGet(UriTemplate = "Environments/{id}")]
        public TestEnvironmentDTO GetEnvironmentById(string id)
        {
            return TestEnvironment.GetEnvironmentById(Int32.Parse(id)).ToDTO();
        }

        #endregion
    }
}
