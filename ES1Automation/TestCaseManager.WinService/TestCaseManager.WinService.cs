using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Core;


namespace TestCaseManager.WinService
{
    public partial class TestCaseManagerWinService : ServiceBase
    {

        private static System.Timers.Timer timer;// = new System.Timers.Timer(1000 * 60 * 60 * 6);//6 hours
        private static bool debug = false;
        
        public TestCaseManagerWinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ATFEnvironment.Log.logger.Info(string.Format("Test Case Manager Windows Service is starting..."));
            timer = new System.Timers.Timer(1000 * 60 * 60 * 6);//6 hours
            timer.Elapsed += MonitorTestCases;
            timer.Start();
        }

        protected override void OnStop()
        {
            ATFEnvironment.Log.logger.Info(string.Format("Test Case Manager Windows Service is stopping..."));
            timer.Stop();
            timer.Close();
        }
        private void MonitorTestCases(object sender, EventArgs e)
        {
            timer.Stop();
            while (debug)
            {
                System.Threading.Thread.Sleep(1000);
            }

            try
            {
                if (Core.Model.ATFConfiguration.IsTestCasesSuitesSyncing())
                {
                    ATFEnvironment.Log.logger.Info("The test cases or suites are syncing now, skip it.");
                    return;
                }
                Core.Model.ATFConfiguration.SetTestCasesSuitesSyncingStartIndicator();
                Core.Management.TestCaseManager.UpdateTestSuite();
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Met error when update test cases and test suites from providers", ex);
            }
            finally
            {
                Core.Model.ATFConfiguration.SetTestCasesSuitesSyncingEndIndicator();
                timer.Start();
            }
        }
    }
}
