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
using ES1SPAutoLib;


namespace ES1.ES1SPAutoLib
{
    public class SPActivity : ES1Activity
    {

        public SPActivity(string activityXmlFile)
            : base(activityXmlFile)
        {
        }

        private static ArchiveActivity ConfigActivity(XmlNode config)
        {
            
            ArchiveActivity activity = new ArchiveActivity();
            XmlNode datasources = config.SelectSingleNode("DataSource");
            if (datasources == null)
                throw new Exception("No datasources for the activity.");

            //Datasources
            AddDatasources(datasources, activity);
            System.Console.WriteLine("Add DataSource successfully.");

            //Content Types
            AddContentTypes(datasources, activity);
            System.Console.WriteLine("Add ContentType successfully.");

            //VersionFilter
            XmlElement nodeVersion = (XmlElement)config.SelectSingleNode("VersionFilter");
            SetVersionOption(nodeVersion, activity);
            System.Console.WriteLine("Set version filter successfully.");

            //DateFilter
            XmlElement nodeDateFilter = (XmlElement)config.SelectSingleNode("DateFilter");
            SetDateFilter(nodeDateFilter, activity);
            System.Console.WriteLine("Set Date filter successfully.");

            //AttachmentFilter
            XmlElement nodeAttachmentFilter = (XmlElement)config.SelectSingleNode("AttachmentFilter");
            SetAttachmentFilter(nodeAttachmentFilter, activity);
            System.Console.WriteLine("Set Attachment filter successfully.");

            //SizeFilter
            XmlElement nodeSizeFilter = (XmlElement)config.SelectSingleNode("SizeFilter");
            SetSizeFilter(nodeSizeFilter, activity);
            System.Console.WriteLine("Set Size filter successfully.");

            //action
            XmlElement nodeAction = (XmlElement)config.SelectSingleNode("Action");
            SetAction(nodeAction, activity);
            System.Console.WriteLine("Set Action successfully.");

            activity.LastSavedUtc = DateTime.UtcNow;
            return activity;
        }

        

        private static void AddDatasources(XmlNode datasource, ArchiveActivity activity)
        {
            if (datasource == null)
            {
                throw new Exception("No data source parameters.");
            }
            else
            {
                ObservableCollection<SharePointPath> paths = new ObservableCollection<SharePointPath>();
                XmlNode includeNewContent = datasource.SelectSingleNode("NewContent");
                if (includeNewContent != null)
                {
                    XmlElement includeNewContentElem = (XmlElement)includeNewContent;
                    string included = includeNewContentElem.GetAttribute("Include");
                    if (!Boolean.Parse(included))
                        activity.IncludeFutureScopes = false;
                }
                XmlNodeList farmNodes = datasource.SelectNodes("Farm");
                if (farmNodes == null || farmNodes.Count == 0)
                {
                    throw new Exception("No farm parameters.");
                }
                else
                {
                    foreach (XmlNode farmNode in farmNodes)
                    {
                        XmlElement farmElem = (XmlElement)farmNode;
                        String farmUrl = farmElem.GetAttribute("Url");

                        if (farmUrl.EndsWith("/"))
                            farmUrl = farmUrl.Substring(0, farmUrl.Length - 1);
                        Uri farmUri = null;
                        Uri.TryCreate(farmUrl, UriKind.Absolute, out farmUri);
                        String userName = "administrator";
                        String password = "emcsiax@QA";
                        NetworkCredential credentials = new NetworkCredential(userName, password,"sosp");
                        IArchiveServiceClient client = IArchiveServiceClient.GetInstance(IArchiveServiceClient.GetSharePointServicesUrl(farmUri),
                                                              credentials);
                        FarmTopography farmTopography = new FarmTopography(client, farmUri);
                        farmTopography.Initialize();
                        SelectedFarm selectFarm = new SelectedFarm(farmUri, farmTopography.SPFarmTransfer.Id);
                        SPFarmTransfer farm = farmTopography.SPFarmTransfer;
                        IEnumerable<SPWebApplicationTransfer> webApps = client.GetWebApplications(farm.Path.FarmUrl);


                        XmlNodeList nwebApps = datasource.SelectNodes("WebApp");

                        foreach (XmlElement nwebApp in nwebApps)
                        {
                            String sPort = nwebApp.GetAttribute("Port");
                            if (String.IsNullOrEmpty(sPort))
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
                }
            }
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
            if (objs == null)
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
                        if (pos < 0)
                        {
                            if (name.Equals("80", StringComparison.OrdinalIgnoreCase))
                                return obj;
                        }
                        else
                        {
                            url = url.Substring(pos + 1);
                            if (url.EndsWith("/"))
                                url = url.Substring(0, url.Length - 1);
                            if (url.Equals(name, StringComparison.OrdinalIgnoreCase))
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

        private static void AddContentTypes(XmlNode datasource, ArchiveActivity activity)
        {
            XmlNode siteCollectionNode = datasource.SelectSingleNode("SiteCollection");
            XmlElement siteCollectionElem = (XmlElement)siteCollectionNode;
            String siteCollectionUrl = siteCollectionElem.GetAttribute("Url");
            String userName = siteCollectionElem.GetAttribute("UserName");
            String password = siteCollectionElem.GetAttribute("Password");
            Uri url = new Uri(siteCollectionUrl);
            NetworkCredential credentials = new NetworkCredential(userName, password);

            SharePointOnlineArchivingClient service = SharePointOnlineArchivingClient.GetInstance(url, credentials);
            SPOnlineWebHierarchy webHierarchy = new SPOnlineWebHierarchy(url, service);
            webHierarchy.Initialize();
            XmlElement contentTypeElem = (XmlElement)datasource.SelectSingleNode("ContentType");
            if (contentTypeElem == null)
            {
                throw new Exception("No contentType parameters.");
            }
            else
            {
                String excludeHiddenString = contentTypeElem.GetAttribute("ExcludeHidden");
                bool excludeHidden = Boolean.Parse(excludeHiddenString);
                XmlNodeList excludedTypeNodes = contentTypeElem.SelectNodes("ExcludedType");
                List<String> excludedTypes = new List<String>();
                foreach (XmlNode excludedTypeNode in excludedTypeNodes)
                {
                    XmlElement excludedTypeElem = (XmlElement)excludedTypeNode;
                    String excludedTypeName = excludedTypeElem.GetAttribute("Name");
                    if (excludedTypeName != null)
                    {
                        excludedTypes.Add(excludedTypeName);
                    }
                }
                foreach (SPWebTransfer web in webHierarchy.Webs)
                {
                    foreach (SPContentTypeTransfer contentType in service.GetContentTypes(web.Path))
                    {
                        ProcessSubContentTypes(contentType, activity, excludeHidden, excludedTypes);
                    }
                }
            }
        }

        public static void ProcessSubContentTypes(SPContentTypeTransfer contentType, ArchiveActivity activity, bool excludeHidden, List<String> excludedTypes)
        {
            if (!excludeHidden||!ExSPContentTypesPage.IsHiddenOrMaskedContentType(contentType))
            {      
                ContentTypeRule rule = new ContentTypeRule(contentType.Id);
                rule.ContentTypeName = contentType.DisplayName;
                if (!excludedTypes.Contains(contentType.DisplayName))
                {
                    activity.ContentTypeRules.Add(rule);
                }
                foreach (SPContentTypeTransfer subType in contentType.Subtypes)
                {
                    ProcessSubContentTypes(subType, activity, excludeHidden, excludedTypes);
                }
            }
            
        }

        public override void Create()
        {
            XmlNode nodeConfig = nodeActivity.SelectSingleNode("Config");
            ArchiveActivity activity = ConfigActivity(nodeConfig);
            String xmlConfig = SharePointUtil.ToXml<ArchiveActivity>(activity);

            exActivity = (IExActivity)SourceOneContext.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Activity);
            exActivity.xConfig = xmlConfig;

            //Set policy
            String sPolicy = nodeActivity.GetAttribute("Policy");
            if (String.IsNullOrEmpty(sPolicy))
                exActivity.policyID = PolicyManager.CreatePolicy("AutoPolicy").id;
            else
                exActivity.policyID = PolicyManager.CreatePolicy(sPolicy).id;
            exActivity.taskTypeID = (int)ActivityID.SPO;

            //Set activity schedule
            XmlElement nodeSchedule = (XmlElement)nodeActivity.SelectSingleNode("Schedule");
            SetActivitySchedule(nodeSchedule, exActivity);

            String sActivityName = nodeActivity.GetAttribute("Name");
            if (String.IsNullOrEmpty(sActivityName))
                exActivity.name = "SPAuto" + DateTime.UtcNow.Ticks;
            else
                exActivity.name = sActivityName;

            String sLogging = nodeActivity.GetAttribute("Logging");
            if (!sLogging.Trim().ToLower().Equals("false"))
                exActivity.optionsMask = exActivity.optionsMask | (int)exActivityOptions.exActivityOptions_EnableLogging;
            exActivity.state = exActivityState.exActivityState_Active;

            string error = SharePointUtil.GetFirstError((IExVector)exActivity.Validate());
            if (error != null)
            {
                throw new ApplicationException("Activity is not valid:" + error);
            }

            //Persist activity
            exActivity.Save();
            System.Console.WriteLine("Create Activity successfully.");
        }

        

        
    }
}
