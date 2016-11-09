using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Diagnostics;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace SourceOneDigitalSignVerifier
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ListFiles(FileSystemInfo info)
        {
            if (!info.Exists)
            {
                log.Error("Folder does not exist!");
                return;
            }
            DirectoryInfo dir = info as DirectoryInfo;
    
            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;

                if (file != null && file.Extension == ".exe")
                {
                    Console.ResetColor();
                    Console.WriteLine("Executable File Name:==================" + "\t");
                    Console.WriteLine(file.FullName);
                    log.Info("Executable File Name:" + file.FullName);
                    ValidateCertification(file);
                    ValidateCopyRight(file);
                }
                else
                {
                    ListFiles(files[i]);
                }
            }
        }

        public static void ValidateCopyRight(FileInfo file)
        {
            FileVersionInfo vInfo = FileVersionInfo.GetVersionInfo(file.FullName);
            if (vInfo.LegalCopyright == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Copyright information!");
                log.Error(file.FullName + "No Copyright information!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Copyright:" + vInfo.LegalCopyright);
                log.Info("Copyright:" + vInfo.LegalCopyright);
            }
        }

        public static void ValidateCertification(FileInfo file)
        {
            try
            {
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript("Get-AuthenticodeSignature \"" + file.FullName + "\"");

                Collection<PSObject> results = pipeline.Invoke();
                runspace.Close();
                Signature signature = results[0].BaseObject as Signature;
                if (signature.SignerCertificate == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No Digital Certification!");
                    log.Error(file.FullName + "No Digital Certification!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Issuer:" + signature.SignerCertificate.Issuer);
                    log.Info(file.FullName + "Issuer:" + signature.SignerCertificate.Issuer);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Status:" + signature.Status);
                    log.Info(file.FullName + "Status:" + signature.Status);
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error when trying to check if file is signed:" + file + " --> " + e.Message);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Please specify the build location:");
            string buildPath = "";
            buildPath = Console.ReadLine();
            log.Info("The build location" + buildPath);
            ListFiles(new DirectoryInfo(buildPath));

            Console.ResetColor();
            Console.WriteLine("Scan Completed.");
            Console.ReadLine();
        }
    }
}
