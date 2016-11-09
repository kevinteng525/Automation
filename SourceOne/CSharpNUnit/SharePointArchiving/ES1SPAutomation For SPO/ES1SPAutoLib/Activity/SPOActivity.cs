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
using EMC.EX.SharePoint.ArchivingOnlineClient;

namespace ES1.ES1SPAutoLib
{
    public class SPOActivity : ES1Activity
    {

        public SPOActivity(string activityXmlFile)
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
                XmlNodeList siteCollectionNodes = datasource.SelectNodes("SiteCollection");
                if (siteCollectionNodes == null || siteCollectionNodes.Count==0)
                {
                    throw new Exception("No site collection parameters.");
                }
                else
                {
                    foreach (XmlNode siteCollection in siteCollectionNodes)
                    {
                        XmlElement siteCollectionElem = (XmlElement)siteCollection;
                        String siteCollectionUrl = siteCollectionElem.GetAttribute("Url");
                        String userName = siteCollectionElem.GetAttribute("UserName");
                        String password = siteCollectionElem.GetAttribute("Password");
                        Uri url = new Uri(siteCollectionUrl);
                        NetworkCredential credentials = new NetworkCredential(userName, password);
                        ArchivingServiceOnline service = new ArchivingServiceOnline(url, credentials);
                        
                        SharePointOnlineArchivingClient client = new SharePointOnlineArchivingClient(service);
                        SPOnlineWebHierarchy webHierarchy = new SPOnlineWebHierarchy(url, client);
                        webHierarchy.Initialize();

                        SelectedFarm selectedSite = new SelectedFarm(url, webHierarchy.SPSite.Id, credentials);
                        
                        XmlNodeList siteNodes = siteCollection.SelectNodes("Site");
                        foreach (XmlNode siteNode in siteNodes)
                        {
                            ProcessSite(siteNode, webHierarchy, webHierarchy.Webs, selectedSite);
                        }

                        activity.SelectedFarms.Add(selectedSite);
                                                
                    }
                }
            }
        }

        private static void ProcessSite(XmlNode siteNode, SPOnlineWebHierarchy webHierarchy, IEnumerable<SPWebTransfer> spWebTransferCol, SelectedFarm selectedSite)
        {
            foreach (SPWebTransfer web in spWebTransferCol)
            {
                if (web.Name.Equals(((XmlElement)siteNode).GetAttribute("Name")))
                {
                    if (((XmlElement)siteNode).GetAttribute("SelectAll").Equals("true"))
                    {
                        selectedSite.SelectedPaths.Add(web.Path);
                    }
                    else
                    {
                        webHierarchy.Expand(web);

                        foreach (XmlNode listNode in siteNode.SelectNodes("List"))
                        {
                            ProcessList(listNode, web.Lists, selectedSite);
                        }
                        foreach (XmlNode subSiteNode in siteNode.SelectNodes("Site"))
                        {
                            ProcessSite(subSiteNode, webHierarchy, web.Sites, selectedSite);
                        }
                    }
                }
            }
        }

        private static void ProcessList(XmlNode listNode, ICollection<SPListTransfer> spListTransferCol, SelectedFarm selectedSite)
        {
            foreach (EMC.EX.SharePoint.ServicesInterface.TransferObjects.SPListTransfer list in spListTransferCol)
            {
                if (list.Title.Equals(((XmlElement)listNode).GetAttribute("Name")))
                {
                    // here path is the selected path or exclued path.
                    SharePointPath path = list.Path;
                    selectedSite.SelectedPaths.Add(path);
                }
            }
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
