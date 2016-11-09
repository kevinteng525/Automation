using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.DTO;
using Core.Model;

namespace ProductManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class ProductManagerService
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances

        [WebGet(UriTemplate = "")]
        public List<ProductDTO> GetCollection()        {
           
            return Product.GetAllProducts().ToDTOs();
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public ProductDTO Create(ProductDTO instance)
        {
            return Product.AddProduct(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "{id}")]
        public ProductDTO Get(string id)
        {
            return Product.GetProductByID(Int32.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public ProductDTO Update(string id, ProductDTO instance)
        {
            instance.ProductId = Int32.Parse(id);
            return Product.UpdateProduct(Int32.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void Delete(string id)
        {
            Product.DeleteProduct(Int32.Parse(id));
        }
    

        [WebGet(UriTemplate = "{id}/Builds")]
        public List<BuildDTO> GetBuildsCollection(string id)
        {

            return Product.GetAllBuildsForProduct(Int32.Parse(id)).ToDTOs();
        }

        [WebGet(UriTemplate = "{id}/Branches?searchBy=type&type={type}")]
        public List<BranchDTO> GetBranchesCollection(string id,int type)
        {

            return Product.GetAllBranchesForProduct(Int32.Parse(id),type).ToDTOs();
        }

        [WebGet(UriTemplate = "{id}/Releases?searchBy=type&type={type}")]
        public List<ReleaseDTO> GetReleasesCollection(string id, int type)
        {

            return Product.GetAllReleasesForProduct(Int32.Parse(id),type).ToDTOs();
        }

    }
}
