using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Core.Model;
using Core.DTO;

namespace UserManager.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class UsersManagerService
    {
        
        [WebGet(UriTemplate = "")]
        public List<UserDTO> GetCollection()
        {
            List<User> userList = User.GetAllUsers();
            return userList.ToDTOs();
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public UserDTO Create(UserDTO instance)
        {
            User u = instance.ToEntity();
            u.PlainText = instance.Password;
            return User.Add(u).ToDTO();
        }

        [WebGet(UriTemplate = "{id}")]
        public UserDTO Get(string id)
        {
            return User.GetUserById(Int32.Parse(id)).ToDTO();
        }

        [WebGet(UriTemplate = "name/{name}")]
        public int GetUserByUserName(string name)
        {
            return User.GetUserByUserName(name).UserId;
        }

        [WebGet(UriTemplate = "{id}/subscribed_projects")]
        public List<ProjectDTO> GetSubscribedProjects(string id)
        {
            return User.GetSubscribedProjects(Int32.Parse(id)).ToDTOs();
        }

        [WebInvoke(UriTemplate = "isUserValid", Method = "POST")]
        public UserDTO validate(UserDTO instance)
        {
            User u =  User.GetUser(instance.Username, instance.Password);
            if (u != null)
            {
                return u.ToDTO();
            }
            else
            {
                return null;
            }
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public UserDTO Update(string id, UserDTO instance)
        {
            int userID = Int32.Parse(id);
            User u = instance.ToEntity();
            u.PlainText = instance.Password;
            u.UserId = userID;
            return User.Update( userID, u).ToDTO();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void Delete(string id)
        {
            // TODO: Remove the instance of SampleItem with the given id from the collection
            User.Delete(Int32.Parse(id));
        }

    }
}
