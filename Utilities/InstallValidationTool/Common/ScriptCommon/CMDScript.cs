using System.Diagnostics;
using Microsoft.Win32;
using Common.SystemCommon;

namespace Common.ScriptCommon
{
    
    public static class CMDScript
    {
        static string output;
        static CMDScript()
        {
            PsExecEulaAgree();
        }

        public static string RumCmd(string filename, string argument, string domain, string username, string password)
        {
            using (new Impersonator(username, domain, password))
            {
                return RumCmd(filename, argument);
            }
        }

        public static string RumCmd(string filename, string argument)
        {
            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = filename,
                    Arguments = string.Format("/C {0}", argument),
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    // This means that it will be redirected to the Process.StandardOutput StreamReader.
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };

            try
            {
                cmd.Start();

                cmd.WaitForExit();

                string output = cmd.StandardOutput.ReadToEnd();

                cmd.Close();

                return output;
            }
            finally
            {
                cmd.Close();
            }
        }

        public static void PsExec(string serverName, string userName, string password, string command)
        {
            string cmdString = string.Format(@"\\{0} -u ""{1}"" -p ""{2}"" ""{3}""", serverName, userName, password, command);

            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = @"PSTools\PsExec.exe",
                    Arguments = cmdString,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };

            cmd.Start();

            cmd.WaitForExit();
            cmd.Close();
        }

        public static string DCOMPerm(string command)
        {

            string cmdString = command;
            output = string.Empty;

            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = @"DCOMTools\dcomperm.exe",
                    Arguments = cmdString,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,

                    RedirectStandardOutput = true,
                }
            };

            cmd.OutputDataReceived += Cmd_OutputDataReceived;
            cmd.Start();
            cmd.BeginOutputReadLine();                      

            cmd.WaitForExit();
            cmd.Close();
            //output = output.Substring()      

            return output;
        }

        private static void Cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output += e.Data;
        }

        public static void PsExecCMD(string serverName, string userName, string password, string command)
        {
            string cmdString = string.Format(@"\\{0} -u ""{1}"" -p ""{2}"" CMD ""{3}""", serverName, userName, password, command);

            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = @"PSTools\PsExec.exe",
                    Arguments = cmdString,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };

            cmd.Start();

            cmd.WaitForExit();
            cmd.Close();
        }

        private static void PsExecEulaAgree()
        {
            //[HKEY_CURRENT_USER\Software\Sysinternals\PsExec]
            //"EulaAccepted"=dword:00000001
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Sysinternals\PsExec");

            if (1 != (int)key.GetValue("EulaAccepted", 0))
            {
                key.SetValue("EulaAccepted", 1, RegistryValueKind.DWord);
            }
        }
    }
}
