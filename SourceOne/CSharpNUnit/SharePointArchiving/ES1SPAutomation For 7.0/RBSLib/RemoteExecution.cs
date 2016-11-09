using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text; 

namespace RBSLib
{
    public class RemoteExecution
    {
        public const string PsExecExe = "PsExec.exe";
        public const string PsExecExeLog = "PsExec.exe.log";
        public static string PsExecExePath { get; protected set; }

        static RemoteExecution()
        {
            PsExecEulaAgree();

            //string workingDir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(RemoteExecution)).Location);
            //PsExecExePath = Path.Combine(workingDir, PsExecExe);

            // because nunit: use relative path
            PsExecExePath = PsExecExe;
        }

        public static void Execute(string remoteServer, string filename, string args)
        {
            Execute(remoteServer, filename, args, true);
        }

        public static void Execute(string remoteServer, string filename, string args, bool copyExe)
        {
            bool localRun = true;

            // if this is on local machine 
            string runFileName = filename;
            string runArgs = args;

            // if this is onremote machine  server.com
            {
                string[] localHostNames = Environment.MachineName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string localHostName = localHostNames[0];

                string[] remoteHostNames = remoteServer.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string remoteHostName = remoteHostNames[0];

                // !!! NOT in the same server
                if (string.Compare(localHostName, remoteHostName, true) != 0)
                {
                    runFileName = PsExecExePath;
                    runArgs = string.Format(@"\\{0} {1} ""{2}"" {3}", remoteServer, copyExe ? "/c /v" : "", filename, args);  // /c => /c /f | /c /v ?
                    localRun = false;
                }
            }

            // PsExec.exe \\qspxdev /c ES1SPAgent.exe blablabla...
            Process process = new Process();
            process.StartInfo.FileName = runFileName;
            process.StartInfo.Arguments = runArgs;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;

            if (localRun) // this does not work in DLL but in APP it could work
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                //process.StartInfo.RedirectStandardError = true;
            }

            process.Start();

            string stdoutput = string.Empty;
            if (localRun)
            {
                stdoutput = process.StandardOutput.ReadToEnd();
                //process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            }

            // wait it ends
            process.WaitForExit();

            // log it
            string cmd = string.Format("{0} running: {1} {2}", DateTime.Now.ToString(), process.StartInfo.FileName, process.StartInfo.Arguments);
            File.AppendAllLines(PsExecExeLog, new string[] { cmd, stdoutput });
        }

        static void PsExecEulaAgree()
        {
            //[HKEY_CURRENT_USER\Software\Sysinternals\PsExec]
            //"EulaAccepted"=dword:00000001
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Sysinternals\PsExec");
            if (1 != (int)key.GetValue("EulaAccepted", 0))
                key.SetValue("EulaAccepted", 1, RegistryValueKind.DWord);
        }
    }
}
