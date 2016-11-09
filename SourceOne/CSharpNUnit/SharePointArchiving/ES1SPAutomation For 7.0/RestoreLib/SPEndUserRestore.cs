using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.SharePoint.SearchWSAssistant;
using EMC.SourceOne.GOS;
using EMC.SourceOne.SharePoint.Archive.Search;
using EMC.SourceOne.SharePoint.Scif;
using EMC.SourceOne.SharePoint.Utilities;
using EMC.SourceOne.SharePoint.SearchRestore;
using EMC.SourceOne.SearchWS;
using Microsoft.SharePoint;

namespace RestoreLib
{
    public class SPEndUserRestore :SPRestore
    {
        public SPEndUserRestore(string webAppUrl, string searchServiceURL, string userName, string password)
            : base(webAppUrl,searchServiceURL, userName, password)
        {
        }

        public void Execute(ConflictResolution conflict)
        {
            Execute(conflict, _items);
        }

        public void Execute(ConflictResolution conflict, List<RestoreItem> items)
        {
            RestoreCriteria criteria = new RestoreCriteria();

            foreach (RestoreItem item in items)
            {
                string target = null;
                if (string.IsNullOrEmpty(item.TargetWebUrl) || string.IsNullOrEmpty(item.TargetSiteUrl))
                    target = item.Target;
                else
                    target = string.Format("{0},{1},{2}", item.Target, item.TargetWebUrl, item.TargetSiteUrl);
                criteria.Input.Add(item.Id, item.SourceType, target, item.Version, item.LastModified, conflict);
            }


            criteria.Output.Config.Server = string.Format("{0}_vti_bin/emces1/restore.svc", _webAppUrl);
            criteria.Output.Config.RestoreAction = conflict;
            criteria.Output.Config.RestoreType = 0; // 0 as end user restore
            criteria.Output.Config.RestoreAccount = @"SHAREPOINT\system";

            string inputSource = criteria.GetInputSourceXml();
            string outputSource = criteria.GetOutputSourceXml();
            string restoreID = searchClientProxy.Client.RestoreResultsExSP(null, inputSource, outputSource, "ENTRYID", "FOLDER", 0, EMC.SourceOne.SearchWS.ExSearchWebService.ExSearchType.ACL);

        }
    }
}
