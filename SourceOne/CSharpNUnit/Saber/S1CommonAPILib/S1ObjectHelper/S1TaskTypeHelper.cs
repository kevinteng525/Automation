using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExBase;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1TaskTypeHelper
    {
        public static List<S1TaskType> GetAllTaskTypes()
        {
            List<S1TaskType> list = new List<S1TaskType>();
            IExTaskTypeFilter filter = (IExTaskTypeFilter)S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
            
            IExVector types = (IExVector)S1Context.JDFAPIMgr.GetTaskTypes(filter);

            foreach (IExTaskType t in types)
            {
                if ((((t.state & (int)EMC.Interop.ExBase.exPluginState.exPluginState_Active) != 0)                   // Only show Active types 
                    && ((t.state & (int)EMC.Interop.ExBase.exPluginState.exPluginState_NotAdminDisplayable) == 0)    // Hide internal types 
                    /*&& (ExMMCAdminApp.Instance.IsSupportedByEnv((EMC.Interop.ExBase.exRunTimeEnv)t.runTimeEnvMask))*/) // Hide inappropriate types
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_Query == t.id)      // special case: Query      <--- TODO
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_RestoreJBC == t.id)      // special case: Restore    <--- TODO   
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_DeleteFromArchiveJBC == t.id)     // special case: Delete     <--- TODO
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_DCQuery == t.id)     // special case: DISCO Query     <--- TODO
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_Restoration == t.id)     // special case: DISCO Export     <--- TODO
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_FileRestoreJBC == t.id) // File Restore 
                    || ((int)exCoreTaskTypes.exCoreTaskTypes_SharePointRestoreJBC == t.id))
                {
                    IExTaskType taskType = null;
                    IExTaskTypeFilter childFilter = (IExTaskTypeFilter)S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
                    childFilter.parentTaskTypeID = t.id;
                    IExVector childTaskTypes = (IExVector)S1Context.JDFAPIMgr.GetTaskTypes(childFilter);
                    if ((null != childTaskTypes) && (0 != childTaskTypes.Count))
                    {
                        // use the first task child type...
                        taskType = (IExTaskType)childTaskTypes[0];
                    }
                    else
                    {
                        // use this type...
                        taskType = t;
                    }


                    S1TaskType taskTypeS1 = new S1TaskType();
                    taskTypeS1.Name = taskType.name;
                    taskTypeS1.TaskTypeId = taskType.id;
                    taskTypeS1.Description = taskType.description;
                    taskTypeS1.State = taskType.state;
                    taskTypeS1.RuntimeEnvironmentMask = taskType.runTimeEnvMask;
                    list.Add(taskTypeS1);
                }
            }

            return list;
        }
    }
}
