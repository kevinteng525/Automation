using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Saber.S1CommonAPILib;

using Saber.TestData.EWS;
using Saber.TestData.PST;

using Saber.TestMetadata;
using Saber.TestData;
using Saber.TestEnvironment;


namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PSTHelper helper = new PSTHelper(@"D:\Demo\TestData\neil2.pst"); 
            helper.IngestPSTMails2MailBoxOfCurrentUser();
            

            S1NativeArchiveServerConfiguration config1 = new S1NativeArchiveServerConfiguration();
            config1.DeserializeFromXMLFile(@"D:\Galaxy_Neil_Dev\ES1Automation\Main\Saber\HistoricalArchiveTest\bin\Debug\TestMetadata\S1NativeArchiveServerConfiguration.xml");
            S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer("Demo Neil1234", "ES1ALL.usc.com", config1);
            
            String dbName = "";
            String dbServer = "";
            String workerName = "";
            S1ComponentConfig config = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.SQLServer)[0].GetConfigOfType(S1HostType.SQLServer);
            dbName = TestEnvironmentHelper.ArchiveDB;
            dbServer = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.SQLServer).First().Name;
            workerName = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.Worker)[0].Name;

            S1ArchiveConnection connection = new S1ArchiveConnection();
            connection.DeserializeFromXMLFile(@"D:\Demo\TestData\S1ArchiveConnection.xml");
            //connection.Name = "Test";
            //connection.DatabaseServer = dbServer;
            //connection.DatabaseName = dbName;
            //connection.Description = "Hello World";
            //connection.ArchiveConnectionType = S1ArchiveConnectionType.NativeArchive;
            S1ArchiveConnectionHelper.CreateArchiveConnection(connection);

            S1NativeArchiveServerConfiguration nativeArchiveConfig = new S1NativeArchiveServerConfiguration();
            nativeArchiveConfig.DeserializeFromXMLFile(@"D:\Demo\TestData\S1NativeArchiveServerConfiguration.xml");
            S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer("Demo 2", workerName, nativeArchiveConfig);


            S1NativeArchiveFolder archiveFolder = new S1NativeArchiveFolder();
            archiveFolder.DeserializeFromXMLFile(@"D:\Demo\TestData\S1NativeArchiveFolder.xml");
            //archiveFolder.ArchiveConnectionName = connection.Name;
            //archiveFolder.Storage_Type = S1ArchiveFolderStorageType.NASContainer;
            //archiveFolder.Name = "TestFolder";
            //S1NASContainerStorageConfig config = new S1NASContainerStorageConfig();
            //config.ArchiveLocation = @"\\ES1All\Share\Archive";

            //archiveFolder.Storage_ArchiveFolderConfig = config;
            //archiveFolder.Storage_CompressContentInContainers = true;
            //archiveFolder.Description = "Test Folder";
            //archiveFolder.Storage_EnableAutoDisposition = true;
            //archiveFolder.Index_EnabledIndexing = false;
            //archiveFolder.Index_MaximumIndexSize = 2048;
            //S1NativeArchiveFolderHelper.CreateArchiveFolder(archiveFolder);

            S1MappedFolder mappedFolder = new S1MappedFolder();
            mappedFolder.DeserializeFromXMLFile(@"D:\Demo\TestData\S1MappedFolder.xml");
            //mappedFolder.ArchiveFolder = archiveFolder.Name;
            //mappedFolder.ArchiveConnection = connection.Name;
            //mappedFolder.Description = "Test Mapped Folder";
            //mappedFolder.Permissions = new List<S1MappedFolderPermission>();
            //mappedFolder.Permissions.Add(new S1MappedFolderPermission("ES1Service", ADObjectType.User));
            //mappedFolder.Permissions.Add(new S1MappedFolderPermission("ES1Master", ADObjectType.User));
            //mappedFolder.Name = "TestMappedFolder";
            //mappedFolder.FolderType = S1BusinessFolderType.Organization;
            //S1MappedFolderHelper.CreateS1MappedFolder(mappedFolder);

            S1OrganizationalPolicy policy = new S1OrganizationalPolicy();
            policy.DeserializeFromXMLFile(@"D:\Demo\TestData\S1OrganizationalPolicy.xml");
            //policy.Name = "NeilTestPolicy";
            //policy.Description = "Hello World";
            //S1OrganizationalPolicyHelper.CreatePolicy(policy);

            S1Activity activity = new S1Activity();
            activity.DeserializeFromXMLFile(@"D:\Demo\TestData\S1Activity.xml");
            //activity.PolicyName = policy.Name;
            //activity.Name = "Neil Test";
            //activity.Activity_Type = S1ActivityType.Archive_Historical;
            //activity.DataSource_Type = S1DataSourceType.MicrosoftExchange;
            //activity.DataSource = new List<S1DataSource>();
            //activity.DataSource.Add(new S1DataSource("ES1Service", ADObjectType.User));
            //activity.DataSource.Add(new S1DataSource("ES1Administrator", ADObjectType.User));
            //activity.DataSource_MailBoxTypeMask = (int)S1MailBoxType.Primary | (int)S1MailBoxType.PersonalArchive;
            //activity.Item_S1MailBoxItemTypeMask = (int)S1MailBoxItemType.IncludeAll;
            //activity.Item_ReprocessItems = true;
            //activity.Folder_MailFolderTypeMask = (int)S1MailFolderType.Calendar |
            //    (int)S1MailFolderType.Contacts |
            //    (int)S1MailFolderType.DeletedItems;
            //activity.Folder_IncludeReadItems = true;
            //activity.Folder_IncludeUnreadItems = true;
            //activity.Folder_IncludeSubFolders = true;
            //activity.Folder_DeletedRetentionSoftDeleteItems = true;

            //activity.UserCreatedFolders = null;
            //activity.Dates_Type = S1ActivityDatesType.UseAll;

            //activity.AttachmentsFilter_ExtensionsExcluded = null;
            //activity.MessageTypesFilter_ExcludeMessageTypes = null;
            //activity.MessageTypesFilter_IncludeMessageTypes = null;

            //activity.MessageSizeFilter_IncludeMessageGreaterThan = 0;
            ////activity.IncludeMessageLessThan = 100;

            //S1BCEConfig bceconfig = new S1BCEConfig();
            //S1AddressFilteringRule rule1 = new S1AddressFilteringRule();
            //rule1.Name = "rule 1";
            //rule1.TargetMappedFolder = "MDAF10";
            //S1AddressFilteringRuleCondition condition = new S1AddressFilteringRuleCondition();
            //condition.FieldType = S1AddressRuleFieldType.WithSpecificWordsInSubject;
            //condition.FilterType = S1AddressRuleFilteringType.Keyword;
            ////condition.PeopleOrDistributionList.Add(new S1DataSource("ES1Service", ADObjectType.User));
            ////condition.PeopleOrDistributionList.Add(new S1DataSource("Administrator", ADObjectType.User));
            //condition.DomainOrSpecificWords.Add("Hello");
            //condition.DomainOrSpecificWords.Add("World");
            //rule1.Conditions.Add(condition);
            //S1AddressFilteringRule rule2 = S1AddressFilteringRuleHelper.CreateCopyMessagesNotMatchAnyRulesToMappedFolderRule(mappedFolder.Name);
            //bceconfig.Rules.Add(rule1);
            //bceconfig.Rules.Add(rule2);
            //S1BCEConfigHelper.CreateBCEConfigForMailBoxTask(bceconfig);

            //activity.BCE_AddressFilteringRules = new List<S1AddressFilteringRule>();
            //S1AddressFilteringRule rule = new S1AddressFilteringRule();
            //rule.TargetMappedFolder = mappedFolder.Name;
            //rule.Name = "test";
            //rule.Conditions = new List<S1AddressFilteringRuleCondition>();
            //S1AddressFilteringRuleCondition addresscondition = new S1AddressFilteringRuleCondition();
            //addresscondition.FilterType = S1AddressRuleFilteringType.CopyMessagesNotMatchAnyRuleTo;
            //rule.Conditions.Add(addresscondition);
            //activity.BCE_AddressFilteringRules.Add(rule);

            //activity.Schedule_StartDate = DateTime.Now;
            //activity.Schedule_StartTime = DateTime.Now;

            //activity.Schedule_Duration = 1;

            //activity.Schedule_ActivityRecurrencePattern = S1ActivityRecurrencePattern.Once;

            //S1WorkerGroup group = new S1WorkerGroup();
            //group.Name = "Test";
            //group.Description = "Hello";
            //group.Workers = new List<string>();
            //group.Workers.Add(workerName);
            //S1WorkerGroupHelper.CreateWorkerGroup(group);

            //activity.WorkerGroup_Name = group.Name;

            S1ActivityHelper.CreateS1HistoricalArchiveActivity(activity);

            #region temp

            //String testcaseConfig = @"C:\SaberAgent\testcase.xml";
            //TestMetadataManager manager = new TestMetadataManager(testcaseConfig);
            //String attr1 = manager.GetParameter("customized", "attr3");
            //String attr2 = manager.GetParameter("customized", "attr2");
            //String sharepoint = manager.GetParameter("sharepoint");

            //S1MappedFolder folder = new S1MappedFolder("Neil Test", "CONN", "AF10", "Hello WOrld");
            //List<S1MappedFolderPermission> permissions = new List<S1MappedFolderPermission>();
            //permissions.Add(new S1MappedFolderPermission("ES1Service", ADObjectType.User, 31));
            //permissions.Add(new S1MappedFolderPermission("Administrator", ADObjectType.User, 31));
            //permissions.Add(new S1MappedFolderPermission("ES1 Admins Group", ADObjectType.Group, 31));
            //permissions.Add(new S1MappedFolderPermission("ES1 Security Group", ADObjectType.Group, 31));
            //folder.mfPermissions = permissions;
            //folder.Type = S1BusinessFolderType.Organization;
            //S1MappedFolderHelper.CreateMappedFolder(folder);

            ////Test for mapped folder
            //S1NativeArchiveServerConfiguration config = new S1NativeArchiveServerConfiguration(@"\\ES1ALL\Shared\MC");
            //config.Archive_Enabled = true;
            //config.Index_Enabled = true;
            //config.Search_Enabled = true;
            //config.Retrieval_Enabled = false;
            //config.Index_ArchiveServersToIndex.Add("ES1ALL.sosp.com");
            //S1NativeArchiveServerConfigurationHelper.ConfigNativeArchiveServer("CONN","ES1ALL.sosp.com",config);

            ////test for BCE config
            //S1BCEConfig config = new S1BCEConfig();
            //S1AddressFilteringRule rule1 = new S1AddressFilteringRule();
            //rule1.Name = "rule 1";
            //rule1.TargetMappedFolder = "MDAF10";
            //S1AddressFilteringRuleCondition condition = new S1AddressFilteringRuleCondition();
            //condition.FieldType = S1AddressRuleFieldType.WithSpecificWordsInSubject;
            //condition.FilterType = S1AddressRuleFilteringType.Keyword;
            ////condition.PeopleOrDistributionList.Add(new S1DataSource("ES1Service", ADObjectType.User));
            ////condition.PeopleOrDistributionList.Add(new S1DataSource("Administrator", ADObjectType.User));
            //condition.DomainOrSpecificWords.Add("Hello");
            //condition.DomainOrSpecificWords.Add("World");
            //rule1.Conditions.Add(condition);
            //S1AddressFilteringRule rule2 = S1AddressFilteringRuleHelper.CreateCopyMessagesNotMatchAnyRulesToMappedFolderRule("MDAF20");
            //config.Rules.Add(rule1);
            //config.Rules.Add(rule2);
            //S1BCEConfigHelper.CreateBCEConfigForMailBoxTask(config);

            //S1WorkerGroup group = new S1WorkerGroup();
            //group.Name = "Neil Test";
            //group.Workers.Add("ES1ALL.sosp.com");
            //S1WorkerGroupHelper.CreateWorkerGroup(group);

            //EWS ews = new EWS();
            //ews.SendEmail("es1service@sosp.com","administrator@sosp.com","es1master@sosp.com",null,"Send by EWS", "Hello World");

            //ews.SendTaskByTSV("task.tsv");
            //ews.SendMeetingByTSV("meetings.tsv");
            //ews.SendMeeting("alanaeraysor@sosp.com","alanaegaymon@sosp.com","allanecluff@sosp.com;alanaeraysor@sosp.com",
            //    "2013-12-09 18:00:00","2","meeting1","body1","shanghai",@"c:\EMC_Archive_Install.log");
            //MDBUtil mdb = new MDBUtil("BaseData.mdb");
            //ews.SendEmailByMDB("BaseData.mdb");
            //ews.SendEmailByTSV("email.tsv");
            //ews.SendMeetingByTSV("meetings.tsv");
            //ews.SendTaskByTSV("task.tsv");
            //ews.SendEmail("micah.hua@emc.com",
            //     null,
            //     null,
            //     "micah.hua@emc.com",
            //     "sendtoself", "bodyof self",
            //     @"C:\EMC_Archive_Batch.log;C:\EMC_Archive_Batch.log", ".......");
            #endregion
        }
    }
}
