using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaberAgent.WindowsFormApp.Supervisor
{
    public class SupervisorWebDeploymentHelper : ProductDeploymentHelper
    {
        public override bool WaitAllMachinesStartedAfterReboot(Core.Model.TestEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public override void InstallProduct(Core.Model.TestEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public override void RebootMachinesAfterProductInstalled(Core.Model.TestEnvironment environment)
        {
            return;
        }

        public override void CheckPreconditionForEachTestExecution(Core.Model.TestEnvironment environment)
        {
            return;
        }
    }
}
