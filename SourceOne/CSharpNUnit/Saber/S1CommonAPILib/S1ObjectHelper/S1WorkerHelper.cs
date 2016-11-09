using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1WorkerHelper
    {
        internal static IExWorker GetByName(String name)
        {
            IExVector workers =  (IExVector)S1Context.JDFAPIMgr.GetWorkers();
            foreach (IExWorker worker in workers)
            {
                if (name.ToLower() == worker.name.ToLower())
                {
                    return worker;
                }
            }
            throw new Exception("Not found.");
        }

        internal static List<S1Worker> GetAllWorkers()
        {
            List<S1Worker> workersList = new List<S1Worker>();
            IExVector workers = (IExVector)S1Context.JDFAPIMgr.GetWorkers();
            foreach (IExWorker worker in workers)
            {
                S1Worker s1Worker = new S1Worker();
                s1Worker.Name = worker.name;
                s1Worker.Id = worker.id;
                workersList.Add(s1Worker);
            }
            return workersList;
        }

        internal static IExWorker GetByID(int id)
        {
            return (IExWorker)S1Context.JDFAPIMgr.GetWorkerByID(id);
        }

        public static void ConfigWorker(S1Worker worker)
        {
            //TODO
        }
    }
}
