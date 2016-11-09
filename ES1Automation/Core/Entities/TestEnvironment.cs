using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.Providers.EnvrionmentProviders;

namespace Core.Model
{
    public enum EnvironmentStatus
    {
        New = 0,
        Setup = 1,
        Ready = 2,
        Discard = 3,
        Disposing = 4,
        Disposed = 5,
        Error = 6,
        //below three status is only used by the Environment Management Windows Service.
        MachinesReady = 7, //Means the machines are ready, then we'll install the Agent Service on the test agent
        AgentServiceInstalledAndReady = 8,//Means the agent service is installed on the test agents and works well, then we'll install the Build on SUT
        BuildInstalled = 9,//The build is installed, then we can change the status to Ready, then start run the test cases.
        AgentServiceInstalling = 10,
        Ocuppied = 11,
    }

    public partial class TestEnvironment
    {
        public IEnvironmentProvider GetEnvironmentProvider()
        {
            try
            {
                return Provider.GetProviderById(ProviderId).CreateProvider() as IEnvironmentProvider;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to initialize the environment provider {0} for environment {1}", this.ProviderId, this.Name), ex);
                return null;
            }

        }

        public EnvironmentStatus EnvironmentStatus
        {
            get { return (EnvironmentStatus)Status; }
            set { if (Status != (int)EnvironmentStatus.Discard || value == EnvironmentStatus.Disposing) Status = (int)value; }//We could not modify the environment status if it's discard, except to change it to disposing
        }

        #region basic operations

        public static List<TestEnvironment> GetAllEnvironments()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.ToList<TestEnvironment>();
            }
        }

        public static TestEnvironment GetEnvironmentById(int environmentId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.Find(environmentId);
            }
        }

        public static TestEnvironment Add(TestEnvironment environment)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                TestEnvironment e = context.TestEnvironments.Add(environment);
                context.SaveChanges();
                return e;
            }
        }

        public void Update()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Entry(this).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Delete(TestEnvironment environment)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.TestEnvironments.Attach(environment);
                context.TestEnvironments.Remove(environment);
                context.SaveChanges();
            }
        }

        public static void Delete(int environmentId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Delete(GetEnvironmentById(environmentId));
            }
        }

        #endregion

        public static TestEnvironment GetEnvironmentByName(string environmentName)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.Where(t => t.Name == environmentName).SingleOrDefault();
            }
        }

        public void RefreshEnvironmentStatus()
        {
            IEnvironmentProvider provider = GetEnvironmentProvider();
            if (null == provider)
            {
                ATFEnvironment.Log.logger.Error(string.Format("Failed to get the environment provider for environment {0}", this.Name));
            }
            else
            {
                provider.RefreshEnvironmentStatus(this);
            }
        }

        public void SetEnvironmentConfig(string config)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.TestEnvironments.Attach(this);
                    context.Entry(this).Reload();

                    this.Config = config;
                    this.ModifyDate = DateTime.UtcNow;
                    context.Entry(this).Property(p => p.Config).IsModified = true;
                    context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
        }

        public void SetEnvironmentStatus(EnvironmentStatus status)
        {
            try
            {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    context.TestEnvironments.Attach(this);
                    context.Entry(this).Reload();
                    if (this.EnvironmentStatus != EnvironmentStatus.Discard || status == EnvironmentStatus.Disposing)//We could not modify the environment status if it's discard, except to change it to disposing
                    {
                        ATFEnvironment.Log.logger.Debug(string.Format("Change Status for Env: {0}, {1} -> {2}", this.Name, this.EnvironmentStatus, status));
                        this.Status = (int)status;
                        this.ModifyDate = DateTime.UtcNow;
                        context.Entry(this).Property(p => p.Status).IsModified = true;
                        context.Entry(this).Property(p => p.ModifyDate).IsModified = true;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }

        }

        public static IList<TestEnvironment> GetEnvironmentByStatus(EnvironmentStatus status)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.Where(e => e.Status == (int)status).ToList<TestEnvironment>();
            }
        }

        public static IList<TestEnvironment> GetEnvironmentByProvider(int providerId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.Where(e => e.ProviderId == providerId).ToList<TestEnvironment>();
            }
        }

        public static IList<TestEnvironment> GetEnvironmentInUsage()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.TestEnvironments.Where(e => e.Status != (int)EnvironmentStatus.Disposed).ToList<TestEnvironment>();
            }
        }

        public static TestEnvironment GetAvalibleStaticTestAgent4SupportedEnvironment(SupportedEnvironment supportEnvironment)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(supportEnvironment.Config);
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                foreach (TestEnvironment environment in context.TestEnvironments.Where(e => e.Type == EnvironmentCreateType.Static && e.Status == (int)EnvironmentStatus.Disposed && e.ProviderId == supportEnvironment.ProviderId))
                {
                    TestEnvironmentConfigHelper eConfig = new TestEnvironmentConfigHelper(environment.Config);
                    if (eConfig.Type == EnvironmentType.TestAgentAlone && eConfig.TestAgentConfiguration.Name == config.TestAgentConfiguration.Name)
                    {
                        return environment;
                    }
                }
                return null;
            }
        }

        public static TestEnvironment GetAvalibleStaticSUT4SupportedEnvironment(SupportedEnvironment supportEnvironment)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(supportEnvironment.Config);
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                foreach (TestEnvironment environment in context.TestEnvironments.Where(e => e.Type == EnvironmentCreateType.Static && e.Status == (int)EnvironmentStatus.Disposed && e.ProviderId == supportEnvironment.ProviderId))
                {
                    TestEnvironmentConfigHelper eConfig = new TestEnvironmentConfigHelper(environment.Config);
                    if (eConfig.Type == EnvironmentType.SUTAlone && eConfig.SUTConfiguration.Name == config.SUTConfiguration.Name)
                    {
                        return environment;
                    }
                }
                return null;
            }
        }
    }
}
