using System;
using System.ServiceProcess;
using System.IO;
using Common.ScriptCommon;
using Common.FileCommon;
using System.Text;

namespace Common.Windows
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

        public static void RemoteInstallWinService
            (
                string server, 
                string username, 
                string password, 
                string serviceBinaryName,
                string serviceName, 
                string sourceInstallPath, 
                string targetSharePath, 
                string targetIntallPath,
                string serviceDisplayName 
            )
        {
            // Check .Net Framkwork 4.0
            if (!File.Exists(string.Format(@"\\{0}\C$\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe", server)))
            {
                throw new Exception("Please check .Net Framework 4.0 is installed in target server");
            }

            // Create service folder
            //FileHelper.CreateFolder(targetSharePath);

            // Copy the installer file
            FileHelper.CopyDirectory(sourceInstallPath, targetSharePath);

            string installServiceFileFullName = string.Format(@"{0}\InstallService.cmd", targetSharePath);
            TXTHelper.ClearTXTContent(installServiceFileFullName);
            //1. set the priviledge for the administrators on the remote machines to logon as servcie.
            TXTHelper.WriteNewLine(installServiceFileFullName, string.Format("secedit /configure /db secedit.sdb /cfg \"{0}\"", System.IO.Path.Combine(targetIntallPath, "secedit.inf")), Encoding.Default);
            
            //2. write the InstallService.cmd            
            string installCmd = string.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe {0}\{1}", targetIntallPath, serviceBinaryName);
            
            TXTHelper.WriteNewLine(installServiceFileFullName, installCmd, Encoding.Default);
            //config the service to run as administrator
            //2. config the windows service to logon on as the user( which is within the administrators group)
            //TXTHelper.WriteNewLine(installServiceFileFullName, string.Format("sc config \"{0}\" obj= \"{1}\" DisplayName= \"{2}\" password= \"{3}\"",  serviceName, username, serviceDisplayName, password), Encoding.Default);
            //3. start the windows service
            TXTHelper.WriteNewLine(installServiceFileFullName, string.Format("sc start \"{0}\"",  serviceName), Encoding.Default);
            CMDScript.PsExec(server, username, password, Path.Combine(targetIntallPath, "InstallService.cmd"));            
        }

        public static void RemoteUninstallWinService
            (
                string server, 
                string username, 
                string password,
                string serviceBinaryName,
                string serviceName, 
                string sourceInstallPath, 
                string targetSharePath, 
                string targetIntallPath
            )
        {
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0} /delete", server));
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0}  {1} /user:{2}", server, password, username));

            if (!File.Exists(string.Format(@"{0}\{1}", targetSharePath, serviceBinaryName)))
            {
                throw new Exception("Services file is not exist in target path!");
            }

            // write the UninstallService.cmd
            string uninstallServiceFileFullName = string.Format(@"{0}\UninstallService.cmd", targetSharePath);
            string uninstallCmd = string.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u {0}\{1}", targetIntallPath, serviceBinaryName);
            TXTHelper.ClearTXTContent(uninstallServiceFileFullName);
            TXTHelper.WriteNewLine(uninstallServiceFileFullName, string.Format(@"sc stop {0}", serviceName), Encoding.Default);   
            TXTHelper.WriteNewLine(uninstallServiceFileFullName, uninstallCmd, Encoding.Default);                     
            CMDScript.PsExec(server, username, password, Path.Combine(targetIntallPath, "UninstallService.cmd"));
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

        public static void StartServiceRemotely(string serviceName, string host, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName, host);

            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public static void StopServiceRemotely(string serviceName, string host, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName, host);

            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }

        public static bool IsServiceRunningRemotely(string serviceName, string host)
        {
            ServiceController service = new ServiceController(serviceName, host);

            service.Refresh();

            return service.Status == ServiceControllerStatus.Running;

        }

        public static bool IsServiceRunning(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);

            service.Refresh();

            return service.Status == ServiceControllerStatus.Running;

        }

        public static void RestartServiceRemotely(string serviceName, string host, int timeoutMilliseconds)
        {
            if (IsServiceRunningRemotely(serviceName, host))
            {
                StopServiceRemotely(serviceName, host, timeoutMilliseconds);
            }
            StartServiceRemotely(serviceName, host, timeoutMilliseconds);
        }

        public static void RestartService(string serviceName,  int timeoutMilliseconds)
        {
            if (IsServiceRunning(serviceName))
            {
                StopService(serviceName, timeoutMilliseconds);
            }
            StartService(serviceName, timeoutMilliseconds);
        }
    }
}
