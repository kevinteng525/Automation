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

namespace SPTestWithNunit
{
    [TestFixture]
    public class EndRestoreBVT
    {
        private string testDataPath = @"TestData\EndRestoreBVT\";
        private string bvtActivtyPath = @"TestActivities\EndRestoreBVT\";
        private string siteTitle = "QA Site";

        private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");

        private string searchServiceURL = "http://es1all/SearchWs/ExSearchWebService.asmx";
        private string userName = "es1service";
        private string password = "emcsiax@QA";
        private string webAppURL = "http://sp2010sim/";

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

        [TestFixtureTearDown]
        public void BVTCleanUp()
        {

        }

        [Test, Description("Test restore a document item as new item successfully!")]
        [Category("BVT")]
        public void EndUserRestoreDocumentNewItem()
        {
            string documentLibName = "BVT_EURestoreDocumentNewItem";
            spoLib.CreateDocumentLib(siteTitle, documentLibName);
            string itemTitle = "RestoreDocumentItem";
            int itemID = spoLib.UploadFile(siteTitle, documentLibName, itemTitle, testDataPath, "RestoreDocumentItem.jpg");

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreDocumentNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;
  
                SPEndUserRestore restore = new SPEndUserRestore(webAppURL,searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, documentLibName, testDataPath + "Caml_EURestoreDocumentNewItem.txt", items.Count+1);
                Assert.IsTrue(restoreSuccess, "Restore one document item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, documentLibName);
            }
        }

        [Test, Description("Test end user restore a announcement item as new version!")]
        [Category("BVT")]
        public void EndUserRestoreAnnouncementNewVersion()
        {
            string annoucementLibName = "BVT_EURestoreAnnounceNewVersion";
            string title = "BVT";
            string body = "BVT test";

            spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreAnnounceNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;
  
                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, annoucementLibName, testDataPath + "Caml_EURestoreAnnounceNewVersion.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore announcement item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, annoucementLibName);
            }

        }

        [Test, Description("Test end user restore a picture item as new version!")]
        [Category("BVT")]
        public void EndUserRestorePictureNewVersion()
        {
            string pictureLibName = "BVT_EURestorePictureNewVersion";
            string title = "BVTPicture";
     
            spoLib.CreatePictureLib(siteTitle, pictureLibName);
            int itemID = spoLib.UploadFile(siteTitle, pictureLibName, title, testDataPath, "PictureLib.jpg");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestorePictureNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Picture", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, pictureLibName, testDataPath + "Caml_EURestorePictureNewVersion.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore picture item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, pictureLibName);
            }

        }

        [Test, Description("Test end user restore a task item as new item!")]
        [Category("BVT")]
        public void EndUserRestoreTaskNewItem()
        {
            string taskLibName = "BVT_EURestoreTaskNewItem";
            string title = "BVT";

            spoLib.CreateTaskLib(siteTitle, taskLibName);
            int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreTaskNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Task", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, taskLibName, testDataPath + "Caml_EURestoreTaskNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore task item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, taskLibName, itemID);
                spoLib.DeleteLib(siteTitle, taskLibName);
            }

        }

        [Test, Description("Test end user restore a announcement item as new item!")]
        [Category("BVT")]
        public void EndUserRestoreAnnouncementNewItem()
        {
            string annoucementLibName = "BVT_EURestoreAnnounceNewItem";
            string title = "BVT";
            string body = "BVT test";

            spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreAnnounceNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, annoucementLibName, testDataPath + "Caml_EURestoreAnnounceNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore announcement item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, annoucementLibName);
            }
        }


        [Test, Description("Test end user restore a task item as new version when delete original list!")]
        [Category("BVT")]
        public void EndUserRestoreTaskNewVersionDeleteList()
        {
            string taskLibName = "BVT_EURestoreTaskNewVersionDL";
            string title = "BVTTask";
   
            spoLib.CreateTaskLib(siteTitle, taskLibName);
            int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);
            
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreTaskNewVersionDL.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteLib(siteTitle, taskLibName);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Task", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, taskLibName, testDataPath + "Caml_EURestoreTaskNewVersionDL.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore task item as new version when delete original list failed!");
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

        [Test, Description("Test end user restore a contact item as new version when delete original list!")]
        [Category("BVT")]
        public void EndUserRestoreContactDeleteList()
        {
            string contactLibName = "BVT_EURestoreContactNewVersionDL";
            string lastname = "BVTContact";

            spoLib.CreateContactLib(siteTitle, contactLibName);
            int itemID = spoLib.AddContactItem(siteTitle, contactLibName, lastname);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreContactNewVersionDL.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteLib(siteTitle, contactLibName);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, contactLibName, testDataPath + "Caml_EURestoreContactNewVersionDL.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore contact item as new version when delete original list failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactLibName);
            }
        }

        [Test, Description("Test end user restore a picture item as new item when delete original list!")]
        [Category("BVT")]
        public void EndUserRestorePictureNewItemDeleteList()
        {
            string pictureLibName = "BVT_EURestorePictureNewItemDL";
            string title = "BVTPicture";

            spoLib.CreatePictureLib(siteTitle, pictureLibName);
            int itemID = spoLib.UploadFile(siteTitle, pictureLibName, title, testDataPath, "PictureLib.jpg");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestorePictureNewItemDL.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteLib(siteTitle, pictureLibName);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Picture", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, pictureLibName, testDataPath + "Caml_EURestorePictureNewItemDL.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore picture item as new item when delete original list failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, pictureLibName);
            }

        }

        [Test, Description("Test end user restore a Link item as new item when delete original item!")]
        [Category("BVT")]
        public void EndUserRestoreLinkNewItemDeleteItem()
        {
            string linkLibName = "BVT_EURestoreLinkNewItemDI";
            string url = "http://www.bvt.com";

            spoLib.CreateLinkLib(siteTitle, linkLibName);
            int itemID = spoLib.AddLinkItem(siteTitle, linkLibName, url);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreLinkNewItemDI.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteItem(siteTitle, linkLibName, itemID);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Link", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, linkLibName, testDataPath + "Caml_EURestoreLinkNewItemDI.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore link item as new item when delete original item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, linkLibName);
            }
        }

        [Test, Description("Test end user restore a announcement item as new item when delete original item!")]
        [Category("BVT")]
        public void EndUserRestoreAnnouncementNewItemDeleteItem()
        {
            string annoucementLibName = "BVT_EURestoreAnnounceNewItemDI";
            string title = "BVTAnnouncement";
            string body = "BVT test";

            spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreAnnounceNewItemDI.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteItem(siteTitle, annoucementLibName, itemID);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, annoucementLibName, testDataPath + "Caml_EURestoreAnnounceNewItemDI.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore announcement item as new item when delete original item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, annoucementLibName);
            }
        }

        [Test, Description("Test end user restore a contact item as new item when delete original item!")]
        [Category("BVT")]
        public void EndUserRestoreContactNewItemDeleteItem()
        {
            string contactLibName = "BVT_EURestoreContactNewItemDI";
            string lastname = "BVTContact";

            spoLib.CreateContactLib(siteTitle, contactLibName);
            int itemID = spoLib.AddContactItem(siteTitle, contactLibName, lastname);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreContactNewItemDI.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteItem(siteTitle, contactLibName,itemID);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, contactLibName, testDataPath + "Caml_EURestoreContactNewItemDI.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore contact item as new item when delete original item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactLibName);
            }
        }

        [Test, Description("Test restore a document item as new item when delete original item successfully!")]
        [Category("BVT")]
        public void EndUserRestoreDocumentNewItemDeleteItem()
        {
            string documentLibName = "BVT_EURestoreDocumentNewItemDI";
            spoLib.CreateDocumentLib(siteTitle, documentLibName);
            string itemTitle = "RestoreDocumentItem";
            int itemID = spoLib.UploadFile(siteTitle, documentLibName, itemTitle, testDataPath, "RestoreDocumentItem.jpg");

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_EURestoreDocumentNewItemDI.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteItem(siteTitle, documentLibName, itemID);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, documentLibName, testDataPath + "Caml_EURestoreDocumentNewItemDI.txt", items.Count);
                Assert.IsTrue(restoreSuccess, "Restore one document item as new item when delete original item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, documentLibName);
            }
        }
    }
}
