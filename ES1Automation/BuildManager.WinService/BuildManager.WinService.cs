using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Core;

namespace BuildManager.WinService
{
    public partial class BuildManagerWinService : ServiceBase
    {
        private static System.Timers.Timer timer = new System.Timers.Timer(1000 * 10 * 5);
        private static bool debug = false;

        public BuildManagerWinService()
        {
            InitializeComponent();            
        }

        protected override void OnStart(string[] args)
        {
            ATFEnvironment.Log.logger.Info(string.Format("Build Manager Windows Service is starting..."));
            timer.Elapsed += MonitorBuilds;
            timer.Start();
        }

        protected override void OnStop()
        {
            ATFEnvironment.Log.logger.Info(string.Format("Build Manager Windows Service is stopping..."));
            timer.Stop();
        }

        private void MonitorBuilds(object sender, EventArgs e)
        {
            timer.Stop();
            while (debug)
            {
                System.Threading.Thread.Sleep(1000);
            }

            try
            {
                Core.Management.BuildManager.UpdateBuildStatus();
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error("Met error when update the build status", ex);
            }
            finally
            {
                timer.Start();
            }
        }
    }
}
