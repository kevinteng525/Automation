using System;
using System.Text;
using System.IO;
using Common.FileCommon;
using Common.ScriptCommon;

namespace RequestLib.Installation
{
    public class ValidationService
    {
        private readonly string server;

        private readonly string username;

        private readonly string password;

        private readonly string sourceInstallPath;

        private readonly string targetIntallPath = @"{0}\S1ValidationWinService";

        private readonly string targetSharePath = @"\\{0}\{1}\S1ValidationWinService";

        public ValidationService
            (
                string server,
                string username,
                string password,
                string sourceInstallPath,
                string targetIntallPath,
                string targetSharePath
            )
        {
            this.server = server;
            this.username = username;
            this.password = password;

            if (!string.IsNullOrWhiteSpace(sourceInstallPath))
            {
                this.sourceInstallPath = sourceInstallPath.Trim();
            }

            if (!string.IsNullOrWhiteSpace(targetIntallPath))
            {
                this.targetIntallPath = string.Format(this.targetIntallPath, targetIntallPath.Trim());
            }

            if (!string.IsNullOrWhiteSpace(targetSharePath))
            {
                this.targetSharePath = string.Format(this.targetSharePath, server, targetSharePath.Trim());
            }
        }

        public static bool IsValidationServiceAvaliable(string server, int port)
        {
            try
            {
                var userClient = new SocketUserClient(server, port);

                return userClient.CanConnectServer();
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidationServiceAvaliable(string server, int port, int timeout)
        {
            try
            {
                var userClient = new SocketUserClient(server, port);

                return userClient.CanConnectServer(timeout);
            }
            catch
            {
                return false;
            }
        }

        public bool InstallWinService()
        {
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0} /delete", server));
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0}  {1} /user:{2}", server, password, username));

            FileHelper.CreateFolder(targetSharePath);

            // Check .Net Framkwork 4.0
            if (!File.Exists(string.Format(@"\\{0}\C$\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe", server)))
            {
                throw new Exception("Please check .Net Framework 4.0 is installed in target server");
            }

            // Create service folder
            FileHelper.CreateFolder(targetSharePath);

            // Copy the installer file
            FileHelper.CopyDirectory(sourceInstallPath, targetSharePath);

            // write the InstallService.cmd
            string installServiceFileFullName = string.Format(@"{0}\InstallService.cmd", targetSharePath);
            string installCmd = string.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe {0}\S1ValidationWinService.exe", targetIntallPath);
            TXTHelper.ClearTXTContent(installServiceFileFullName);
            TXTHelper.WriteNewLine(installServiceFileFullName, installCmd, Encoding.Default);

            CMDScript.PsExec(server, username, password, Path.Combine(targetIntallPath, "InstallService.cmd"));
            CMDScript.RumCmd("cmd.exe", string.Format(@"sc \\{0} start SourceOneValidationService", server));

            return IsValidationServiceAvaliable(server, 5000);
        }

        public bool UninstallWinService()
        {
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0} /delete", server));
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0}  {1} /user:{2}", server, password, username));

            if (!File.Exists(string.Format(@"{0}\S1ValidationWinService.exe", targetSharePath)))
            {
                throw new Exception("S1ValidationWinSerive.exe in not exist in target path!");
            }

            // write the UninstallService.cmd
            string uninstallServiceFileFullName = string.Format(@"{0}\UninstallService.cmd", targetSharePath);
            string uninstallCmd = string.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u {0}\S1ValidationWinService.exe", targetIntallPath);
            TXTHelper.ClearTXTContent(uninstallServiceFileFullName);
            TXTHelper.WriteNewLine(uninstallServiceFileFullName, uninstallCmd, Encoding.Default);

            CMDScript.RumCmd("cmd.exe", string.Format(@"sc \\{0} stop SourceOneValidationService", server));
            CMDScript.PsExec(server, username, password, Path.Combine(targetIntallPath, "UninstallService.cmd"));

            return !IsValidationServiceAvaliable(server, 5000);
        }

        public bool ReinstallWinService()
        {
            UninstallWinService();

            return (InstallWinService());
        }
    }
}
