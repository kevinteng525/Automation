using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExBase;

namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1ArchiveConnectionType
    {
        [Description("exDataProviderType_Unknown")]
        Unknown = 0,
        [Description("exDataProviderType_Ex4X")]
        EMailXtender4X = 5,
        [Description("exDataProviderType_ExAS")]
        NativeArchive = 7,
        [Description("exDataProviderType_ExAsIPM")]
        InPlaceMigratedNativeArchive = 12,
    }
    public class S1ArchiveConnectionHelper
    {
        public static bool IsReplaceTheFormerDataWhenCreate = false; 

        public static int CreateArchiveConnection(S1ArchiveConnection connection)
        {
            CoExJDFAPIMgr apiMgr = new CoExJDFAPIMgr();
            string xmlConfig = @"<ExASProviderConfig><DBServer>" + connection.DatabaseServer + @"</DBServer><DBName>" + connection.DatabaseName + @"</DBName></ExASProviderConfig>";
            IExProviderTypeConfig providerTypeConfig = (IExProviderTypeConfig)apiMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_ProviderTypeConfig);
            providerTypeConfig.name = connection.Name;
            providerTypeConfig.description = connection.Description;
            providerTypeConfig.providerTypeID = (int)connection.ArchiveConnectionType;
            providerTypeConfig.xConfig = xmlConfig;

            return CreateArchiveConnection(providerTypeConfig).id;

        }

        internal static IExProviderTypeConfig GetByName(String name)
        {
            IExVector configs = S1Context.JDFAPIMgr.GetProviderTypeConfigs() as IExVector;
    
            if (configs != null)
            {
                foreach (IExProviderTypeConfig config in configs)
                {
                    if (config.name.Equals(name) && (config.state == exProviderConfigState.exProviderConfigState_Active))
                    {
                        return config;
                    }
                }
            }

            return null;
        }        

        internal static  IExProviderTypeConfig GetById(int id)
        {
            return S1Context.JDFAPIMgr.GetProviderTypeConfigByID(id);
        }

        static public List<IExProviderTypeConfig> GetAll()
        {
            CoExVector configsVector = S1Context.JDFAPIMgr.GetProviderTypeConfigs();
            List<IExProviderTypeConfig> configs = new List<IExProviderTypeConfig>();
            foreach (IExProviderTypeConfig config in configsVector)
            {
                configs.Add(config);
            }
            return configs;
        }

        static public bool IsExistsByName(string name)
        {
            IExProviderTypeConfig config = S1ArchiveConnectionHelper.GetByName(name);
            if (null != config)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static IExProviderTypeConfig CreateArchiveConnection(IExProviderTypeConfig config)
        {
            IExProviderTypeConfig tempConfig = GetByName(config.name);
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
