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

namespace SPTestWithNunit
{
    [TestFixture]
    public class EndRestoreRegression
    {
        private string testDataPath = @"TestData\EndUserRestoreRegression\";
        private string ActivtyPath = @"TestActivities\EndUserRestoreRegression\";
        private string siteTitle = "QA Site";

        private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");

        private string searchServiceURL = "http://es1all/SearchWs/ExSearchWebService.asmx";
        private string userName = "es1service";
        private string password = "emcsiax@QA";
        private string webAppURL = "http://sp2010sim/";

        [TestFixtureSetUp]
        public void BatchSetup()
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
        public void BatchCleanUp()
        {

        }

        [Test, Description("Test end user restore announcements item as new item!")]
        [Category("EndUser")]
        public void EndUserAnnouncementNewItem()
        {
            string annolibName = "EndUser_RestoreAnnouncementNewItem";
            string title = "EndUserAnnouncement";
            try
            {
                spoLib.CreateAnnouncementLib(siteTitle, annolibName);
                int itemID = spoLib.AddAnnouncementItem(siteTitle, annolibName, title, "AnnouncementBody");

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreAnnouncementNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, annolibName, testDataPath + "Caml_EndUser_RestoreAnnouncementNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "EndUser Restore Announcement item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, annolibName);
            }
        }

        [Test, Description("Test end user restore contact item as new item!")]
        [Category("EndUser")]
        public void EndUserContactNewItem()
        {
            string contactlibName = "EndUser_RestoreContactNewItem";
            try
            {
                spoLib.CreateContactLib(siteTitle, contactlibName);
                int itemID = spoLib.AddContactItem(siteTitle, contactlibName, "ContactName");

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreContactNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, contactlibName, testDataPath + "Caml_EndUser_RestoreContactNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "EndUser Restore contact item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactlibName);
            }
        }

        [Test, Description("Test end user restore DiscussionBoard item as new item!")]
        [Category("EndUser")]
        public void EndUserDiscussionBoardNewItem()
        {
            string discussionListTitle = "EndUser_RestoreDiscussionBoardNewItem";
            string discussionTitle = "TestDiscussion";
            string discussionBody = "TestDiscussionBody";
            try
            {
                spoLib.CreateDiscussionBoardLib(siteTitle, discussionListTitle);
                int itemID = spoLib.AddDiscussionBoardItem(siteTitle, discussionListTitle, discussionTitle, discussionBody);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreDiscussionBoardNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Discussion", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, discussionListTitle, testDataPath + "Caml_EndUser_RestoreDiscussionBoardNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "End User Restore discussion board as new item failed!");
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

        [Test, Description("Test end user restore a link item as new item!")]
        [Category("EndUser")]
        public void EndUserRestoreLinkNewItem()
        {
            string LibName = "EndUser_RestoreLinkNewItem";
            string url = "http://www.batch.com";
            try
            {
                spoLib.CreateLinkLib(siteTitle, LibName);

                int itemID = spoLib.AddLinkItem(siteTitle, LibName, url);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreLinkNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Link", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, LibName, testDataPath + "Caml_EndUser_RestoreLinkNewItem.txt", items.Count + 1);

                Assert.IsTrue(restoreSuccess, "End User Restore link item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, LibName);
            }
        }

        [Test, Description("Test end user restore a Calendar item as new item!")]
        [Category("EndUser")]
        public void EndUserRestoreEventNewItem()
        {
            string LibName = "EndUser_RestoreEventNewItem";
            string title = "RestoreEvent";
            try
            {
                spoLib.CreateEventLib(siteTitle, LibName);
                int itemID = spoLib.AddEventItem(siteTitle, LibName, title);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreEventNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Event", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, LibName, testDataPath + "Caml_EndUser_RestoreEventNewItem.txt", items.Count + 1);

                Assert.IsTrue(restoreSuccess, "End user Restore Event item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, LibName);
            }

        }

        [Test, Description("Test end user restore contact item with Attach as new item!")]
        [Category("EndUser")]
        public void EndUserContactWithAttachNewItem()
        {
            string contactlibName = "EndUser_RestoreContactWithAttchNewItem";
            string attachmentFile = "Attach.txt";
            try
            {
                spoLib.CreateContactLib(siteTitle, contactlibName);
                int itemID = spoLib.AddContactItem(siteTitle, contactlibName, "ContactWithAttachName");

                spoLib.AddAttachment(contactlibName, testDataPath + attachmentFile, itemID.ToString(), attachmentFile);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreContactWithAttchNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, contactlibName, testDataPath + "Caml_EndUser_RestoreContactWithAttchNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "End user Restore item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactlibName);
            }
        }

        [Test, Description("Test end user restore a pic item as new item!")]
        [Category("EndUser")]
        public void EndUserRestorePictureNewItem()
        {
            string doclibName = "EndUser_RestorePictureNewItem";
            string title = "Picture";
            try
            {
                spoLib.CreatePictureLib(siteTitle, doclibName);
                int itemID = spoLib.UploadFile(siteTitle, doclibName, title, testDataPath, "RestoreDocumentItem.jpg");

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestorePictureNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Picture", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, doclibName, testDataPath + "Caml_EndUser_RestorePictureNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "End User Restore picture item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, doclibName);
            }

        }

        [Test, Description("Test end user restore a document item as new version")]
        [Category("EndUser")]
        public void EndUserRestoreDocumentNewVersion()
        {
            string documentLibName = "EndUser_RestoreDocumentNewVersion";
            spoLib.CreateDocumentLib(siteTitle, documentLibName);
            spoLib.EnableListVersion(siteTitle, documentLibName);
            string itemTitle = "RestoreDocumentItem";
            int itemID = spoLib.UploadFile(siteTitle, documentLibName, itemTitle, testDataPath, "RestoreDocumentItem.jpg");

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreDocumentNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "3.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, documentLibName, testDataPath + "Caml_EndUser_RestoreDocumentNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "Restore one document item as new version failed!");
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

        [Test, Description("Test end user restore contact item as new item!")]
        [Category("EndUser")]
        public void EndUserContactNewVersion()
        {
            string contactlibName = "EndUser_RestoreContactNewVersion";
            try
            {
                spoLib.CreateContactLib(siteTitle, contactlibName);
                spoLib.EnableListVersion(siteTitle, contactlibName);
                int itemID = spoLib.AddContactItem(siteTitle, contactlibName, "ContactName");

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreContactNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, contactlibName, testDataPath + "Caml_EndUser_RestoreContactNewItem.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "EndUser Restore contact item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactlibName);
            }
        }

        [Test, Description("Test end user restore DiscussionBoard item as new version!")]
        [Category("EndUser")]
        public void EndUserDiscussionBoardNewVersion()
        {
            string discussionListTitle = "EndUser_RestoreDiscussionBoardNewVersion";
            string discussionTitle = "TestDiscussion";
            string discussionBody = "TestDiscussionBody";
            try
            {
                spoLib.CreateDiscussionBoardLib(siteTitle, discussionListTitle);
                spoLib.EnableListVersion(siteTitle, discussionListTitle);
                int itemID = spoLib.AddDiscussionBoardItem(siteTitle, discussionListTitle, discussionTitle, discussionBody);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreDiscussionBoardNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Discussion", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, discussionListTitle, testDataPath + "Caml_EndUser_RestoreDiscussionBoardNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "End User Restore discussion board as new version failed!");
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

        [Test, Description("Test end user restore link item as new version!")]
        [Category("EndUser")]
        public void EndUserRestoreLinkNewVersion()
        {
            string LibName = "EndUser_RestoreLinkNewVersion";
            string url = "http://www.batch.com";
            try
            {
                spoLib.CreateLinkLib(siteTitle, LibName);
                spoLib.EnableListVersion(siteTitle, LibName);

                int itemID = spoLib.AddLinkItem(siteTitle, LibName, url);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreLinkNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Link", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();
                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, LibName, testDataPath + "Caml_EndUser_RestoreLinkNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "End User Restore link item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, LibName);
            }
        }

        [Test, Description("Test end user restore a Calendar item as new version!")]
        [Category("EndUser")]
        public void EndUserRestoreEventNewVersion()
        {
            string LibName = "EndUser_RestoreEventNewVersion";
            string title = "RestoreEvent";
            try
            {
                spoLib.CreateEventLib(siteTitle, LibName);
                spoLib.EnableListVersion(siteTitle, LibName);
                int itemID = spoLib.AddEventItem(siteTitle, LibName, title);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreEventNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Event", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, LibName, testDataPath + "Caml_EndUser_RestoreEventNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "End user Restore Event item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, LibName);
            }

        }

        [Test, Description("Test end user restore a task item as new version!")]
        [Category("EndUser")]
        public void EndUserRestoreTaskNewVersion()
        {
            string taskLibName = "EndUser_RestoreTaskNewVersion";
            string title = "Task";

            spoLib.CreateTaskLib(siteTitle, taskLibName);
            spoLib.EnableListVersion(siteTitle, taskLibName);

            int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);

            Hashtable fieldValues = new Hashtable();
            fieldValues["Title"] = title + "_V2";
            spoLib.UpdateItem(siteTitle, taskLibName, itemID, fieldValues);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreTaskNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Task", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "3.0";
                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, taskLibName, testDataPath + "Caml_EndUser_RestoreTaskNewVersion.txt", resultItems);

                Assert.IsTrue(restoreSuccess, "End User Restore task item as new version failed!");
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

        [Test, Description("Test end user restore contact item with Attach as new version!")]
        [Category("EndUser")]
        public void EndUserContactWithAttachNewVersion()
        {
            string contactlibName = "EndUser_RestoreContactWithAttchNewVersion";
            string attachmentFile = "Attach.txt";
            try
            {
                spoLib.CreateContactLib(siteTitle, contactlibName);
                spoLib.EnableListVersion(siteTitle, contactlibName);
                int itemID = spoLib.AddContactItem(siteTitle, contactlibName, "ContactWithAttachName");

                spoLib.AddAttachment(contactlibName, testDataPath + attachmentFile, itemID.ToString(), attachmentFile);

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreContactWithAttchNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "3.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, contactlibName, testDataPath + "Caml_EndUser_RestoreContactWithAttchNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "End user Restore item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, contactlibName);
            }
        }

        [Test, Description("Test end user restore a document set item as new item")]
        [Category("EndUser")]
        public void EndUserDocumentSetNewItem()
        {
            string doclibName = "EndUser_RestoreDocumentSetNewItem";
            try
            {
                spoLib.CreateDocumentLib(siteTitle, doclibName);
                string documentSetTtile = "TestDocSet";
                int itemID = spoLib.AddDocumentSetItem(siteTitle, doclibName, documentSetTtile, DateTime.Now.ToString(), DateTime.Now.ToString());

                ES1Activity activity1 = ActivityFactory.CreateActivity(ActivtyPath + "EndUser_RestoreDocumentSetNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                spoLib.DeleteItem(siteTitle, doclibName, itemID);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document Set", 1);
                int count = search.resultCount;

                SPEndUserRestore restore = new SPEndUserRestore(webAppURL, searchServiceURL, userName, password);
                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, doclibName, testDataPath + "Caml_EndUser_RestoreDocumentSetNewItem.txt", 1);
                Assert.IsTrue(restoreSuccess, "End User Restore one document set as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, doclibName);
            }
        }
    }
}
