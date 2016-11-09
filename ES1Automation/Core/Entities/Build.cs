using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;
using Core.Providers.BuildProviders;
using System.Data.Entity;

namespace Core.Model
{
    public enum BuildType
    {
        BuildMachine = 0,
        LocalMachine = 1,
    }

    public enum BuildStatus
    {
        NotExist = 0,
        Success = 1,
        Failed = 2,
        Delete = 3,
    }

    public partial class Build
    {
        protected IBuildProvider BuildProvider
        {
            get
            {
                return Provider.GetProviderById(ProviderId).CreateProvider() as IBuildProvider;
            }
        }

        public BuildType BuildType
        {
            get { return (BuildType)Type; }
            set { Type = (int)value; }
        }

        public BuildStatus BuildStatus
        {
            get { return (BuildStatus)Status; }
            set { Status = (int)value; }
        }

        #region basic operations
        public void UpdateStatus()
        {
            BuildProvider.UpdateBuildStatus(this);
        }

        public static List<Build> GetAllBuilds()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.OrderBy(b=>b.Name).ToList<Build>();
            }
        }

        public static Build GetBuildById(int buildId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.Find(buildId);
            }
        }

        public static Build GetBuildByLocation(string location)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.Where(b => b.Location == location).FirstOrDefault();
            }
        }

        public static Build Add(Build build)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Build b = context.Builds.Add(build);
                context.SaveChanges();
                return b;
            }
        }

        public static List<Build> GetBuildsByStatus(BuildStatus status)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.Where(b => b.Status == (int)status).OrderBy(b=>b.Name).ToList();
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

        public static Build Update(int buildId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Build build = context.Builds.Find(buildId);
                if (build != null)
                {
                    context.Entry(build).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                    return build;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void Delete(Build build)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Builds.Attach(build);
                context.Builds.Remove(build);
                context.SaveChanges();
            }
        }

        public static void Delete(int buildId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Delete(GetBuildById(buildId));
            }
        }

        #endregion

        public static IList<Build> GetAllBuildsByProduct(int productId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.Where(b => b.ProductId == productId).ToList();
            }
        }

        public static IList<Build> GetAllBuildsByProductAndStatus(int productId, BuildStatus status)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Builds.Where(b => b.ProductId == productId && b.Status == (int)status).OrderBy(b => b.Name).ToList();
            }
        }

        public static List<Build> GetBuildByProductAndBranch(int productId, int branchId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {

                return (
                            from b in context.Builds
                            where b.ProductId == productId
                               && b.BranchId == branchId
                            select b
                        ).ToList<Build>();
            }
        }

        public static Build GeSpecifictBuildByProductBranchReleaseAndNumber(int productId, int branchId,int releaseId, string number)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (
                            from b in context.Builds
                            where b.ProductId == productId
                               && b.BranchId == branchId
                               && b.ReleaseId == releaseId
                               && b.Number == number
                            select b
                        ).SingleOrDefault();
            }
        }
    }
}
