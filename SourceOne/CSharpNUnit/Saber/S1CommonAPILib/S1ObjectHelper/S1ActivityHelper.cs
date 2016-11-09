using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExBase;
using EMC.Interop.ExMBTaskAPI;
using EMC.Interop.ExProvider_2;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExExchProvider;


namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1ActivityType
    {
        [Description("Mailbox Management Archive JBS")]
        Archive_Historical,
        Archive_MicrosoftExchangePublicFolders_TBD,
        Archive_PersonalMailFiles_TBD,
        Archive_UserDirectedArchive_TBD,
        Delete_Historical_TBD,
        Delete_Archive_MicrosoftExchangePublicFolders_TBD,
        Delete_UserDirectedArchive_TBD,
        Delete_UserInitiatedDelete_TBD,
        FileArchive_Historical_TBD,
        FileDelete_Historical_TBD,
        FileIndexInPlace_Historical_TBD,
        FileRemoval_TBD,
        FileRestore_Historical_TBD,
        FileShortcuts_Historical_TBD,
        FileShortcutsRestore_Historical_TBD,
        Find_MicrosoftOfficeOutlookPST_TBD,
        Journal_TBD,
        Migrate_MicrosoftOfficeOutlookPST_TBD,
        [Description("Mailbox Management Shortcut Restore JBS")]
        RestoreShortcuts_HistoricalAndUserDirectedArchive,
        RestoreShortcuts_MicrosoftExchangePublicFolders_TBD,
        [Description("Mailbox Management Shortcut JBS")]
        Shortcut_Historical,
        Shortcuts_MicrosoftExchangePublicFolders_TBD,
        Shortcuts_UserDirectedArchive_TBD,
        UpdateShortcuts_HistoricalAndUserDirectedArchive_TBD,
        Undefined
    }

    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1ActivityState
    {
        [Description("exActivityState_Defined")]
        Defined = 0,
        [Description("exActivityState_Active")]
        Active = 1,
        [Description("exActivityState_ReadOnly")]
        ReadOnly = 2,
        [Description("exActivityState_Suspended")]
        Suspended = 8,
        [Description("exActivityState_Complete_Success")]
        CompleteSuccess = 16,
        [Description("exActivityState_Complete_Failed")]
        CompleteFailed = 32,
        [Description("exActivityState_System_Terminated")]
        SystemTerminated = 64,
        [Description("exActivityState_User_Terminated")]
        UserTerminated = 256,
        [Description("exActivityState_Expired")]
        Expired = 512,
    }

    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1DataSourceType
    {
        [Description("exDataProviderType_Unknown")]
        Unknown = 0,
        [Description("exDataProviderType_Exchange")]
        MicrosoftExchange = 1,
        [Description("exDataProviderType_Notes")]
        IBMLotusDomino = 2,
        [Description("exDataProviderType_SMTP")]
        SMTP = 3,
        [Description("exDataProviderType_DCTM")]
        DCTM = 4,
        [Description("exDataProviderType_Ex4X")]
        Ex4X = 5,
        [Description("exDataProviderType_Bloomberg")]
        Bloomberg = 6,
        [Description("exDataProviderType_ExAS")]
        ExAS = 7,
        [Description("exDataProviderType_URL")]
        URL = 8,
        [Description("exDataProviderType_Directory")]
        Directory = 9,
        [Description("exDataProviderType_SharePoint")]
        SharePoint = 10,
        [Description("exDataProviderType_SOCS")]
        SOCS = 11,
        [Description("exDataProviderType_ExAsIPM")]
        ExAsIPM = 12,
    }

    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1MailBoxItemType
    {
        [Description("exMailboxTaskCfgItemType_IncludeAll")]
        IncludeAll = -1,
        [Description("exMailboxTaskCfgItemType_Undefined")]
        Undefined = 0,
        [Description("exMailboxTaskCfgItemType_Email")]
        EmailMessages = 1,
        [Description("exMailboxTaskCfgItemType_Contact")]
        Contacts = 2,
        [Description("exMailboxTaskCfgItemType_Appointment")]
        Appointments = 4,
        [Description("exMailboxTaskCfgItemType_Task")]
        Tasks = 8,
        [Description("exMailboxTaskCfgItemType_Post")]
        PostedMessages = 16,
        [Description("exMailboxTaskCfgItemType_Activity")]
        Activities = 32,
        [Description("exMailboxTaskCfgItemType_StickyNote")]
        StickyNotes = 64,
        [Description("exMailboxTaskCfgItemType_Schedule")]
        Schedule = 128,
        [Description("exMailboxTaskCfgItemType_Document")]
        Documents = 256,
        [Description("exMailboxTaskCfgItemType_ReportDelivery")]
        ReportDeliveries = 512,
        [Description("exMailboxTaskCfgItemType_ReportReadReceipt")]
        ReportReadReceipts = 1024,
        [Description("exMailboxTaskCfgItemType_ReportOther")]
        ReportOthers = 2048,
        [Description("exMailboxTaskCfgItemType_Reports")]
        Reports = 3584,
        [Description("exMailboxTaskCfgItemType_Other")]
        Others = 4096,
        [Description("exMailboxTaskCfgItemType_Anniversary")]
        Anniversaries = 8192,
        [Description("exMailboxTaskCfgItemType_Event")]
        Events = 16384,
        [Description("exMailboxTaskCfgItemType_Reminder")]
        Reminders = 32768,
        [Description("exMailboxTaskCfgItemType_Meeting")]
        Meetings = 65536,
        [Description("exMailboxTaskCfgItemType_Notice")]
        Notices = 131072,
        [Description("exMailboxTaskCfgItemType_JournalEntry")]
        JournalEntries = 262144,
        [Description("exMailboxTaskCfgItemType_InstantMsg")]
        InstantMsgs = 524288,
    }

    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1MailFolderType
    {
        IncludeAll = -1,
        [Description("exMailConnectorFolderType_Calendar")]
        Calendar = 1,
        [Description("exMailConnectorFolderType_Contacts")]
        Contacts = 2,
        [Description("exMailConnectorFolderType_DeletedItems")]
        DeletedItems = 4,
        [Description("exMailConnectorFolderType_Inbox")]
        Inbox = 16,
        //below two type are from the EMC.Interop.ExExchProvider -> exExchangeConnectorFolderType 
        [Description("exExchangeConnectorFolderType_Notes")]
        Notes = 64,
        [Description("exExchangeConnectorFolderType_Outbox")]
        Outbox = 128,
        [Description("exMailConnectorFolderType_SentItems")]
        SentItems = 256,
        [Description("exMailConnectorFolderType_Tasks")]
        Tasks = 512,
        [Description("exMailConnectorFolderType_JunkEmail")]
        JunkEmail = 1024,
        [Description("exMailConnectorFolderType_UserDefined")]
        UserDefined = 2048,
    }

    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1MailBoxType
    {
        [Description("exMailboxType_Unknown")]
        Unknown = 0,
        [Description("exMailboxType_Primary")]
        Primary = 1,
        [Description("exMailboxType_PersonalArchive")]
        PersonalArchive = 2,
    }

    #region The classes for S1ActivityRecurrencePattern
    public enum S1ActivityRecurrencePattern
    {
        Once,
        Daily,
        Weekly,
        Monthly,
    }
    
    public class S1ActivityRecurrencePatternDailyConfig : IS1Object
    {
        public int RunDays { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
    }
    
    public class S1ActivityRecurrencePatternWeeklyConfig : IS1Object
    {
        public int EveryXWeeks { get; set; }
        public int WeekDays { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
    }
    
    public class S1ActivityRecurrencePatternMonthlyConfig : IS1Object
    {
        public int XDays { get; set; }
        public int OfXMonthes { get; set; }
        public int Xth { get; set; }
        public int WeekDay { get; set; }
        public int EveryXMonthes { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region the classes for the S1ActivityDatesType
    public enum S1ActivityDatesType
    {
        UseAll,
        Dated,
        Aged,
    }

    public class S1ActivityDatesDatedConfig : IS1Object
    {
        public DATEFILTERMETHOD BeforeAfterOrBetween { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
    }
    
    public class S1ActivityDatesAgedConfig : IS1Object
    {
        public DATEFILTERMETHOD NewerElderOrSpanDays { get; set; }
        public int Days { get; set; }
        public int Days2 { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    public class S1ActivityHelper
    {
        #region old_implementation
        //static public IExActivity3 CreateHistoricalArchiveActivity()
        //{
        //    IExActivity3 activity = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
        //    //policy
        //    activity.policyID = S1OrganizationalPolicyHelper.GetPolicyByName("Neil Test").id;
        //    //State
        //    String state = "exActivityState_Active";
        //    activity.state = (exActivityState)Enum.Parse(typeof(exActivityState), state, true);
        //    //description
        //    activity.description = "Hello World"; 
        //    //taskType
        //    String taskType = "Mailbox Management Archive JBS";
        //    CoExTaskTypeFilter filter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
        //    IExVector taskTypes = S1Context.JDFAPIMgr.GetTaskTypes(filter);            
        //    foreach (IExTaskType t in taskTypes)
        //    {
        //        if (t.name == taskType)
        //            activity.taskTypeID = t.id;
        //    }

        //    #region xConfig
        //    //xConfig
        //    IExArchiveTaskConfig config = new CoExArchiveTaskConfig();
        //    //type of data source
        //    config.MailSystem = exDataProviderType.exDataProviderType_Exchange;
        //    //data source            
        //    CoExVector vdataSources = new CoExVector();
        //    String userMailBox = "ES1Service";
        //    IExVector dataSources = S1Context.JDFAPIMgr.GetDataSources(S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_DataSourceFilter));
        //    foreach (IExDataSource3 ds in dataSources)
        //    {
        //        if(ds.friendlyName == userMailBox)
        //            vdataSources.Add(ds);

        //    }
        //    config.DataSources = vdataSources;
        //    //data source types
        //    config.DataSourceTypeMask = (int)exMailboxType.exMailboxType_Primary | (int)exMailboxType.exMailboxType_PersonalArchive;

        //    //items type
        //    config.ItemTypesMask = (int)exMailboxTaskCfgItemType.exMailboxTaskCfgItemType_IncludeAll;
        //    //reprocessing options
        //    config.RearchiveFlag = true;
        //    //folders
        //    config.FoldersMask = (int)exMailConnectorFolderType.exMailConnectorFolderType_Calendar                                
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_Contacts
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_DeletedItems
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_JunkEmail
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_SentItems
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_Tasks
        //                        | (int)exMailConnectorFolderType.exMailConnectorFolderType_UserDefined;
        //    config.IncludeSubFolders = true;
        //    config.ReadItems = true;
        //    config.UnreadItems = true;
        //    config.SoftDeletes = false;
        //    //user created folder
        //    CoExVector userFolders = new CoExVector();
        //    userFolders.Add("Test Folder 1");
        //    userFolders.Add("Test Folder 2");
        //    config.UserFolders = userFolders;
        //    //Date
        //    config.DateFilterMethod = DATEFILTERMETHOD.Date_Range;
        //    config.Days = 10;
        //    config.Days2 = 20;
        //    //DATEFILTERMETHOD.Date_After,DATEFILTERMETHOD.Date_Before,DATEFILTERMETHOD.Date_Newer,DATEFILTERMETHOD.Date_Older,DATEFILTERMETHOD.Date_Range,DATEFILTERMETHOD.Date_Span_Days,DATEFILTERMETHOD.Date_UseAll;
        //    config.DateFilterProperty = DATEFILTERPROPERTY.Date_UseReceivedDate;//DATEFILTERPROPERTY.Date_UseLastModifiedDate,DATEFILTERPROPERTY.Date_UseArchivedDate
        //    //Attachment filter
        //    string excludeAttachTypes = ".pdf,.txt";
        //    config.ExcludeAttachTypes = excludeAttachTypes;
        //    //Message tyle filter
        //    string excludeMsgClasses = "";
        //    config.ExcludeMsgClasses = excludeMsgClasses;
        //    //Message size filter
        //    config.MaxMsgSize = 100;//-1 for no limit 
        //    config.MinMsgSize = 0;      
            

        //    config.Date1 = DateTime.UtcNow;
        //    config.Date2 = DateTime.UtcNow;

        //    activity.xConfig = config.GetConfigXML();
        //    #endregion

        //    //schedule
        //    activity.runFrequency = exActivityFrequency.exActivityFrequency_JustOnce;
        //    activity.startDate = DateTime.Now.AddDays(1);
        //    activity.startDate = DateTime.Now.AddDays(1);
        //    activity.endDate = DateTime.Now.AddDays(2);
        //    activity.duration = 2;//hours
        //    activity.runDays = 10;
        //    activity.runPattern = 1;
        //    #region BCE
        //    //Bussiness component
        //    CoExVector vBCEs = new CoExVector();
        //    CoExBCEObjectConfig bceConfig = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_BCEObjectConfig);
        //    bceConfig.name = "Address Rules" + DateTime.Now.ToString();
        //    //bceConfig.bceObjectID = 0;
        //    CoExVector bces = S1Context.JDFAPIMgr.GetBCEObjects(S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_BCEObjectFilter));
        //    foreach (IExBCEObject bce in bces)
        //    {
        //        if (bce.name == bceConfig.name)
        //        {
        //            bceConfig.bceObjectID = bce.id;
        //            break;
        //        }
        //    }
        //    if (bceConfig.bceObjectID == 0)
        //    {
        //        //throw new Exception("Could not find BEC named: " + bceConfig.name);
        //    }

        //    String mapFolderName = "MDAF1";

        //    IExJanusFolder_2 mapFolder = S1Context.FolderMgr.FindFolderByName(mapFolderName);
        //    int bceConfigId = S1BCEHelper.CreateBCEConfigWithLeastRuleForMailBoxTask(bceConfig.name, mapFolder.FolderId);

        //    vBCEs.Add(S1Context.JDFAPIMgr.GetBCEObjectConfigByID(bceConfigId));
        //    activity.bceConfigs = vBCEs;
        //    #endregion
        //    //name
        //    activity.name = "Neil Test" + DateTime.UtcNow.ToString();
        //    //workers group
        //    //activity.workerGroupID = -1;
        //    //logging
        //    activity.optionsMask = (int)exActivityOptions.exActivityOptions_EnableLogging;
        //    //persistant
        //    activity.Save();
        //    return activity;
        //}
        #endregion
        /// <summary>
        /// Create a historical archive activity
        /// </summary>
        /// <param name="s1Activity">The parameters needed to create the activity.</param>
        /// <returns>true if successfully created activity, else return false</returns>
        static public int CreateS1HistoricalArchiveActivity(S1Activity s1Activity)
        {
            IExActivity3 activity = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
            //policy
            activity.policyID = S1OrganizationalPolicyHelper.GetByName(s1Activity.PolicyName).id;
            //State
            activity.state = (exActivityState)s1Activity.State;
            //description
            activity.description = s1Activity.Description;
            //taskType
            String taskType = EnumToStringUsingDescription.GetS1EnumByDescription(s1Activity.Activity_Type);
            CoExTaskTypeFilter filter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
            IExVector taskTypes = S1Context.JDFAPIMgr.GetTaskTypes(filter);
            Console.WriteLine("taskType = " + taskType);
            foreach (IExTaskType t in taskTypes)
            {
                //Console.WriteLine(t.name);
                if (t.name == taskType)
                {
                    activity.taskTypeID = t.id;
                }
            }
            if (activity.taskTypeID <= 0)
            {
                throw new Exception("Can not find the task types with name:" + taskType);
            }
            #region xConfig
            //xConfig
            IExArchiveTaskConfig config = new CoExArchiveTaskConfig();
            //type of data source
            config.MailSystem = (exDataProviderType)s1Activity.DataSource_Type;
            //data source, TODO            
            CoExVector vdataSources = new CoExVector();
            foreach (S1DataSource source in s1Activity.DataSource)
            {
                vdataSources.Add(S1DataSourceHelper.CreateDataSource(source));
            }
            config.DataSources = vdataSources;
            //data source types
            config.DataSourceTypeMask = s1Activity.DataSource_MailBoxTypeMask;
            //items type
            config.ItemTypesMask = s1Activity.Item_S1MailBoxItemTypeMask;
            //reprocessing options
            config.RearchiveFlag = s1Activity.Item_ReprocessItems;
            //folders
            config.FoldersMask = s1Activity.Folder_MailFolderTypeMask;
            config.IncludeSubFolders = s1Activity.Folder_IncludeSubFolders;
            config.ReadItems = s1Activity.Folder_IncludeReadItems;
            config.UnreadItems = s1Activity.Folder_IncludeUnreadItems;
            config.SoftDeletes = s1Activity.Folder_DeletedRetentionSoftDeleteItems;
            //user created folder
            CoExVector userFolders = new CoExVector();
            foreach (string folder in s1Activity.UserCreatedFolders)
            {
                userFolders.Add(folder);
            }
            config.UserFolders = userFolders;
            //Date
            switch (s1Activity.Dates_Type)
            {
                case S1ActivityDatesType.UseAll:
                    config.DateFilterMethod = DATEFILTERMETHOD.Date_UseAll;
                    break;
                case S1ActivityDatesType.Dated:
                    config.DateFilterMethod = s1Activity.Dates_DatedConfig.BeforeAfterOrBetween;
                    config.Date1 = s1Activity.Dates_DatedConfig.Date1;
                    config.Date2 = s1Activity.Dates_DatedConfig.Date2;
                    break;
                case S1ActivityDatesType.Aged:
                    config.DateFilterMethod = s1Activity.Dates_AgedConfig.NewerElderOrSpanDays;
                    config.Days = s1Activity.Dates_AgedConfig.Days;
                    config.Days2 = s1Activity.Dates_AgedConfig.Days2;
                    break;
            }

            //DATEFILTERMETHOD.Date_After,DATEFILTERMETHOD.Date_Before,DATEFILTERMETHOD.Date_Newer,DATEFILTERMETHOD.Date_Older,DATEFILTERMETHOD.Date_Range,DATEFILTERMETHOD.Date_Span_Days,DATEFILTERMETHOD.Date_UseAll;
            config.DateFilterProperty = s1Activity.Dates_BasedUpon;//DATEFILTERPROPERTY.Date_UseLastModifiedDate,DATEFILTERPROPERTY.Date_UseArchivedDate
            //Attachment filter            
            config.ExcludeAttachTypes = s1Activity.AttachmentsFilter_ExtensionsExcluded;
            //Message tyle filter            
            config.ExcludeMsgClasses = s1Activity.MessageTypesFilter_ExcludeMessageTypes;
            config.IncludeMsgClasses = s1Activity.MessageTypesFilter_IncludeMessageTypes;
            //Message size filter
            config.MaxMsgSize = s1Activity.MessageSizeFilter_IncludeMessageLessThan;//-1 for no limit 
            config.MinMsgSize = s1Activity.MessageSizeFilter_IncludeMessageGreaterThan;

            activity.xConfig = config.GetConfigXML();
            #endregion

            //schedule
            activity.startDate = s1Activity.Schedule_StartDate;
            activity.startTime = s1Activity.Schedule_StartTime;
            activity.duration = s1Activity.Schedule_Duration;
            switch (s1Activity.Schedule_ActivityRecurrencePattern)
            {
                case S1ActivityRecurrencePattern.Once:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_JustOnce;
                    break;
                case S1ActivityRecurrencePattern.Daily:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_Daily;
                    activity.runDays = s1Activity.Schedule_DailyConfig.RunDays;
                    break;
                case S1ActivityRecurrencePattern.Weekly:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_Weekly;
                    activity.runDays = s1Activity.Schedule_WeeklyConfig.WeekDays;
                    activity.runPattern = s1Activity.Schedule_WeeklyConfig.EveryXWeeks;
                    break;
                case S1ActivityRecurrencePattern.Monthly:
                    if (s1Activity.Schedule_MonthlyConfig.XDays != 0)
                    {
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDate;
                        activity.runDays = s1Activity.Schedule_MonthlyConfig.XDays;
                        activity.runPattern = s1Activity.Schedule_MonthlyConfig.EveryXMonthes;
                    }
                    else//TODO
                    {
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDOW;
                        activity.runPattern = s1Activity.Schedule_MonthlyConfig.OfXMonthes;//TODO
                        activity.runDays = s1Activity.Schedule_MonthlyConfig.WeekDay;
                    }
                    break;
            }
            activity.endDate = s1Activity.Schedule_EndBy;
            #region BCE
            //Bussiness component
            S1BCEConfig bceConfig = new S1BCEConfig();
            if (s1Activity.BCE_CopyMessageDonotMatchAnyRuleTo_Enable)
            {
                bceConfig.Rules.Add(S1AddressFilteringRuleHelper.CreateCopyMessagesNotMatchAnyRulesToMappedFolderRule(s1Activity.BCE_CopyMessageDonotMatchAnyRuleTo_Folder));
            }
            foreach (S1AddressFilteringRule rule in s1Activity.BCE_AddressFilteringRules)
            {
                bceConfig.Rules.Add(rule);
            }
            
            CoExVector vBCEs = new CoExVector();
            vBCEs.Add(S1BCEConfigHelper.GetByID(S1BCEConfigHelper.CreateBCEConfigForMailBoxTask(bceConfig)));  
            activity.bceConfigs = vBCEs;

            #endregion
            //name
            activity.name = s1Activity.Name;
            //workers group
            if (String.IsNullOrEmpty(s1Activity.WorkerGroup_Name))
            {
                //activity.workerGroupID = -1;
            }
            else
            {
                activity.workerGroupID = S1WorkerGroupHelper.GetByName(s1Activity.WorkerGroup_Name).id;
            }
            //logging
            if (s1Activity.EnableDetailedLogging)
            {
                activity.optionsMask = (int)exActivityOptions.exActivityOptions_EnableLogging;
            }
            else
            {
                activity.optionsMask = (int)exActivityOptions.exActivityOptions_Unknown;
            }
            //persistant
            activity.Save();
            return activity.id;
        }

        static public int CreateS1HistoricalShortcutActivity(S1Activity s1Activity)
        {
            IExActivity3 activity = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
            //policy
            activity.policyID = S1OrganizationalPolicyHelper.GetByName(s1Activity.PolicyName).id;
            //State
            activity.state = (exActivityState)s1Activity.State;
            //description
            activity.description = s1Activity.Description;
            //taskType
            String taskType = EnumToStringUsingDescription.GetS1EnumByDescription(s1Activity.Activity_Type);
            if (taskType == null )
            {
                throw new Exception("Tasktype enum result is null");
            }
            CoExTaskTypeFilter filter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
            IExVector taskTypes = S1Context.JDFAPIMgr.GetTaskTypes(filter);
            Console.WriteLine("taskType = " + taskType);
            foreach (IExTaskType t in taskTypes)
            {
                //Console.WriteLine(t.name);
                if (t.name == taskType)
                {
                    
                    activity.taskTypeID = t.id;
                }
            }
            if (activity.taskTypeID <= 0)
            {
                throw new Exception("Can not find the task types with name:" + taskType);
            }
            #region xConfig
            //xConfig
            IExShortcutTaskConfig config = new CoExShortcutTaskConfig();
            //IExArchiveTaskConfig config = new CoExArchiveTaskConfig();
            //type of data source
            config.MailSystem = (exDataProviderType)s1Activity.DataSource_Type;
            //data source, TODO            
            CoExVector vdataSources = new CoExVector();
            foreach (S1DataSource source in s1Activity.DataSource)
            {
                vdataSources.Add(S1DataSourceHelper.CreateDataSource(source));
            }
            config.DataSources = vdataSources;
            //data source types
            config.DataSourceTypeMask = s1Activity.DataSource_MailBoxTypeMask;
            //items type
            config.ItemTypesMask = s1Activity.Item_S1MailBoxItemTypeMask;
            //reprocessing options
            //config.RearchiveFlag = s1Activity.Item_ReprocessItems;
            //folders
            config.FoldersMask = s1Activity.Folder_MailFolderTypeMask;
            config.IncludeSubFolders = s1Activity.Folder_IncludeSubFolders;
            config.ReadItems = s1Activity.Folder_IncludeReadItems;
            config.UnreadItems = s1Activity.Folder_IncludeUnreadItems;
            config.SoftDeletes = s1Activity.Folder_DeletedRetentionSoftDeleteItems;
            //user created folder
            CoExVector userFolders = new CoExVector();
            foreach (string folder in s1Activity.UserCreatedFolders)
            {
                userFolders.Add(folder);
            }
            config.UserFolders = userFolders;

            //shortcut options
            config.Language = s1Activity.ShortcutLanguageID;
            config.Options = s1Activity.ShortcutIncludeMessageBody;
            config.InlineImageLimit = s1Activity.ShortcutInlineImageGreaterThan;            

            //Date
            switch (s1Activity.Dates_Type)
            {
                case S1ActivityDatesType.UseAll:
                    config.DateFilterMethod = DATEFILTERMETHOD.Date_UseAll;
                    break;
                case S1ActivityDatesType.Dated:
                    config.DateFilterMethod = s1Activity.Dates_DatedConfig.BeforeAfterOrBetween;
                    config.Date1 = s1Activity.Dates_DatedConfig.Date1;
                    config.Date2 = s1Activity.Dates_DatedConfig.Date2;
                    break;
                case S1ActivityDatesType.Aged:
                    config.DateFilterMethod = s1Activity.Dates_AgedConfig.NewerElderOrSpanDays;
                    config.Days = s1Activity.Dates_AgedConfig.Days;
                    config.Days2 = s1Activity.Dates_AgedConfig.Days2;
                    break;
            }

            //DATEFILTERMETHOD.Date_After,DATEFILTERMETHOD.Date_Before,DATEFILTERMETHOD.Date_Newer,DATEFILTERMETHOD.Date_Older,DATEFILTERMETHOD.Date_Range,DATEFILTERMETHOD.Date_Span_Days,DATEFILTERMETHOD.Date_UseAll;
            config.DateFilterProperty = s1Activity.Dates_BasedUpon;//DATEFILTERPROPERTY.Date_UseLastModifiedDate,DATEFILTERPROPERTY.Date_UseArchivedDate
            //Attachment filter            
            config.ExcludeAttachTypes = s1Activity.AttachmentsFilter_ExtensionsExcluded;
            //Message style filter            
            config.ExcludeMsgClasses = s1Activity.MessageTypesFilter_ExcludeMessageTypes;
            config.IncludeMsgClasses = s1Activity.MessageTypesFilter_IncludeMessageTypes;
            //Message size filter
            config.MaxMsgSize = s1Activity.MessageSizeFilter_IncludeMessageLessThan;//-1 for no limit 
            config.MinMsgSize = s1Activity.MessageSizeFilter_IncludeMessageGreaterThan;

            activity.xConfig = config.GetConfigXML();
            #endregion

            //schedule
            activity.startDate = s1Activity.Schedule_StartDate;
            activity.startTime = s1Activity.Schedule_StartTime;
            activity.duration = s1Activity.Schedule_Duration;
            switch (s1Activity.Schedule_ActivityRecurrencePattern)
            {
                case S1ActivityRecurrencePattern.Once:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_JustOnce;
                    break;
                case S1ActivityRecurrencePattern.Daily:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_Daily;
                    activity.runDays = s1Activity.Schedule_DailyConfig.RunDays;
                    break;
                case S1ActivityRecurrencePattern.Weekly:
                    activity.runFrequency = exActivityFrequency.exActivityFrequency_Weekly;
                    activity.runDays = s1Activity.Schedule_WeeklyConfig.WeekDays;
                    activity.runPattern = s1Activity.Schedule_WeeklyConfig.EveryXWeeks;
                    break;
                case S1ActivityRecurrencePattern.Monthly:
                    if (s1Activity.Schedule_MonthlyConfig.XDays != 0)
                    {
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDate;
                        activity.runDays = s1Activity.Schedule_MonthlyConfig.XDays;
                        activity.runPattern = s1Activity.Schedule_MonthlyConfig.EveryXMonthes;
                    }
                    else//TODO
                    {
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDOW;
                        activity.runPattern = s1Activity.Schedule_MonthlyConfig.OfXMonthes;//TODO
                        activity.runDays = s1Activity.Schedule_MonthlyConfig.WeekDay;
                    }
                    break;
            }
            activity.endDate = s1Activity.Schedule_EndBy;
            #region BCE
            //Bussiness component
            S1BCEConfig bceConfig = new S1BCEConfig();
            if (s1Activity.BCE_CopyMessageDonotMatchAnyRuleTo_Enable)
            {
                bceConfig.Rules.Add(S1AddressFilteringRuleHelper.CreateCopyMessagesNotMatchAnyRulesToMappedFolderRule(s1Activity.BCE_CopyMessageDonotMatchAnyRuleTo_Folder));
            }
            foreach (S1AddressFilteringRule rule in s1Activity.BCE_AddressFilteringRules)
            {
                bceConfig.Rules.Add(rule);
            }

            CoExVector vBCEs = new CoExVector();
            vBCEs.Add(S1BCEConfigHelper.GetByID(S1BCEConfigHelper.CreateBCEConfigForMailBoxTask(bceConfig)));
            activity.bceConfigs = vBCEs;

            #endregion
            //name
            activity.name = s1Activity.Name;
            //workers group
            if (String.IsNullOrEmpty(s1Activity.WorkerGroup_Name))
            {
                //activity.workerGroupID = -1;
            }
            else
            {
                activity.workerGroupID = S1WorkerGroupHelper.GetByName(s1Activity.WorkerGroup_Name).id;
            }
            //logging
            if (s1Activity.EnableDetailedLogging)
            {
                activity.optionsMask = (int)exActivityOptions.exActivityOptions_EnableLogging;
            }
            else
            {
                activity.optionsMask = (int)exActivityOptions.exActivityOptions_Unknown;
            }
            //persistant
            activity.Save();
            return activity.id;
        }

        static public int CreateS1ShortcutRestoreActivity(S1Activity s1Activity)
        {
            {
                IExActivity3 activity = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
                //policy
                activity.policyID = S1OrganizationalPolicyHelper.GetByName(s1Activity.PolicyName).id;
                //State
                activity.state = (exActivityState)s1Activity.State;
                //description
                activity.description = s1Activity.Description;
                //taskType
                String taskType = EnumToStringUsingDescription.GetS1EnumByDescription(s1Activity.Activity_Type);
                CoExTaskTypeFilter filter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_TaskTypeFilter);
                IExVector taskTypes = S1Context.JDFAPIMgr.GetTaskTypes(filter);
                List<string> names = new List<string>();
                foreach (IExTaskType t in taskTypes)
                {
                    if (t.name == taskType)
                    {
                        activity.taskTypeID = t.id;

                    }
                    names.Add(t.name);
                }
                if (activity.taskTypeID <= 0)
                {
                    throw new Exception("Can not find the task types with name:" + taskType);
                }
                #region xConfig
                //xConfig
                IExShortcutRestoreTaskConfig config = new CoExShortcutRestoreTaskConfig();
                //type of data source
                config.MailSystem = (exDataProviderType)s1Activity.DataSource_Type;
                //data source, TODO            
                CoExVector vdataSources = new CoExVector();
                foreach (S1DataSource source in s1Activity.DataSource)
                {
                    vdataSources.Add(S1DataSourceHelper.CreateDataSource(source));
                }
                config.DataSources = vdataSources;
                //data source types
                config.DataSourceTypeMask = s1Activity.DataSource_MailBoxTypeMask;
                //items type
                config.ItemTypesMask = s1Activity.Item_S1MailBoxItemTypeMask;
                //reprocessing options
                //            config.RearchiveFlag = s1Activity.Item_ReprocessItems;
                //folders
                config.FoldersMask = s1Activity.Folder_MailFolderTypeMask;
                config.IncludeSubFolders = s1Activity.Folder_IncludeSubFolders;
                config.ReadItems = s1Activity.Folder_IncludeReadItems;
                config.UnreadItems = s1Activity.Folder_IncludeUnreadItems;
                //            config.SoftDeletes = s1Activity.Folder_DeletedRetentionSoftDeleteItems;
                //user created folder
                CoExVector userFolders = new CoExVector();
                foreach (string folder in s1Activity.UserCreatedFolders)
                {
                    userFolders.Add(folder);
                }
                config.UserFolders = userFolders;
                //Date
                switch (s1Activity.Dates_Type)
                {
                    case S1ActivityDatesType.UseAll:
                        config.DateFilterMethod = DATEFILTERMETHOD.Date_UseAll;
                        break;
                    case S1ActivityDatesType.Dated:
                        config.DateFilterMethod = s1Activity.Dates_DatedConfig.BeforeAfterOrBetween;
                        config.Date1 = s1Activity.Dates_DatedConfig.Date1;
                        config.Date2 = s1Activity.Dates_DatedConfig.Date2;
                        break;
                    case S1ActivityDatesType.Aged:
                        config.DateFilterMethod = s1Activity.Dates_AgedConfig.NewerElderOrSpanDays;
                        config.Days = s1Activity.Dates_AgedConfig.Days;
                        config.Days2 = s1Activity.Dates_AgedConfig.Days2;
                        break;
                }

                //DATEFILTERMETHOD.Date_After,DATEFILTERMETHOD.Date_Before,DATEFILTERMETHOD.Date_Newer,DATEFILTERMETHOD.Date_Older,DATEFILTERMETHOD.Date_Range,DATEFILTERMETHOD.Date_Span_Days,DATEFILTERMETHOD.Date_UseAll;
                config.DateFilterProperty = s1Activity.Dates_BasedUpon;//DATEFILTERPROPERTY.Date_UseLastModifiedDate,DATEFILTERPROPERTY.Date_UseArchivedDate
                //Attachment filter            
                config.ExcludeAttachTypes = s1Activity.AttachmentsFilter_ExtensionsExcluded;
                //config.IncludeItemsOnlyWithAttach = s1Activity.AttachmentsFilter_IncludeItemsOnlyWithAttach;
                //Message tyle filter            
                config.ExcludeMsgClasses = s1Activity.MessageTypesFilter_ExcludeMessageTypes;
                config.IncludeMsgClasses = s1Activity.MessageTypesFilter_IncludeMessageTypes;
                //Message size filter
                config.MaxMsgSize = s1Activity.MessageSizeFilter_IncludeMessageLessThan;//-1 for no limit 
                config.MinMsgSize = s1Activity.MessageSizeFilter_IncludeMessageGreaterThan;

                activity.xConfig = config.GetConfigXML();
                #endregion

                #region schedule
                //schedule
                activity.startDate = s1Activity.Schedule_StartDate;
                activity.startTime = s1Activity.Schedule_StartTime;
                activity.duration = s1Activity.Schedule_Duration;
                switch (s1Activity.Schedule_ActivityRecurrencePattern)
                {
                    case S1ActivityRecurrencePattern.Once:
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_JustOnce;
                        break;
                    case S1ActivityRecurrencePattern.Daily:
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_Daily;
                        activity.runDays = s1Activity.Schedule_DailyConfig.RunDays;
                        break;
                    case S1ActivityRecurrencePattern.Weekly:
                        activity.runFrequency = exActivityFrequency.exActivityFrequency_Weekly;
                        activity.runDays = s1Activity.Schedule_WeeklyConfig.WeekDays;
                        activity.runPattern = s1Activity.Schedule_WeeklyConfig.EveryXWeeks;
                        break;
                    case S1ActivityRecurrencePattern.Monthly:
                        if (s1Activity.Schedule_MonthlyConfig.XDays != 0)
                        {
                            activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDate;
                            activity.runDays = s1Activity.Schedule_MonthlyConfig.XDays;
                            activity.runPattern = s1Activity.Schedule_MonthlyConfig.EveryXMonthes;
                        }
                        else//TODO
                        {
                            activity.runFrequency = exActivityFrequency.exActivityFrequency_MonthlyDOW;
                            activity.runPattern = s1Activity.Schedule_MonthlyConfig.OfXMonthes;//TODO
                            activity.runDays = s1Activity.Schedule_MonthlyConfig.WeekDay;
                        }
                        break;
                }
                activity.endDate = s1Activity.Schedule_EndBy;
                #endregion

                //name
                activity.name = s1Activity.Name;
                //workers group
                if (String.IsNullOrEmpty(s1Activity.WorkerGroup_Name))
                {
                    //activity.workerGroupID = -1;
                }
                else
                {
                    activity.workerGroupID = S1WorkerGroupHelper.GetByName(s1Activity.WorkerGroup_Name).id;
                }
                //logging
                if (s1Activity.EnableDetailedLogging)
                {
                    activity.optionsMask = (int)exActivityOptions.exActivityOptions_EnableLogging;
                }
                else
                {
                    activity.optionsMask = (int)exActivityOptions.exActivityOptions_Unknown;
                }
                //           activity.name += DateTime.Now.Ticks.ToString();
                //persistant
                activity.Save();
                return activity.id;
            }
        }

        /// <summary>
        /// Get the activity by its id
        /// </summary>
        /// <param name="id">the id of the activity</param>
        /// <returns></returns>
        static public IS1Object GetById(int id)
        {
            return new S1Activity(S1Context.JDFAPIMgr.GetActivityByID(id));
        }

        /// <summary>
        /// Get the activity by its name
        /// </summary>
        /// <param name="name">the name of the activity</param>
        /// <returns></returns>
        static internal IS1Object GetByName(string name)
        {
            CoExActivityFilter activityFilter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_ActivityFilter);
            CoExVector activities = S1Context.JDFAPIMgr.GetActivities(activityFilter);
            foreach (CoExActivity activity in activities)
            {
                if (name.ToLower() == activity.name.ToLower())
                {
                    return new S1Activity(activity);
                }                
            }
            return null;
        }

        /// <summary>
        /// Get all the activities
        /// </summary>
        /// <returns></returns>
        static public List<IS1Object> GetAll()
        {
            List<IS1Object> activities = new List<IS1Object>();
            CoExActivityFilter activityFilter = S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_ActivityFilter);
            CoExVector activitiesVector = S1Context.JDFAPIMgr.GetActivities(activityFilter);
            foreach (CoExActivity activity in activitiesVector)
            {
                activities.Add(new S1Activity(activity));
            }
            return activities;
        }

        /// <summary>
        /// Check whether the activity with the name exists or not
        /// </summary>
        /// <param name="name">the name of the activity want to check</param>
        /// <returns></returns>
        static public bool IsExistsByName(string name)
        {
            IS1Object activity = S1ActivityHelper.GetByName(name);
            if (null == activity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
