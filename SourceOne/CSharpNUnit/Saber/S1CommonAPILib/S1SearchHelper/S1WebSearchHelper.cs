using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.Xml;
using System.Net;
//using System.Web.Caching;
using System.Web.Services.Protocols;

using Saber.S1CommonAPILib.localhost;
using Saber.S1CommonAPILib.S1SearchWrapper;

namespace Saber.S1CommonAPILib.S1SearchHelper
{
    /// <summary>
    /// A simple class that inherits from the actual web service, so
    /// we can write a constructor that takes care of all of the
    /// initialization of the web service.
    /// </summary>
    public class SearchWebService : ExSearchWebService
    {
        private const string urlstr = "http://localhost/SearchWS/ExSearchWebService.asmx";
        public SearchWebService()
        {
            // point web service at actual url
            this.Url = urlstr;
            this.PreAuthenticate = true;

            // create creds object
            NetworkCredential netcreds = new NetworkCredential("windows:doc", "doc");
            CredentialCache cache = new CredentialCache();
            Uri uri = new Uri(urlstr);
            cache.Add(uri, "Basic", netcreds);
            this.Credentials = cache;
        }
        public SearchWebService(string username, string password)
        {
            // point web service at actual url
            this.Url = urlstr;
            this.PreAuthenticate = true;

            // create creds object
            NetworkCredential netcreds = new NetworkCredential(username, password);
            CredentialCache cache = new CredentialCache();
            Uri uri = new Uri(urlstr);
            cache.Add(uri, "Basic", netcreds);
            this.Credentials = cache;
        }
    }
    /// <summary>
    /// Class wraps access to ExSearch Web Service
    /// </summary>
    public class SearchHelper
    {
        private SearchWebService wsExSearch = null;
        private ExSearchType m_defaultUserType = ExSearchType.Owner;
        private String default_searchScope = String.Empty;
        private String default_columnsInResult = String.Empty;
        private Int32 default_maxHits = 0;
        private bool default_bRemoveDuplicates = true;
        private bool default_searchEmbeddedMsgs = false;
        private Int32 default_searchTimeout = 0;
        private SearchPresentation _sp = new SearchPresentation();
        
        /// <summary>
        /// Constructor
        /// </summary>
        public SearchHelper()
        {
            wsExSearch = new SearchWebService(@"usc\es1service","emcsiax@QA");
        }

        /// <summary>
        /// The underlying call to the webservice really does nothing here.
        /// To suport the call, however, it must set up the authentication
        /// before hand. Effectively, this is just an empty call to trigger the auth.
        /// The webservice throws an error if the authentication fails, so we catch
        /// the throw and return false here instead.
        /// </summary>
        /// <returns>returns true if the client Authentication succeeded</returns>
        public bool AuthenticateUser()
        {
            //ExSearchTrace trace = new ExSearchTrace();
            
            bool bAuth = false;

            try
            {
                //Returns true if it does not throw
                bAuth = wsExSearch.AuthenticateUser();
            }
            catch (Exception ex)
            {
                //trace.TraceError(ex.ToString());
                
                //Interpret Authentication Exception
                if (ex.Message.Contains("401"))
                {
                    //SearchError.SetError(Resources.SearchResStrings.Error_Authenticate);
                }
                else
                {
                    //If not a "401: Access Denied" error,
                    //Show a "Server Unavailable" error
                    //SearchError.SetError(Resources.SearchResStrings.Error_Auth_Unavailable);
                }

                bAuth = false;
            }

            return bAuth;

        }

        #region Query_Scope_And_ResultAttributeList_Generation

        //Builds the XML query based upon the current Search fields and values
        protected String GenerateQuery(bool bAllowEmptySearch, string keyword)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sQuery = String.Empty;

            String sKeywordValue = keyword;


            List<ISearchFieldControl> listFields = new List<ISearchFieldControl>();// sfcs.DeserializeSFClist(xmlListFields, true);

            String sObjectId = "";// (string)Session[SearchSessionKeys.currObjectTypeId];

            String sSearchType = SearchTypeKeys.Administrator;// (string)Session[SearchSessionKeys.sUserType];

            if (listFields != null || !String.IsNullOrEmpty(sKeywordValue))
            {
                SearchQueryBuilder qb = new SearchQueryBuilder(bAllowEmptySearch);
                string env = SearchMailEnv.EXCHANGE;//(string)(Session[SearchSessionKeys.sMailEnv]);
                sQuery = qb.CreateXMLQuery(sKeywordValue, listFields, sObjectId, sSearchType, env);
            }

            return sQuery;
        }

        //Generate the Folder scope xml string from the Session variable
        private String GenerateSearchScope(List<String> mappedFolders)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sScope = String.Empty;

            //DEBUG: override with csv folder list from web.config if it exists
            /*
            if (ConfigurationManager.AppSettings["DebugOverrideSearchFolders_CSV"] != null)
            {
                String sList = ConfigurationManager.AppSettings["DebugOverrideSearchFolders_CSV"];
                ArrayList aList = new ArrayList();
                foreach(string s in sList.Split(new char[]{','}))
                {
                    aList.Add(s);
                }
                Session[SearchSessionKeys.listSearchFolderScopeIDs] = aList;
            }
             * */
            //END DEBUG

            if (mappedFolders != null)
            {
                //Folder Scope was specified
                List<String> arrFolders = mappedFolders;
                if (arrFolders.Count > 0)
                {
                    XmlWriter xWriter;
                    XmlWriterSettings xSet = new XmlWriterSettings();
                    xSet.OmitXmlDeclaration = true;
                    StringBuilder sbXml = new StringBuilder();
                    xWriter = XmlWriter.Create(sbXml, xSet);
                    xWriter.WriteStartElement("SourceList");
                    foreach (String sFolder in arrFolders)
                    {
                        xWriter.WriteStartElement("Source");
                        //fields are AND'd together
                        xWriter.WriteAttributeString("name", sFolder);
                        xWriter.WriteAttributeString("folder", sFolder);
                        xWriter.WriteEndElement();//Source
                    }
                    xWriter.WriteEndElement();//SourceList
                    xWriter.Close();
                    sScope = sbXml.ToString();
                }
            }
            else
            {
                //Folder scope was not specified, Search All Folders
                sScope = GetSearchScopeXML();
            }

            return sScope;
        }

        private String GetSearchScopeXML()
        {
            string sScope = String.Empty;
            string sXmlData = String.Empty;
            string sUserType = SearchTypeKeys.Administrator;

            
            bool isDelegate = false;
            string sDelegateDN = "";
            //Check for Delegate Mode
            if (isDelegate)
            {
                //DELEGATE MODE
                sXmlData = EnumerateArchiveFoldersAsDelegate(sUserType, sDelegateDN);
            }
            else
            {
                //NORMAL MODE
                sXmlData = EnumerateArchiveFolders(sUserType);
            }

            if (!String.IsNullOrEmpty(sXmlData))
            {
                //check if folder list contains folders
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(new System.IO.StringReader(sXmlData));
                System.Xml.XmlNodeList xnlist;
                xnlist = xmlDoc.GetElementsByTagName("Source");
                if (xnlist.Count > 0)
                {
                    sScope = sXmlData;
                }
            }

            return sScope;
        }


        private String GenerateResultAttributeList()
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sColumns = String.Empty;

            //Required Columns -- Generate list of Columns that we must always return, needed to drive UI
            //***USERSELECT AND RESULTID are always returned and don't need to be requested.
            ArrayList listColumns = new ArrayList();
            listColumns.Add(SearchPropKeys.ENTRYID);
            listColumns.Add(SearchPropKeys.FOLDER);
            listColumns.Add(SearchPropKeys.ITEMTYPE);
            listColumns.Add(SearchPropKeys.PLATFORMTYPE);
            listColumns.Add(SearchPropKeys.DATE);
            listColumns.Add(SearchPropKeys.SIZE);

            string currObjectTypeId = "";
            int currObjectColViewId = 0;
            if (currObjectTypeId != null &&
                currObjectColViewId != null)
            {
                PresentationObject obj = _sp.Model.GetObject(currObjectTypeId);

                if (obj != null)
                {
                    if (obj.ResultViews != null &&
                        obj.ResultViews.Count > 0)
                    {
                        PresentationResultView view = obj.ResultViews[currObjectColViewId];

                        if (view.ColumnList.Count > 0)
                        {
                            //Sort the new set of columns based on xml defined column index
                            view.ColumnList.Sort();

                            foreach (PresentationColumn col in view.ColumnList)
                            {
                                //Add each VISIBLE column from the view to the result attribute list
                                PresentationProperty prop = col.AssociatedProperty;
                                if (prop != null &&
                                    prop.IsSearchHit &&
                                    col.IsVisible)
                                {
                                    if (!prop.IsPresentationOnlyProperty)
                                    {
                                        string sTag = _sp.GetTagFromProp(prop);
                                        if (!listColumns.Contains(sTag))
                                        {
                                            listColumns.Add(sTag);
                                        }
                                    }
                                    else
                                    {
                                        if (obj.AssociatedGroup is PresentationGroup)
                                        {
                                            GetAliasPropsForGroup((PresentationGroup)obj.AssociatedGroup, ref listColumns, prop.PresentationPropId);
                                        }
                                    }
                                }
                            }

                            //Save the requested result column list
                            //We will use this saved list inthe options page so we know what columns were requested and can warn
                            //the user if they add columns they will have to rerun the search to see the data.
                            //Session[SearchSessionKeys.listRequestedColumns] = listColumns;

                            //WRITE THE XML
                            XmlWriter xWriter;
                            XmlWriterSettings xSet = new XmlWriterSettings();
                            xSet.OmitXmlDeclaration = true;
                            StringBuilder sbXml = new StringBuilder();
                            xWriter = XmlWriter.Create(sbXml, xSet);
                            xWriter.WriteStartElement("ResultAttributeList");

                            foreach (string sTag in listColumns)
                            {
                                PresentationProperty prop = _sp.GetPropFromTag(sTag);
                                if (prop != null)
                                {
                                    WriteResultAttribute(ref xWriter, prop.NPMPropID.ToString(), sTag, true, prop.MaxLength);
                                }
                            }

                            xWriter.WriteEndElement();//ResultAttributeList
                            xWriter.Close();
                            sColumns = sbXml.ToString();
                        }
                    }
                }
            }

            return sColumns;
        }

        private void WriteResultAttribute(ref XmlWriter xWriter, string propID, string name, bool sortable, int length)
        {
            //Write the Result Attribute entry to the XML
            xWriter.WriteStartElement("ResultAttribute");

            xWriter.WriteAttributeString("npmPropertyId", propID);

            xWriter.WriteAttributeString("displayName", name);

            //sorted - can't handle "false" right now. Eventually this will determine whether or not a result column is created in the DB for this field.
            xWriter.WriteAttributeString("sortable", "true" /*sortable.ToString()*/);

            if (length <= 0)
            {
                length = 255;
            }
            xWriter.WriteAttributeString("length", length.ToString());

            xWriter.WriteEndElement();//ResultAttribute

        }

        private void GetAliasPropsForGroup(PresentationGroup group, ref ArrayList listProps, string presPropId)
        {
            PresentationObject obj = group.AssociatedObject;

            if (obj != null)
            {
                PresentationPropertyAlias alias = obj.GetPropertyAlias(presPropId);
                if (alias != null &&
                    alias.AssociatedNPMProperty != null)
                {
                    string sTag = _sp.GetTagFromProp(alias.AssociatedNPMProperty);
                    if (!listProps.Contains(sTag))
                    {
                        listProps.Add(sTag);
                    }
                }
            }

            if (group.GroupItems != null)
            {
                foreach (PresentationGroupItem item in group.GroupItems)
                {
                    PresentationObject itemObj = item.AssociatedObject;
                    if (itemObj != null)
                    {
                        PresentationPropertyAlias alias = itemObj.GetPropertyAlias(presPropId);
                        if (alias != null &&
                            alias.AssociatedNPMProperty != null)
                        {
                            string sTag = _sp.GetTagFromProp(alias.AssociatedNPMProperty);
                            if (!listProps.Contains(sTag))
                            {
                                listProps.Add(sTag);
                            }
                        }
                    }
                }
            }

            if (group.Groups != null)
            {
                foreach (PresentationGroup subGroup in group.Groups)
                {
                    GetAliasPropsForGroup(subGroup, ref listProps, presPropId);
                }
            }
        }

        #endregion


        public void NeilTest()
        {
            List<String> mappedFolders = new List<string>();
            mappedFolders.Add("MFolder1");
            XmlNode node = wsExSearch.EnumerateArchiveFolders(ExSearchType.Administrator);
        }

        /// <summary>
        /// Performs a search
        /// </summary>
        /// <param name="query">[in] xml query</param>
        /// <param name="scope">[in] folders to search</param>
        /// <param name="columns">[in] columns to include in the result set table</param>
        /// <param name="searchtype">[in] type of search, e.g. Administrator, Owner</param>
        /// <param name="maxHits">[in] max number of hits to return</param>
        /// <param name="bRemoveDups">[in] remove duplicates from search results</param>
        /// <param name="bEmbeddedMsgs">[in] always search embedded messages</param>
        /// <param name="timeout">[in] max time in seconds for search to run</param>
        /// <returns>queryid string uniquely identifying query</returns>
        public string Search(string query, string scope, string columns, string searchtype, int maxHits, bool bRemoveDups, bool bEmbeddedMsgs, int timeout)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sResultID = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(searchtype))
            {
                stype = ConvertType(searchtype);
            }

            try
            {
                sResultID = wsExSearch.SearchEx(query, scope, columns, null /*additionalOwners*/, null /*options*/, stype, maxHits, bRemoveDups, bEmbeddedMsgs, timeout);
            }
            catch (SoapException ex)
            {
                //ProcessSoapException(ex, Resources.SearchResStrings.Error_Search_Request);
            }
            catch(Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Search_Request);
            }
            
            return sResultID;
        }

        /// <summary>
        /// Performs a search
        /// </summary>
        /// <param name="query">[in] xml query</param>
        /// <param name="scope">[in] folders to search</param>
        /// <param name="columns">[in] columns to include in the result set table</param>
        /// <param name="searchtype">[in] type of search, e.g. Administrator, Owner</param>
        /// <param name="maxHits">[in] max number of hits to return</param>
        /// <param name="bRemoveDups">[in] remove duplicates from search results</param>
        /// <param name="bEmbeddedMsgs">[in] always search embedded messages</param>
        /// <param name="timeout">[in] max time in seconds for search to run</param>
        /// <param name="sDN">[in] distinguished name of the user to perform the search as</param>
        /// <returns>queryid string uniquely identifying query</returns>
        public string SearchAsDelegate(string query, string scope, string columns, string searchtype, int maxHits, bool bRemoveDups, bool bEmbeddedMsgs, int timeout, string sDN)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Executing search as delegate user: " + sDN);

            String sResultID = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(searchtype))
            {
                stype = ConvertType(searchtype);
            }

            try
            {
                sResultID = wsExSearch.SearchAsDelegateEx(query, scope, columns, null /*additionalOwners*/, null /*options*/, stype, maxHits, bRemoveDups, bEmbeddedMsgs, timeout, sDN);
            }
            catch (SoapException ex)
            {
                //string errorMsg = Resources.SearchResStrings.Error_Search_Request;
                ExTaskErrorCode codeName = (ExTaskErrorCode)Enum.Parse(typeof(ExTaskErrorCode), ex.Code.Name, true);

                // if the error is a permissions error, it is most likely thrown 
                // because the user does not have access to the item types he used in the search query.
                if (codeName == ExTaskErrorCode.EX_TASK_ERROR_PERMISSIONS)
                {
                    //errorMsg = Resources.SearchResStrings.Error_Delegate_Access_Denied;
                }
                //ProcessException(ex, errorMsg);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Search_Request);
            }

            return sResultID;
        }

        /// <summary>
        /// Stops a running search.
        /// </summary>
        /// <param name="queryid">[in] id of search to cancel</param>
        public void StopSearch(string queryid)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            try
            {
                if (!String.IsNullOrEmpty(queryid))
                {
                    //No scope specified, search all folders                    
                    wsExSearch.StopSearch(queryid);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Search_Stop);
            }

        }

        /// <summary>
        /// Gets a single page of items from the Result Set table
        /// </summary>
        /// <param name="queryid">[in] ID of the query</param>
        /// <param name="sortColumn">[in] Sort expression, such as RESULT_ID DESC</param>
        /// <param name="startindex">[in] Index if first element</param>
        /// <param name="pagesize">[in] Number of items requested</param>
        /// <returns>DataSet containing the requested page of results</returns>
        public DataSet GetPagedResults(String resultID, String sortColumn, Int32 startIndex, Int32 pageSize)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            DataSet retData = null;

            if (String.IsNullOrEmpty(sortColumn))
            {
                //if (HttpContext.Current.Session[SearchSessionKeys.DefaultSortColumn] != null &&
                //    HttpContext.Current.Session[SearchSessionKeys.DefaultSortDirection] != null)
                //{
                //    sortColumn = (string)HttpContext.Current.Session[SearchSessionKeys.DefaultSortColumn] +
                //                 (string)HttpContext.Current.Session[SearchSessionKeys.DefaultSortDirection];
                //}
            }

            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    retData = wsExSearch.GetPagedResults(resultID, sortColumn, startIndex, pageSize);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Paged_Results);
            }

            //if (retData != null &&
            //    retData.Tables.Count > 0 /*&&
            //    HttpContext.Current.Session[SearchSessionKeys.listRequestedColumns] == null*/)
            //{
            //    //Save the list of columns that are part of this dataset
            //    //We will use this saved list in the options page so we know what columns were requested and can warn
            //    //the user if they add columns they will have to rerun the search to see the data.
            //    ArrayList listColumns = new ArrayList(retData.Tables[0].Columns.Count);
            //    foreach (DataColumn col in retData.Tables[0].Columns)
            //    {
            //        listColumns.Add(col.ColumnName);
            //    }
            //    //HttpContext.Current.Session[SearchSessionKeys.listRequestedColumns] = listColumns;
            //}

            
            if (retData == null)
            {
                retData = new DataSet();
            }
            if (retData.Tables.Count <= 0)
            {
                DataTable tempTable = new DataTable();
                retData.Tables.Add(tempTable);
            }

            //trace.TraceInfo("Leaving GetPagedResults...");

            return retData;
        }

        /// <summary>
        /// Wrapper for GetPagedResults, used to overload default sort when paging through Errors/Failure List
        /// </summary>
        /// <param name="queryid">[in] ID of the query</param>
        /// <param name="sortColumn">[in] Sort expression, such as RESULT_ID DESC</param>
        /// <param name="startindex">[in] Index if first element</param>
        /// <param name="pagesize">[in] Number of items requested</param>
        /// <returns>DataSet containing the requested page of results</returns>
        public DataSet GetPagedResults2(String resultID, String sortColumn, Int32 startIndex, Int32 pageSize)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (String.IsNullOrEmpty(sortColumn))
            {
                sortColumn = "DATE DESC";
            }

            return GetPagedResults(resultID, sortColumn, startIndex, pageSize);
        }

        /// <summary>
        /// Returns the number of results in the current result set.
        /// </summary>
        /// <param name="queryid">[in] ID of the query</param>
        /// <returns>Number of results in the result set</returns>
        public int GetNumResults(String resultID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int numResults = 0;

            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    numResults = wsExSearch.GetNumResults(resultID);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Num_Results);
            }

            return numResults;
        }


        /// <summary>
        /// Gets the status of a Query
        /// </summary>
        /// <param name="queryid">[in] ID of the query</param>
        /// <returns>ExSearch.ExTaskStatus indicating status of query</returns>
        public ExTaskStatus GetQueryStatus(String resultID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            ExTaskStatus status = ExTaskStatus.unknown;

            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    status = wsExSearch.GetQueryStatus(resultID);
                    //trace.TraceInfo("QUERYSTATUS: " + status.ToString());
                }
            }
            catch (Exception ex)
            {
                //trace.TraceInfo("Caught and consumed exception - calling GetQueryStatus() for result ID " + resultID + ", " + ex.ToString());
            }

            return status;
        }

        /// <summary>
        /// Gets the detailed status of a Job
        /// </summary>
        /// <param name="jobid">[in] ID of the job</param>
        /// <returns>xml string indicating status of job</returns>
        public string GetStatusDetails(String jobID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            XmlNode node = null;
            string sData = String.Empty;

            try
            {
                if (!String.IsNullOrEmpty(jobID))
                {
                    node = wsExSearch.GetStatusDetails(jobID, ExTaskStatusVerbosity.EX_TASK_STATUS_ERROR);
                    if (node != null)
                    {
                        sData = node.OuterXml;
                    }
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Job_Status);
            }

            return sData;
        }

        /// <summary>
        /// Searches Exchange address book.
        /// </summary>
        /// <param name="name">[in] name or substring of name to search on, e.g. Dave, Da*</param>
        /// <param name="email">[in] email or substring of email address to search on</param>
        /// <param name="firstname">[in] name or substring of name to search on, e.g. Dave, Da*</param>
        /// <param name="lastname">[in] name or substring of name to search on, e.g. Smith, Sm*</param>
        /// <param name="company">[in] name or substring of the company to search on, e.g. EMC, EM*</param>
        /// <param name="office">[in] name or substring of the office to search on, e.g. Nashua, Nas*</param>
        /// <returns>xml document containing matching address entries</returns>
        public XmlNode SearchExchangeAddressBook(string name, string email, string firstname, string lastname, string company, string office)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            XmlNode doc = null;
            
            try
            {                
                doc = wsExSearch.SearchExchangeAddressBook(name, email, firstname, lastname, company, office);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_AddrBook);
            }
            
            return doc;
        }

        /// <summary>
        /// Searches Notes address book.
        /// </summary>
        /// <param name="name">[in] name or substring of name to search on, e.g. Dave, Da*</param>
        /// <param name="email">[in] email or substring of email address to search on</param>
        /// <param name="firstname">[in] name or substring of name to search on, e.g. Dave, Da*</param>
        /// <param name="lastname">[in] name or substring of name to search on, e.g. Smith, Sm*</param>
        /// <param name="company">[in] name or substring of the company to search on, e.g. EMC, EM*</param>
        /// <param name="office">[in] name or substring of the office to search on, e.g. Nashua, Nas*</param>
        /// <returns>xml document containing matching address entries</returns>
        public XmlNode SearchNotesAddressBook(string name, string email, string firstname, string lastname, string company, string office)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            XmlNode doc = null;

            try
            {
                doc = wsExSearch.SearchNotesAddressBook(name, email, firstname, lastname, company, office);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_AddrBook);
            }

            return doc;
        }

        /// <summary>
        /// This method enumerates the notes address books associated with the profile that was setup
        /// for the ES1 services. It does not use LDAP, and uses native notes apis.
        /// </summary>
        /// <returns>xml document containing the names and indexes of the address books</returns>
        public XmlNode EnumerateNotesAddressBooks()
        {
            XmlNode doc = null;
            
            try
            {
                doc = wsExSearch.EnumerateNotesAddressBooks();
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_AddrBook);
            }

            return doc;
        }

        /// <summary>
        /// Searches the Native Notes address book, not LDAP
        /// </summary>
        /// <param name="abIndex"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxEntries"></param>
        /// <param name="searchstr"></param>
        /// <returns></returns>
        public XmlNode GetNotesAddressBookEntries(int abIndex, int startIndex, int maxEntries, string searchstr)
        {
            XmlNode doc = null;

            try
            {
                doc = wsExSearch.GetNotesAddressBookEntries(abIndex, startIndex, maxEntries, searchstr);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_AddrBook);
            }

            return doc;
        }

        /// <summary>
        /// Resolves the email addresses for the specified input address book entries
        /// </summary>
        /// <param name="mtype"></param>
        /// <param name="entryList"></param>
        /// <returns></returns>
        public ExSearchAddrEntry[] ResolveEmailAddresses(string mtype, ExSearchAddrEntry[] entryList)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            ExSearchAddrEntry[] resolvedAddrs = null;

            try
            {
                ExMailType mailType = ConvertMailType(mtype);

                switch (mailType)
                {
                    case ExMailType.Notes:
                        resolvedAddrs = wsExSearch.ResolveEmailAddresses(mailType, entryList);
                        break;
                    case ExMailType.Exchange:
                    case ExMailType.Unknown:
                    default:
                        //trace.TraceError("Unsupported mail type: " + mailType.ToString());
                        //SearchError.SetError(Resources.SearchResStrings.Error_AddrBook);
                        break;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_AddrBook);
            }
            
            return resolvedAddrs;
        }



        /// <summary>
        /// This method returns an xml document describing all of the
        /// folders that the current user has access to. 
        /// </summary>
        /// <param name="sUserType">[in] type of search, e.g. Administrator, Owner</param>
        /// <returns>XML document describing the folder plan</returns>
        public string EnumerateArchiveFolders(string sUserType)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Enumerating archive folders as logged in user. Type: " + sUserType);

            XmlNode node = null;
            String sScope = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(sUserType))
            {
                stype = ConvertType(sUserType);
            }

            try
            {
                node = wsExSearch.EnumerateArchiveFolders2(stype, false);
                if (node != null)
                {
                    sScope = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_EnumArchiveFolder);
            }

            return sScope;
        }

        /// <summary>
        /// This method returns an xml document describing all of the
        /// folders that this delegate user has access to. 
        /// </summary>
        /// <param name="sUserType">[in] type of search, e.g. Administrator, Owner</param>
        /// <param name="sDN">[in] distinguished name of the user to enumerate folders as</param>
        /// <returns>XML document describing the folder plan</returns>
        public string EnumerateArchiveFoldersAsDelegate(string sUserType, string sDN)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Enumerating archive folders as: " + sDN + " Type: " + sUserType);

            XmlNode node = null;
            String sScope = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(sUserType))
            {
                stype = ConvertType(sUserType);
            }

            try
            {
                node = wsExSearch.EnumerateArchiveFoldersAsDelegate2(stype, sDN, false);
                if (node != null)
                {
                    sScope = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_EnumArchiveFolder);
            }

            return sScope;
        }

        /// <summary>
        /// This method returns an xml document describing all of the folders
        /// that are in the mailbox specified by the dn.
        /// </summary>
        /// <param name="dn">[in] distinguished name of mailbox we are interested in</param>
        /// <param name="stype">[in] type of mailbox [notes|exchange]</param>
        /// <returns>xml document containing folders of users mailbox</returns>
        public string EnumerateMailboxFolders(string dn, string stype)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            
            XmlNode node = null;
            String sFolders = String.Empty;

            ExMailType mailType = ConvertMailType(stype);

            //If param is empty, pass in null to use current user
            if (String.IsNullOrEmpty(dn))
            {
                dn = null;
            }
            
            try
            {
                node = wsExSearch.EnumerateMailboxFolders(dn, mailType);
                if (node != null)
                {
                    sFolders = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_EnumMailboxFolder);
            }

            return sFolders;
        }

        public string EnumerateMailboxSubFolders(string dn, string stype, string parentPath, bool topLevelOnly)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            XmlNode node = null;
            String sFolders = String.Empty;

            ExMailType mailType = ConvertMailType(stype);

            //If param is empty, pass in null to use current user
            if (String.IsNullOrEmpty(dn))
            {
                dn = null;
            }

            try
            {
                node = wsExSearch.EnumerateMailboxSubFolders(dn, mailType, parentPath, topLevelOnly);
                if (node != null)
                {
                    sFolders = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_EnumMailboxFolder);
            }

            return sFolders;
        }


        /// <summary>
        /// Select results in the specified result set given the an array
        /// of result ids.
        /// </summary>
        /// <param name="queryid">[in] the result set to modify</param>
        /// <param name="resultIds">[in] the result ids to update</param>
        /// <returns>status on update</returns>
        public string SelectResults(string queryid, int[] resultIds)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            string sResult = String.Empty;
            try
            {
                sResult = wsExSearch.SelectResults(queryid, resultIds);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Selection_Update);
            }

            return sResult;
        }


        /// <summary>
        /// Select results in the specified result set given the an array
        /// of result ids.
        /// </summary>
        /// <param name="queryid">[in] the result set to modify</param>
        /// <param name="resultIds">[in] the result ids to update</param>
        /// <returns>status on update</returns>
        public string UnselectResults(string queryid, int[] resultIds)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            string sResult = String.Empty;

            try
            {
                sResult = wsExSearch.UnselectResults(queryid, resultIds);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Selection_Update);
            }

            return sResult;
        }


        /// <summary>
        /// Select all results in a result set.
        /// </summary>
        /// <param name="queryid">[in] queryid of results to update</param>
        /// <returns>status on update</returns>
        public string SelectAllResults(string queryid)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            string sResult = String.Empty;

            try
            {
                sResult = wsExSearch.SelectAllResults(queryid);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Selection_Update);
            }

            return sResult;
        }


        /// <summary>
        /// Unselect all results in a result set.
        /// </summary>
        /// <param name="queryid">[in] queryid of results to update</param>
        /// <returns>status on update</returns>
        public string UnselectAllResults(string queryid)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            string sResult = String.Empty;

            try
            {
                sResult = wsExSearch.UnselectAllResults(queryid);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Selection_Update);
            }

            return sResult;
        }

        /// <summary>
        /// Returns the number of selections in the result set.
        /// </summary>
        /// <param name="queryid">[in] ID of the result set</param>
        /// <returns>Number of selections in the result set</returns>
        public int GetNumSelected(String resultID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int numSelections = 0;

            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    numSelections = wsExSearch.GetNumSelected(resultID);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Num_Selections);
            }

            return numSelections;
		  }

		  /// <summary>
		  /// Returns the number of selections (FILES only) in the result set
		  /// </summary>
		  /// <param name="queryid">[in] ID of the result set</param>
		  /// <returns>Number of selections in the result set</returns>
		 public int GetNumSelectedFiles(String resultID)
		  {
			  //ExSearchTrace trace = new ExSearchTrace();

			  int numSelections = 0;

			  try
			  {
				  if (!String.IsNullOrEmpty(resultID))
				  {
					  numSelections = wsExSearch.GetNumSelectedFiles(resultID);
				  }
			  }
			  catch (Exception ex)
			  {
				  //ProcessException(ex, Resources.SearchResStrings.Error_Num_Selections);
			  }

			  return numSelections;
		  }

        /// <summary>
        /// Restore selected results
        /// </summary>
        /// <param name="resultId">[in] id of result set to use as input</param>
        /// <param name="outputSource">[in] outputSource of restore. XML params identifying type, user, path, etc</param>
        /// <returns>id of restore job</returns>
        public string RestoreResults(string resultId, string outputSource)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sID = String.Empty;

            try
            {
                sID = wsExSearch.RestoreResultsEx(resultId, null /*inputSource*/, outputSource, SearchPropKeys.ENTRYID, SearchPropKeys.FOLDER, 0 /*timeout*/);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Restore_Request);
            }

            return sID;
        }

        /// <summary>
        /// Restore selected results
        /// </summary>
        /// <param name="resultId">[in] id of result set to use as input</param>
        /// <param name="outputSource">[in] outputSource of restore. XML params identifying type, user, path, etc</param>
        /// <param name="sUserType">[in] type of search, e.g. Administrator, Owner</param>
        /// <returns>id of restore job</returns>
        public string RestoreResults2(string resultId, string outputSource, string sUserType, ExTaskType taskType)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            String sID = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(sUserType))
            {
                stype = ConvertType(sUserType);
            }

            try
            {
                sID = wsExSearch.RestoreResultsEx3(resultId, null /*inputSource*/, outputSource, SearchPropKeys.ENTRYID, SearchPropKeys.FOLDER, 0 /*timeout*/, stype, null, null, taskType);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Restore_Request);
            }

            return sID;
        }

        /// <summary>
        /// Delete items from archive
        /// </summary>
        /// <param name="resultId">[in] id of result set to use as input</param>
        /// <param name="sUserType">[in] type of search, e.g. Administrator, Owner</param>
        /// <param name="inputSource">[in] inputSource of restore. XML params identifying type, item id's, etc</param>
        /// <returns>id of delete job</returns>
        public string DeleteResults(string resultId, string sUserType, string inputSource)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Deleting items as logged in user. Type: " + sUserType);

            String sID = String.Empty;

            ExSearchType stype = m_defaultUserType;
            if (!String.IsNullOrEmpty(sUserType))
            {
                stype = ConvertType(sUserType);
            }

            try
            {
                // Check to see if the Sender (i.e. From) column is currently displayed - if not, 
                // then we won't include it in the temp table:
                string senderColumnName = null;
                //ArrayList listColumns = (ArrayList)HttpContext.Current.Session[SearchSessionKeys.listRequestedColumns];
                //if (listColumns.Contains(SearchPropKeys.FROM))
                //{
                    senderColumnName = SearchPropKeys.FROM;
                //}

                sID = wsExSearch.DeleteResultsEx3(stype, resultId, inputSource, SearchPropKeys.ENTRYID, SearchPropKeys.FOLDER, SearchPropKeys.SUBJECT, SearchPropKeys.DATE, SearchPropKeys.PLATFORMTYPE, SearchPropKeys.ITEMTYPE, SearchPropKeys.SIZE, senderColumnName, 0 /*timeout*/);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Delete_Request);
            }

            return sID;
        }


        /// <summary>
        /// Retrieves all tasks for this user, (search, restore, copy, delete)
        /// </summary>
        /// <returns>Dataset containing tasks for this user.</returns>
        public DataSet EnumerateTasks()
        {
            //ExSearchTrace trace = new ExSearchTrace();

            DataSet retData = null;

            //TASKMGR:
            /* 
            try
            {
                retData = wsExSearch.EnumerateTasks();

            }
            catch (Exception ex)
            {
                ProcessException(ex, Resources.SearchResStrings.Error_Tasks_Enumerate);
            }
            */

            return retData;
        }
        

        /// <summary>
        /// Lists all the saved queries of a user.
        /// </summary>
        /// <returns>Dataset containing saved queries.</returns>
        public DataSet EnumerateQueries()
        {
            //ExSearchTrace trace = new ExSearchTrace();

            DataSet retData = null;

            try
            {
                retData = wsExSearch.EnumerateQueries();
                
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Query_Enumerate);
            }

            return retData;
        }

        /// <summary>
        /// Adds a query to the current users list of saved queries
        /// </summary>
        /// <param name="name">[in] a friendly name for the query</param>
        /// <param name="query">[in] the actual query</param>
        /// <param name="type">[in] the type of query (user,admin,uda, etc)</param>
        /// <returns>queryid of added query, 0 on failure</returns>
        public int AddQuery(string name, string type, string query)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int queryID = 0;

            try
            {
                queryID = wsExSearch.AddQuery(name, ConvertType(type), query);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Query_Save);
            }

            if (queryID == 0)
            {
                //Indicates a failure has occurred
                //trace.TraceError("An error occurred while saving the query. Query ID is 0.");
                //SearchError.SetError(Resources.SearchResStrings.Error_Query_Save);
            }

            return queryID;
        }

        //TASKMGR:
        /*
        /// <summary>
        /// Adds a query to the current users list of saved queries, and links it to a result id
        /// </summary>
        /// <param name="name">[in] a friendly name for the query</param>
        /// <param name="query">[in] the actual query</param>
        /// <param name="type">[in] the type of query (user,admin,uda, etc)</param>
        /// <param name="resultid">[in] the resultid to associate with this saved query</param>
        /// <returns>queryid of added query, 0 on failure</returns>
        public int AddQueryWithResult(string name, string type, string query, int resultid)
        {
            ExSearchTrace trace = new ExSearchTrace();

            int queryID = 0;

            try
            {
                queryID = wsExSearch.AddQueryWithResults(name, ConvertType(type), query, resultid);
            }
            catch (Exception ex)
            {
                ProcessException(ex, Resources.SearchResStrings.Error_Query_Save);
            }

            if (queryID == 0)
            {
                //Indicates a failure has occurred
                trace.TraceError("An error occurred while saving the query and linking the resultid. Query ID is 0.");
                SearchError.SetError(Resources.SearchResStrings.Error_Query_Save);
            }

            return queryID;
        }
        */

        /// <summary>
        /// Modify an existing query for the current user
        /// </summary>
        /// <param name="queryid">[in] queryid</param>
        /// <param name="name">[in] friendly name of query</param>
        /// <param name="query">[in] the actual query</param>
        /// <param name="type">[in] type of query [Administrator|Owner|Contributor|ReadAll]</param>
        /// <returns>0 on success, non-zero on failure</returns>
        public int ModifyQuery(int queryid, string name, string query, string type)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int nRet = -1;

            try
            {
                nRet = wsExSearch.ModifyQuery(queryid, name, query, ConvertType(type));
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Query_Modify);
            }

            if (nRet != 0)
            {
                //Indicates a failure has occurred
                //trace.TraceError("An error occurred while modifying the query. Query ID is 0.");
                //SearchError.SetError(Resources.SearchResStrings.Error_Query_Modify);
            }

            return nRet;
        }


        /// <summary>
        /// Gets the current users distinguished name
        /// </summary>
        /// <returns>the current users distinguished name</returns>
        public string GetUserDistinguishedName()
        {
            //ExSearchTrace trace = new ExSearchTrace();
            String sDN = String.Empty;

            try
            {
                sDN = wsExSearch.GetUserDistinguishedName();
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_GetUserProperties);
            }

            return sDN;
        }

        /// <summary>
        /// Gets the detailed status of a Job
        /// </summary>
        /// <param name="sDN">[in] Distinguished Name of the User. Pass Null to use the current user. </param>
        /// <returns>XML containing user's information. Includes DN, and Supported SearchTypes</returns>
        public string GetUserInfo(String sDN)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            XmlNode node = null;
            string sData = String.Empty;

            try
            {
                node = wsExSearch.GetUserInfo(sDN);
                if (node != null)
                {
                    sData = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Get_User_Info);
            }

            return sData;
        }


        /// <summary>
        /// Delete a saved query for the current user
        /// </summary>
        /// <param name="queryid">[in] queryid of query to delete</param>
        /// <returns>0 on success, -1 on failure</returns>
        public int DeleteQuery(int queryid)
        {
           
            //ExSearchTrace trace = new ExSearchTrace();

            int nRet = -1;

            try
            {
                nRet = wsExSearch.DeleteQuery(queryid);
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Query_Delete);
            }

            if (nRet != 0)
            {
                //Indicates a failure has occurred
                //trace.TraceError("The web service was unable to delete the query.");
                //SearchError.SetError(Resources.SearchResStrings.Error_Query_Delete);
            }

            return nRet;
        }

        
        /// <summary>
        /// Returns true if the current user has delegate access to the specified user.
        /// </summary>
        /// <param name="sDN">[in] distinguished name of the user to check delegate access for</param>
        /// <returns>Returns true if the current user has delegate access to the specified user.</returns>
        public bool HasDelegateAccess(string sDN)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            bool bHasAccess = false;

            try
            {
                bHasAccess = wsExSearch.HasDelegateAccess(sDN);
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
                bHasAccess = false;
            }

            return bHasAccess;
        }

        /// <summary>
        /// Returns saved settings for a user
        /// </summary>
        /// <returns>String (xml) containing saved settings.</returns>
        public string GetUserSettings()
        {
            //ExSearchTrace trace = new ExSearchTrace();
            string sXML = String.Empty;
            DataSet retData = null;

            try
            {
                retData = wsExSearch.GetUserSettings();
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
                //TODO - Set error for display?
            }

            //pull xml string from dataset
            if (retData != null && 
                retData.Tables != null &&
                retData.Tables[0] != null &&
                retData.Tables[0].Rows.Count > 0)
            {
                sXML = (string)(retData.Tables[0].Rows[0]["data"]);
            }

            return sXML;
        }

        /// <summary>
        /// Saves settings for the current user
        /// </summary>
        /// <param name="data">[in] the xml settings</param>
        /// <returns>id of added settings data, 0 on failure</returns>
        public int SaveUserSettings(string data)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int nId = 0;

            try
            {
                nId = wsExSearch.SaveUserSettings(data);
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
                //TODO - Set error for user display?
            }

            if (nId == 0)
            {
                //Indicates a failure has occurred
                //trace.TraceError("An error occurred while saving user settings. Returned ID is 0.");
                //TODO - Set error for user display?
            }

            return nId;
        }

        /// <summary>
        /// Returns the Last Run query for the current user
        /// </summary>
        /// <returns>Dataset containing saved query.</returns>
        public DataSet GetLastRunQuery()
        {
            //ExSearchTrace trace = new ExSearchTrace();
            string sXML = String.Empty;
            DataSet retData = null;

            try
            {
                retData = wsExSearch.GetLastRunQuery();
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
                //TODO - Set error for display?
            }

            if (retData != null &&
                retData.Tables != null &&
                retData.Tables[0] != null &&
                retData.Tables[0].Rows.Count > 0)
            {/*data is valid*/}
            else { retData = null; }

            return retData;
        }

        /// <summary>
        /// Saves the last run query for the current user
        /// </summary>
        /// <param name="type">[in] type of search Administrator|Owner|Contributor|ReadAll</param>
        /// <param name="xmlData">[in] the xml data</param>
        /// <returns>id of added query, 0 on failure</returns>
        public int SaveLastRunQuery(string type, string xmlData)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int nId = 0;

            try
            {
                nId = wsExSearch.SaveLastRunQuery(ConvertType(type), xmlData);
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
                //TODO - Set error for user display?
            }

            if (nId == 0)
            {
                //Indicates a failure has occurred
                //trace.TraceError("An error occurred while saving the LastRunQuery. Returned ID is 0.");
                //TODO - Set error for user display?
            }

            return nId;
        }

        /// <summary>
        /// Returns the number of selections (FILES only) in the result set
        /// </summary>
        /// <param name="queryid">[in] ID of the result set</param>
        /// <param name="platformFieldName">[in] the field name of platform type at database table 'result_*'</param>
        /// <returns>Number of selections in the result set</returns>
        public List<String> GetPlatformTypesOfSelectedItems(String resultID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            List<String> platformTypes = new List<string>();
            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    String[] result = wsExSearch.GetPlatformTypesOfSelectedItems(resultID, SearchPropKeys.PLATFORMTYPE);
                    platformTypes.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Num_Selections);
            }

            return platformTypes;
        }

        /// <summary>
        /// Returns a list of folders selected items belong to
        /// </summary>
        /// <param name="queryid">[in] the result set to check</param>
        /// <returns>a list of folders of selected items</returns>
        public List<String> GetFoldersofSelectedFiles(String resultID)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            List<String> folders = new List<string>();
            try
            {
                if (!String.IsNullOrEmpty(resultID))
                {
                    String[] result = wsExSearch.GetFoldersofSelectedFiles(resultID);
                    folders.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_Num_Selections);
            }

            return folders;
        }

        /// <summary>
        /// Returns whether the given folder is virtual or not
        /// </summary>
        /// <param name="folderName">[in] the map folder name</param>
        /// <returns>true if the given folder is virtual</returns>
        public Boolean IsVirtualFolder(String folderName)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            Boolean IsVirtual = false;
            try
            {
                if (!String.IsNullOrEmpty(folderName))
                {
                    IsVirtual = wsExSearch.IsVirtualFolder(folderName);
                }
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
            }

            return IsVirtual;
        }

        /// <summary>
        /// Returns whether the given folder is organization or not
        /// </summary>
        /// <param name="folderName">[in] the map folder name</param>
        /// <returns>true if the given folder is organization</returns>
        public Boolean IsOrganization(String folderName)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            Boolean IsOrganization = false;
            try
            {
                if (!String.IsNullOrEmpty(folderName))
                {
                    IsOrganization = wsExSearch.IsOrganization(folderName);
                }
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
            }

            return IsOrganization;
        }

        /// <summary>
        /// Release the results associated with this id.
        /// </summary>
        /// <param name="resultid">[in] resultid</param>
        public void ReleaseResults(string resultid)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            try
            {
                wsExSearch.ReleaseResults(resultid);                
            }
            catch (Exception ex)
            {
                ProcessException(ex, null);
            }
        }

        /// <summary>
        /// This method retrieves the configuration information for the web app server
        /// </summary>
        /// <returns>xml data document containing search server configuration</returns>
        public string GetServerConfiguration()
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Retrieving server configuration from web service.");

            XmlNode node = null;
            String sXmlVal = String.Empty;

            try
            {
                node = wsExSearch.GetServerConfiguration();
                if (node != null)
                {
                    sXmlVal = node.OuterXml;
                }
            }
            catch (WebException wex)
            {
                //if(wex.Message.Contains("401"))
                    //ProcessException(wex, Resources.SearchResStrings.Error_Auth_WebService);
                //else
                    //ProcessException(wex, Resources.SearchResStrings.Error_GetServerConfig);

            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_GetServerConfig);
            }

            return sXmlVal;
        }

        private ExSearchType ConvertType(String inputType)
        {
            ExSearchType searchtype = ExSearchType.Administrator;

            if (0 == (String.Compare(inputType, SearchTypeKeys.Owner, true)))
            {
                searchtype = ExSearchType.Owner;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.Administrator, true)))
            {
                searchtype = ExSearchType.Administrator;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.Contributor, true)))
            {
                searchtype = ExSearchType.Contributor;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.ReadAll, true)))
            {
                searchtype = ExSearchType.ReadAll;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.ACL, true)))
            {
                searchtype = ExSearchType.ACL;
            }
            return searchtype;
        }


        private ExMailType ConvertMailType(String inputType)
        {
            ExMailType mailType = ExMailType.Unknown;

            if (0 == (String.Compare(inputType, SearchMailEnv.NOTES, true)))
            {
                mailType = ExMailType.Notes;
            }
            else if (0 == (String.Compare(inputType, SearchMailEnv.EXCHANGE, true)))
            {
                mailType = ExMailType.Exchange;
            }
            return mailType;
        }

        ////Function to log error, then process info status messages
        //private void ProcessSoapException(SoapException ex, string defaultUserMsg)
        //{
        //    //ExSearchTrace trace = new ExSearchTrace();
        //    //trace.TraceError(ex.ToString());
            
        //    if (ex.Code != null &&
        //        !String.IsNullOrEmpty(ex.Code.Name))
        //    {
        //        string sUserType = String.Empty;
        //        if (HttpContext.Current.Session[SearchSessionKeys.sUserType] != null)
        //        {
        //            sUserType = (string)HttpContext.Current.Session[SearchSessionKeys.sUserType];
        //        }
        //        SearchError.SetInfoFromTaskCode(ex.Code.Name, defaultUserMsg, sUserType);
        //    }
        //    else
        //    {
        //        //Show generic Error message
        //        SearchError.SetInfo(defaultUserMsg);            
        //    }
        //}

        private void ProcessException(Exception ex, string defaultUserMsg)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            
            //trace.TraceError(ex.ToString());

            if (!String.IsNullOrEmpty(defaultUserMsg))
            {
                //SearchError.SetError(defaultUserMsg);
            }
        }

        /// <summary>
        /// This method returns an ExXmlFile document identified by the input parameter
        /// </summary>
        /// <param name="fileType">[in] the Xml file to retrieve</param>
        /// <returns>the requested Xml document</returns>
        public string GetXmlFile(ExXmlFile fileType)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //trace.TraceInfo("Retrieving Xml file: " + fileType.ToString());

            XmlNode node = null;
            String sXml = String.Empty;

            try
            {
                node = wsExSearch.GetXmlFile(fileType);
                if (node != null)
                {
                    sXml = node.OuterXml;
                }
            }
            catch (Exception ex)
            {
                //ProcessException(ex, Resources.SearchResStrings.Error_GetXmlFile);
            }

            return sXml;
        }
    };


    



}
