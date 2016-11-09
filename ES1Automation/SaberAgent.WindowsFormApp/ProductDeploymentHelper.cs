using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;

namespace SaberAgent.WindowsFormApp
{
    public abstract class ProductDeploymentHelper
    {
        public abstract bool WaitAllMachinesStartedAfterReboot(TestEnvironment environment);
        public abstract void InstallProduct(TestEnvironment environment);
        public abstract void RebootMachinesAfterProductInstalled(TestEnvironment environment);
        public abstract void CheckPreconditionForEachTestExecution(TestEnvironment environment);
        public abstract void CollectProductionLogsAfterExecutionToLocal(TestEnvironment environment, string resultRootPath);
    }
}
