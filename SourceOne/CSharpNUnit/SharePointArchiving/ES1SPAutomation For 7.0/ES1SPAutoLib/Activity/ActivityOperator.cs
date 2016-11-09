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


namespace ES1.ES1SPAutoLib
{
    public class ActivityOperator
    {
        private static ArchiveActivity ConfigActivity(XmlNode config)
        {
            ArchiveActivity activity = new ArchiveActivity();
            XmlNode datasources = config.SelectSingleNode("DataSource");
            if(datasources == null)
                throw new Exception("No datasources for the activity.");
            
            //datasources
            AddDatasources(datasources, activity);
            
            //VersionFilter
            XmlElement nodeVersion = (XmlElement)config.SelectSingleNode("VersionFilter");
            SetVersionOption(nodeVersion, activity);

            //DateFilter
            XmlElement nodeDateFilter = (XmlElement)config.SelectSingleNode("DateFilter");
            SetDateFilter(nodeDateFilter, activity);

            //AttachmentFilter
            XmlElement nodeAttachmentFilter = (XmlElement)config.SelectSingleNode("AttachmentFilter");
            SetAttachmentFilter(nodeAttachmentFilter, activity);

            //SizeFilter
            XmlElement nodeSizeFilter = (XmlElement)config.SelectSingleNode("SizeFilter");
            SetSizeFilter(nodeSizeFilter, activity);

            //action
            XmlElement nodeAction = (XmlElement)config.SelectSingleNode("Action");
            SetAction(nodeAction, activity);

            activity.LastSavedUtc = DateTime.UtcNow;
            return activity;
        }

        private static void SetAction(XmlElement nodeAction, ArchiveActivity activity)
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

        private static void SetSizeFilter(XmlElement nodeSizeFilter, ArchiveActivity activity)
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
                if (up < down)
                    throw new Exception("SizeFilter_Up should be large than SizeFilter_Down.");
                activity.ItemSizeFilter = new ItemSizeFilter(down, up);
            }
        }

        private static void SetAttachmentFilter(XmlElement nodeAttachmentFilter, ArchiveActivity activity)
        {
            if (nodeAttachmentFilter != null)
            {
                String sAttachFilter = nodeAttachmentFilter.GetAttribute("Types");
                if(String.IsNullOrEmpty(sAttachFilter))
                    return;
                String[] types = sAttachFilter.Trim().Split(new char[]{'`'});
                foreach (String type in types)
                {
                    activity.ExcludedExtensions.Add(type);
                }
            }
        }

        private static void SetVersionOption(XmlElement nodeVersion, ArchiveActivity activity)
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

        private static void SetDateFilter(XmlElement nodeDateFilter, ArchiveActivity activity)
        {
            DateTimeFilter filter = null;
            if (nodeDateFilter != null)
            {
                String sDateFilterType = nodeDateFilter.GetAttribute("Type");
                if (String.IsNullOrEmpty(sDateFilterType))
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
            }
            String baseUpon = nodeDateFilter.GetAttribute("BaseUpon").Trim().ToLower(); ;
            if (baseUpon == "" || baseUpon.Equals(ConstantItems.Activity_BaseUpon_Created))
                activity.CreatedFilter = filter;
            else if (baseUpon.Equals(ConstantItems.Activity_BaseUpon_Modified))
                activity.ModifiedFilter = filter;
            else
                throw new Exception("Not supported BaseUpon: " + baseUpon);
        }

        private static int ConvertToAgedDays(String value)
        {
            String[] values = value.Split(new char[]{'_'});
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
            if(ivalue < 0)
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

        private static DateTimeFilterType ConvertToDateTimeFilterType(String type, DateFilterMethod method)
        {
            type = type.Trim().ToLower();
            switch(method)
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

        private static void AddDatasources(XmlNode datasource, ArchiveActivity activity)
        {
            ObservableCollection<SharePointPath> paths = new ObservableCollection<SharePointPath>();
            //XmlDocument doc = new XmlDocument();
            //doc.Load(spath);
            //XmlNode activity = doc.SelectSingleNode("/Activity");
            XmlNode farmUrlNode = datasource.SelectSingleNode("FarmUrl");
            String farmUrl = farmUrlNode.InnerText.Trim();
            if (farmUrl.EndsWith("/"))
                farmUrl = farmUrl.Substring(0, farmUrl.Length - 1);
            Uri farmUri = null;
            Uri.TryCreate(farmUrl, UriKind.Absolute, out farmUri);
            IArchiveServiceClient client = IArchiveServiceClient.GetInstance(IArchiveServiceClient.GetSharePointServicesUrl(farmUri),
                                                  CredentialCache.DefaultNetworkCredentials);
            FarmTopography farmTopography = new FarmTopography(client, farmUri);
            farmTopography.Initialize();
            SelectedFarm selectFarm = new SelectedFarm(farmUri, farmTopography.SPFarmTransfer.Id);
            SPFarmTransfer farm = farmTopography.SPFarmTransfer;
            IEnumerable<SPWebApplicationTransfer> webApps = client.GetWebApplications(farm.Path.FarmUrl);

            XmlNodeList nwebApps = datasource.SelectNodes("WebApp");
            foreach (XmlElement nwebApp in nwebApps)
            {
                String sPort = nwebApp.GetAttribute("Port");
                if(String.IsNullOrEmpty(sPort))
                    sPort = "80";
                SPWebApplicationTransfer webApp = (SPWebApplicationTransfer)FindSPTransferObject((IEnumerable<SPTransferObject>)webApps, sPort.Trim());
                if (webApp == null)
                    throw new Exception("No such WebApp: " + nwebApp.GetAttribute("Port"));
                if (nwebApp.GetAttribute("SelectAll").Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                    paths.Add(webApp.Path);
                else
                {
                    IEnumerable<SPSiteTransfer> siteConnections = client.GetSiteCollections(webApp.Path);
                    XmlNodeList nsiteCollections = nwebApp.SelectNodes("SiteCollection");
                    foreach (XmlElement nsiteConnection in nsiteCollections)
                    {
                        SPSiteTransfer siteConnection = (SPSiteTransfer)FindSPTransferObject((IEnumerable<SPTransferObject>)siteConnections, nsiteConnection.GetAttribute("Name"));
                        if (siteConnection == null)
                            throw new Exception("No such SiteConnection: " + nsiteConnection.GetAttribute("Name"));
                        if (nsiteConnection.GetAttribute("SelectAll").Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                            paths.Add(siteConnection.Path);
                        else
                        {
                            IEnumerable<SPWebTransfer> sites = client.GetSites(siteConnection.Path);
                            XmlNodeList nodeSites = nsiteConnection.SelectNodes("Site");
                            foreach (XmlElement nodeSite in nodeSites)
                            {
                                SPWebTransfer site = (SPWebTransfer)FindSPTransferObject((IEnumerable<SPTransferObject>)sites, nodeSite.GetAttribute("Name"));
                                if (site == null)
                                    throw new Exception("No such Site: " + nodeSite.GetAttribute("Name"));
                                CollectPathsInSite(client, site, nodeSite, paths);
                            }
                        }
                    }
                }
            }
            selectFarm.SelectedPaths = paths;
            activity.SelectedFarms.Add(selectFarm);
        }

        private static void CollectPathsInSite(IArchiveServiceClient client, SPWebTransfer site,
            XmlElement nodeSite, ObservableCollection<SharePointPath> paths)
        {
            //collect lists
            IEnumerable<SPListTransfer> lists = client.GetLists(site.Path);
            XmlNodeList nodeLists = nodeSite.SelectNodes("List");
            foreach (XmlElement nodeList in nodeLists)
            {
                SPListTransfer list = (SPListTransfer)FindSPTransferObject((IEnumerable<SPTransferObject>)lists, nodeList.GetAttribute("Name"));
                if (list == null)
                    throw new Exception("No such List: " + nodeList.GetAttribute("Name"));
                paths.Add(list.Path);
            }

            //subsites
            IEnumerable<SPWebTransfer> subSites = client.GetSites(site.Path);
            XmlNodeList nodeSubSites = nodeSite.SelectNodes("Site");
            foreach (XmlElement nodeSubSite in nodeSubSites)
            {
                SPWebTransfer subSite = (SPWebTransfer)FindSPTransferObject((IEnumerable<SPTransferObject>)subSites, nodeSubSite.GetAttribute("Name"));
                if (subSite == null)
                    throw new Exception("No such SubSite: " + nodeSubSite.GetAttribute("Name"));
                if (nodeSubSite.GetAttribute("SelectAll").Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                    paths.Add(subSite.Path);
                else
                    CollectPathsInSite(client, subSite, nodeSubSite, paths);
            }

        }

        private static SPTransferObject FindSPTransferObject(IEnumerable<SPTransferObject> objs, String name)
        {
            if(objs == null)
                return null;
            if (String.IsNullOrEmpty(name))
                return null;
            IEnumerator<SPTransferObject> tenum = objs.GetEnumerator();
            if (!tenum.MoveNext())
                return null;
            SharePointLevel level = tenum.Current.Path.Level;
            switch (level)
            {
                case SharePointLevel.WebApplication:
                    foreach (SPTransferObject obj in objs)
                    {
                        SPWebApplicationTransfer wa = (SPWebApplicationTransfer)obj;
                        String url = wa.Uri.AbsoluteUri;
                        url = url.Substring(7);
                        int pos = url.IndexOf(':');
                        if(pos < 0)
                        {
                            if(name.Equals("80", StringComparison.OrdinalIgnoreCase))
                                return obj;
                        }
                        else
                        {
                            url = url.Substring(pos + 1);
                            if(url.EndsWith("/"))
                                url = url.Substring(0, url.Length - 1);
                            if(url.Equals(name, StringComparison.OrdinalIgnoreCase))
                                return obj;
                        }
                        
                    }
                    break;
                case SharePointLevel.SiteCollection:
                    foreach (SPTransferObject obj in objs)
                    {
                        SPSiteTransfer site = (SPSiteTransfer)obj;
                        String url = site.Url.AbsolutePath;
                        if (!url.Equals("/") && url.EndsWith("/"))
                            url = url.Substring(0, url.Length - 1);
                        if (url.Equals(name, StringComparison.OrdinalIgnoreCase))
                            return obj;
                    }
                    break;
                case SharePointLevel.Site:
                case SharePointLevel.Subsite:
                    foreach (SPTransferObject obj in objs)
                    {
                        SPWebTransfer web = (SPWebTransfer)obj;
                        String url = web.Name.Trim();
                        if (url.Equals(name, StringComparison.OrdinalIgnoreCase))
                            return obj;
                    }
                    break;
                case SharePointLevel.List:
                    foreach (SPTransferObject obj in objs)
                    {
                        SPListTransfer list = (SPListTransfer)obj;
                        String url = list.Title.Trim();
                        if (url.Equals(name, StringComparison.OrdinalIgnoreCase))
                            return obj;
                    }
                    break;
            }
            return null;
        }

        private static void SetActivitySchedule(XmlElement nodeSchedule, IExActivity activity)
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
                    if(pattern == null)
                        throw new Exception("No Schedule_Pattern information.");
                    String sInterval = pattern.GetAttribute("Interval");
                    int interv = 0;
                    if(!Int32.TryParse(sInterval, out interv) || interv < 1)
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
            activity.duration = duration;
            activity.runFrequency = runFrequency;
            activity.runPattern = runPattern;
            activity.runDays = runDays;
        }

        

        public static void CreateActivity(String activityXmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(activityXmlFile);//"Activity-1.xml"
            XmlElement nodeActivity = (XmlElement)doc.SelectSingleNode("/Activity");
            if(nodeActivity == null)
                throw new Exception("No activity information in Xml: " + activityXmlFile);

            XmlNode nodeConfig = nodeActivity.SelectSingleNode("Config");
            ArchiveActivity activity = ConfigActivity(nodeConfig);
            String xmlConfig = SharePointUtil.ToXml<ArchiveActivity>(activity);

            IExActivity exActivity = (IExActivity)SourceOneContext.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
            exActivity.xConfig = xmlConfig;

            //Set policy
            String sPolicy = nodeActivity.GetAttribute("Policy");
            if (String.IsNullOrEmpty(sPolicy))
                exActivity.policyID = PolicyManager.CreatePolicy("AutoPolicy").id;
            else
                exActivity.policyID = PolicyManager.CreatePolicy(sPolicy).id;
            exActivity.taskTypeID = 286660062;

            //Set activity schedule
            XmlElement nodeSchedule = (XmlElement)nodeActivity.SelectSingleNode("Schedule");
            SetActivitySchedule(nodeSchedule, exActivity);

            String sActivityName = nodeActivity.GetAttribute("Name");
            if(String.IsNullOrEmpty(sActivityName))
                exActivity.name = "SPAuto" + DateTime.UtcNow.Ticks;
            else
                exActivity.name = sActivityName;

            String sLogging = nodeActivity.GetAttribute("Logging");
            if(!sLogging.Trim().ToLower().Equals("false"))
                exActivity.optionsMask = exActivity.optionsMask | (int)exActivityOptions.exActivityOptions_EnableLogging;
            exActivity.state = exActivityState.exActivityState_Active;

            string error = SharePointUtil.GetFirstError((IExVector)exActivity.Validate());
            if (error != null)
            {
                throw new ApplicationException("Activity is not valid:" + error);
            }

            //Persist activity
            exActivity.Save();
        }

        public static void TestCreateActivity()
        {
            //DateTime date1;
            //bool res = DateTime.TryParse("2010-11-15 16:06:40", out date1);
            //System.Console.WriteLine(date1);
            
            CreateActivity("Activity-1.xml");

            /*bool res = MappedFolderOperator.IsFolderExist("Good");
            res = MappedFolderOperator.IsFolderExist("MDAF1");
            res = MappedFolderOperator.IsFolderExist("mdaf1");
            System.Console.WriteLine("Good job.");*/
        }
    }
}
