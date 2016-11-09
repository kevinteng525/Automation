using System;
using System.Reflection;
using System.Text;
using Common.ScriptCommon;
using Common.Windows;

namespace ATFExchangeCentre
{
    public static class Machine
    {
        public readonly static string ServerName = Environment.MachineName;

        public readonly static string DomainName = Environment.UserDomainName;

        public readonly static string OSName = ServerStatus.GetOSFriendlyName();

        public readonly static string OSVersion = Environment.OSVersion.ToString();

        public readonly static bool Is64BitOS = Environment.Is64BitOperatingSystem;

        public readonly static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // for the service is run under 32-bit, so in 64-bit OS ProgramFiles should be C:\ProgramFiles (X86)
        public readonly static string ProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        // A common application data is shared by all users
        public readonly static string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string RunCMDSript(string command, string args)
        {
            return CMDScript.RumCmd(command, args);
        }

        public static string RunCMDSript(string command, string args, string domain, string username, string password)
        {
            return CMDScript.RumCmd(command, args, domain, username, password);
        }

        public static string RunPSSript(string script)
        {
            return PSScript.RunScript(script);
        }

        public static string GetStatus()
        {
            StringBuilder status = new StringBuilder();

            status.AppendFormat("DomainName = {0};", DomainName);
            status.AppendFormat("ServerName = {0};", ServerName);
            status.AppendFormat("OSName = {0};", OSName);
            status.AppendFormat("OSVersion = {0};", OSVersion);
            status.AppendFormat("Is64BitOS = {0};", Is64BitOS);
            status.AppendFormat("ServiceVersion = {0};", GetValidationServiceVersion());
            status.AppendFormat("ProgramFilesX86 = {0};", ProgramFilesX86);
            status.AppendFormat("ProgramData = {0};", ProgramData);
            status.AppendFormat("AppData = {0};", AppData);

            return status.ToString();
        }

        public static string GetValidationServiceVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
