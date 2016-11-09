using System;
using System.ServiceProcess;

namespace Common.SystemCommon
{
    public static class WindowsServices
    {
        public static bool IsServiceInstalled(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                {
                    return true;
                }
            }

            return false;
        }
 
        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);

            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);

            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
    }
}
