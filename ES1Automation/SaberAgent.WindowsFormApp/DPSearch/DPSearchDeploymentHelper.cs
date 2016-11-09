using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.SSH;
using Core.Model;

namespace SaberAgent.WindowsFormApp.DPSearch
{
    class DPSearchDeploymentHelper : ProductDeploymentHelper
    {
        public override bool WaitAllMachinesStartedAfterReboot(Core.Model.TestEnvironment environment)
        {
            return true;
        }

        public override void InstallProduct(Core.Model.TestEnvironment environment)
        {
            try
            {
                DPSearchBuilderInstallerHelper.Initialize(environment);
                DPSearchBuilderInstallerHelper.InstallDPSearchOnSUTEnvironment();
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
                throw e;
            }
           
        }

        public override void RebootMachinesAfterProductInstalled(Core.Model.TestEnvironment environment)
        {

        }

        public override void CheckPreconditionForEachTestExecution(Core.Model.TestEnvironment environment)
        {
            
        }

        public override void CollectProductionLogsAfterExecutionToLocal(Core.Model.TestEnvironment environment, string resultRootPath)
        {

        }
    }
}
