using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.SharePoint.SearchRestore;
using EMC.SourceOne.GOS;
using EMC.SourceOne.SharePoint.Archive.Search;
using Microsoft.SharePoint.Administration;
using EMC.SourceOne.SearchWS.ExSearchWebService;
using EMC.SourceOne.SharePoint.Archive.Search.Configuration;
using System.Xml;
using System.Data;

namespace RestoreLib
{
    public class SPPowerUserRestore:SPRestore
    {
        public SPPowerUserRestore(string webAppUrl, string searchServiceURL, string userName, string password)
            : base(webAppUrl,searchServiceURL, userName, password)
        {
        }

        public void Execute(ConflictResolution conflict)
        {
            Execute(conflict, _webAppUrl, _items);
        }

        public int Execute(ConflictResolution conflict, List<RestoreItem> items)
        {
            return Execute(conflict, _webAppUrl, items);
        }

        public void Execute(ConflictResolution conflict, string backupLocation)
        {
            Execute(conflict, backupLocation, _items);
        }

        public int Execute(ConflictResolution conflict, string backupLocation, List<RestoreItem> items)
        {
            List<string> targets = new List<string>();
            foreach (RestoreItem item in items)
            {
                if (!targets.Contains(item.Target))
                {
                    targets.Add(item.Target);
                }
            }
            return Execute(conflict, backupLocation, targets);
        }

        public int Execute(ConflictResolution conflict, string backupLocation, List<string> targets)
        {
            StringBuilder expressionBuilder = new StringBuilder();
            expressionBuilder.Append("<ExpressionSet logicalOperator=\"OR\">");
            foreach (string target in targets)
            {
                expressionBuilder.Append("<SimpleAttributeExpression npmPropertyId=\"58\" displayName=\"Source Location\" searchOperation=\"EQUALS\" dataType=\"string\" caseSensitive=\"false\">" +target + "</SimpleAttributeExpression>");
            }
            expressionBuilder.Append("</ExpressionSet>");

            string queryExpression = expressionBuilder.ToString();

            string resultAttribute = ResultAttrs;
            XmlNode archiveFoldersNode = searchClientProxy.Client.EnumerateArchiveFolders(ExSearchType.Administrator);
            string queryID = searchClientProxy.Client.SearchEx(queryExpression, archiveFoldersNode.OuterXml, ResultAttrs, null, null, ExSearchType.ReadAll, 500, false, false, 0);
            ExTaskStatus status = this.searchClientProxy.Client.GetQueryStatus(queryID);
            while (this.searchClientProxy.Client.GetQueryStatus(queryID) != ExTaskStatus.complete) ;

            DataSet ds = this.searchClientProxy.Client.GetPagedResults(queryID, "DATE", 0, 500);
            int resultCount = ds.Tables[0].Rows.Count;
            RestoreCriteria restoreCriteria = new RestoreCriteria();
            
            restoreCriteria.Output.Config.Server = string.Format("{0}_vti_bin/emces1/restore.svc", _webAppUrl);
            restoreCriteria.Output.Config.RestoreAction = conflict;
            restoreCriteria.Output.Config.BackupLocation = backupLocation;
            restoreCriteria.Output.Config.RestoreType = 1; // 1 as power user restore
            restoreCriteria.Output.Config.RestoreAccount = @"SHAREPOINT\system";
            string outputSource = restoreCriteria.GetOutputSourceXml();
            searchClientProxy.Client.SelectAllResults(queryID);
            string restoreID = searchClientProxy.Client.RestoreResultsExSP(queryID, null, outputSource, "ENTRYID", "FOLDER", 0, ExSearchType.Administrator); //restore ID should be saved in list. - // power user restore: has 10 threads in JBC
            return resultCount;       
        }
    }
}
