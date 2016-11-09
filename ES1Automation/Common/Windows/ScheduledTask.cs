using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.ScriptCommon;
using Common.FileCommon;
using Common.Network;
using TaskScheduler;

namespace Common.Windows
{
    public class ScheduledTask
    {
        public static void CreateWindowsScheduledTaskUsingXML(string xmlFile,string domain, string user, string password)
        {
            string cmd = string.Format(@"schtasks /Create /XML {0}",xmlFile);
            string cmdFile = Environment.CurrentDirectory + @"\" + System.DateTime.UtcNow.Millisecond.ToString() + ".bat";
            TXTHelper.ClearTXTContent(cmd);
            TXTHelper.WriteNewLine(cmdFile, cmd, Encoding.Default);
            CMDScript.RumCmd(cmdFile, string.Empty, domain, user, password);
        }

        /// <summary>
        /// Create the windows scheduled one time trigger task
        /// </summary>
        /// <param name="administrator">The Administrator of Current machine, such as "WIN2kR2\Administrator"</param>
        /// <param name="password">Password of administrator</param>
        /// <param name="taskName">The task name</param>
        /// <param name="taskDescription">Task description</param>
        /// <param name="programPath">The program's path to run</param>
        /// <param name="programParameters">The program's parameters</param>
        /// <param name="programeWorkingDirectory">The program's working directory</param>
        /// <param name="startDateTime">The start time</param>
        public static void CreateWindowsScheduledOneTimeTask(string administrator, string password, string taskName, string taskDescription, string programPath, string programParameters, string programeWorkingDirectory, DateTime startDateTime)
        {
            TaskScheduler.ITaskService service = new TaskScheduler.TaskScheduler();
            service.Connect();

            var folder = service.GetFolder(@"\");

            TaskScheduler.ITaskDefinition taskDefinition = service.NewTask(0);

            TaskScheduler.IRegistrationInfo regInfo = taskDefinition.RegistrationInfo;
            regInfo.Author =administrator;
            regInfo.Description = taskDescription;


            TaskScheduler.IPrincipal principal = taskDefinition.Principal;
            principal.LogonType = TaskScheduler._TASK_LOGON_TYPE.TASK_LOGON_PASSWORD;
            principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            principal.UserId = administrator;

            TaskScheduler.ITaskSettings settings = taskDefinition.Settings;
            settings.Enabled = true;
            settings.Hidden = false;
            settings.StartWhenAvailable = true;
            settings.AllowDemandStart = true;

            TaskScheduler.ITriggerCollection triggers = taskDefinition.Triggers;
            TaskScheduler.ITimeTrigger timeTrigger = triggers.Create(TaskScheduler._TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME) as TaskScheduler.ITimeTrigger;

            timeTrigger.Enabled = true;
            if (System.DateTime.UtcNow.CompareTo(startDateTime.ToUniversalTime()) < 0)
            {
                timeTrigger.StartBoundary = startDateTime.AddSeconds(30).ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else
            {
                timeTrigger.StartBoundary = System.DateTime.Now.AddSeconds(30).ToString("yyyy-MM-ddTHH:mm:ss");
            }
            timeTrigger.Id = System.Guid.NewGuid().ToString();
            timeTrigger.ExecutionTimeLimit = "PT5M";//five minutes

            TaskScheduler.IActionCollection actions = taskDefinition.Actions;
            TaskScheduler.IExecAction action = actions.Create(TaskScheduler._TASK_ACTION_TYPE.TASK_ACTION_EXEC) as IExecAction;
            action.Path = programPath;
            action.WorkingDirectory = programeWorkingDirectory;
            action.Arguments = programParameters;

            folder.RegisterTaskDefinition(taskName, taskDefinition, (int)TaskScheduler._TASK_CREATION.TASK_CREATE_OR_UPDATE, administrator, password, _TASK_LOGON_TYPE.TASK_LOGON_PASSWORD);
        }

        /// <summary>
        /// Create the windows scheduled weekly trigger task
        /// </summary>
        /// <param name="administrator">The Administrator of Current machine, such as "WIN2kR2\Administrator"</param>
        /// <param name="password">Password of administrator</param>
        /// <param name="taskName">The task name</param>
        /// <param name="taskDescription">Task description</param>
        /// <param name="programPath">The program's path to run</param>
        /// <param name="programParameters">The program's parameters</param>
        /// <param name="programeWorkingDirectory">The program's working directory</param>
        /// <param name="daysOfWeek">The days of week to run</param>
        /// <param name="weeksInterval">Every how many weeks to run</param>
        /// <param name="startDateTime">Start time</param>
        public static void CreateWindowsScheduledWeeklyTask(string administrator, string password, string taskName, string taskDescription, string programPath, string programParameters, string programeWorkingDirectory, short daysOfWeek, short weeksInterval, DateTime startDateTime)
        {
            TaskScheduler.ITaskService service = new TaskScheduler.TaskScheduler();
            service.Connect();

            var folder = service.GetFolder(@"\");

            TaskScheduler.ITaskDefinition taskDefinition = service.NewTask(0);

            TaskScheduler.IRegistrationInfo regInfo = taskDefinition.RegistrationInfo;
            regInfo.Author = administrator;
            regInfo.Description = taskDescription;


            TaskScheduler.IPrincipal principal = taskDefinition.Principal;
            principal.LogonType = TaskScheduler._TASK_LOGON_TYPE.TASK_LOGON_PASSWORD;
            principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            principal.UserId = administrator;

            TaskScheduler.ITaskSettings settings = taskDefinition.Settings;
            settings.Enabled = true;
            settings.Hidden = false;
            settings.StartWhenAvailable = true;
            settings.AllowDemandStart = true;

            TaskScheduler.ITriggerCollection triggers = taskDefinition.Triggers;
            TaskScheduler.IWeeklyTrigger timeTrigger = triggers.Create(TaskScheduler._TASK_TRIGGER_TYPE2.TASK_TRIGGER_WEEKLY) as TaskScheduler.IWeeklyTrigger;

            timeTrigger.Enabled = true;
            timeTrigger.DaysOfWeek = daysOfWeek;
            timeTrigger.WeeksInterval = weeksInterval;
            timeTrigger.StartBoundary = startDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            timeTrigger.Id = System.Guid.NewGuid().ToString();
            timeTrigger.ExecutionTimeLimit = "PT5M";//five minutes

            TaskScheduler.IActionCollection actions = taskDefinition.Actions;
            TaskScheduler.IExecAction action = actions.Create(TaskScheduler._TASK_ACTION_TYPE.TASK_ACTION_EXEC) as IExecAction;
            action.Path = programPath;
            action.WorkingDirectory = programeWorkingDirectory;
            action.Arguments = programParameters;

            folder.RegisterTaskDefinition(taskName, taskDefinition, (int)TaskScheduler._TASK_CREATION.TASK_CREATE_OR_UPDATE, administrator, password, _TASK_LOGON_TYPE.TASK_LOGON_PASSWORD);
        }

        public static void DeleteWindowsScheduledTask(string taskName)
        {
            TaskScheduler.ITaskService service = new TaskScheduler.TaskScheduler();
            service.Connect();

            var rootFolder = service.GetFolder(@"\");
            var task = rootFolder.GetTask(taskName);
            if (task != null)
            {
                task.Stop(0);
            }
            rootFolder.DeleteTask(taskName, 0);
        }

        public static bool DeleteWindowsScheduleTaskRemotely(string taskName, string remoteHost, string administrator, string password)
        {

            bool ret = false;

            string temp = System.IO.Path.GetTempFileName() + ".bat";

            string targetTemp = string.Format(@"\\{0}\C$\SaberAgent", remoteHost);

            try
            {
                NetUseHelper.NetUserMachine(remoteHost, administrator, password);

                string cmd = string.Format("schtasks /Delete /F /TN \"{0}\"", taskName);

                Common.FileCommon.TXTHelper.ClearTXTContent(temp);

                Common.FileCommon.TXTHelper.WriteNewLine(temp, cmd, System.Text.Encoding.Default);                

                Common.FileCommon.FileHelper.CopyFile(temp, targetTemp, true);

                string result = CMDScript.PsExec(remoteHost, administrator, password, string.Format(@"C:\SaberAgent\{0}", System.IO.Path.GetFileName(temp)));

                if (result.ToLower().Contains("the system cannot find the file specified"))
                {
                    ret = true;
                }
                else if (result.ToLower().Contains("success"))
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                Common.FileCommon.FileHelper.DeleteFile(temp);

                Common.FileCommon.FileHelper.DeleteFile(string.Format(@"{0}\{1}", targetTemp, System.IO.Path.GetFileName(temp)));
            }
            return ret;
        }
    }
}
