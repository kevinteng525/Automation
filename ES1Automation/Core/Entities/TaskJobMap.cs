using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class  TaskJobMap
    {
        public static List<TaskJobMap> GetAllMaps()
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    List<TaskJobMap> maps = context.TaskJobMaps.ToList<TaskJobMap>();
                    return maps;
                }
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
            return null;
        }

        public static TaskJobMap GetTaskJobMaps(int mapId)
        {
            using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
            {
                return context.TaskJobMaps.Find(mapId);
            }
        }

        public static bool CreateMap(TaskJobMap instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    context.TaskJobMaps.Add(instance);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

            return true;
        }

        //public bool Update()
        //{
        //    return true;
        //}

        public static bool Delete(int mapId)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    TaskJobMap map = context.TaskJobMaps.Find(mapId);
                    if (map == null)
                        return false;

                    context.TaskJobMaps.Remove(map);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

            return true;
        }

        public static bool Delete(TaskJobMap instance)
        {
            try
            {
                using (ES1AutomationEntities context = new Core.Model.ES1AutomationEntities())
                {
                    //if (!context.TaskJobMaps.Contains(instance))
                    //    return false;
                    context.TaskJobMaps.Attach(instance);

                    context.TaskJobMaps.Remove(instance);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return false;
            }

            return true;
        }

    }
}
