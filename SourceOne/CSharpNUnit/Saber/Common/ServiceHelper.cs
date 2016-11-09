using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceProcess;

namespace Saber.Common
{
    public class ServiceHelper
    {
        public void StopService(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();


                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName && service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                        Console.WriteLine("Stopped service name: " + service.ServiceName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception();    
            }
        }

        public void StartService(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();


                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName && service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 60));
                        Console.WriteLine("Started service name: " + service.ServiceName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception();
            }
        }

        public void RestartService(string serviceName)
        {
            StopService(serviceName);
            StartService(serviceName);
        }
                
    }
}
