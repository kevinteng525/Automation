using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Core.Model
{
    public partial class SupportedEnvironment
    {
        #region basic operations

        public static List<SupportedEnvironment> GetSupportedEnvironments()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.SupportedEnvironments.ToList<SupportedEnvironment>();
            }
        }

        public static SupportedEnvironment GetSupportedEnvironmentById(int environmentId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.SupportedEnvironments.Find(environmentId);
            }
        }

        public static SupportedEnvironment Add(SupportedEnvironment supportedEnvironment)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                int environmentId = 0;
                try
                {
                    environmentId = (from e in context.SupportedEnvironments select e.EnvironmentId).Max() + 1;
                }
                catch (Exception e)
                {
                    ATFEnvironment.Log.logger.Error(e);
                    environmentId = 1;
                }
                if (SupportedEnvironment.GetSupportedEnvironmentById(environmentId) != null)
                {
                    return null;
                }

                supportedEnvironment.EnvironmentId = environmentId;
                SupportedEnvironment supportEnvironmentAdded = context.SupportedEnvironments.Add(supportedEnvironment);
                context.SaveChanges();
                return supportEnvironmentAdded;
            }
        }

        public static SupportedEnvironment Update(int supportedEnvironmentID, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                SupportedEnvironment environment = context.SupportedEnvironments.Find(supportedEnvironmentID);
                context.Entry(environment).CurrentValues.SetValues(instance);
                context.SaveChanges();
                return environment;
            }
        }

        public static void Delete(SupportedEnvironment supportedEnvironment)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.SupportedEnvironments.Attach(supportedEnvironment);
                context.SupportedEnvironments.Remove(supportedEnvironment);
                context.SaveChanges();
            }
        }

        public static void Delete(int environmentId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                SupportedEnvironment environment = GetSupportedEnvironmentById(environmentId);

                if (environment != null)
                {
                    Delete(environment);
                }

            }
        }

        public Platform GetPlatformOfEnvironment()
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(this.Config);
            foreach (string category in config.TestAgentConfiguration.Categories)
            {
                if (category.ToLower().Contains("platform"))//handle the category "platform=Exchange" or "platform = Exchange"
                {
                    if (category.ToLower().Contains("exchange"))
                    {
                        return Platform.Exchange;
                    }
                    else if (category.ToLower().Contains("domino"))
                    {
                        return Platform.Domino;
                    }
                }
            }
            return Platform.Undefined;
        }

        #endregion
    }
}
