using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.SearchWS;
using System.Xml;
using EMC.SourceOne.SharePoint.Utilities;
using EMC.SourceOne.SharePoint.Scif;
using EMC.SourceOne.SharePoint.Archive.Search;
using EMC.SourceOne.SearchWS.ExSearchWebService;
using System.Data;
using EMC.SourceOne.SharePoint.SearchRestore;

namespace RestoreLib
{
    public class SearchUtil
    {
        private SearchProxy searchClientProxy = null;
        private int maxHits = 0;
        private int timeout = 0;
        private XmlNode archiveFoldersNode = null;
        static private string _resultAttrs;
        public int resultCount = 0;
    

        public SearchUtil(string searchServiceURL, string userName, string password)
        {
            this.searchClientProxy = new SearchProxy();
            this.searchClientProxy.Initialize(searchServiceURL, false, new Credentials(userName, false), new EncryptDecryptSecureString(password));
        }

        static public string OwnerList
        {
            get
            {
                Owners o = new Owners();
                o.LoadUserInfo();
                return Serialize.ToXmlString(o);
            }
        }

        static private string ResultAttrs
        {
            get
            {
                if (_resultAttrs == null)
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(Environment.CurrentDirectory+"\\ResultColumns.xml"))
                    {
                        StringBuilder sb = new StringBuilder();

                        try
                        {
                            while (xmlReader.Read())
                                sb.Append(xmlReader.ReadOuterXml());
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        _resultAttrs = sb.ToString();
                    }
                }
                return _resultAttrs;
            }
        }

        public int MaxHits
        {
            get
            {
                if (0 == maxHits)
                    return 2000;
                else
                    return maxHits;
            }
            set
            {
                maxHits = value;
            }
        }

        public int Timeout
        {
            get
            {
                if (0 == timeout)
                    return 100;
                else
                    return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        private void GetArchiveFolders(ExSearchType searchType)
        {
            archiveFoldersNode = this.searchClientProxy.Client.EnumerateArchiveFolders(searchType);
           
        }

        private DataSet GetSearchResult(string queryExpression, ExSearchType searchType, string sortColumn)
        {
            string result = this.searchClientProxy.Client.GetServerConfiguration().InnerXml;
            if (String.IsNullOrEmpty(sortColumn))
                throw new ArgumentNullException("sortColumn");
            GetArchiveFolders(searchType);
            string queryId = this.searchClientProxy.Client.SearchEx(queryExpression, archiveFoldersNode.OuterXml, ResultAttrs, null, null, searchType, 500, false, false, Timeout);
            //string queryId = this.searchClientProxy.Client.Search(queryExpression, archiveFoldersNode.OuterXml, searchType, 500, false, false, Timeout);
            ExTaskStatus status = this.searchClientProxy.Client.GetQueryStatus(queryId);
        
            while (this.searchClientProxy.Client.GetQueryStatus(queryId) != ExTaskStatus.complete) ;

            DataSet ds = this.searchClientProxy.Client.GetPagedResults(queryId, sortColumn, 0, 500);
            resultCount = ds.Tables[0].Rows.Count;
            return ds;
        }

        public List<RestoreItem> GetRestoreItems(SearchField field, string value, int count)
        {
            DataSet ds = SearchBy(field, value);
            List<RestoreItem> items = new List<RestoreItem>();
            for (int i = 0; i < count; i++)
            {
                RestoreItem item = new RestoreItem();
                item.Id = ds.Tables[0].Rows[resultCount - i - 1].ItemArray[0].ToString();
                item.SourceType = ds.Tables[0].Rows[resultCount - i - 1].ItemArray[7].ToString();
                item.Version = ds.Tables[0].Rows[resultCount - i - 1].ItemArray[13].ToString();
                string target = ds.Tables[0].Rows[resultCount - i - 1].ItemArray[8].ToString();
                string[] targets = target.Split(new char[]{','});
                item.Target = targets[0];
                item.TargetWebUrl = targets[1];
                item.TargetSiteUrl = targets[2];
                items.Add(item);
            }
            return items;
        }

        public DataSet SearchBy(SearchField field, string value)
        {
            string expression = "<ExpressionSet logicalOperator=\"AND\"><SimpleAttributeExpression npmPropertyId=\"23\" displayName=\"Platform Object ID2\" searchOperation=\"EQUALS\" dataType=\"string\" caseSensitive=\"false\">" + value + "</SimpleAttributeExpression></ExpressionSet>";
         
            switch (field)
            {
                case SearchField.GUID:
                    expression = "<ExpressionSet logicalOperator=\"AND\"><SimpleAttributeExpression npmPropertyId=\"22\" displayName=\"Platform Object ID\" searchOperation=\"EQUALS\" dataType=\"string\" caseSensitive=\"false\">" + value + "</SimpleAttributeExpression></ExpressionSet>";
                    break;
                case SearchField.SourceType:
                    expression = "<ExpressionSet logicalOperator=\"AND\"><SimpleAttributeExpression npmPropertyId=\"50\" displayName=\"Source Type\" searchOperation=\"EQUALS\" dataType=\"string\" caseSensitive=\"false\">" + value + "</SimpleAttributeExpression></ExpressionSet>";
                    break;
                case SearchField.Subject:
                    expression = "<ExpressionSet logicalOperator=\"AND\"><SimpleAttributeExpression npmPropertyId=\"20\" displayName=\"Subject\" searchOperation=\"EQUALS\" dataType=\"string\" caseSensitive=\"false\">" + value + "</SimpleAttributeExpression></ExpressionSet>";
                    break;
            }
            return GetSearchResult(expression, ExSearchType.Administrator, "DATE");
        }

        public enum SearchField
        {
            SourceType,
            GUID,
            Subject
        }
    }
    
}
