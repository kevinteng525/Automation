using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Core.Model;
using Core.Providers;
using Core.Providers.BuildProviders;

namespace Core.Management
{
    public class BuildManager
    {
        public static void UpdateBuildStatus()
        {
            ATFEnvironment.Log.logger.Info(string.Format("Start to sync builds from build file server."));
            foreach (Provider provider in Provider.GetProvidersByCategory(ProviderCategory.Build))
            {
                try
                {
                    ATFEnvironment.Log.logger.Info(string.Format("Start the syncing of builds for provider {0}.", provider.Name));

                    IBuildProvider buildProvider = provider.CreateProvider() as IBuildProvider;
                    buildProvider.SyncAllBuilds();
                    ATFEnvironment.Log.logger.Info(string.Format("Finished the syncing of builds from build file server."));

                    ATFEnvironment.Log.logger.Info(string.Format("Start the updating of builds."));
                    ATFEnvironment.Log.logger.Info(string.Format("Start to update the status of successful builds."));
                    foreach (Build successBuild in Build.GetBuildsByStatus(BuildStatus.Success).Where(b=>b.ProviderId == provider.ProviderId))
                    {
                        successBuild.UpdateStatus();
                    }
                    ATFEnvironment.Log.logger.Info(string.Format("Start to update the status of failed builds."));

                    foreach (Build failedBuid in Build.GetBuildsByStatus(BuildStatus.Failed).Where(b=>b.ProviderId==provider.ProviderId))
                    {
                        failedBuid.UpdateStatus();
                    }
                }
                catch (Exception ex)
                {
                    ATFEnvironment.Log.logger.Error(string.Format("Met error when update build for provider {0}", provider.Name), ex);
                }
            }
            
            ATFEnvironment.Log.logger.Info(string.Format("Finished the updating of builds."));
        }

        public static Build GetLatestBuild(AutomationTask task)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                IList<Build> builds = (from b in context.Builds
                                      where b.ProductId == task.ProductId &&
                                          b.BranchId == task.BranchId &&
                                          b.ReleaseId == task.ReleaseId
                                      select b
                        ).ToList();
                return builds.OrderByDescending(b => int.Parse(b.Number.Split(new string[] { "." }, StringSplitOptions.None)[b.Number.Split(new string[] { "." }, StringSplitOptions.None).Length-1]))
                        .FirstOrDefault();
                
            }
        }


        public static Build GetLatestBuildByProductBranchAndRelease(int productId, int branchId, int releaseId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                IList<Build> builds = context.Builds.Where(b => b.ProductId == productId && b.BranchId == branchId && b.ReleaseId == releaseId).ToList();
                return builds.OrderByDescending(b => int.Parse(b.Number.Split(new string[] { "." }, StringSplitOptions.None)[b.Number.Split(new string[] { "." }, StringSplitOptions.None).Length - 1])).FirstOrDefault();
            }
        }
    }
}
