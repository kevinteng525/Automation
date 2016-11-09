using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Core.Model
{
    public partial class ProjectEnvironmentMap
    {
        public static List<ProjectEnvironmentMap> GetAllProductEnvironmentMap()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.ProjectEnvironmentMaps.ToList<ProjectEnvironmentMap>();
            }
        }

        public static ProjectEnvironmentMap GetProductEnvironmentMapById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.ProjectEnvironmentMaps.Find(id);
            }
        }

        public static ProjectEnvironmentMap Update(int productEnvironmentId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                ProjectEnvironmentMap p = context.ProjectEnvironmentMaps.Find(productEnvironmentId);
                context.Entry(p).CurrentValues.SetValues(instance);
                context.SaveChanges();
                return p;
            }
        }

        public static ProjectEnvironmentMap Add(ProjectEnvironmentMap instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                var map = (from m in context.ProjectEnvironmentMaps
                                 where m.ProjectId == instance.ProjectId &&
                                 (m.EnvironmentId == instance.EnvironmentId)
                           select m).FirstOrDefault<ProjectEnvironmentMap>();
                if(map!=null)
                {
                    return map;
                }
                else
                {
                    ProjectEnvironmentMap p = context.ProjectEnvironmentMaps.Add(instance);
                    context.SaveChanges();
                    return p;
                }
                
            }
        }

        public static void Delete(int productEnvironmentId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                ProjectEnvironmentMap p = context.ProjectEnvironmentMaps.Find(productEnvironmentId);

                if (p != null)
                {
                    context.ProjectEnvironmentMaps.Remove(p);
                }
            }
        }
    }

}
