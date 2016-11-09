using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Core.Model;
using System.Timers;
using ES1Common.Logs;
using System.Threading;
using Core.Providers.EnvrionmentProviders;
using Core.Management;
using Core;


namespace TaskManager.WinService
{
    public partial class TaskManagerWinService : ServiceBase
    {
        static bool debug = false;
        static bool bOnStop = false;
       
        private static System.Timers.Timer timer;

        public TaskManagerWinService()
        {
            InitializeComponent();            
            ATFEnvironment.Log.logger.Info(string.Format("Task Manager Windows Service Is Started at {0}", System.DateTime.UtcNow.ToString()));            
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer();
            int interval =
                string.IsNullOrEmpty(ConfigurationManager.AppSettings["TaskMonitorInterval"]) ?
                10 * 1000 : int.Parse(ConfigurationManager.AppSettings["TaskMonitorInterval"]);
            timer.Interval = interval;
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
            bOnStop = false;
        }

        protected override void OnStop()
        {
            bOnStop = true;
            this.RequestAdditionalTime(2000);//
            Thread.Sleep(2000);
            timer.Enabled = false;
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

            //ATFEnvironment.Log.logger.Info("Start to handle the tasks");

            // For attach process to debug
            while (debug)
            {
                ATFEnvironment.Log.logger.Info("while loop, waitting to be attached for debugging .....");
                Thread.Sleep(10 * 1000);
            }

            try
            {
                if (!bOnStop)
                {
                    TaskManagement.MonitorScheduledTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorDispatchedTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorRunningTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorCancellingTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorCancelledTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorCompleteTasks();

                    Thread.Sleep(5000);

                    TaskManagement.MonitorFailedTasks();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Exception happened when monitor the tasks. exception detail {0}", ex.Message + ex.StackTrace);
                ATFEnvironment.Log.logger.Error(message);
            }
            finally
            {
                timer.Start();
            }
        }
    }
}
