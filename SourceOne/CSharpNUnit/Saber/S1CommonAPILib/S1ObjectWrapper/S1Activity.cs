using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

using Saber.Common; 

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExBase;
using EMC.Interop.ExMBTaskAPI;
using EMC.Interop.ExProvider_2;
using EMC.Interop.ExProviderGW;

namespace Saber.S1CommonAPILib
{
    public class S1Activity : IS1Object
    {
        /// <summary>
        /// The default contruction, set the default value for below parameters:
        /// 
        /// </summary>
        public S1Activity()
        {
            this.MessageSizeFilter_IncludeMessageLessThan = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public S1Activity(IExActivity3 activity)
        {
            if (null == activity)
            {
                throw new Exception();
            }
            else
            {
                this.Name = activity.name;
                this.PolicyName = ((CoExPolicy)S1Context.JDFAPIMgr.GetPolicyByID(activity.policyID)).name;
                this.State = (S1ActivityState)activity.state;
                this.ID = activity.id;
            }
        }

        //Zero page, before create the activity, we'll first specify the policy it's built under.
        public string PolicyName { get; set; }
        public int ID { get; internal set; }
        public S1ActivityState State { get; set; }
        public string Description { get; set; }
        //First page, select the activity type
        public S1ActivityType Activity_Type { get; set; }
        //Second page, select the data source type
        public S1DataSourceType DataSource_Type { get; set; }
        //Third page, select data source
        public List<S1DataSource> DataSource { get; set; }
        public int DataSource_MailBoxTypeMask { get; set; }
        //Fourth page, select the items types
        public int Item_S1MailBoxItemTypeMask { get; set; }
        public bool Item_ReprocessItems { get; set; }
        //Fifth page, select folders
        public int Folder_MailFolderTypeMask { get; set; }
        public bool Folder_IncludeSubFolders { get; set; }
        public bool Folder_IncludeReadItems { get; set; }
        public bool Folder_IncludeUnreadItems { get; set; }
        public bool Folder_DeletedRetentionSoftDeleteItems { get; set; }
        //Sixth page, user created folders to include
        public List<string> UserCreatedFolders { get; set; }
        //Seventh page, select the dates for your activity
        public S1ActivityDatesType Dates_Type { get; set; }
        public S1ActivityDatesAgedConfig Dates_AgedConfig { get; set; }
        public S1ActivityDatesDatedConfig Dates_DatedConfig { get; set; }
        public DATEFILTERPROPERTY Dates_BasedUpon { get; set; }
        //Eighth page, attachments filter
        public string AttachmentsFilter_ExtensionsExcluded { get; set; }
        //Ninth page, message types filter
        public string MessageTypesFilter_ExcludeMessageTypes { get; set; }
        public string MessageTypesFilter_IncludeMessageTypes { get; set; }
        //Teenth page, message size filter
        public int MessageSizeFilter_IncludeMessageGreaterThan { get; set; }
        public int MessageSizeFilter_IncludeMessageLessThan { get; set; }
        //Eleventh page, specify activity component extension to use, TODO(Other BCE?)
        //public List<String> BusinessComponents { get; set; }//Here the "Address Rules" will be used by default
        public List<S1AddressFilteringRule> BCE_AddressFilteringRules { get; set; }
        public bool BCE_CopyMessageDonotMatchAnyRuleTo_Enable { get; set; }
        public String BCE_CopyMessageDonotMatchAnyRuleTo_Folder { get; set; }

        // Historical Shortcut options
        public int ShortcutLanguageID { get; set; }
        public int ShortcutIncludeMessageBody { get; set; }        
        public int ShortcutInlineImageGreaterThan { get; set; }

        //Twelvth page, specify the schedule for your activity
        //12-1, activity
        public DateTime Schedule_StartDate { get; set; }
        public DateTime Schedule_StartTime { get; set; }
        public int Schedule_Duration { get; set; }
        //12-2, recurrent pattern
        public S1ActivityRecurrencePattern Schedule_ActivityRecurrencePattern { get; set; }
        public S1ActivityRecurrencePatternDailyConfig Schedule_DailyConfig { get; set; }
        public S1ActivityRecurrencePatternWeeklyConfig Schedule_WeeklyConfig { get; set; }
        public S1ActivityRecurrencePatternMonthlyConfig Schedule_MonthlyConfig { get; set; }
        //12-3, range of recurrence
        public DateTime Schedule_EndBy { get; set; }
        //Thirteenth page, select worker group
        public string WorkerGroup_Name { get; set; }
        //Fourteenth page, specify the name for your activity, and the logging option
        public string Name { get; set; }
        public bool EnableDetailedLogging { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            return true;
        }

        public bool DeserializeFromXMLFile(string filePath, int activityId)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Can not find the file specified.");
            }
            XDocument document = XDocument.Load(filePath);

            try
            {
                XElement element = document.Root.Elements("activity")
                    .Single(x => (int)x.Attribute("id") == activityId);
                return DeserializeFromXElement(element);
            }        

            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }

        public bool DeserializeFromXElement(System.Xml.Linq.XElement element)
        {
            String policyname = XMLHelper.GetElementValue(element, "policyname");
            if (!String.IsNullOrEmpty(policyname))
            {
                this.PolicyName = policyname;
            }
            else
            {
                throw new Exception("policy name is required to create an activity.");
            }
            String state = XMLHelper.GetElementValue(element, "state");
            this.State = S1ActivityState.Active;
            String description = XMLHelper.GetElementValue(element, "description");
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }
            String type = XMLHelper.GetElementValue(element, "type");
            switch (type)
            {
                case "Archive_Historical":
                    this.Activity_Type = S1ActivityType.Archive_Historical;
                    break;
                case "Archive_Shortcut":
                    this.Activity_Type = S1ActivityType.Shortcut_Historical;
                    break;
                case "Shortcut_Restore":
                    this.Activity_Type = S1ActivityType.RestoreShortcuts_HistoricalAndUserDirectedArchive;
                    break;
                default:
                    throw new Exception("This type of activity is not supported by Saber yet. type:" + type);
            }
            String datasourcetype = XMLHelper.GetElementValue(element, "datasourcetype");
            if (!String.IsNullOrEmpty(datasourcetype))
            {
                switch (datasourcetype)
                {
                    case "MicrosoftExchange":
                        this.DataSource_Type = S1DataSourceType.MicrosoftExchange;
                        break;
                    default:
                        throw new Exception("This data source type is not supported by saber yet.");
                }
            }
            else
            {
                throw new Exception("The data source type must be specified.");
            }
            XElement datasources = XMLHelper.GetElement(element, "datasources");
            List<S1DataSource> sourcesList = new List<S1DataSource>();
            if (null != datasources)
            {
                foreach (XElement source in datasources.Elements())
                {
                    S1DataSource s = new S1DataSource();
                    s.DeserializeFromXElement(source);
                    sourcesList.Add(s);
                }
            }
            else
            {
                throw new Exception("The data source must be specified.");
            }
            if (sourcesList.Count > 0)
            {
                this.DataSource = sourcesList;
            }
            else
            {
                throw new Exception("The data source must be specified.");
            }
            String datasourcemailboxtype = XMLHelper.GetElementValue(element, "datasourcemailboxtype");
            if (!String.IsNullOrEmpty(datasourcemailboxtype))
            {
                int mailboxType = 0;
                String[] types = datasourcemailboxtype.Split(',');
                foreach (String t in types)
                {
                    if (t.Equals("Primary"))
                    {
                        mailboxType |= (int)S1MailBoxType.Primary;
                    }
                    else if (t.Equals("PersonalArchive"))
                    {
                        mailboxType |= (int)S1MailBoxType.PersonalArchive;
                    }
                    else
                    {
                        throw new Exception("The mail box type is not valid.");
                    }
                }
                this.DataSource_MailBoxTypeMask = mailboxType;
            }
            String mailboxitemtypes = XMLHelper.GetElementValue(element, "mailboxitemtypes");
            if (!String.IsNullOrEmpty(mailboxitemtypes))
            {

                if (mailboxitemtypes.Equals("IncludeAll"))
                {
                    this.Item_S1MailBoxItemTypeMask = (int)S1MailBoxItemType.IncludeAll;
                }
                else
                {
                    //TODO
                    throw new NotImplementedException();
                }
            }
            String reprocessitems = XMLHelper.GetElementValue(element, "reprocessitems");
            if (!String.IsNullOrEmpty(reprocessitems))
            {
                this.Item_ReprocessItems = bool.Parse(reprocessitems);
            }
            String mailfoldertypes = XMLHelper.GetElementValue(element, "mailfoldertypes");
            if (!String.IsNullOrEmpty(mailfoldertypes))
            {
                int folderTypes = 0;
                foreach (String t in mailfoldertypes.Split(',').ToList<String>())
                {
                    if (t.Equals("IncludeAll"))
                    {
                        folderTypes = -1;
                        break;
                    }
                    switch (t)
                    {
                        case "Calendar":                            
                            folderTypes |= (int)S1MailFolderType.Calendar;
                            break;
                        case "Contacts":
                            folderTypes |= (int)S1MailFolderType.Contacts;
                            break;
                        case "Inbox":
                            folderTypes |= (int)S1MailFolderType.Inbox;
                            break;
                        case "DeletedItems":
                            folderTypes |= (int)S1MailFolderType.DeletedItems;
                            break;
                        case "Notes":
                            folderTypes |= (int)S1MailFolderType.Notes;
                            break;
                        case "Outbox":
                            folderTypes |= (int)S1MailFolderType.Outbox;
                            break;
                        case "SentItems":
                            folderTypes |= (int)S1MailFolderType.SentItems;
                            break;
                        case "Tasks":
                            folderTypes |= (int)S1MailFolderType.Tasks;
                            break;
                        case "JunkEmail":
                            folderTypes |= (int)S1MailFolderType.JunkEmail;
                            break;
                        case "UserDefined":
                            folderTypes |= (int)S1MailFolderType.UserDefined;
                            break;
                        default: 
                            throw new Exception("The folder type is not valid. " + t);
                    }
                }
                this.Folder_MailFolderTypeMask = folderTypes;
            }
            String inlcudesubfolders = XMLHelper.GetElementValue(element, "inlcudesubfolders");
            if (!String.IsNullOrEmpty(inlcudesubfolders))
            {
                this.Folder_IncludeSubFolders = bool.Parse(inlcudesubfolders);
            }
            String includereaditems = XMLHelper.GetElementValue(element, "includereaditems");
            if (!String.IsNullOrEmpty(includereaditems))
            {
                this.Folder_IncludeReadItems = bool.Parse(includereaditems);
            }
            String includeunreaditems = XMLHelper.GetElementValue(element, "includeunreaditems");
            if (!String.IsNullOrEmpty(includeunreaditems))
            {
                this.Folder_IncludeUnreadItems = bool.Parse(includeunreaditems);
            }
            String includesoftdeleteditems = XMLHelper.GetElementValue(element, "includesoftdeleteditems");
            if (!String.IsNullOrEmpty(includesoftdeleteditems))
            {
                this.Folder_DeletedRetentionSoftDeleteItems = bool.Parse(includesoftdeleteditems);
            }

            String usercreatedfolders = XMLHelper.GetElementValue(element, "usercreatedfolders");
            if (!String.IsNullOrEmpty(usercreatedfolders))
            {
                this.UserCreatedFolders = usercreatedfolders.Split(',').ToList<String>();
            }
            else
            {
                this.UserCreatedFolders = new List<string>();
            }

            String datestype = XMLHelper.GetElementValue(element, "datestype");
            if (!String.IsNullOrEmpty(datestype))
            {
                if (datestype.Equals("UseAll"))
                {
                    this.Dates_Type = S1ActivityDatesType.UseAll;
                }
                else
                {
                    throw new Exception("Not supported by Saber yet.");
                }
            }
            else
            {
                //throw new Exception("Please specify the date type");
            }
            String basedupon = XMLHelper.GetElementValue(element, "basedupon");
            if (!String.IsNullOrEmpty(basedupon))
            {
                if (basedupon.Equals("UseReceivedDate"))
                {
                    this.Dates_BasedUpon = DATEFILTERPROPERTY.Date_UseReceivedDate;
                }
                else if (basedupon.Equals("UseLastModifiedDate"))
                {
                    this.Dates_BasedUpon = DATEFILTERPROPERTY.Date_UseLastModifiedDate;
                }
                else if (basedupon.Equals("UseArchivedDate"))
                {
                    this.Dates_BasedUpon = DATEFILTERPROPERTY.Date_UseArchivedDate;
                }
                else
                {
                    throw new Exception("The based upon option is not valid.");
                }
            }
            String attachmentsextensionsexcluded = XMLHelper.GetElementValue(element, "attachmentsextensionsexcluded");
            if (!String.IsNullOrEmpty(attachmentsextensionsexcluded))
            {
                this.AttachmentsFilter_ExtensionsExcluded = attachmentsextensionsexcluded;
            }
            String excludemessagetypes = XMLHelper.GetElementValue(element, "excludemessagetypes");
            if (!String.IsNullOrEmpty(excludemessagetypes))
            {
                this.MessageTypesFilter_ExcludeMessageTypes = excludemessagetypes;
            }
            String includemessagetypes = XMLHelper.GetElementValue(element, "includemessagetypes");
            if (!String.IsNullOrEmpty(includemessagetypes))
            {
                this.MessageTypesFilter_IncludeMessageTypes = includemessagetypes;
            }
            String includemessagegreaterthan = XMLHelper.GetElementValue(element, "includemessagegreaterthan");
            if (!String.IsNullOrEmpty(includemessagegreaterthan))
            {
                this.MessageSizeFilter_IncludeMessageGreaterThan = int.Parse(includemessagegreaterthan);
            }
            String includemessagelessthan = XMLHelper.GetElementValue(element, "includemessagelessthan");
            if (!String.IsNullOrEmpty(includemessagelessthan))
            {
                this.MessageSizeFilter_IncludeMessageLessThan = int.Parse(includemessagelessthan);
            }
            String enablecopymessagedonotmatchanyruleto = XMLHelper.GetElementValue(element, "enablecopymessagedonotmatchanyruleto");
            if (!String.IsNullOrEmpty(enablecopymessagedonotmatchanyruleto))
            {
                this.BCE_CopyMessageDonotMatchAnyRuleTo_Enable = bool.Parse(enablecopymessagedonotmatchanyruleto);
            }
            String ablecopymessagedonotmatchanyruletofolder = XMLHelper.GetElementValue(element, "ablecopymessagedonotmatchanyruletofolder");
            if (!String.IsNullOrEmpty(ablecopymessagedonotmatchanyruletofolder))
            {
                this.BCE_CopyMessageDonotMatchAnyRuleTo_Folder = ablecopymessagedonotmatchanyruletofolder;
            }
            XElement addressfilteringrules = XMLHelper.GetElement(element, "addressfilteringrules");
            List<S1AddressFilteringRule> rulesList = new List<S1AddressFilteringRule>();
            if (null != addressfilteringrules)
            {
                foreach (XElement rule in addressfilteringrules.Elements())
                {
                    S1AddressFilteringRule r = new S1AddressFilteringRule();
                    r.DeserializeFromXElement(rule);
                    rulesList.Add(r);
                }
            }
            if (rulesList.Count > 0)
            {
                this.BCE_AddressFilteringRules = rulesList;
            }
            String startdate = XMLHelper.GetElementValue(element, "startdate");
            if (!String.IsNullOrEmpty(startdate))
            {

                this.Schedule_StartDate = ParserDateTime(startdate);

            }
            String starttime = XMLHelper.GetElementValue(element, "starttime");
            if (!String.IsNullOrEmpty(starttime))
            {

                this.Schedule_StartTime = ParserDateTime(starttime);

            }
            String duration = XMLHelper.GetElementValue(element, "duration");
            if (!String.IsNullOrEmpty(duration))
            {
                this.Schedule_Duration = int.Parse(duration) * 60;//change hour to minites
            }
            String recurrencepattern = XMLHelper.GetElementValue(element, "recurrencepattern");
            if (!String.IsNullOrEmpty(recurrencepattern))
            {
                if (recurrencepattern.Equals("Once"))
                {
                    this.Schedule_ActivityRecurrencePattern = S1ActivityRecurrencePattern.Once;
                }
                else
                {
                    throw new Exception("Not supported by Saber yet.");
                }
            }

            String endby = XMLHelper.GetElementValue(element, "endby");
            if (!String.IsNullOrEmpty(endby))
            {
                this.Schedule_EndBy = ParserDateTime(endby);
            }

            String workergroupname = XMLHelper.GetElementValue(element, "workergroupname");
            if (!String.IsNullOrEmpty(workergroupname))
            {
                this.WorkerGroup_Name = workergroupname;
            }

            String name = XMLHelper.GetElementValue(element, "name");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }

            String enabledetailedlogging = XMLHelper.GetElementValue(element, "enabledetailedlogging");
            if (!String.IsNullOrEmpty(enabledetailedlogging))
            {
                this.EnableDetailedLogging = bool.Parse(enabledetailedlogging);
            }                   
                        
            String shortcutlanguage = XMLHelper.GetElementValue(element, "shortcutlanguage");
            if (!String.IsNullOrEmpty(shortcutlanguage))
            {
                this.ShortcutLanguageID = int.Parse(shortcutlanguage);
            }
            String includemessagebody = XMLHelper.GetElementValue(element, "shortcutincludemessagebody");
            if (!String.IsNullOrEmpty(includemessagebody))
            {
                this.ShortcutIncludeMessageBody = int.Parse(includemessagebody);
            }            
            String shortcutinlineimagegreatethan = XMLHelper.GetElementValue(element, "shortcutinlineimagegreaterthan");
            if (!String.IsNullOrEmpty(shortcutinlineimagegreatethan))
            {
                this.ShortcutInlineImageGreaterThan = int.Parse(shortcutinlineimagegreatethan);
            }

            

            return true;
        }
        private DateTime ParserDateTime(String date)
        {
            if (date.Contains("+"))
            {
                String[] temp = date.Split('+');
                if (temp.Count() == 2)
                {
                    if (temp[0].Equals("NOW"))
                    {
                        return DateTime.UtcNow.AddDays(int.Parse(temp[1]));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (date.Contains("-"))
            {
                String[] temp = date.Split('-');
                if (temp.Count() == 2)
                {
                    if (temp[0].Equals("NOW"))
                    {
                        return DateTime.UtcNow.AddDays( - int.Parse(temp[1]) );
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (date.Equals("NOW"))
            {
                return DateTime.UtcNow;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
