using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExBase;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1TaskHelper
    {
        public static List<IExTask> GetTasksOfActivity(int activityId)
        {
            IExTaskFilter taskFilter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskFilter);
            taskFilter.activityID = activityId;
            IExVector tasks = S1Context.JDFAPIMgr.GetTasks(taskFilter);
            List<IExTask> taskList = new List<IExTask>();
            foreach (IExTask task in tasks)
            {
                taskList.Add(task);                
            }
            return taskList;
        }
    }
}
