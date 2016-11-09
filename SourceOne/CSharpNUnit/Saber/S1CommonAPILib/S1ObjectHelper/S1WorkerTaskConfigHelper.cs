using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1WorkerTaskConfigHelper
    {

        public static int CreateWorkerTaskConfig(S1WorkerTaskConfig workerTaskConfig)
        { 
            IExWorkerTaskConfig config = (IExWorkerTaskConfig)S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_WorkerTaskConfig);
            config.id = workerTaskConfig.WorkerId;
            config.taskTypeID = workerTaskConfig.TaskTypeId;
            config.quota = workerTaskConfig.Quota;
            config.state = exJDFWorkerTaskCfgState.exJDFWorkerTaskCfgState_Enabled;
            config.Save();
            return config.id;
        }

        public static IExWorkerTaskConfig GetWorkerTaskConfig(int workerId, int taskTypeId)
        {
            IExWorker worker = S1Context.JDFAPIMgr.GetWorkerByID(workerId);
            IExVector Configs = worker.taskCfgs;
            foreach (IExWorkerTaskConfig config in Configs)
            {
                if (config.taskTypeID == taskTypeId)
                {
                    return config;
                }                
            }
            return null;
        }

        public static void ConfigAllTaskTypeToAllWorkers()
        {
            List<S1Worker> workers = S1WorkerHelper.GetAllWorkers();
            List<S1TaskType> types = S1TaskTypeHelper.GetAllTaskTypes();
            foreach (S1Worker w in workers)
            {
                foreach (S1TaskType t in types)
                {

                    IExWorkerTaskConfig config = GetWorkerTaskConfig(w.Id, t.TaskTypeId);
                    if (null == config)
                    {
                        config = (IExWorkerTaskConfig)S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_WorkerTaskConfig);
                        config.id = w.Id;
                        config.taskTypeID = t.TaskTypeId;
                        config.quota = 4;
                        config.state = exJDFWorkerTaskCfgState.exJDFWorkerTaskCfgState_Enabled;
                        config.Save();
                    }
                    else
                    {
                        config.state = exJDFWorkerTaskCfgState.exJDFWorkerTaskCfgState_Enabled;
                        config.Save();
                    }
                }
            }            
        }
    }
}
