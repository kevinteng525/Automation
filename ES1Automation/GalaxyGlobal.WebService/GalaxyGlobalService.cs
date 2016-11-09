using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

using Core.Model;
using Core.DTO;

namespace System.WebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class GalaxyGlobalService
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances
        #region Rankings
        //Rankings
        [WebGet(UriTemplate = "Ranking")]
        public List<RankingDTO> GetRankingCollection()
        {
            return Ranking.GetAllRankings().ToDTOs();
        }

        [WebInvoke(UriTemplate = "Ranking", Method = "POST")]
        public RankingDTO CreateRanking(RankingDTO instance)
        {
            return Ranking.AddOrUpdateRanking(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Ranking/{id}")]
        public RankingDTO GetRanking(string id)
        {
            return Ranking.GetRankingById(int.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Ranking/{id}", Method = "PUT")]
        public RankingDTO UpdateRanking(string id, RankingDTO instance)
        {
            return Ranking.UpdateRanking(int.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "Ranking/{id}", Method = "DELETE")]
        public void DeleteRanking(string id)
        {
            Ranking.DeleteRanking(int.Parse(id));
        }
        #endregion

        #region Branches
        //Branches
        [WebGet(UriTemplate = "Branch?searchBy=type&type={type}")]
        public List<BranchDTO> GetBranchCollection(int type)
        {
            return Branch.GetAllBranchesByType(type).ToDTOs();
        }

        [WebInvoke(UriTemplate = "Branch", Method = "POST")]
        public BranchDTO CreateBranch(BranchDTO instance)
        {
            return Branch.AddOrUpdateBranch(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Branch/{id}")]
        public BranchDTO GetBranch(string id)
        {
            return Branch.GetBranchById(int.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Branch/{id}", Method = "PUT")]
        public BranchDTO UpdateBranch(string id, BranchDTO instance)
        {
            return Branch.UpdateBranch(int.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "Branch/{id}", Method = "DELETE")]
        public void DeleteBranch(string id)
        {
            Branch.DeleteBranchById(int.Parse(id));
        }
        #endregion

        #region Releases
        //Releases
        [WebGet(UriTemplate = "Release?searchBy=type&type={type}")]
        public List<ReleaseDTO> GetReleasehCollection(int type)
        {
            return Release.GetAllReleasesByType(type).ToDTOs();
        }

        [WebInvoke(UriTemplate = "Release", Method = "POST")]
        public ReleaseDTO CreateRelease(ReleaseDTO instance)
        {
            return Release.AddOrUpdateRelease(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Release/{id}")]
        public ReleaseDTO GetRelease(string id)
        {
            return Release.GetReleaseById(int.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Release/{id}", Method = "PUT")]
        public ReleaseDTO UpdateReleae(string id, ReleaseDTO instance)
        {
            return Release.UpdateRelease(int.Parse(id), instance).ToDTO();
        }

        [WebInvoke(UriTemplate = "Release/{id}", Method = "DELETE")]
        public void DeleteRelease(string id)
        {
            Release.DeleteRelease(int.Parse(id));
        }
        #endregion

        #region Subscriber
        //Subscriber
        [WebGet(UriTemplate = "Subscriber")]
        public List<SubscriberDTO> GetSubscriberCollection()
        {
            return Subscriber.GetAllSubscribers().ToDTOs();
        }

        [WebInvoke(UriTemplate = "Subscriber", Method = "POST")]
        public SubscriberDTO CreateSubscriber(SubscriberDTO instance)
        {
            instance.CreateTime = DateTime.UtcNow;
            return Subscriber.AddOrUpdateSubscriber(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "Subscriber/{id}")]
        public SubscriberDTO GetSubscriber(string id)
        {
            return Subscriber.GetSubscriberById(int.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "Subscriber/{id}", Method = "PUT")]
        public SubscriberDTO UpdateSubscriber(string id, SubscriberDTO instance)
        {
            instance.SubscriberId = int.Parse(id);
            return Subscriber.AddOrUpdateSubscriber(instance.ToEntity()).ToDTO();
        }

        [WebInvoke(UriTemplate = "Subscriber/{id}", Method = "DELETE")]
        public void DeleteSubscriber(string id)
        {
            Subscriber.DeleteSubscriber(int.Parse(id));
        }
        #endregion

        #region DiagnosticLog
        //DiagnosticLog
        [WebGet(UriTemplate = "DiagnosticLog")]
        public List<DiagnosticLogDTO> GetDiagnosticLogCollection()
        {
            return DiagnosticLog.GetAllDiagnosticLogs().ToDTOs();
        }

        [WebInvoke(UriTemplate = "DiagnosticLog", Method = "POST")]
        public DiagnosticLogDTO CreateDiagnosticLog(DiagnosticLogDTO instance)
        {
            return DiagnosticLog.AddDiagnosticLog(instance.ToEntity()).ToDTO();
        }

        [WebGet(UriTemplate = "DiagnosticLog/{id}")]
        public DiagnosticLogDTO GetDiagnosticLog(string id)
        {
            return DiagnosticLog.GetDiagnosticLogById(int.Parse(id)).ToDTO();
        }

        [WebInvoke(UriTemplate = "DiagnosticLog/{id}", Method = "PUT")]
        public DiagnosticLogDTO UpdateDiagnosticLog(string id, DiagnosticLogDTO instance)
        {
            return DiagnosticLog.UpdateDiagnosticLog(int.Parse(id),instance.ToEntity()).ToDTO();
        }

        [WebInvoke(UriTemplate = "DiagnosticLog/{id}", Method = "DELETE")]
        public void DeleteDiagnosticLog(string id)
        {
            DiagnosticLog.DeleteDiagnosticLog(int.Parse(id));
        }
        #endregion

    }
}
