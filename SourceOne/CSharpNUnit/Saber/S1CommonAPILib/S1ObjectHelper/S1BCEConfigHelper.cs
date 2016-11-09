using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExAddressRulesBCECfg;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1BCEConfigHelper
    {
        public static int CreateBCEConfigForJournal(S1BCEConfig config)
        {
            IExBCEObjectConfig bceConfig = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_BCEObjectConfig);
            bceConfig.bceObjectID = 1;//Address Rule BCE
            bceConfig.state = exBCEObjectConfigState.exBCEObjectConfigState_Active;
            bceConfig.visibility = exBCEObjectConfigVisibility.exBCEObjectConfigVisibility_Public;
            bceConfig.name = config.Name;
            IExRuleSet bceRuleSet = new CoExRuleSet();
            foreach(S1AddressFilteringRule rule in config.Rules)
            {
                bceRuleSet.set_Rule(bceRuleSet.RuleCount, S1AddressFilteringRuleHelper.CreateAddressFilterringRule(rule));
            }
            bceConfig.xConfig = bceRuleSet.GetXMLAsString((int)SupportedFolderType.JournalSupported);
            bceConfig.Save();
            return bceConfig.id;
        }

        public static int CreateBCEConfigForMailBoxTask(S1BCEConfig config)
        {
            IExBCEObjectConfig bceConfig = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_BCEObjectConfig);
            bceConfig.bceObjectID = 1;//Address Rule BCE
            bceConfig.state = exBCEObjectConfigState.exBCEObjectConfigState_Active;
            bceConfig.visibility = exBCEObjectConfigVisibility.exBCEObjectConfigVisibility_Public;
            bceConfig.name = config.Name;
            IExRuleSet bceRuleSet = new CoExRuleSet();
            foreach (S1AddressFilteringRule rule in config.Rules)
            {
                bceRuleSet.set_Rule(bceRuleSet.RuleCount, S1AddressFilteringRuleHelper.CreateAddressFilterringRule(rule));
            }
            bceConfig.xConfig = bceRuleSet.GetXMLAsString((int)SupportedFolderType.MailBoxTaskSupported);
            bceConfig.Save();
            return bceConfig.id;
        }

        internal static IExBCEObjectConfig GetByID(int configID)
        {
            return (IExBCEObjectConfig)S1Context.JDFAPIMgr.GetBCEObjectConfigByID(configID);
        }

        internal static List<IExBCEObjectConfig> GetAll()
        {
            List<IExBCEObjectConfig> configs = new List<IExBCEObjectConfig>();
            IExVector configVector = S1Context.JDFAPIMgr.GetBCEObjectConfigs(null);
            foreach (IExBCEObjectConfig config in configVector)
            {
                configs.Add(config);
            }
            return configs;
        }
    }
}
