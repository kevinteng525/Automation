using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class Project
    {
        public static Project GetProjectById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from p in context.Projects
                        where p.ProjectId == id
                        select p).FirstOrDefault();
            }
        }

        public static List<Project> GetAllProjects()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Projects.OrderBy(p => p.ProjectId).ToList();
            }
        }

        public static Project AddProject(Project instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Projects.Add(instance);
                context.SaveChanges();
                return instance;
            }
        }

        public static AutomationTask GetTaskWithLatestBuildForProject(int projectId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                IList<AutomationTask> tasks = (from task in context.AutomationTasks
                                        from build in context.Builds
                                        where task.ProjectId == projectId &&
                                            //task.NotifyStakeholders == true &&
                                            task.BuildId == build.BuildId &&
                                            (task.Status == (int)TaskStatus.Complete || task.Status == (int)TaskStatus.End || task.Status == (int)TaskStatus.Failed) &&
                                            task.RecurrencePattern.Value == (int)RecurrencePattern.AtOnce
                                            orderby task.TaskId descending
                                        select task).Take(50).ToList();
                IList<Build> builds = (from task in context.AutomationTasks
                                       from build in context.Builds
                                       where task.BuildId == build.BuildId &&
                                       task.ProjectId == projectId &&
                                           //task.NotifyStakeholders == true &&
                                       (task.Status == (int)TaskStatus.Complete || task.Status == (int)TaskStatus.End || task.Status == (int)TaskStatus.Failed) &&
                                       task.RecurrencePattern.Value == (int)RecurrencePattern.AtOnce
                                       orderby task.TaskId descending
                                       select build).ToList();
                int r;
                AutomationTask t = (from build in builds
                                               from task in tasks
                                               where build.BuildId == task.BuildId
                                               orderby (int.TryParse(build.Number, out r) ? r : 0) descending
                                               select task).FirstOrDefault();
                return t;
                
            }
        }

        public static List<AutomationTask> GetLatestNAutomationTasksForProject(int projectId, int count)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                IList<AutomationTask> tasks = (from task in context.AutomationTasks
                                      from build in context.Builds
                                      where task.BuildId == build.BuildId &&
                                      task.ProjectId == projectId &&
                                          //task.NotifyStakeholders == true &&
                                      (task.Status == (int)TaskStatus.Complete || task.Status == (int)TaskStatus.End || task.Status == (int)TaskStatus.Failed) &&
                                      task.RecurrencePattern.Value == (int)RecurrencePattern.AtOnce
                                      orderby task.TaskId descending
                                      select task).ToList();
                IList<Build> builds = (from task in context.AutomationTasks
                                      from build in context.Builds
                                      where task.BuildId == build.BuildId &&
                                      task.ProjectId == projectId &&
                                          //task.NotifyStakeholders == true &&
                                      (task.Status == (int)TaskStatus.Complete || task.Status == (int)TaskStatus.End || task.Status == (int)TaskStatus.Failed) &&
                                      task.RecurrencePattern.Value == (int)RecurrencePattern.AtOnce
                                      orderby task.TaskId descending
                                      select build).ToList();
                int r;
                List<AutomationTask> nTasks = (from build in builds
                                               from task in tasks
                                               where build.BuildId == task.BuildId
                                               orderby (int.TryParse(build.Number, out r) ? r : 0) descending
                                               select task).Take(count).ToList();
                
                return nTasks;
            }
        }

        public static Project UpdateProject(int projectId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Project project = context.Projects.Find(projectId);
                context.Entry(project).CurrentValues.SetValues(instance);
                context.SaveChanges();
                return project;
            }
        }

        public void Update()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Entry(this).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void DeleteProjectById(int projectId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Project project = context.Projects.Find(projectId);
                if (project != null)
                {
                    context.Projects.Remove(project);
                }
            }
        }

        public static List<SupportedEnvironment> GetAllSupportedEnvironmentsForProject(int projectId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (from se in context.SupportedEnvironments
                        where ((from m in context.ProjectEnvironmentMaps
                                where m.ProjectId == projectId
                                select m.EnvironmentId).Contains(se.EnvironmentId))
                        select se).OrderBy(se=>se.Name).ToList();        
            }
        }
    }
}
