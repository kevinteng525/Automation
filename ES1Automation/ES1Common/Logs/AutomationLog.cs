using log4net;
using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ES1Common.Logs
{
    public class AutomationLog
    {
        public ILog logger { get; private set; }

        public AutomationLog(string logName)
        {
            logger = LogManager.GetLogger(logName);
        }
    }
}
