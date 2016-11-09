using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1WorkerGroupHelper
    {
        public static int CreateWorkerGroup(S1WorkerGroup group)
        {
            IExWorkerGroup workerGroup = (IExWorkerGroup)S1Context.JDFAPIMgr.CreateNewObject(EMC.Interop.ExJDFAPI.exJDFObjectType.exJDFObjectType_WorkerGroup);
            workerGroup.name = group.Name;
            workerGroup.description = group.Description;
            workerGroup.state = exWorkerGroupState.exWorkerGroupState_Active;
            CoExVector workers = new CoExVector();
            foreach (String worker in group.Workers)
            {
                workers.Add(S1WorkerHelper.GetByName(worker));
            }
            workerGroup.workers = workers;
            workerGroup.Save();
            return workerGroup.id;

        }

        internal static IExWorkerGroup GetByID(int id)
        {
            return (IExWorkerGroup)S1Context.JDFAPIMgr.GetWorkerGroupByID(id);
        }

        internal static IExWorkerGroup GetByName(String name)
        {
            IExVector groups = (IExVector)S1Context.JDFAPIMgr.GetWorkerGroups();
            foreach (IExWorkerGroup group in groups)
            {
                if (name.ToLower() == group.name.ToLower())
                    return group;
            }
            throw new Exception("Not found.");
        }
    }
}
