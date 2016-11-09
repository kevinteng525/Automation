using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExSTLContainers;

namespace ES1.ES1SPAutoLib
{
    public class ADOperator
    {

        /*public static String BuildQueryForEmailAddress(String emailAddress, EmailAddressType mType, EmailAddressFormatType fType)
        {
            String objectClass;
            String exfilter;


            switch (mType)
            {
                case EmailAddressType.MailBox:
                    objectClass = "user"; break;
                case EmailAddressType.DistributeList:
                    objectClass = "group"; break;
                default:
                    throw new Exception("not support email address type");
            }

            switch (fType)
            {
                case EmailAddressFormatType.EX:
                    exfilter = "legacyExchangeDN=" + emailAddress;
                    break;
                case EmailAddressFormatType.SMTP:
                    exfilter = "mail=" + emailAddress;
                    break;
                default:
                    throw new Exception("not supported email address  format type");
            }

            String filter = "(&(objectClass=" + objectClass + ")(" + exfilter + "))";
            return filter;
        }

        public static String BuildQueryForADObject(String objectName, ADObjectType adObjectType)
        {
            String queryString;
            switch (adObjectType)
            {
                case ADObjectType.User:
                    queryString = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                    break;
                case ADObjectType.Group:
                    queryString = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
                default:
                    throw new Exception("not supported adobject type for searchObjFromForest");
            }
            return queryString;
        }*/
        private static String BuildGuid(byte[] guid)
        {
            String id = "{";
            char[] hex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            for (int i = 3; i >= 0; --i) id = id + hex[guid[i] / 16] + hex[guid[i] % 16];
            id += "-";
            for (int i = 5; i >= 4; --i) id = id + hex[guid[i] / 16] + hex[guid[i] % 16];
            id += "-";
            for (int i = 7; i >= 6; --i) id = id + hex[guid[i] / 16] + hex[guid[i] % 16];
            id += "-";
            for (int i = 8; i <= 9; ++i) id = id + hex[guid[i] / 16] + hex[guid[i] % 16];
            id += "-";
            for (int i = 10; i <= 15; ++i) id = id + hex[guid[i] / 16] + hex[guid[i] % 16];
            id += "}";
            return id;
        }

        public static String GetProperty(SearchResult searchResult, String propertyName)
        {
            if (propertyName.ToLower().Equals("objectguid"))
                return BuildGuid((byte[])searchResult.Properties[propertyName][0]);
            return searchResult.Properties.Contains(propertyName) ? searchResult.Properties[propertyName][0].ToString() : string.Empty;
        }

        public static SearchResult ExecuteLDAPQueryFromLDAPServer(String ldapQueryString)
        {
            String ldapPath = "LDAP://" + Configuration.ADServer;

            DirectoryEntry entry = new DirectoryEntry(ldapPath, Configuration.ADUserName, Configuration.ADUserPassword);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = ldapQueryString;
            SearchResultCollection results = dSearch.FindAll();
            if (results.Count > 0)
                return results[0];
            else
                return null;
        }
        /*
        public static SearchResultCollection ExecuteLDAPQueryFromForest(String ldapQueryString, String forestName, String loginAccountName, String passowrd)
        {
            DirectoryContext dct;
            if (String.IsNullOrEmpty(loginAccountName))
            {
                dct = new DirectoryContext(DirectoryContextType.Forest, forestName);
            }
            else
            {
                dct = new DirectoryContext(DirectoryContextType.Forest, forestName, loginAccountName, passowrd);

            }
            string gcPath;
            try
            {
                Forest forest = Forest.GetForest(dct);
                GlobalCatalog gc = forest.FindGlobalCatalog();
                gcPath = "GC://" + gc.Name;
            }
            catch
            {
                gcPath = "GC://" + forestName;
            }
            
            DirectoryEntry entry = new DirectoryEntry(gcPath);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = ldapQueryString;
            SearchResultCollection results = dSearch.FindAll();
            return results;
        }


        public static IExDataSource FillDataSourceFromSearchResult(SearchResult result, IExDataSource dataSource, bool isInfoOnExchangeNeed)
        {
            DirectoryEntry directoryEntry = result.GetDirectoryEntry();

            exDataSourceType dataSourceType = exDataSourceType.exDataSourceType_Undefined;
            String friendlyName = "";

            if (directoryEntry.Properties.Contains("objectClass"))
            {
                object[] objType = (object[])directoryEntry.Properties["objectClass"].Value;

                if (objType == null)
                {
                    dataSourceType = exDataSourceType.exDataSourceType_Undefined;
                }
                else
                {
                    foreach (string s in objType)
                    {
                        if (s.Equals("user"))
                        {
                            dataSourceType = exDataSourceType.exDataSourceType_Mailbox;
                            friendlyName = directoryEntry.Properties["sAMAccountName"].Value.ToString();
                            break;
                        }
                        if (s.Equals("group"))
                        {
                            dataSourceType = exDataSourceType.exDataSourceType_DistList;
                            friendlyName = directoryEntry.Properties["cn"].Value.ToString();
                            break;
                        }
                    }
                }
            }

            dataSource.type = dataSourceType;

            if (directoryEntry.Properties.Contains("distinguishedName"))
            {
                dataSource.nativeName = directoryEntry.Properties["distinguishedName"].Value.ToString();
            }
            else
            {
                throw new Exception("There is not distinguishedName found for this object");
            }
            

            if (isInfoOnExchangeNeed && (dataSourceType == exDataSourceType.exDataSourceType_Mailbox))
            {
                if (directoryEntry.Properties.Contains("legacyExchangeDN"))
                {
                    dataSource.dataSourceName = directoryEntry.Properties["legacyExchangeDN"].Value.ToString();
                }
                else
                {
                    dataSource.dataSourceName = directoryEntry.Properties["distinguishedName"].Value.ToString();
                }

                if (directoryEntry.Properties.Contains("msExchHomeServerName"))
                    dataSource.server = directoryEntry.Properties["msExchHomeServerName"].Value.ToString();
            }
            else
            {
                dataSource.dataSourceName = dataSource.nativeName;
            }

            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Exchange;
            dataSource.nativeObjectID = "{" + directoryEntry.Guid.ToString().ToUpper() + "}";
            dataSource.friendlyName = friendlyName;


            dataSource.state = 0;


            return dataSource;
        }


        public static IExDataSource GetDataSourceFromADObject(String objectName, ADObjectType objectType, String serverOrForestName, String loginAccountName, String password, QueryOptions queryOptions)
        {
            String queryString = BuildQueryForADObject(objectName, objectType);
            SearchResultCollection results;
            switch (queryOptions)
            {
                case QueryOptions.FromForest:
                    results = ExecuteLDAPQueryFromForest(queryString, serverOrForestName, loginAccountName, password);
                    break;
                case QueryOptions.FromLDAPServer:
                    results = ExecuteLDAPQueryFromLDAPServer(queryString, serverOrForestName, loginAccountName, password);
                    break;
                default:
                    throw new Exception("not supported query options, only forest and ldap are supported");
            }

            if (results == null || results.Count <= 0)
            {
                throw new NullReferenceException("unable to find the AD object");
            }

            IExDataSource dataSource = DataSourceManager.PrepareDataSource();
            FillDataSourceFromSearchResult(results[0], dataSource, true);

            return dataSource;

        }

        public static IExDataSource GetDataSourceFromEmailAddress(String emailAddress, EmailAddressType mType, EmailAddressFormatType fType, String serverOrForestName, QueryOptions queryOptions)
        {
            String queryString = BuildQueryForEmailAddress(emailAddress, mType, fType);
            SearchResultCollection results;
            switch (queryOptions)
            {
                case QueryOptions.FromForest:
                    results = ExecuteLDAPQueryFromForest(queryString, serverOrForestName, null, null);
                    break;
                case QueryOptions.FromLDAPServer:
                    results = ExecuteLDAPQueryFromLDAPServer(queryString, serverOrForestName, null, null);
                    break;
                default:
                    throw new Exception("not supported query options, only forest and ldap are supported");
            }

            if (results == null || results.Count <= 0)
            {
                throw new NullReferenceException("unable to find the AD object");
            }

            IExDataSource dataSource = DataSourceManager.PrepareDataSource();

            FillDataSourceFromSearchResult(results[0], dataSource, true);
            return dataSource;
        }

        public static IExDataSource GetADUserDataSource(String userName, String forestName)
        {
            return GetDataSourceFromADObject(userName, ADObjectType.User, forestName, null, null, QueryOptions.FromForest);
        }

        public static IExDataSource GetADGroupDataSource(String groupName, String forestName)
        {
            return GetDataSourceFromADObject(groupName, ADObjectType.Group, forestName, null, null, QueryOptions.FromForest);
        }*/

        /*public static IExDataSource GetMailBoxDataSource(String emailAddress, EmailAddressFormatType mType, String forestName)
        {
            return GetDataSourceFromEmailAddress(emailAddress, EmailAddressType.MailBox, mType, forestName,
                                                 QueryOptions.FromForest);
        }

        public static IExDataSource GetDistListDataSource(String emailAddress, EmailAddressFormatType mType, String forestName)
        {
            return GetDataSourceFromEmailAddress(emailAddress, EmailAddressType.DistributeList, mType, forestName,
                                                 QueryOptions.FromForest);
        }

        public static IExDataSource GetLDAPQueryDataSource(String ldapQueryString, String ldapServerName)
        {
            return GetLDAPQueryDataSource(ldapQueryString, ldapServerName, null, null);
        }

        public static IExDataSource GetLDAPQueryDataSource(String ldapQueryString, int ldapServerId)
        {
            IExDataSource dataSource = DataSourceManager.PrepareDataSource();
            dataSource.type = exDataSourceType.exDataSourceType_LDAPQuery;
            dataSource.dataSourceName = ldapQueryString;
            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Unknown;
            IExVector ldapServerIds = new CoExVectorClass();
            ldapServerIds.Add(ldapServerId);
            dataSource.LDAPServerIDs = ldapServerIds;
            return dataSource;
        }

        public static IExDataSource GetLDAPQueryDataSource(String ldapQueryString, String ldapServerName,String loginAccounName,String password)
        {
            IExLDAPServer ldapServer = DataSourceManager.GetLDAPSererResourceByServerName(ldapServerName);

            if (ldapServer == null)
            {
                ldapServer = DataSourceManager.CreateLDAPServerResource(ldapServerName, loginAccounName, password);
            }
            return GetLDAPQueryDataSource(ldapQueryString, ldapServer.id);
        }

        public static IExDataSource GetLDAPQueryDataSource(String ldapQueryString, String ldapServerName, 
                                                           string address, String loginAccounName, 
                                                           String password, string searchBase)
        {
            //modified by wayne
            //IExLDAPServer ldapServer = DataSourceManager.GetLDAPSererResourceByServerName(ldapServerName);

            //if (ldapServer == null)
            //{
            //    ldapServer = DataSourceManager.CreateLDAPServerResource(ldapServerName, loginAccounName, password, searchBase);
            //}
            IExLDAPServer ldapServer = DataSourceManager.CreateLDAPServerResource(ldapServerName, address, loginAccounName, password, searchBase);

            return GetLDAPQueryDataSource(ldapQueryString, ldapServer.id);
        }

        public static List<IExDataSource> GetDataSourcesByQueryLDAP(IExLDAPServer ldapServerResource, String ldapQueryString)
        {
            return GetDataSourcesByQueryLDAP(ldapServerResource.name, ldapServerResource.accountName,
                                             ldapServerResource.accountPassword, ldapQueryString);
        }

        public static List<IExDataSource> GetDataSourcesByQueryLDAP(String ldapServerName, String loginAccountName, String password, String ldapQueryString)
        {
            List<IExDataSource> dataSources = new List<IExDataSource>();
            SearchResultCollection queryResults =
                ExecuteLDAPQueryFromLDAPServer(ldapQueryString, ldapServerName, loginAccountName, password);

            foreach (SearchResult result in queryResults)
            {
                IExDataSource dataSource = DataSourceManager.PrepareDataSource();
                FillDataSourceFromSearchResult(result, dataSource, true);
                dataSources.Add(dataSource);
            }
            return dataSources;
        }
        public static List<IExDataSource> GetDataSourcesByQueryLDAP(String ldapServerName,String ldapQueryString)
        {
            return GetDataSourcesByQueryLDAP(ldapServerName, null,null, ldapQueryString);
        }*/
    }
}