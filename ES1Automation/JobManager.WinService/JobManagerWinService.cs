using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading;
using Core;
using Core.Providers.EnvrionmentProviders;
using Core.Management;
using ES1Common.Logs;


namespace JobManager.WinService
{
    public partial class JobManagerWinService : ServiceBase
    {
        static bool debug = false;
        static bool bOnStop = false;
        private static System.Timers.Timer timer;

        public JobManagerWinService()
        {
            InitializeComponent();

            ATFEnvironment.Log.logger.Info(string.Format("Job Manager Windows Service Is Started at {0}", System.DateTime.UtcNow.ToString())); 
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer(10000);

            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            timer.Start();

            bOnStop = false;
        }

        protected override void OnStop()
        {
            bOnStop = true;
            this.RequestAdditionalTime(2000);
            Thread.Sleep(2000);
            timer.Stop();
            timer.Close();
            bOnStop = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (bOnStop)
            {
                return;
            }

            timer.Stop();

            ATFEnvironment.Log.logger.Info("Start to handle the jobs.");

            while (debug)
            {
                ATFEnvironment.Log.logger.Info("while loop, waitting to be attached for debugging .....");
                Thread.Sleep(10 * 1000);
            }

            try
            {
                if (!bOnStop)
                {
                    JobManagement.MonitorAssignedJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorPreparingJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorRunningJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorCompleteJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorFailedJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorTimeoutJobs();

                    Thread.Sleep(5000);

                    JobManagement.MonitorCancelledJobs();

                }

            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex);
            }
            finally
            {
                timer.Start();
            }
        }
    }
}
