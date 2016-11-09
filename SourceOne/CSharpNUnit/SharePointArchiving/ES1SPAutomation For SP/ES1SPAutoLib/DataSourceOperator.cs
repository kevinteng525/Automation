using System.DirectoryServices;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;

namespace ES1.ES1SPAutoLib
{
    public class DataSourceOperator
    {
        public static IExJDFAPIMgr GetJDFAPIMgr()
        {
            return new CoExJDFAPIMgr();
        }

        public static IExDataSource BuildDataSourceFromUser(SearchResult searchResult)
        {
            IExDataSource dataSource = (IExDataSource)GetJDFAPIMgr().CreateNewObject(exJDFObjectType.exJDFObjectType_DataSource);
            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Exchange;
            dataSource.type = exDataSourceType.exDataSourceType_Mailbox;
            dataSource.nativeName = ADOperator.GetProperty(searchResult, "distinguishedName");
            dataSource.dataSourceName = ADOperator.GetProperty(searchResult, "legacyExchangeDN");
            dataSource.friendlyName = ADOperator.GetProperty(searchResult, "displayName");
            if (string.IsNullOrEmpty(dataSource.dataSourceName))
            {
                dataSource.dataSourceName = dataSource.nativeName;
            }
            dataSource.server = ADOperator.GetProperty(searchResult, "msExchHomeServerName");
            dataSource.nativeServer = "";
            dataSource.nativeObjectID = ADOperator.GetProperty(searchResult, "objectGUID");
            return dataSource;
        }

        public static IExDataSource BuildDataSoruceFromGroup(SearchResult searchResult)
        {
            IExDataSource dataSource = (IExDataSource)GetJDFAPIMgr().CreateNewObject(exJDFObjectType.exJDFObjectType_DataSource);
            dataSource.providerTypeID = (int)exDataProviderType.exDataProviderType_Exchange;
            dataSource.type = exDataSourceType.exDataSourceType_DistList;
            dataSource.nativeName = ADOperator.GetProperty(searchResult, "distinguishedName");
            dataSource.dataSourceName = dataSource.nativeName;
            dataSource.friendlyName = ADOperator.GetProperty(searchResult, "name");
            dataSource.server = "";
            dataSource.nativeServer = "";
            dataSource.nativeObjectID = ADOperator.GetProperty(searchResult, "objectGUID");
            return dataSource;
        }
    }

    
}
