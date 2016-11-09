using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;
using ES1.ES1SPAutoLib;
using EMC.Interop.ExBase;

namespace ES1SPAutoLib
{
    public class ConnectionOperator
    {
        public static bool IsReplaceTheFormerDataWhenCreate = false;

        public static IExProviderTypeConfig CreateArchiveConnection(string connectionName, string dbServer, string dbName)
        {
            CoExJDFAPIMgr apiMgr = new CoExJDFAPIMgr();
            string xmlConfig = @"<ExASProviderConfig><DBServer>" + dbServer + @"</DBServer><DBName>" + dbName + @"</DBName></ExASProviderConfig>";
            IExProviderTypeConfig providerTypeConfig = (IExProviderTypeConfig)apiMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_ProviderTypeConfig);
            providerTypeConfig.name = connectionName;
            providerTypeConfig.providerTypeID = 7;
            providerTypeConfig.xConfig = xmlConfig;

            return CreateArchiveConnection(providerTypeConfig);

        }

        public static IExProviderTypeConfig GetArchiveConnection(String archiveConnectionName)
        {
            IExVector configs = SourceOneContext.JDFAPIMgr.GetProviderTypeConfigs() as IExVector;
    
            if (configs != null)
            {
                foreach (IExProviderTypeConfig config in configs)
                {
                    if (config.name.Equals(archiveConnectionName) && (config.state == exProviderConfigState.exProviderConfigState_Active))
                    {
                        return config;
                    }
                }
            }

            return null;
        }

        public static IExProviderTypeConfig CreateArchiveConnection(IExProviderTypeConfig config)
        {
            IExProviderTypeConfig tempConfig = GetArchiveConnection(config.name);
            if (tempConfig != null)
            {
                if (IsReplaceTheFormerDataWhenCreate)
                {
                    tempConfig.Delete();
                }
                else
                {
                    return tempConfig;
                }

            }
            else
            {
                config.Save();
            }
            return config;
        }
    }
}
