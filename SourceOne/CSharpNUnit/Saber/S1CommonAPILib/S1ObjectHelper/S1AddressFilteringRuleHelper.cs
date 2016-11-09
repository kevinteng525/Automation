using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExBase;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExAddressRulesBCECfg;

namespace Saber.S1CommonAPILib
{
    public class S1AddressFilteringRuleHelper
    {
        public static IExRule CreateAddressFilterringRule(S1AddressFilteringRule rule)
        {
            IExRule bceRule = new CoExRule();
            bceRule.BusinessFolderId = S1MappedFolderHelper.GetByName(rule.TargetMappedFolder).FolderId;
            bceRule.ActionType = exRuleActions.exRuleActionArchiveFolder;
            bceRule.Name = rule.Name;
            foreach (S1AddressFilteringRuleCondition condition in rule.Conditions)
            {
                IExRuleFilter filter = new CoExRuleFilter();
                
                filter.FilterType = (exRuleFilterType)condition.FilterType;//TODO
                filter.Field = (exRuleFieldType)condition.FieldType;//TODO
                CoExVector dataSources = new CoExVector();;
                switch (condition.FilterType)
                {
                    case S1AddressRuleFilteringType.CopyMessagesNotMatchAnyRuleTo:
                        filter.Field = exRuleFieldType.exRuleFieldAll;//No matter the user specified, set the field as exRuleFieldAll
                        break;
                    case S1AddressRuleFilteringType.Address:
                    case S1AddressRuleFilteringType.DirectlyAddress:                        
                        foreach (S1DataSource source in condition.PeopleOrDistributionList)
                        {
                            dataSources.Add(S1DataSourceHelper.CreateDataSource(source));
                        }
                        break;
                    case S1AddressRuleFilteringType.Domain:
                    case S1AddressRuleFilteringType.Keyword:                        
                        foreach (String source in condition.DomainOrSpecificWords)
                        {
                            dataSources.Add(source);
                        }
                        break;
                }  
                filter.Value = dataSources;                
                bceRule.set_Filter(bceRule.FilterCount, (CoExRuleFilter)filter);
            }
            
            return bceRule;
        }

        public static S1AddressFilteringRule CreateCopyMessagesNotMatchAnyRulesToMappedFolderRule(String mappedFolderName)
        {
            S1AddressFilteringRule rule = new S1AddressFilteringRule();
            rule.Name = "Created By Saber at " + DateTime.Now.ToString();
            rule.TargetMappedFolder = mappedFolderName;            
            S1AddressFilteringRuleCondition copyMessagesNotMatchAnyRuleToFolder = new S1AddressFilteringRuleCondition();
            copyMessagesNotMatchAnyRuleToFolder.FilterType = S1AddressRuleFilteringType.CopyMessagesNotMatchAnyRuleTo;
            copyMessagesNotMatchAnyRuleToFolder.FieldType = S1AddressRuleFieldType.All;
            copyMessagesNotMatchAnyRuleToFolder.PeopleOrDistributionList = null;
            rule.Conditions.Add(copyMessagesNotMatchAnyRuleToFolder);
            return rule;
        }
    }
}
