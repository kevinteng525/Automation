using System.Configuration;
using Core.Management;
using Core.Model;
using ES1Common.Exceptions;
using ES1Common.Logs;
using System;

namespace Core
{
    public static class ATFEnvironment
    {
        public static readonly AutomationLog Log;

        public static readonly EnvironmentManager EnvironmentMgr = new EnvironmentManager();

        public static int DefaultTestCaseRetryTimes = ATFConfiguration.GetIntValue("DefaultTestCaseRetryTimes");

        public static int DefaultTestCaseTimeout = ATFConfiguration.GetIntValue("DefaultTestCaseTimeout");

        static ATFEnvironment()
        {
            string loggerName = ConfigurationManager.AppSettings["loggerName"];

            if (string.IsNullOrWhiteSpace(loggerName))
            {
                // todo: create default logger
            }
            else
            {
                Log = new AutomationLog(loggerName);
            }
        }
    }
}
