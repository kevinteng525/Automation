///////////////////////////////////////////////////////////////////////////////////////
//		Copyright © 1998 - 2007 EMC Corporation. All rights reserved.
//		This software contains the intellectual property of EMC Corporation
//		or is licensed to EMC Corporation from third parties. Use of this software
//		and the intellectual property contained therein is expressly limited to
//		the terms and conditions of the License Agreement under which it is
//		provided by or on behalf of EMC.
//						  EMC Corporation,
//					      176 South St.,
//					  Hopkinton, MA  01748.
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace Saber.S1CommonAPILib.S1SearchWrapper
{
    /// <summary>
    /// Keys for storing Session objects
    /// </summary>
    public static class SearchSessionKeys
    {
        public const string sUserType = "sUserType";
        public const string sMailEnv = "sMailEnv";
        public const string sDelegateDN = "sDelegateDN";
        public const string sDelegateDisplayName = "sDelegateDisplayName";
        public const string sUserDN = "sUserDN";
        public const string sXmlSearchCriteria = "sXmlSearchCriteria";
        public const string sErrorMsg = "sErrorMsg";
        public const string sInfoMsgs = "sInfoMsgs";
        public const string listSearchFolderScopeIDs = "listSearchFolderScopeIDs";
        public const string sSearchFolderScopeNames = "sSearchFolderScopeNames";
        public const string SearchFieldKeyword = "SearchFieldKeyword";
        public const string SearchFieldKeywordNew = "SearchFieldKeywordNew";
        public const string bLoadQuery = "bLoadQuery";
        public const string bUpdateSearchPane = "bUpdateSearchPane";
        public const string SearchFieldControlList = "SearchFieldControlList";
        public const string credentials = "credentials";
        public const string currCopyId = "currCopyId";
        public const string currRestoreId = "currRestoreId";
        public const string currDeleteId = "currDeleteId";
        public const string currResultId = "currResultId";
        public const string currNumResults = "currNumResults";
        public const string downloadAttachIndex = "downloadAttachIndex";
        public const string downloadItemMsgId = "downloadItemMsgId";
        public const string downloadItemFolder = "downloadItemFolder";
        public const string downloadItemPlatformType = "downloadItemPlatformType";
        public const string downloadInProgress = "downloadInProgress";
        public const string downloadForceNative = "EnableOneTimeNativeDownload";
        public const string bSearching = "bSearching";
        public const string bStretchColumns = "bStretchColumns";
        public const string bAlwaysSearchEmbeddedMsgs = "bAlwaysSearchEmbeddedMsgs";
        public const string bRemoveDuplicates = "bRemoveDuplicates";
        public const string bDupsRemovedFromLastSearch = "bDupsRemovedFromLastSearch";
        public const string numItemsPerPage = "numItemsPerPage";
        public const string maxNumItemsPerPage = "maxNumItemsPerPage";
        public const string numMaxResults = "numMaxResults";
        public const string numMaxMaxResults = "numMaxMaxResults";
        public const string numTimeoutMinutes = "numTimeoutMinutes";
        public const string bShowSearchFields = "bShowSearchFields";
        public const string bSelectAll = "bSelectAll";
        public const string bPromptOverwriteQuery = "bPromptOverwriteQuery";
        public const string numStatusRefreshInterval = "numStatusRefreshInterval";
        public const string numSearchRefreshInterval = "numSearchRefreshInterval";
        public const string numSearchPollCount = "numSearchPollCount";
        public const string numSearchRefreshIterations = "numSearchRefreshIterations";
        public const string numSearchRefreshIntervalContinue = "numSearchRefreshIntervalContinue";
        public const string listVisibleColumns = "listVisibleColumns";
        public const string listRequestedColumns = "listRequestedColumns";
        public const string dominoDirectoryIndex = "dominoDirectoryIndex";
        public const string previewLocation = "previewLocation";
        public const string tasksLocation = "tasksLocation";
        public const string currDetailsId = "currDetailsId";
        public const string TraceVerbosity = "TraceVerbosity";
        public const string currObjectTypeId = "currObjectTypeId";
        public const string currObjectColViewId = "currObjectColViewId";
        public const string currObjectCriteriaViewId = "currObjectCriteriaViewId";
        public const string PresentationModel = "PresentationModel";
        public const string DefaultSortColumn = "DefaultSortColumn";
        public const string DefaultSortDirection = "DefaultSortDirection";
        public const string bShowClickFinishPrompt = "bShowClickFinishPrompt";
        public const string bDirtyNumItemsPerPage = "bDirtyNumItemsPerPage";
        public const string bDirtyNumMaxResults = "bDirtyNumMaxResults";
        public const string listSupportedSearchTypes = "listSupportedSearchTypes";
        public const string dictAddressBooks = "dictAddressBooks";
        public const string bSearchFieldControlChanged = "bSearchFieldControlChanged";
        public const string currResultSelections = "currResultSelections";
        public const string currSearchJobId = "currSearchJobId";
        public const string bAuditedUser = "auditedUser";
        public const string bGridResultsColumnOrderChanged = "bGridResultsColumnOrderChanged";
        public const string dtAddressBookResults = "dtAddressBookResults";
        public const string dictObjectColumnOrders = "dictObjectColumnOrders";
        public const string bSupportJava = "bSupportJava";
        public const string htAuthorizationGroups = "htAuthorizationGroups";
        public const string currRestorePlatformType = "currRestorePlatformType";
        public const string browserCPU = "browserCPU";
    }

    /// <summary>
    /// Keys for storing Session objects
    /// </summary>
    public static class SearchApplicationKeys
    {
        public const string EnableAutoUpdateOfWebConfigSettings = "EnableAutoUpdateOfWebConfigSettings";
        public const string ExSearchWebServiceURL = "ExSearchWebServiceURL";
        public const string ExDocRetrievalServerName = "ExDocRetrievalServerName";
        public const string DefaultMaxResults = "DefaultMaxResults";
        public const string MaxResults = "MaxResults";
        public const string DefaultNumItemsPerPage = "DefaultNumItemsPerPage";
        public const string MaxNumItemsPerPage = "MaxNumItemsPerPage";
        public const string ApplicationSessionTimeout = "ApplicationSessionTimeout";
        public const string MaxSavedQueriesPerUser = "MaxSavedQueriesPerUser";
        public const string DefaultSearchTimeout = "DefaultSearchTimeout";
        public const string RestrictedAttachExtList = "RestrictedAttachExtList";
        public const string DefaultSearchType = "DefaultSearchType";
        public const string DefaultMailEnvironment = "DefaultMailEnvironment";
        public const string SupportedEnvironments = "SupportedEnvironments";
        public const string PreviewCacheTimeoutSeconds = "PreviewCacheTimeoutSeconds";
        public const string ExDocRetrievalServerPort = "ExDocRetrievalServerPort";
        public const string ExDocRetrievalServerBindingType = "ExDocRetrievalServerBindingType";
        public const string DefaultShowSearchFields = "DefaultShowSearchFields";
        public const string DefaultRemoveDuplicatesFromSearch = "DefaultRemoveDuplicatesFromSearch";
        public const string DefaultAlwaysSearchEmbeddedMsgs = "DefaultAlwaysSearchEmbeddedMsgs";
        public const string EnableTaskMgr = "EnableTaskMgr";
        public const string EnableONMViewerWebDownload = "EnableONMViewerWebDownload";
        public const string EnableNativeDominoDirectory = "EnableNativeDominoDirectory";
        public const string ONMViewerWebDownloadFilePath = "ONMViewerWebDownloadFilePath";
        public const string listHelpLanguageCodes = "listHelpLanguageCodes";
        public const string EnableSearchTypeAdmin = "EnableSearchTypeAdmin";
        public const string StatusRefreshIntervalSeconds = "StatusRefreshIntervalSeconds";
        public const string SearchRefreshIntervalSecondsStart = "SearchRefreshIntervalSecondsStart";
        public const string SearchRefreshIterationsBeforeIncrease = "SearchRefreshIterationsBeforeIncrease";
        public const string SearchRefreshIntervalSecondsContinue = "SearchRefreshIntervalSecondsContinue";
        public const string UsePersistentFormsAuthCookie = "UsePersistentFormsAuthCookie";
        public const string TraceVerbosity = "TraceVerbosity";
        public const string PresentationXml = "PresentationXml";
        public const string NPMXml = "NPMXml";
        public const string ExtensibilityXml = "ExtensibilityXml";
        public const string LoadExtensibilityXml = "LoadExtensibilityXml";
        public const string DefaultObjectTypeId = "DefaultObjectTypeId";
        public const string DefaultObjectColViewId = "DefaultObjectColViewId";
        public const string DefaultObjectCriteriaViewId = "DefaultObjectCriteriaViewId";
        public const string PresentationModelXml = "PresentationModelXml";
        public const string DefaultDateRangeEnabled = "DefaultDateRangeEnabled";
        public const string DefaultDateRangeOnOrBeforeDaysAgo = "DefaultDateRangeOnOrBeforeDaysAgo";
        public const string DefaultDateRangeOnOrAfterDaysAgo = "DefaultDateRangeOnOrAfterDaysAgo";
        public const string DisableAdminPreview = "DisableAdminPreview";
        public const string DisableAdminOpen = "DisableAdminOpen";
        public const string DefaultMappedFolderList = "DefaultMappedFolderList";
        public const string LoadLocalNPMFileName = "LoadLocalNPMFileName";
        public const string LoadLocalPXMLFileName = "LoadLocalPXMLFileName";
        public const string EnableDelegateSearch = "EnableDelegateSearch";
        public const string EnableAdditionalSearchErrorInfo = "EnableAdditionalSearchErrorInfo";
        public const string RestoreFoldersLoadOnDemand = "RestoreFoldersLoadOnDemand";
        public const string EnableNativePlatformDownload = "EnableNativePlatformDownload";
        public const string EnableContributorToMyItemsStringReplacement = "EnableContributorToMyItemsStringReplacement";
        public const string DisableNoCriteriaConfirmationPrompt = "DisableNoCriteriaConfirmationPrompt";
        public const string UserDeleteAllowedRangeDays = "UserDeleteAllowedRangeDays";
        public const string EnableOpenMIMENative = "EnableOpenMIMENative";
        public const string EnableRestoreToMailbox = "EnableRestoreToMailbox";
        public const string EnableOptions = "EnableOptions";
        public const string EnableSearchFolderPicker = "EnableSearchFolderPicker";

        // agadzik - 1/22/2010 - The virtual path for the SearchFieldControl.ascx control.  This is needed for the LoadControl method used through the porject
        public const string SearchFieldControlVirtualPath = "SearchFieldControl.ascx";

        //maojun - EMAIL-3271 - clear shared authentication cache for all applications in current IE session
        public const string ClearSharedAuthenticationCache = "ClearSharedAuthenticationCache";
        public const string EnableFileExport = "EnableFileExport";
    }

    /// <summary>
    /// Mail Environments
    /// </summary>
    public static class SearchMailEnv
    {
        public const string NOTES = "NOTES";
        public const string EXCHANGE = "EXCHANGE";
        public const string SMTP = "SMTP";
        public const string SOCS = "SOCS";
    }

    /// <summary>
    /// Search Types
    /// </summary>
    public static class SearchTypeKeys
    {
        public const string Administrator = "Administrator";
        public const string Owner = "Owner";
        public const string Contributor = "Contributor";
        public const string ReadAll = "ReadAll";
        public const string ACL = "ACL";
        public const string UserDelete = "UserDelete";
        public const string AuditedUser = "AuditedUser";
    }

    ///// <summary>
    ///// different task types for search table
    ///// </summary>
    //public enum ExTaskType
    //{
    //    exUnknown = 0,
    //    exQueryJob = 8,
    //    exRestoreJob = 17,
    //    exDeleteFromArchiveJob = 21,
    //    exDiscoQueryJob = 31,
    //    exRestoreFilesJob = 44,
    //}

    public enum ExTaskRestoreSubType
    {
        exUnknown = 0,
        exRestoreToMailbox = 1,
        exCopyToFolder = 2,
        exExport = 3,
    }

    /// <summary>
    /// Property Keys (not complete), these ones are defined because they need special processing logic
    /// </summary>
    public static class SearchPropKeys
    {
        public const string USERSELECT = "USERSELECT";
        public const string RESULTID = "RESULT_ID";
        public const string FULLTEXT = "FULLTEXT";
        public const string SUMMARY = "SUMMARY";
        public const string SENDERORRECIPIENT = "SENDERORRECIPIENT";
        public const string ANYUNINDEXEDCONTENT = "ANYUNINDEXEDCONTENT";
        public const string ENTRYID = "ID__0__1__EntryID";
        public const string DATE = "ID__0__4__Date";
        public const string FOLDER = "ID__0__6__Folder";
        public const string KEYWORD = "ID__0__10__Keyword";
        public const string OWNER = "ID__0__11__Owner";
        public const string RECIPIENT = "ID__0__26__Recipient";
        public const string SENDER = "ID__0__27__Sender";
        public const string ITEMTYPE = "ID__0__1056__Class";
        public const string PLATFORMTYPE = "ID__0__21__PlatformType";
        public const string SOURCELOCATION = "ID__0__58__SourceLocation";
        public const string PWDPROTECTEDATTACH = "ID__0__28__PasswordProtectedAttachment";
        public const string ENCRYPTED = "ID__0__5__Encrypted";
        public const string INDEXINGERROR = "ID__0__29__IndexingError";
        public const string SIZE = "ID__0__2__Size";
        public const string SENDER_DOMAIN = "ID__0__37__SenderDomain";
        public const string RECIPIENT_DOMAIN = "ID__0__36__RecipientDomain";


        //Needed for Preview:
        public const string SUBJECT = "ID__0__20__Subject";
        public const string FROM = "ID__0__1060__From";
        public const string TO = "ID__0__1061__To";
        public const string CC = "ID__0__1062__CC";
        public const string BCC = "ID__0__1063__BCC";
        public const string BODY = "ID__0__1065__Body";
        //DATE also needed for preview but defined above


        public const string PLATFORMTYPE_DATA = "PLATFORMTYPE_DATA";
        public const string ITEMTYPE_DATA = "ITEMTYPE_DATA";

    }

    /// <summary>
    /// Property IDs (not complete), these ones are defined because they need special processing logic
    /// </summary>
    public static class SearchPropIDs
    {
        //Needed for Preview:
        public const int ATTACHMENTS = 1073;
        public const int SIZE = 2;
        //Needed for Result Column Display
        public const int ITEMTYPE = 1056;
        public const int SOURCELOCATION = 58;
        public const int CREATIONDATE = 53;
        //File attributes
        public const int FILEATTRIBUTES = 3007;
        public const int LASTACCESSTIME = 3005;
    }

    /// <summary>
    /// Status Details Column Keys
    /// </summary>
    public static class SearchStatusDetailsKeys
    {
        public const string RESULT_ID = "RESULT_ID";
        public const string ENTRY_ID = "ENTRY_ID";
        public const string SUMMARY = "SUMMARY";
        public const string DATE = "DATE";
        public const string DESTINATION = "DESTINATION";
        public const string TASK_ERROR = "TASK_ERROR";
        public const string CODE = "CODE";
        public const string COMMENT = "COMMENT";
        public const string DESCRIPTION = "DESCRIPTION";
    }

    /// <summary>
    /// Object Keys (not complete), these ones are defined because they need special processing logic
    /// </summary>
    public static class SearchObjectKeys
    {
        public const string EVERYTHING = "All";
        public const string EMAIL = "65";
        public const string DOCUMENT = "73";
        public const string FILE = "3000";
    }

    /// <summary>
    /// Platform Types
    /// </summary>
    public static class SearchPlatformTypeKeys
    {
        public const string SMTP = "49";
        public const string EXCHANGE = "50";
        public const string NOTES = "51";
        public const string SHAREPOINT = "52";
        public const string FILES = "53";
    }

    /// <summary>
    /// Object Keys (not complete), these ones are defined because they need special processing logic
    /// </summary>
    public static class SearchUserSettings
    {
        public const string USERSETTINGS = "UserSettings";
        public const string VERSION = "version";
        public const string RESULTVIEWS = "ResultViews";
        public const string OBJECT = "Object";
        public const string OBJECTID = "oID";
        public const string VIEW = "View";
        public const string VIEWID = "vID";
        public const string PROP = "Prop";
        public const string PRESPROPID = "presID";
        public const string PROPID = "pID";
        public const string VISIBILITY = "vis";
        public const string RESULTINDEX = "idx";
    }

    /// <summary>
    /// Exception Strings
    /// </summary>
    public static class SearchExceptionStrings
    {
        public const string NoCriteriaSpecified = "NoCriteriaSpecified";
    }

    public static class SearchSavedQueryVersion
    {
        public const string XMLSCHEMAVERSIONNAME = "version";
        public const int XMLSCHEMAVERSION = 2;
    }

    public static class SearchAddressBookXmlConstants
    {
        public const string TYPE = "TYPE";
        public const string DISPLAYNAME = "DISPLAYNAME";
        public const string PHONE = "PHONE";
        public const string OFFICE = "OFFICE";
        public const string COMPANY = "COMPANY";
        public const string EMAIL = "EMAIL";
        public const string DN = "DN";
        public const string INDEX = "INDEX";
        public const string DOCUMENTID = "DOCUMENTID";
    }

    /// <summary>
    /// Added for verification of Telerik control license
    /// </summary>
    public static class SearchUIControls
    {
        /// <summary>
        /// This string must match Telerik.Web.AssemblyProtection.ApplicationName
        /// for proper display / operation of Telerik controls!!
        /// </summary>
        public const string TelerikApplicationName = "EMC SourceOne Search";
    }

    public enum SearchRetrieveType
    {
        Preview = 0,
        Retrieve = 1,
    }
}
