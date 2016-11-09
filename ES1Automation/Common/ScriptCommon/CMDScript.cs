using System;
using System.Diagnostics;
using Microsoft.Win32;
using Common.Windows;
using Common.Network;

namespace Common.ScriptCommon
{
    public static class CMDScript
    {
        private static string output = string.Empty;
        private static string error = string.Empty;

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

        /// <summary>
        /// run a bat file with the parameters
        /// </summary>
        /// <param name="filename">the bat file path</param>
        /// <param name="argument">the parameters</param>
        /// <returns></returns>
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
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };

            try
            {
                cmd.Start();

                string output = string.Empty;
                output = cmd.StandardError.ReadToEnd().Length == 0 ? string.Format("[Output:{0}]", cmd.StandardOutput.ReadToEnd()) : string.Format("[Output:{0}] [Error:{1}]", cmd.StandardOutput.ReadToEnd(), cmd.StandardError.ReadToEnd());


                cmd.WaitForExit();

                cmd.Close();

                return output;
            }
            finally
            {
                cmd.Close();
            }
        }

        public static string RumCmdWithWindowsVisible(string filename, string argument)
        {
            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = filename,
                    Arguments = string.Format("/C {0}", argument),
                    UseShellExecute = true,
                    RedirectStandardInput = false,                    
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                }
            };

            try
            {
                cmd.Start();                

                cmd.WaitForExit();

                cmd.Close();

                return string.Empty;
            }
            finally
            {
                cmd.Close();
            }
        }

        public static string PsExec(string serverName, string userName, string password, string command)
        {
            output = string.Empty;

            error = string.Empty;

            string targetTemp = string.Format(@"\\{0}\C$\SaberAgent", serverName);

            string cmdString = string.Format(@"\\{0} -u ""{1}"" -p ""{2}"" ""{3}""", serverName, userName, password, command);

            var cmd = new Process
            {
                StartInfo =
                {
                    //FileName = @"PSTools\PsExec.exe",
                    FileName = @"PSTools\paexec.exe",
                    Arguments = cmdString,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            cmd.ErrorDataReceived += new DataReceivedEventHandler(PSExecErrorHandler);
            cmd.OutputDataReceived += new DataReceivedEventHandler(PSExecOutputHandler);
            cmd.Start();
            
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();

            cmd.WaitForExit();
            cmd.Close();
            string result = string.Format("Output:[{0}]; Error:[{1}]", output, error);
            return result;
        }

        private static void PSExecOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                output += outLine.Data;
            }
        }

        private static void PSExecErrorHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                output += outLine.Data;
            }
        }

        public static string PsExecCMD(string serverName, string userName, string password, string command)
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
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };

            cmd.Start();
            string result = string.Empty;
            result = cmd.StandardError.ReadToEnd().Length == 0 ? string.Format("[Output:{0}]", cmd.StandardOutput.ReadToEnd()) : string.Format("[Output:{0}] [Error:{1}]", cmd.StandardOutput.ReadToEnd(), cmd.StandardError.ReadToEnd());
            cmd.WaitForExit();
            cmd.Close();
            return result;
        }

        /// <summary>
        /// Close the app remotely using the image name forcely and all child processes will be terminated too.
        /// </summary>
        /// <param name="targetMachine">The remote machine's name or IP</param>
        /// <param name="userName">The administrator user</param>
        /// <param name="password">Administrator's password</param>
        /// <param name="imageName">The image's name(such as notepad.exe)</param>
        /// <returns>True if succeed, else false</returns>
        public static bool CloseRunningApplicationRemotely(string targetMachine, string userName, string password, string imageName)
        {
            bool ret = false;

            string temp = System.IO.Path.GetTempFileName() + ".bat";

            string targetTemp = string.Format(@"\\{0}\C$\SaberAgent", targetMachine);

            try
            {
                NetUseHelper.NetUserMachine(targetMachine, userName, password);

                string cmd = string.Format(@"taskkill /F /T /IM {0}", imageName);

                Common.FileCommon.TXTHelper.ClearTXTContent(temp);

                Common.FileCommon.TXTHelper.WriteNewLine(temp, cmd, System.Text.Encoding.Default);

                Common.FileCommon.FileHelper.CopyFile(temp, targetTemp, true);

                string result = CMDScript.PsExec(targetMachine, userName, password, string.Format(@"C:\SaberAgent\{0}", System.IO.Path.GetFileName(temp)));

                if ((result.ToLower().Contains("has been terminated") && result.ToLower().Contains("success")) || (result.ToLower().Contains("error") && result.ToLower().Contains("not found")))
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

        public static bool FlushDNSRemotely(string targetMachine, string userName, string password)
        {
            bool ret = false;

            string temp = System.IO.Path.GetTempFileName() + ".bat";

            string targetTemp = string.Format(@"\\{0}\C$\SaberAgent", targetMachine);

            try
            {
                NetUseHelper.NetUserMachine(targetMachine, userName, password);

                Common.Windows.WindowsServices.RestartServiceRemotely("Dnscache", targetMachine, 60 * 1000);//restart the DNS Client

                System.Threading.Thread.Sleep(1000);

                Common.FileCommon.TXTHelper.ClearTXTContent(temp);

                string cmd = string.Format(@"ipconfig /flushdns");

                Common.FileCommon.TXTHelper.WriteNewLine(temp, cmd, System.Text.Encoding.Default);

                cmd = string.Format(@"ipconfig /registerdns");

                Common.FileCommon.TXTHelper.WriteNewLine(temp, cmd, System.Text.Encoding.Default);

                Common.FileCommon.FileHelper.CopyFile(temp, targetTemp, true);

                string result = CMDScript.PsExec(targetMachine, userName, password, string.Format(@"C:\SaberAgent\{0}", System.IO.Path.GetFileName(temp)));

                if (result.ToLower().Contains("with error code 0"))
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
