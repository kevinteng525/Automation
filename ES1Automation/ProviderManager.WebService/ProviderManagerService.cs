using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.Model;
using Core.DTO;

namespace ProviderManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class ProviderManagerService
    {

        [WebGet(UriTemplate = "?searchBy=category&category={category}")]
        public List<ProviderDTO> GetProviderByCategory(string category)
        {
            switch (category) {
                case "TestCase": return Provider.GetProvidersByCategory(ProviderCategory.TestCase).ToDTOs();
                case "Build":  return Provider.GetProvidersByCategory(ProviderCategory.Build).ToDTOs();
                case "Environment" : return Provider.GetProvidersByCategory(ProviderCategory.Environment).ToDTOs();
            }
            return new List<ProviderDTO>();
           
        }
       
       
    }

}
