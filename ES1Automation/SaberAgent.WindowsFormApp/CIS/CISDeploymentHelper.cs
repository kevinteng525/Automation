using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaberAgent.WindowsFormApp.CIS;

namespace SaberAgent.WindowsFormApp
{
    public class CISDeploymentHelper : ProductDeploymentHelper
    {
        public override bool WaitAllMachinesStartedAfterReboot(Core.Model.TestEnvironment environment)
        {
            //Do nothing for CIS because no reboot needed
            return true;
        }

        public override void InstallProduct(Core.Model.TestEnvironment environment)
        {
            CISBuilderInstallerHelper.Initialize(environment);
            CISBuilderInstallerHelper.InstallCISComponentsOnEnvironment();
        }

        public override void RebootMachinesAfterProductInstalled(Core.Model.TestEnvironment environment)
        {
            //Do nothing for CIS because no reboot needed
        }

        public override void CheckPreconditionForEachTestExecution(Core.Model.TestEnvironment environment)
        {
            //Do nothing for CIS 
        }

        public override void CollectProductionLogsAfterExecutionToLocal(Core.Model.TestEnvironment environment, string localResultRootPath)
        {
            //collect the CIS logs and copy back them from the CIS to Saber Agent, and copy them to the result folder
            //We do nothing here for CIS, because we've integrated this into the test bash and the bash will collect the logs and copy to the result folder.
            //CISBuilderInstallerHelper.Initialize(environment);
            //CISBuilderInstallerHelper.CollectProductLogsAndCopyBackToAgent(localResultRootPath);
        }
    }
}
