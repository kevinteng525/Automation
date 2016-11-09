using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExBase;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public class S1JobHelper
    {
        public static bool WaitAllJobsFinishForActivityWithId(int activityId)
        {
            IExVector jobs = getJobsForActivity(activityId);
            while (jobs == null || jobs.Count == 0)
            {
                //TODO, we may need to refactor the code here to get a reliable method to wait the job to start with good performance.
                System.Threading.Thread.Sleep(1 * 1000);
                jobs = getJobsForActivity(activityId);
            }
            try
            {
                //Note that, here maybe not all the jobs have been started, but it doesn't matter because the job first started will not be completed if the sub jobs are not started or not finished. 
                jobs = getJobsForActivity(activityId);
                while (!isAllJobsFinishedSuccessfully(jobs))
                {
                    System.Threading.Thread.Sleep(2 * 1000);
                    jobs = getJobsForActivity(activityId);
                }
            }
            catch (Exception e)//some jobs are failed
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private static IExVector getJobsForActivity(int activityId)
        {
            IExJobFilter jobFilter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_JobFilter);
            jobFilter.AddCriteria(exJDFFieldID.exJDFFieldID_Job_ActivityID, exFilterCriteriaCondition.exFilterCriteriaCondition_Equals, activityId);
            return S1Context.JDFAPIMgr.GetJobs(jobFilter);
        }

        private static bool isAllJobsFinishedSuccessfully(IExVector jobs)
        {
            foreach (IExJDFJob job in jobs)
            {
                exJDFJobState state = job.state;
                if (state == exJDFJobState.exJDFJobState_Completed)
                {
                    //done
                    continue;
                }
                else if (state == exJDFJobState.exJDFJobState_Active || state == exJDFJobState.exJDFJobState_WaitForResource || state == exJDFJobState.exJDFJobState_Available)
                {
                    //wait
                    return false;
                }
                else
                {
                    //error met
                    throw new Exception("The job failed! JobId:" + job.id + " Job State:" + job.state.ToString());
                }                
            }
            return true;
        }
    }
}
