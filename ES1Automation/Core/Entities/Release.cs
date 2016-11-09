using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public enum SyncSourceType
    {
        FromBuildServer = 0,
        FromRQM = 1,
        
    }

    public partial class Release
    {
        public static List<Release> GetAllReleases()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Releases.ToList<Release>();
            }
        }
        public static List<Release> GetAllReleasesByType(int type)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (
                        from release in context.Releases.Where ( r => r.Type==type).OrderBy( r =>r.Name)
                        select release
                    ).ToList();
            }
        }
        public static Release GetReleaseById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Releases.Find(id);
            }
        }

        public static Release GetReleaseByName(string name)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Releases.Where(r => r.Name.ToLower() == name.ToLower()).FirstOrDefault();
            }
        }

        public static Release UpdateRelease(int id, object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Release release = context.Releases.Find(id);
                if (release != null)
                {
                    context.Entry(release).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                    return release;
                }
                else
                {
                    return null;
                }
            }
        }

        public static Release AddOrUpdateRelease(Release release)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Release existingRelease = context.Releases.Where(r => r.Name.ToLower() == release.Name.ToLower()&& r.Type== release.Type && r.ProductId == release.ProductId).FirstOrDefault();
                if (existingRelease != null)
                {
                    existingRelease.Path = release.Path;
                    existingRelease.Description = release.Description;
                    context.Entry(existingRelease).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return existingRelease;
                }
                else
                {
                    Release r = context.Releases.Add(release);
                    context.SaveChanges();
                    return r;
                }
            }
        }

        public static void DeleteRelease(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Release release = context.Releases.Find(id);
                if (release != null)
                {
                    context.Releases.Remove(release);
                    context.SaveChanges();
                }
            }
        }
    }
}
