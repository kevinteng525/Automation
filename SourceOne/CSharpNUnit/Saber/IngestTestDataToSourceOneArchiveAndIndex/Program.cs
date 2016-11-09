using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Saber.TestEnvironment;
using Saber.S1CommonAPILib;
using Saber.BaseTest;
using Saber.TestData;
using Saber.TestData.EWS;
using Saber.TestData.PST;

namespace IngestTestDataToSourceOneArchiveAndIndex
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string testDataPath = string.Empty;
                if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
                {
                    testDataPath = args[0];
                }
                else
                {
                    testDataPath = Path.Combine(GetAssemblePath(), "TestData");
                }
                S1WorkerTaskConfigHelper.ConfigAllTaskTypeToAllWorkers();
                //traverse all the files in the TestData folder, send the tsv files and ingest all .pst files
                string testMetadataPath = Path.Combine(GetAssemblePath(), "TestMetadata");
                
                if (!Directory.Exists(testDataPath) || !Directory.Exists(testMetadataPath))
                {
                    Console.WriteLine("The folder of test data or metadata doesn't exist!");
                    return;
                }

                Console.WriteLine("Start to prepare test data:");
                DirectoryInfo d = new DirectoryInfo(testDataPath);
                foreach (FileInfo file in d.GetFiles())
                {
                    if (file.Extension.ToLower() == ".tsv")
                    {
                        EWS ews = new EWS();
                        Console.WriteLine("Send the mails to exchange server using the tsv file: " + file.FullName);
                        ews.SendEmailByTSV(file.FullName);
                    }
                    else if (file.Extension.ToLower() == ".pst")
                    {
                        PSTHelper helper = new PSTHelper(file.FullName);
                        Console.WriteLine("Ingest the mails to exchange server using the pst file: " + file.FullName);
                        helper.IngestPSTMails2MailBoxOfCurrentUser();
                    }
                }

                string archiveDBName = TestEnvironmentHelper.ArchiveDB;
                string dbServer = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.SQLServer)[0].Name;
                string workerName = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.Worker)[0].Name;

                //Create connection
                Console.WriteLine("Create the archive connection");
                S1ArchiveConnection connection = new S1ArchiveConnection();
                connection.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1ArchiveConnection.xml"));
                S1ArchiveConnectionHelper.CreateArchiveConnection(connection);
                
                //Config the native archive servers
                Console.WriteLine("Config the native archive servers");
                foreach (S1ComponentHost host in TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.NativeArchive))
                {
                    String name = host.FullName;
                    S1NativeArchiveServerConfiguration naConfig = new S1NativeArchiveServerConfiguration();
                    naConfig.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1NativeArchiveServerConfiguration.xml"));
                    S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer(connection.Name, name, naConfig);
                }

                //config the workers' roles
                S1WorkerTaskConfigHelper.ConfigAllTaskTypeToAllWorkers();

                //Create the archive folder
                Console.WriteLine("Create the archive folder");
                S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
                archiveFolder.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1NativeArchiveFolder.xml"));
                S1NativeArchiveFolderHelper.CreateArchiveFolder(archiveFolder);
                
                //Create the mapped folder
                Console.WriteLine("Create the mapped folder");
                S1MappedFolder mappedFolder = new S1MappedFolder();
                mappedFolder.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1MappedFolder.xml"));
                S1MappedFolderHelper.CreateS1MappedFolder(mappedFolder);
                
                //Create the policy
                Console.WriteLine("Create the policy");
                S1OrganizationalPolicy policy = new S1OrganizationalPolicy();
                policy.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1OrganizationalPolicy.xml"));
                S1OrganizationalPolicyHelper.CreatePolicy(policy);
               
                //Create the historical archive activity
                Console.WriteLine("Create the historical archive activity");
                S1Activity activity = new S1Activity();
                activity.DeserializeFromXMLFile(Path.Combine(testMetadataPath, "S1Activity.xml"));
                int activityId = S1ActivityHelper.CreateS1HistoricalArchiveActivity(activity);
                
                //Wait the archive and index finishing
                Console.WriteLine("Wait the finishing of archiving");
                S1JobHelper.WaitAllJobsFinishForActivityWithId(activityId);
                Console.WriteLine("Wait the finishing of indexing");
                S1IndexHelper.WaitForIndexAccomplishOfArchiveFolder(connection.Name, archiveFolder.Name, 30);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);                
            }
        }

        static public String GetAssemblePath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
