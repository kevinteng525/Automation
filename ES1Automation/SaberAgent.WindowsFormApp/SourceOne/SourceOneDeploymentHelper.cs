using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaberAgent.WindowsFormApp
{
    public class SourceOneDeploymentHelper : ProductDeploymentHelper
    {
        public override void InstallProduct(Core.Model.TestEnvironment environment)
        {
            try
            {
                S1BuilderInstallHelper.Initialize(environment);
                S1BuilderInstallHelper.CheckThePreconditionForS1ComponentsInstallation();
                S1BuilderInstallHelper.InstallS1OnSUTEnvironment();
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
                throw e;
            }
        }

        public override bool WaitAllMachinesStartedAfterReboot(Core.Model.TestEnvironment environment)
        {
            try
            {
                S1BuilderInstallHelper.Initialize(environment);
                return S1BuilderInstallHelper.WaitMachinesRebootedAfterS1ComponentsInstallation();
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
                return false;
            }
        }

        public override void RebootMachinesAfterProductInstalled(Core.Model.TestEnvironment environment)
        {
            try
            {
                S1BuilderInstallHelper.Initialize(environment);
                S1BuilderInstallHelper.RestartAllMachinesWithS1ComponentInstalled();
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
            }
        }

        public override void CheckPreconditionForEachTestExecution(Core.Model.TestEnvironment environment)
        {
            try 
            {
                S1BuilderInstallHelper.Initialize(environment);
                S1BuilderInstallHelper.MakeSureAllS1ServicesAreStarted();
            }
            catch (Exception e)
            {
                SaberAgent.Log.logger.Error(e.Message, e);
            }
        }

        public override void CollectProductionLogsAfterExecutionToLocal(Core.Model.TestEnvironment environment, string resultRootPath)
        {

        }
    }
}
