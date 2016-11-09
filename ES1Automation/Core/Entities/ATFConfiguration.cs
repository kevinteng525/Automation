using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class ATFConfiguration
    {
        public static string GetStringValue(string configName)
        {
            using(var context = new ES1AutomationEntities())
            {
                if (context.ATFConfigurations.Find(configName) != null)
                {
                    return context.ATFConfigurations.Find(configName).ConfigValue;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int GetIntValue(string configName)
        {
            if (GetStringValue(configName) != null)
            {
                return int.Parse(GetStringValue(configName));
            }
            else
            {
                return 0;
            }
        }

        public static ATFConfiguration SetConfig(string configName, string configValue, string description = "")
        {
            using (var context = new ES1AutomationEntities())
            {
                ATFConfiguration config = new ATFConfiguration { ConfigName = configName, ConfigValue = configValue, Description = description, };
                ATFConfiguration existing = context.ATFConfigurations.Find(configName);
                if (existing != null)
                {
                    existing.ConfigValue = configValue;
                    existing.Description = description;
                    context.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.ATFConfigurations.Add(config);
                }
                context.SaveChanges();
                return config;
            }
        }

        public static void SetTestCasesSuitesSyncingStartIndicator()
        {
            ATFConfiguration.SetConfig("TestCasesSuitesSyncingIndicator", "Syncing", string.Format("Start syncing at {0}", System.DateTime.UtcNow.ToString()));
        }

        public static void SetTestCasesSuitesSyncingEndIndicator()
        {
            ATFConfiguration.SetConfig("TestCasesSuitesSyncingIndicator", "End", string.Format("Finish syncing at {0}", System.DateTime.UtcNow.ToString()));
        }

        public static bool IsTestCasesSuitesSyncing()
        {
            if (ATFConfiguration.GetStringValue("TestCasesSuitesSyncingIndicator") == "Syncing")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
