using System;
using Common.FileCommon;
using Common.ScriptCommon;
using System.IO;
using RequestLib.Requests;

namespace RequestLib.Installation
{
    public class AutoIt
    {
        public string SourceServer { get; set; }

        public string SourceUsername { get; set; }

        public string SourcePassword { get; set; }

        public string SourceInstallerPath { get; set; }

        public string TargetServer { get; set; }

        public string TargetDomain { get; set; }

        public string TargetUsername { get; set; }

        public string TargetPassword { get; set; }

        public string TargetInstallerPath { get; set; }

        public string AutoItScriptPath { get; set; }

        public string AutoItScriptName { get; set; }

        private void GrantServerAccess(string server, string username, string password)
        {
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0} /delete", server));
            CMDScript.RumCmd("cmd.exe", string.Format(@"net  use \\{0}  {1} /user:{2}", server, password, username));
        }

        public void RunAutoItScritp()
        {
            // Grant Access to files
            if (!string.IsNullOrWhiteSpace(SourceUsername))
            {
                GrantServerAccess(SourceServer, SourceUsername, SourcePassword);
            }

            if (!string.IsNullOrWhiteSpace(TargetUsername))
            {
                GrantServerAccess(TargetServer, string.Format(@"{0}\{1}", TargetDomain, TargetUsername), TargetPassword);
            }

            // Create Target Directory
            FileHelper.CreateFolder(TargetInstallerPath);

            // Copy installer
            FileHelper.CopyFile(SourceInstallerPath, TargetInstallerPath, true);

            // Copy Script
            FileHelper.CopyFile(Path.Combine(AutoItScriptPath, AutoItScriptName), TargetInstallerPath, true);

            string script = string.Format(@"C:\Master.exe -params1 I");

            // Run Script
            RunCMDRequest request = new RunCMDRequest(TargetServer, 5000)
            {
                Filename = "cmd.exe",
                CMDScript = script,
                Domain = TargetDomain,
                Username = TargetUsername,
                Password = TargetPassword
            };

            string str = request.RequestServer();

            Console.WriteLine(str);
        }
    }
}
