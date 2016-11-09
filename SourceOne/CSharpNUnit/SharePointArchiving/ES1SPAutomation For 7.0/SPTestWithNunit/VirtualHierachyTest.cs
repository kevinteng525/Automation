using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ES1SPAutoLib;
using ES1SPAutoLib.Activity;
using ES1.ES1SPAutoLib;
using TaskScheduler;
using SharepointOnline;
using Microsoft.SharePoint.Client;
using NUnit.Framework;
using RestoreLib;
using System.Data;
using EMC.SourceOne.SharePoint.SearchRestore;
using System.Collections;
using System.Xml;

namespace SPTestWithNunit
{
    [TestFixture]
    public class VirtualHierachyTest
    {
        private string farmID = @"34e06ce6-25bd-49eb-8764-9fde3cb8bd47";
        private string vhActivtyPath = @"TestActivities\VirtualHierachyTest\";
        private string testDataPath = @"TestData\VirtualHierachyTest\";
        private string siteTitle = "QA Site";

        private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");

        private string searchServiceURL = "http://es1all/SearchWs/ExSearchWebService.asmx";
        private string userName = "es1service";
        private string password = "emcsiax@QA";
        private string webAppURL = "http://sp2010sim/";
        private string mapFolder1 = "SPMDAF1";
        private string mapFolder2 = "SPMDAF2";
        private string mapFolder3 = "SPMDAF3";
        private string mapFolder4 = "SPMDAF4";
        private string mapFolder5 = "SPMDAF5";


        [TestFixtureSetUp]
        public void BVTSetup()
        {
            Configuration.FillInValues("Configuration.xml");
            Helper.ConfigES1();
        }

        [SetUp]
        public void CaseSetup()
        {

        }

        [TearDown]
        public void ClearUp()
        {

        }

        [TestFixtureTearDown]
        public void BVTCleanUp()
        {

        }

        [Test, Description("Test input duplicate map folder should return only one!")]
        [Category("VH")]
        public void InputDuplicateMapFolder()
        {
            VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
            List<String> folders = new List<string>();
            folders.Add(mapFolder3);
            folders.Add(mapFolder3);
            XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
            string folderID = vh.GetBusGUID(mapFolder3);
            string xpath = @"//BusinessFolder[@GUID='" + folderID + @"']";
            XmlNodeList busFolderNodeList = folderXML.SelectNodes(xpath);
            Assert.AreEqual(1, busFolderNodeList.Count, "Input duplicate Map folder will return two records!");
        }

        [Test, Description("Test archive same items to different map folders should have location in both map folders!")]
        [Category("VH")]
        public void ArchiveSameItemToDiffMapFolders()
        {
            string itemTitle = "vhTest";
            string listTitle = "VH_ArchiveSameItemToDiffMapFolders";
            spoLib.CreateDocumentLib(siteTitle, listTitle);
            int itemID = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ScheduledDaily.txt");
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder1);
                folders.Add(mapFolder2);
                XmlDocument folderXMLBefore = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXMLBefore, mapFolder1);
                List<String> f2LocationsBefore = vh.GetLocations(folderXMLBefore, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveSameItemToDiffMapFolders1.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items to SPMDAF1 failed");

                ES1Activity activity2 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveSameItemToDiffMapFolders2.xml", ActivityTypes.Sharepoint);
                activity2.Create();
                activity2.Run(600);
                Assert.AreEqual(1, activity2.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder1);
                List<String> f2LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);

                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                List<String> f2MoreLocations = GetMoreLocations(f2LocationsBefore, f2LocationsAfter);
                Assert.AreEqual(1, f1MoreLocations.Count, "SPMDAF1 location count is not correct!");
                Assert.AreEqual(GetListLocation(webAppURL, listTitle, false).ToLower(), f1MoreLocations[0], "SPMDAF1 location is not correct!");
                Assert.AreEqual(1, f2MoreLocations.Count, "SPMDAF2 location count is not correct!");
                Assert.AreEqual(GetListLocation(webAppURL, listTitle, false).ToLower(), f2MoreLocations[0], "SPMDAF2 location is not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, listTitle);
            }
        }

        

        [Test, Description("Test archive to single map folders should have location in that folders!")]
        [Category("VH")]
        public void ArchiveToSingleMapFolder()
        {
            string discussionListTitle = "VH_ArchiveToSingleMapFolder";
            string discussionTitle = "VHDiscussion";
            string discussionBody = "TestDiscussionBody";
            try
            {
                spoLib.CreateDiscussionBoardLib(siteTitle, discussionListTitle);
                int itemID = spoLib.AddDiscussionBoardItem(siteTitle, discussionListTitle, discussionTitle, discussionBody);

                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder1);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder1);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveToSingleMapFolder.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items to SPMDAF1 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder1);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(1, f1MoreLocations.Count, "SPMDAF1 location count is not correct!");
                Assert.AreEqual(GetListLocation(webAppURL, discussionListTitle, true).ToLower(), f1MoreLocations[0], "SPMDAF1 location is not correct!");
                
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, discussionListTitle);
            }
        }

        [Test, Description("Test archive different items to multiple map folders should have locations in all map folders!")]
        [Category("VH")]
        public void ArchiveDiffItemToDiffMapFolder()
        {
            string item1Title = "vhTest1";
            string list1Title = "VH_ArchiveDiffItemToDiffMapFolder1";
            spoLib.CreateDocumentLib(siteTitle, list1Title);
            int itemID1 = spoLib.UploadFile(siteTitle, list1Title, item1Title, testDataPath, "ScheduledDaily.txt");

            string contactLibName = "VH_ArchiveDiffItemToDiffMapFolder2";
            string lastname = "vhTest2";
            spoLib.CreateContactLib(siteTitle, contactLibName);
            int itemID2 = spoLib.AddContactItem(siteTitle, contactLibName, lastname);
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder1);
                folders.Add(mapFolder2);
                XmlDocument folderXMLBefore = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXMLBefore, mapFolder1);
                List<String> f2LocationsBefore = vh.GetLocations(folderXMLBefore, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveDiffItemToDiffMapFolder1.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items to SPMDAF1 failed");

                ES1Activity activity2 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveDiffItemToDiffMapFolder2.xml", ActivityTypes.Sharepoint);
                activity2.Create();
                activity2.Run(600);
                Assert.AreEqual(1, activity2.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder1);
                List<String> f2LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);

                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                List<String> f2MoreLocations = GetMoreLocations(f2LocationsBefore, f2LocationsAfter);
                Assert.AreEqual(1, f1MoreLocations.Count, "SPMDAF1 location count is not correct!");
                Assert.AreEqual(GetListLocation(webAppURL, list1Title, false).ToLower(), f1MoreLocations[0], "SPMDAF1 location is not correct!");
                Assert.AreEqual(1, f2MoreLocations.Count, "SPMDAF2 location count is not correct!");
                Assert.AreEqual(GetListLocation(webAppURL, contactLibName, true).ToLower(), f2MoreLocations[0], "SPMDAF2 location is not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, list1Title);
                spoLib.DeleteLib(siteTitle, contactLibName);
            }
        }

        [Test, Description("Test not input map folder should return all map folders!")]
        [Category("VH")]
        public void NotInputMapFolder()
        {
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                XmlDocument folderXML1 = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                folders.Add(mapFolder1);
                folders.Add(mapFolder2);
                folders.Add(mapFolder3);
                folders.Add(mapFolder4);
                folders.Add(mapFolder5);
                XmlDocument folderXML2 = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                Assert.AreEqual(folderXML1.InnerXml, folderXML2.InnerXml, "Not input map folder will not return all map folders!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test, Description("Test not input domain ID should return all domains location!")]
        [Category("VH")]
        public void NotInputDomainID()
        {
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder3);
                folders.Add(mapFolder3);
                folders.Add(mapFolder5);
                folders.Add("SPMDAF7");
                folders.Add(mapFolder1);
                folders.Add(mapFolder2);
                folders.Add(mapFolder4);
                XmlDocument result1 = vh.GetArchiveFolderHierarchyXML(folders, "");
                XmlDocument result2 = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                Assert.AreEqual(result1.InnerXml, result2.InnerXml, "Not input Domain ID will not return all locations!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test, Description("Test input wrong map folder should not return locations!")]
        [Category("VH")]
        public void InputWrongMapFolder()
        {
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder3);
                folders.Add(mapFolder4);
                folders.Add(mapFolder5);
                folders.Add("SPMDAFWrong");
                List<String> folderIDs = vh.GetBusGUIDs(folders);
                folderIDs.Add("12321");
                XmlDocument result1 = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                XmlDocument result2 = vh.GetArchiveFolderHierarchyXMLByIDs(folderIDs, farmID);

                Assert.AreEqual(result1.InnerXml, result2.InnerXml, "Input wrong map folder is not correct!");
            }
            
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test, Description("Test input wrong domain ID should not return locations!")]
        [Category("VH")]
        public void InputWrongDomainID()
        {
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder5);
                folders.Add(mapFolder1);
                folders.Add(mapFolder2);
                folders.Add(mapFolder4);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, "wrong-domain-id");
                List<String> locations = vh.GetLocations(folderXML, mapFolder1);
            }
            
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test, Description("Test archive items under subsite should return correct location!")]
        [Category("VH")]
        public void ArchiveItemsUnderSubSite()
        {       
            string subSiteTitle = "VH_SubSite_ArchiveItemsUnderSubSite";
            Web subSite = spoLib.CreateWeb(subSiteTitle, "STS#0");
            string annoucementLibName = "VH_Announcement_ArchiveItemsUnderSubSite";
            string title = "VH_Anno1_ArchiveItemsUnderSubSite";
            string body = "Body for Anno1";
            try
            {
                spoLib.CreateAnnouncementLib(subSiteTitle, annoucementLibName);
                int itemID = spoLib.AddAnnouncementItem(subSiteTitle, annoucementLibName, title, body);
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder1);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder1);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveItemsUnderSubSite.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items to SPMDAF1 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder1);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(3, f1MoreLocations.Count, "SPMDAF1 location count is not correct!");
                Assert.AreEqual(GetListLocation(TrimURL(webAppURL) + subSite.ServerRelativeUrl, annoucementLibName, true).ToLower(), f1MoreLocations[0], "SPMDAF1 location is not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteWeb(subSite);
            }
        }

        [Test, Description("Test archive items under subsubsite should return correct location!")]
        [Category("VH")]
        public void ArchiveItemsUnderSubSubSite()
        {
            string subSiteTitle = "VH_SubSite_ArchiveItemsUnderSubSubSite";
            Web subSite = spoLib.CreateWeb(subSiteTitle, "STS#0");
            spoLib.ReNewClientContext(TrimURL(webAppURL) + subSite.ServerRelativeUrl);
            string subSubSiteTitle = "VH_SubSubSite_ArchiveItemsUnderSubSubSite";
            Web subSubSite = spoLib.CreateWeb(subSubSiteTitle, "STS#0");
            string pictureLibName = "Pic_ArchiveItemsUnderSubSubSite";
            string title = "PicTitle";
           
            try
            {
                spoLib.CreatePictureLib(subSubSite, pictureLibName);
                int itemID = spoLib.UploadFile(subSubSiteTitle, pictureLibName, title, testDataPath, "PictureLib.jpg");
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder2);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveItemsUnderSubSubSite.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(3, f1MoreLocations.Count, "Location count is not correct!");
                Assert.AreEqual(GetListLocation(TrimURL(webAppURL) + subSubSite.ServerRelativeUrl, pictureLibName, false).ToLower(), f1MoreLocations[0], "Location is not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteWeb(subSubSite);
                spoLib.ReNewClientContext(webAppURL);
                spoLib.DeleteWeb(subSiteTitle);
            }
        }

        [Test, Description("Test archive items under folder should return correct location!")]
        [Category("VH")]
        public void ArchiveItemsUnderFolder()
        {
            string listTitle = "VH_ArchiveItemsUnderFolder";
            spoLib.CreateDocumentLib(siteTitle, listTitle);
            string folderName = "folder1";
            string itemTitle = "VH_Item_ArchiveItemsUnderFolder";
            spoLib.AddFolder(siteTitle, listTitle, folderName);
            spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ScheduledDaily.txt", folderName);
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder2);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveItemsUnderFolder.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(2, f1MoreLocations.Count, "Location count is not correct!");
                List<String> expectedLocations = new List<string>();
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), listTitle, false).ToLower());
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), listTitle, false).ToLower() + "/" + folderName);
                Assert.IsTrue(ValidateLocations(expectedLocations, f1MoreLocations), "Locations are not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, listTitle);
            }
        }

        [Test, Description("Test archive items under sub folder should return correct location!")]
        [Category("VH")]
        public void ArchiveItemsUnderSubFolder()
        {
            string taskLibName = "VH_ArchiveItemsUnderSubFolder";
            string title = "Title for Task";
            spoLib.CreateTaskLib(siteTitle, taskLibName);
            string folderName = "TaskFolder";
            string subFolderName = "SubFolder";
            spoLib.AddFolder(siteTitle, taskLibName, folderName);
            spoLib.AddSubFolder(siteTitle, taskLibName, folderName, subFolderName);
            spoLib.AddTaskItem(siteTitle, taskLibName, title, GetListLocation(TrimURL(webAppURL), taskLibName, true) + "/" + folderName + "/" + subFolderName);
            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder2);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveItemsUnderSubFolder.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(3, f1MoreLocations.Count, "Location count is not correct!");
                List<String> expectedLocations  = new List<string>();
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), taskLibName, true).ToLower());
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), taskLibName, true).ToLower() + "/" + folderName);
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), taskLibName, true).ToLower() + "/" + folderName + "/" + subFolderName);
                Assert.IsTrue(ValidateLocations(expectedLocations, f1MoreLocations), "Locations are not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, taskLibName);
            }
        }

        [Test, Description("Test archive items under different web applications should return correct location!")]
        [Category("VH")]
        public void ArchiveItemsUnderDifferentWebApps()
        {
            string listTitle = "VH_ArchiveItemsUnderDifferentWebApps";
            string itemTitle = "Item1";
            spoLib.CreateDocumentLib(siteTitle, listTitle);
            int itemID1 = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ItemMultiSites.doc");
            SPOLib spoLib8088 = new SPOLib(@"SharePoint\SP2010sim8088.xml");
            
            string siteTitle8088 = "DEMO Site";
            spoLib8088.CreateDocumentLib(siteTitle8088, listTitle);
            int itemID2 = spoLib8088.UploadFile(siteTitle8088, listTitle, itemTitle, testDataPath, "ItemMultiSites.doc");

            try
            {
                VirtualHierachy vh = new VirtualHierachy(webAppURL, searchServiceURL, userName, password);
                List<String> folders = new List<string>();
                folders.Add(mapFolder2);
                XmlDocument folderXML = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsBefore = vh.GetLocations(folderXML, mapFolder2);

                ES1Activity activity1 = ActivityFactory.CreateActivity(vhActivtyPath + "VH_ArchiveItemsUnderDifferentWebApps.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items to SPMDAF2 failed");

                XmlDocument folderXMLAfter = vh.GetArchiveFolderHierarchyXML(folders, farmID);
                List<String> f1LocationsAfter = vh.GetLocations(folderXMLAfter, mapFolder2);
                List<String> f1MoreLocations = GetMoreLocations(f1LocationsBefore, f1LocationsAfter);
                Assert.AreEqual(2, f1MoreLocations.Count, "Location count is not correct!");
                List<String> expectedLocations = new List<string>();
                expectedLocations.Add(GetListLocation(TrimURL(webAppURL), listTitle, false).ToLower());
                expectedLocations.Add(GetListLocation(TrimURL(spoLib8088.webUrl), listTitle, false).ToLower());
                Assert.IsTrue(ValidateLocations(expectedLocations, f1MoreLocations), "Locations are not correct!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, listTitle);
                spoLib8088.DeleteLib(siteTitle8088, listTitle);
            }
        }

        public List<String> GetMoreLocations(List<String> locationsBefore, List<String> locationsAfter)
        {
            List<String> locations = new List<string>();
            if (locationsAfter.Count <= locationsBefore.Count)
                return locations;
            foreach (String location in locationsAfter)
            {
                if (!locationsBefore.Contains(location))
                    locations.Add(location);
            }
            return locations;
        }

        public string GetListLocation(string site, string listname, bool isList)
        {
            if (isList)
                return TrimURL(site) + "/lists/" + listname;
            return TrimURL(site) + "/" + listname;
        }

        private string TrimURL(string url)
        {
            if (url[url.Length-1] == '/')
            {
                string trimURL = url.Substring(0, url.Length - 1);
                return trimURL;
            }
            return url;
        }

        private Boolean ValidateLocations(List<String> expectedList, List<String> resultList)
        {
            if (expectedList.Count != resultList.Count)
                return false;
            foreach (String expected in expectedList)
            {
                if (!resultList.Contains(expected))
                    return false;
            }
            return true;
        }
    }
}
