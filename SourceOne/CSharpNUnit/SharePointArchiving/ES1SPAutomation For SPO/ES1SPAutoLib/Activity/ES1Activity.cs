using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using EMC.EX.SharePoint.ServicesInterface;
using EMC.EX.SharePoint.ServicesInterface.Configuration;
using EMC.EX.SharePoint.ServicesInterface.TransferObjects;
using System.Xml;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExSTLContainers;
using EMC.EX.SharePoint.ArchivingClient;
using EMC.EX.ExJBSharePointArchiveUI;
using System.IO;
using System.Diagnostics;
using ES1.ES1SPAutoLib;
using TaskScheduler;

namespace ES1SPAutoLib
{
    public class ES1Activity
    {
        protected string activityConfig;
        protected XmlElement nodeActivity;
        public int initalItemCount = 0;
        public int finalItemCount = 0;
        public int archivedItemCount = 0;
        protected int items = 0;
        protected IExActivity exActivity = null;
        protected String[] initTaskNames;

        public ES1Activity(string activityXmlFile)
        {
            activityConfig = activityXmlFile;
            XmlDocument doc = new XmlDocument();
            doc.Load(activityConfig);//"SPOActivityTest1.xml"
            nodeActivity = (XmlElement)doc.SelectSingleNode("/Activity");
            if (nodeActivity == null)
                throw new Exception("No activity information in Xml: " + activityConfig);
            ScheduledTasks st = new ScheduledTasks();
            initTaskNames = st.GetTaskNames();
        }



        protected static void SetAction(XmlElement nodeAction, ArchiveActivity activity)
        {
            if (nodeAction == null)
            {
                throw new Exception("No action parameters.");
            }
            else
            {
                String mf = nodeAction.GetAttribute("MappedFolder");
                if (String.IsNullOrEmpty(mf))
                    throw new Exception("No MappedFolder information.");
                if (!MappedFolderOperator.IsFolderExist(mf))
                    throw new Exception("MappedFolder does NOT exist.");
                activity.ArchiveFolder = mf;

                String type = nodeAction.GetAttribute("Type");
                if (String.IsNullOrEmpty(type) || type.Trim().ToLower().Equals(ConstantItems.Activity_Action_Copy))
                    activity.Action = ActivityAction.Copy;
                else if (type.Trim().ToLower().Equals(ConstantItems.Activity_Action_Move))
                    activity.Action = ActivityAction.Move;
                else
                    throw new Exception("Not supported action type: " + type);

                String sync = nodeAction.GetAttribute("SyncSecurity");
                if (!String.IsNullOrEmpty(sync) && sync.Trim().ToLower().Equals("true"))
                    activity.SynchronizeSecurity = true;
                else
                    activity.SynchronizeSecurity = false;

            }
        }

        protected static void SetSizeFilter(XmlElement nodeSizeFilter, ArchiveActivity activity)
        {
            if (nodeSizeFilter == null)
            {
                activity.ItemSizeFilter = new ItemSizeFilter(-1, -1);
            }
            else
            {
                int up = -1, down = -1;
                String sUp = nodeSizeFilter.GetAttribute("Up");
                if (!String.IsNullOrEmpty(sUp))
                {
                    try
                    {
                        up = Int32.Parse(sUp);
                        if (up < 0)
                            up = -1;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid value of SizeFilter_Up.", e);
                    }
                }
                String sDown = nodeSizeFilter.GetAttribute("Down");
                if (!String.IsNullOrEmpty(sDown))
                {
                    try
                    {
                        down = Int32.Parse(sDown);
                        if (down < 0)
                            down = -1;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid value of SizeFilter_Down: " + sDown, e);
                    }
                }
                if (up < down && up !=-1)
                    throw new Exception("SizeFilter_Up should be large than SizeFilter_Down.");
                activity.ItemSizeFilter = new ItemSizeFilter(down, up);
            }
        }

        protected static void SetAttachmentFilter(XmlElement nodeAttachmentFilter, ArchiveActivity activity)
        {
            if (nodeAttachmentFilter != null)
            {
                String sAttachFilter = nodeAttachmentFilter.GetAttribute("Types");
                if (String.IsNullOrEmpty(sAttachFilter))
                    return;
                String[] types = sAttachFilter.Trim().Split(new char[] { '`' });
                foreach (String type in types)
                {
                    activity.ExcludedExtensions.Add(type);
                }
            }
        }

        protected static void SetVersionOption(XmlElement nodeVersion, ArchiveActivity activity)
        {
            if (nodeVersion == null)
            {
                activity.SelectedVersions = VersionOption.All;
            }
            else
            {
                if (nodeVersion.GetAttribute("Type").Trim().Equals(ConstantItems.Activity_Version_Latest, StringComparison.OrdinalIgnoreCase))
                    activity.SelectedVersions = VersionOption.Latest;
                else
                    activity.SelectedVersions = VersionOption.All;
            }
        }

        protected static void SetDateFilter(XmlElement nodeDateFilter, ArchiveActivity activity)
        {
            DateTimeFilter filter = null;
            if (nodeDateFilter != null)
            {
                String sDateFilterType = nodeDateFilter.GetAttribute("Type");
                if (!String.IsNullOrEmpty(sDateFilterType))
                {
                    sDateFilterType = sDateFilterType.Trim().ToLower();
                    if (sDateFilterType.Equals(ConstantItems.Activity_Date_Dated))
                    {
                        filter = new DateTimeFilter();
                        filter.TypeOfComparison = ConvertToDateTimeFilterType(nodeDateFilter.GetAttribute("Operator"), DateFilterMethod.Dated);
                        DateTime originalUtc;
                        if (!DateTime.TryParse(nodeDateFilter.GetAttribute("Data1"), out originalUtc))
                            throw new Exception("Incorrect time 'Data1': " + nodeDateFilter.GetAttribute("Data1"));
                        originalUtc = new DateTime(originalUtc.Year, originalUtc.Month, originalUtc.Day);
                        originalUtc = originalUtc.ToUniversalTime();

                        if (filter.TypeOfComparison == DateTimeFilterType.LaterThan)
                        {
                            originalUtc = originalUtc.AddDays(1);
                            originalUtc = originalUtc.AddMilliseconds(-1);
                        }
                        filter.OriginUtc = originalUtc;
                        if (filter.TypeOfComparison == DateTimeFilterType.BetweenTwoDates)
                        {
                            // end of the later date.
                            DateTime laterDateValue;
                            if (!DateTime.TryParse(nodeDateFilter.GetAttribute("Data2"), out laterDateValue))
                                throw new Exception("Incorrect time 'Data2': " + nodeDateFilter.GetAttribute("Data2"));
                            laterDateValue = laterDateValue.ToUniversalTime();
                            laterDateValue = laterDateValue.AddDays(1);
                            laterDateValue = laterDateValue.AddMilliseconds(-1);
                            if (laterDateValue < originalUtc)
                                throw new Exception("Data2 (" + nodeDateFilter.GetAttribute("Data2") + ") should be bigger than Data1 (" + nodeDateFilter.GetAttribute("Data1") + ").");
                            filter.Span = laterDateValue.Subtract(filter.OriginUtc);
                        }
                        else
                        {
                            filter.Span = TimeSpan.Zero;
                        }
                    }
                    else if (sDateFilterType.Equals(ConstantItems.Activity_Date_Aged))
                    {
                        filter = new DateTimeFilter();
                        filter.TypeOfComparison = ConvertToDateTimeFilterType(nodeDateFilter.GetAttribute("Operator"), DateFilterMethod.Aged);
                        int days1 = ConvertToAgedDays(nodeDateFilter.GetAttribute("Data1"));
                        if (filter.TypeOfComparison == DateTimeFilterType.AgedBetween)
                        {
                            int days2 = ConvertToAgedDays(nodeDateFilter.GetAttribute("Data2")) - days1;
                            if (days2 < 0)
                                throw new Exception("Data2 (" + nodeDateFilter.GetAttribute("Data2") + ") should be bigger than Data1 (" + nodeDateFilter.GetAttribute("Data1") + ").");
                            filter.Offset = new TimeSpan(days1, 0, 0, 0);
                            filter.Span = new TimeSpan(days2, 0, 0, 0);
                        }
                        else
                        {
                            filter.Span = new TimeSpan(days1, 0, 0, 0); ;
                            filter.Offset = null;
                        }
                    }
                    else if (!sDateFilterType.Equals(ConstantItems.Activity_Date_All))
                        throw new Exception("Invalid DateFilter_Type: " + sDateFilterType);
                }

                String baseUpon = nodeDateFilter.GetAttribute("BaseUpon").Trim().ToLower();
                if (baseUpon == "" || baseUpon.Equals(ConstantItems.Activity_BaseUpon_Created))
                    activity.CreatedFilter = filter;
                else if (baseUpon.Equals(ConstantItems.Activity_BaseUpon_Modified))
                    activity.ModifiedFilter = filter;
                else
                    throw new Exception("Not supported BaseUpon: " + baseUpon);
            }
            else
            {
                activity.CreatedFilter = filter;
            }
        }

        protected static int ConvertToAgedDays(String value)
        {
            String[] values = value.Split(new char[] { '_' });
            if (values.Length != 2)
                throw new Exception("Invalid Aged days/weeks/months/years: " + value);
            int ivalue = -1;
            try
            {
                ivalue = Int32.Parse(values[0]);
            }
            catch (Exception e)
            {
                throw new Exception("Invalid Aged days/weeks/months/years: " + value, e);
            }
            if (ivalue < 0)
                throw new Exception("Invalid Aged days/weeks/months/years: " + value);
            if (values[1].ToLower().Equals(ConstantItems.Activity_Aged_Days))
                return ivalue;
            if (values[1].ToLower().Equals(ConstantItems.Activity_Aged_Weeks))
                return ivalue * (int)AgeUnit.Weeks;
            if (values[1].ToLower().Equals(ConstantItems.Activity_Aged_Months))
                return ivalue * (int)AgeUnit.Months;
            if (values[1].ToLower().Equals(ConstantItems.Activity_Aged_Years))
                return ivalue * (int)AgeUnit.Years;
            throw new Exception("Invalid Aged days/weeks/months/years: " + value);
        }

        protected static DateTimeFilterType ConvertToDateTimeFilterType(String type, DateFilterMethod method)
        {
            type = type.Trim().ToLower();
            switch (method)
            {
                case DateFilterMethod.Dated:
                    if (type.Equals(ConstantItems.Activity_Dated_After))
                        return DateTimeFilterType.LaterThan;
                    if (type.Equals(ConstantItems.Activity_Dated_Before))
                        return DateTimeFilterType.EarlierThan;
                    if (type.Equals(ConstantItems.Activity_Dated_Between))
                        return DateTimeFilterType.BetweenTwoDates;
                    if (type.Equals(ConstantItems.Activity_Dated_On))
                        return DateTimeFilterType.ExactDate;
                    throw new Exception("Operator value is not supported for DateFilter_Dated: " + type);
                case DateFilterMethod.Aged:
                    if (type.Equals(ConstantItems.Activity_Aged_Newer))
                        return DateTimeFilterType.NewerThan;
                    if (type.Equals(ConstantItems.Activity_Aged_Older))
                        return DateTimeFilterType.OlderThan;
                    if (type.Equals(ConstantItems.Activity_Aged_Between))
                        return DateTimeFilterType.AgedBetween;
                    throw new Exception("Operator value is not supported for DateFilter_Aged: " + type);
            }
            throw new Exception("DateFilterMethod type is not supported: " + method);
        }



        protected static void SetActivitySchedule(XmlElement nodeSchedule, IExActivity activity)
        {
            //default values
            DateTime startTime = DateTime.UtcNow.AddSeconds(2);
            int duration = -1;
            exActivityFrequency runFrequency = exActivityFrequency.exActivityFrequency_JustOnce;
            int runPattern = 1;
            int runDays = 0;

            if (nodeSchedule != null)
            {
                String sStartTime = nodeSchedule.GetAttribute("StartTime");
                if (!String.IsNullOrEmpty(sStartTime) && !sStartTime.Equals(ConstantItems.Activity_Schedule_Now))
                {
                    DateTime dt;
                    if (!DateTime.TryParse(sStartTime, out dt))
                        throw new Exception("Invalid Schedule_StartTime: " + sStartTime);
                    startTime = dt;
                }

                String sDuration = nodeSchedule.GetAttribute("Duration");
                if (!String.IsNullOrEmpty(sDuration))
                {
                    sDuration = sDuration.Trim();
                    int dur = 0;
                    if (!Int32.TryParse(sDuration, out dur))
                        throw new Exception("Invalid Schedule_Duration: " + sDuration);
                    if (dur == -1 || (dur <= 12 && dur >= 1) || dur == 24)
                        duration = dur;
                    else
                        throw new Exception("Invalid Schedule_Duration: " + sDuration);
                }

                String sFrequency = nodeSchedule.GetAttribute("Frequency").Trim().ToLower();
                if (!String.IsNullOrEmpty(sFrequency) && !sFrequency.Equals(ConstantItems.Activity_Schedule_Once))
                {
                    XmlElement pattern = (XmlElement)nodeSchedule.SelectSingleNode("Pattern");
                    if (pattern == null)
                        throw new Exception("No Schedule_Pattern information.");
                    String sInterval = pattern.GetAttribute("Interval");
                    int interv = 0;
                    if (!Int32.TryParse(sInterval, out interv) || interv < 1)
                        throw new Exception("No or invalid Schedule_Interval: " + sInterval);
                    runPattern = interv;

                    if (sFrequency.Equals(ConstantItems.Activity_Schedule_Weekly))
                    {
                        runFrequency = exActivityFrequency.exActivityFrequency_Weekly;
                        String sDays = pattern.GetAttribute("Days");
                        if (String.IsNullOrEmpty(sDays))
                            throw new Exception("No Schedule_Days information for ActivityFrequency_Weekly.");
                        String[] days = sDays.Split(new char[] { '_' });
                        foreach (String day in days)
                        {
                            if (day.Equals(ConstantItems.Activity_Schedule_Monday))
                                runDays |= (int)exWeekdays.exWeekdays_Monday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Tuesday))
                                runDays |= (int)exWeekdays.exWeekdays_Tuesday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Wednesday))
                                runDays |= (int)exWeekdays.exWeekdays_Wednesday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Thursday))
                                runDays |= (int)exWeekdays.exWeekdays_Thursday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Friday))
                                runDays |= (int)exWeekdays.exWeekdays_Friday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Saturday))
                                runDays |= (int)exWeekdays.exWeekdays_Saturday;
                            else if (day.Equals(ConstantItems.Activity_Schedule_Sunday))
                                runDays |= (int)exWeekdays.exWeekdays_Sunday;
                            else
                                throw new Exception("Invalid week days: " + sDays);
                        }
                        if (runDays == 0)
                            throw new Exception("Invalid week days: " + sDays);
                    }
                    else if (sFrequency.Equals(ConstantItems.Activity_Schedule_Daily))
                        runFrequency = exActivityFrequency.exActivityFrequency_Daily;
                    else
                        throw new Exception("Invalid Schedule_Frequency: " + sFrequency);
                }
            }

            activity.startDate = startTime;
            activity.startTime = startTime;
            activity.duration = duration * 60;
            activity.runFrequency = runFrequency;
            activity.runPattern = runPattern;
            activity.runDays = runDays;
        }



        public virtual void Create()
        {
        }

        public void Delete()
        {
            if (exActivity != null)
            {
                exActivity.ApplyAction(exActivityActions.exActivityAction_Delete);
            }
        }

        public void Run(int timeout)
        {
            Run(timeout, true);
        }

        public void Run(int timeout, bool WithJBC)
        {
            XmlNode nodeConfig = nodeActivity.SelectSingleNode("Config");
            XmlNode nodeAction = nodeConfig.SelectSingleNode("Action");
            XmlElement elemAction = (XmlElement)nodeAction;
            string mapFolder = elemAction.GetAttribute("MappedFolder");

            string mcFolderPath = GetMCFolderPath(mapFolder);
            DirectoryInfo dir = new DirectoryInfo(mcFolderPath);
            initalItemCount = GetItemCount(dir);
            if (WithJBC)
            {
                WaitForJBCStarted(240);
                WaitForJBSStopped(timeout);
                finalItemCount = GetItemCount(dir);
                archivedItemCount = finalItemCount - initalItemCount;
            }
            else
            {
                if (WaitForProcessStarted(ProcessName.SPO_JBS_ProcessName, 60))
                {
                    if (WaitForProcessStopped(ProcessName.SPO_JBS_ProcessName, 60))
                    {
                        finalItemCount = GetItemCount(dir);
                        archivedItemCount = finalItemCount - initalItemCount;
                    }
                    else
                    {
                        throw new Exception(ProcessName.SPO_JBS_ProcessName + " can not stop within timeout!");
                    }
                }
                else
                {
                    throw new Exception(ProcessName.SPO_JBS_ProcessName + " can not start within timeout!");
                }
            }
        }

        public Task GetScheduledTask(int timeout)
        {
            ScheduledTasks st = new ScheduledTasks();
            String[] taskNames = st.GetTaskNames();
            int i=0;
            while (i < timeout)
            {    
                if (taskNames.Length > initTaskNames.Length)
                    break;
                System.Threading.Thread.Sleep(1000);
                st = new ScheduledTasks();
                taskNames = st.GetTaskNames();
            }
 
            if (taskNames.Length <= initTaskNames.Length)
                return null;
            foreach (String name in taskNames)
            {
                if (!Contains(initTaskNames, name))
                    return st.OpenTask(name);
            }
            return null;
        }

        /// <summary>
        /// Verify whether a list contains value
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Contains(String[] list, String value)
        {
            if (list.Length == 0)
                return false;
            foreach (String actual in list)
            {
                if (actual == value)
                    return true;
            }
            return false;
        }

        protected void WaitForJBCStarted(int timeout)
        {
            if (WaitForProcessStarted(ProcessName.SPO_JBS_ProcessName, timeout))
            {
                if (WaitForProcessStarted(ProcessName.SPO_JBC_ProcessName, timeout))
                {
                    return;
                }
                else
                {
                    throw new Exception(ProcessName.SPO_JBC_ProcessName + " can not start within timeout!");
                }
            }
            else
            {
                throw new Exception(ProcessName.SPO_JBS_ProcessName + " can not start within timeout!");
            }
        }

        protected void WaitForJBSStopped(int timeout)
        {
            if (WaitForProcessStopped(ProcessName.SPO_JBC_ProcessName, timeout))
            {
                if (WaitForProcessStopped(ProcessName.SPO_JBS_ProcessName, timeout))
                {
                    return;
                }
                else
                {
                    throw new Exception(ProcessName.SPO_JBS_ProcessName + " can not stop within timeout!");
                }
            }
            else
            {
                throw new Exception(ProcessName.SPO_JBC_ProcessName + " can not stop within timeout!");
            }
        }

        protected bool WaitForProcessStarted(String proName, int timeout)
        {
            int i = 0;
            while (i < timeout)
            {
                Process[] proList = Process.GetProcessesByName(proName);
                if (proList.Length > 0)
                    break;
                System.Threading.Thread.Sleep(1000);
                i++;
            }
            if (i == timeout)
                return false;
            return true;
        }

        protected bool WaitForProcessStopped(String proName, int timeout)
        {
            int i = 0;
            while (i < timeout)
            {
                Process[] proList = Process.GetProcessesByName(proName);
                if (proList.Length == 0)
                    break;
                System.Threading.Thread.Sleep(1000);
                i++;
            }
            if (i == timeout)
                return false;
            return true;
        }

        protected void WaitForArchiveFinished(int interval, DirectoryInfo folder)
        {
            int i=0;
            int tempCount1 = GetItemCount(folder);
            int tempCount2 = GetItemCount(folder);
            while (true)
            {
                tempCount1 = tempCount2;
                tempCount2 = GetItemCount(folder);
                if (tempCount2 == tempCount1)
                {
                    while (i <= interval)
                    {
                        tempCount1 = tempCount2;
                        tempCount2 = GetItemCount(folder);
                        if (tempCount2 > tempCount1)
                            break;
                        System.Threading.Thread.Sleep(1000);
                        i++;
                    }
                    if (i > interval)
                        break;
                }
            }
        }

        protected int GetItemCount(DirectoryInfo folder)
        {
            items = 0;
            GetFolderFileCount(folder);
            return items;
        }

        protected void GetFolderFileCount(DirectoryInfo folder)
        {
            try
            {
                FileSystemInfo[] fileinfos = folder.GetFileSystemInfos();
                foreach (FileSystemInfo fileinfo in fileinfos)
                {
                    if (fileinfo is DirectoryInfo)
                    {
                        GetFolderFileCount((DirectoryInfo)fileinfo);
                    }
                    else if (fileinfo.Extension == "")
                    {
                        items++;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        protected static string GetMCFolderPath(string mapFolder)
        {
            string archiveFolder = "";
            string mcFolderPath = "";
            foreach (MappedFolder mf in Configuration.MappedFolders)
            {
                if (mf.Name.Equals(mapFolder))
                {
                    archiveFolder = mf.ArchiveFolder;
                    break;
                }
            }
            if (archiveFolder.Equals(""))
            {
                throw new Exception("Archive folder is " + archiveFolder);
            }
            mcFolderPath = Configuration.MCLocation + "\\Message_Center\\" + archiveFolder;
            if (mcFolderPath.Equals(""))
            {
                throw new Exception("Archive folder location is " + mcFolderPath);
            }
            return mcFolderPath;
        }

        protected static string GetArchiveFolderPath(string mapFolder)
        {
            string archiveFolder = "";
            string archiveFolderPath = "";
            foreach (MappedFolder mf in Configuration.MappedFolders)
            {
                if (mf.Name.Equals(mapFolder))
                {
                    archiveFolder = mf.ArchiveFolder;
                    break;
                }
            }
            if (archiveFolder.Equals(""))
            {
                throw new Exception("Archive folder is " + archiveFolder);
            }
            foreach (ArchiveFolder af in Configuration.ArchiveFolders)
            {
                if (af.Name.Equals(archiveFolder))
                {
                    archiveFolderPath = af.ArchiveLocation + "\\" + af.Name;
                }
            }
            if (archiveFolderPath.Equals(""))
            {
                throw new Exception("Archive folder location is " + archiveFolderPath);
            }
            return archiveFolderPath;
        }

        
    }
}
