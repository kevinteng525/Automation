using System;
using System.Text;
using System.Xml.Linq;
using Common.SystemCommon;
using Common.FileCommon;
using Common.ScriptCommon;
using VerifyLib;
using RequestLib.Installation;

namespace TestConsoleClient
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("*****************************************************************************");
            Console.WriteLine("*      EMC SourceOne Installation Validation Tool Test Console Client       *");
            Console.WriteLine("*****************************************************************************");
            Console.WriteLine("");

            try
            {
                GACAssembly.GetGACAssemblyVersions();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:{0}", ex.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        private static void AutoItTest()
        {
            AutoIt autoIt = new AutoIt
            {
                SourceServer = "10.37.11.119",
                SourceUsername = @"es1\liut3",
                SourcePassword = "liut3",
                SourceInstallerPath = @"\\10.37.11.119\SourceOne Release Builds\ES1 7.0\GA\ES1_EM_7.00.2301\Setup\Windows\ES1_MasterSetup.exe",

                TargetServer = "10.37.11.70",
                TargetDomain = "QAES1",
                TargetUsername = @"es1service",
                TargetPassword = "emcsiax@QA",
                TargetInstallerPath = @"\\10.37.11.70\Share\Builds",
                AutoItScriptPath = @"\\10.32.175.136\Share\Personal\Hanson Wang\Automation",
                AutoItScriptName = @"Master.exe",

            };

            autoIt.RunAutoItScritp();
        }

        private static void ServiceInstallation()
        {
            Console.Write("Input the server to verify (default: 10.37.5.184):");
            string server = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(server))
            {
                server = @"10.37.5.184";
            }

            Console.Write("Install or Uninstall Service on Target Server(i=instll, u=uninstall, r=reinstall):");
            string ops = Console.ReadLine();

            if (ops == "i")
            {
                InstallWinService(server);
            }
            else if (ops == "u")
            {
                UninstallWinService(server);
            }
            else if (ops == "r")
            {
                ReinstallWinService(server);
            }
            else
            {
                Console.WriteLine("INPUT ERROR!");
            }
        }

        private static void Verify()
        {
            // config
            Console.Write("Input the path of Verify Config (default: {0}):", @"Configs\VerifyItems.xml");
            string configFilePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                configFilePath = @"Configs\VerifyItems.xml";
            }

            XElement config = XElement.Load(configFilePath);

            // install path
            Console.Write("Input the install path (default: {0}):", @"C:\Program Files (x86)\EMC SourceOne");
            string installPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(installPath))
            {
                installPath = @"C:\Program Files (x86)\EMC SourceOne";
            }

            // version
            Console.Write("Input the version of Verify Group (Example: {0}):", "7.0.0.2301");
            string version = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(version))
            {
                version = "7.0.0.2301";
            }

            // result view
            Console.Write("Input the Result View (0. All  1. SuccessOnly  2. FailedOnly   default: 0):");
            string view = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(view))
            {
                view = "ALL";
            }

            Console.WriteLine("");
            Console.WriteLine("-------------------- Verified Result----------------------");

            var verify = new Verify();
            verify.VerifyEnvironment.SetVersion(version);
            verify.VerifyEnvironment.ProgramFilePath = installPath;
            verify.VerifyEnvironment.VerifyConfig = config;

            verify.DoVerify();

            const string historyFolder = "ValidationHistory";

            // log result
            FileHelper.CreateFolder(historyFolder);

            string historyFileName = string.Format(@"{0}\Validation_{1}", historyFolder, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            TXTHelper.WriteTXTByLines(new[] {verify.ToXML()}, historyFileName + ".xml", Encoding.UTF8);

            Console.WriteLine("Verify Complete. View validation history under {0}", historyFolder);
        }

        private static void InstallWinService(string serverName)
        {
            // copy files to target
            string source = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\S1ValidationWinService\bin\Release";
            string target = string.Format(@"\\{0}\Share\S1ValidationWinService", serverName);
            FileHelper.CopyDirectory(source, target);

            // install service
            CMDScript.PsExec(serverName, @"qaes1\administrator", "emcsiax@QA", @"C:\Share\S1ValidationWinService\installService.cmd");
        }

        private static void UninstallWinService(string serverName)
        {
            CMDScript.PsExec(serverName, @"qaes1\administrator", "emcsiax@QA", @"C:\Share\S1ValidationWinService\UninstallService.cmd");
        }

        private static void ReinstallWinService(string serverName)
        {
            UninstallWinService(serverName);

            InstallWinService(serverName);
        }
    }
}
