using System;
using System.ServiceProcess;
using System.Timers;
using Core;

namespace EnvironmentManager.WinService
{
    public partial class EnvrionmentMgrService : ServiceBase
    {
        private static Timer testEnvironmentMonitor;
        private bool debug = false;

        public EnvrionmentMgrService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ATFEnvironment.Log.logger.Info("Environment Mangager Service Start");

            try
            {
                ATFEnvironment.Log.logger.Info("Initialization test environment monitor");
                testEnvironmentMonitor = new Timer(1000 * 30) { Enabled = true };
                testEnvironmentMonitor.Start();
                
                testEnvironmentMonitor.Elapsed += MoniorTestEnvironment;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
        }

        protected override void OnStop()
        {
            ATFEnvironment.Log.logger.Info("Environment Mangager Service Stop");
            testEnvironmentMonitor.Stop();
            testEnvironmentMonitor.Dispose();
        }

        private void MoniorTestEnvironment(object sender, EventArgs e)
        {
            testEnvironmentMonitor.Stop();

            while (debug)
            {
                System.Threading.Thread.Sleep(10 * 1000);
                ATFEnvironment.Log.logger.Info("Waiting the process to be attached for debugging...");
            }
            try
            {

                ATFEnvironment.EnvironmentMgr.UpdateEnvironmentStatus();
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Error happened when update environment status.", ex);
            }
            finally
            {
                testEnvironmentMonitor.Start();
            }
        }
    }
}
