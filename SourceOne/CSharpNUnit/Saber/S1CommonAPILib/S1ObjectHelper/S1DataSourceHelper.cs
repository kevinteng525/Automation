using System;
using System.DirectoryServices;
using System.ComponentModel;

using Saber.Common;
using Saber.TestEnvironment;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;

namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1DataSourceProviderType
    {
        [Description("exDataProviderType_Exchange")]
        Exchange = 1,
        [Description("exDataProviderType_Notes")]
        Notes_TBD = 2,
    }
    public enum S1DataSourceChooseBy
    {
        AddressBook,
        ServerHierarchy_TBD,
        LDAP_TBD,
    }

    public class S1DataSourceHelper
    {
        internal static IExDataSource CreateDataSource(S1DataSource dataSource)
        {
            
            int providerTypeId = -1;
            IExDataSource temp = null;
            switch (dataSource.DataSourceProviderType)
            {
                case S1DataSourceProviderType.Exchange:
                    providerTypeId = (int)exDataProviderType.exDataProviderType_Exchange;
                    break;
                case S1DataSourceProviderType.Notes_TBD:
                    throw new NotImplementedException();
            }
            
            switch (dataSource.DataSourceChooseBy)
            {
                case S1DataSourceChooseBy.AddressBook:
                    if (dataSource.Type == ADObjectType.User)
                    {
                        temp =  BuildDataSourceFromUser( dataSource.UserName);
                    }
                    else 
                    {
                        temp = BuildDataSourceFromGroup( dataSource.DistributionGroup);
                    }
                    break;
                case S1DataSourceChooseBy.LDAP_TBD:
                case S1DataSourceChooseBy.ServerHierarchy_TBD:
                    throw new NotImplementedException();

            }
            temp.providerTypeID = providerTypeId;
            return temp;
        }


        internal static IExDataSource BuildDataSourceFromUser( String userName)
        {
            IExDataSource dataSource = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_DataSource);
            
            SearchResult searchResult = ADManager.ExecuteLDAPQueryFromLDAPServer("(cn=" + userName + ")", TestEnvironmentHelper.DomainName, TestEnvironmentHelper.DomainAdministrator, TestEnvironmentHelper.DomainAdminPassword);
            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Exchange;
            dataSource.type = exDataSourceType.exDataSourceType_Mailbox;
            dataSource.nativeName = ADManager.GetProperty(searchResult, "distinguishedName");
            dataSource.dataSourceName = ADManager.GetProperty(searchResult, "legacyExchangeDN");
            dataSource.friendlyName = ADManager.GetProperty(searchResult, "displayName");
            if (string.IsNullOrEmpty(dataSource.dataSourceName))
            {
                dataSource.dataSourceName = dataSource.nativeName;
            }
            dataSource.server = ADManager.GetProperty(searchResult, "msExchHomeServerName");
            dataSource.nativeServer = "";
            dataSource.nativeObjectID = ADManager.GetProperty(searchResult, "objectGUID");
            return dataSource;
        }



        internal static IExDataSource BuildDataSourceFromGroup(String groupName)
        {
            IExDataSource dataSource = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_DataSource);
            SearchResult searchResult = ADManager.ExecuteLDAPQueryFromLDAPServer("(cn=" + groupName + ")", TestEnvironmentHelper.DomainName, TestEnvironmentHelper.DomainAdministrator, TestEnvironmentHelper.DomainAdminPassword);
            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Exchange;
            dataSource.type = exDataSourceType.exDataSourceType_DistList;
            dataSource.nativeName = ADManager.GetProperty(searchResult, "distinguishedName");
            dataSource.dataSourceName = dataSource.nativeName;
            dataSource.friendlyName = ADManager.GetProperty(searchResult, "name");
            dataSource.server = "";
            dataSource.nativeServer = "";
            dataSource.nativeObjectID = ADManager.GetProperty(searchResult, "objectGUID");
            return dataSource;
        }
    }

    
}
