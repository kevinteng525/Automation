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
    public class BatchRestoreBVT
    {
        private string testDataPath = @"TestData\BatchRestoreBVT\";
        private string bvtActivtyPath = @"TestActivities\BatchRestoreBVT\";
        private string siteTitle = "QA Site";
        
        private string listTitle = "AddedByBatchRestoreBVTAuto";
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

        [TearDown]
        public void ClearUp()
        {
            
        }

        [TestFixtureTearDown]
        public void BVTCleanUp()
        {
            
        }

        [Test, Description("Test batch restore a post as new item successfully!")]
        [Category("BVT")]
        public void BatchRestorePostNewItem()
        {
            string blogSiteTitle = "BVT_PURestorePostNewItem";
            spoLib.CreateWeb(blogSiteTitle, "BLOG#0");
            int itemID = spoLib.AddPostItem(blogSiteTitle, "Posts", "TestBlog", "TestBody", "1", DateTime.Now.ToString(), DateTime.Now.ToString());
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestorePostNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Post", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(blogSiteTitle, "Posts", testDataPath + "Caml_PURestorePostNewItem.txt", batchItemsCount + 2);
                Assert.IsTrue(restoreSuccess, "Test batch restore a post as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteWeb(blogSiteTitle);
            }
        }

        [Test, Description("Test batch restore a comment as new item successfully!")]
        [Category("BVT")]
        public void BatchRestoreCommentNewItem()
        {
            string blogSiteTitle = "BVT_PURestoreCommentNewItem";
            spoLib.CreateWeb(blogSiteTitle, "BLOG#0");
            int itemID = spoLib.AddPostItem(blogSiteTitle, "Posts", "TestBlog", "TestBody", "1", DateTime.Now.ToLocalTime().ToString(), DateTime.Now.ToLocalTime().ToString());
            spoLib.AddCommentItem(blogSiteTitle, "Comments", "TestComment", "TestCommentBody", itemID.ToString(), DateTime.Now.ToLocalTime().ToString(), DateTime.Now.ToString());
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreCommentNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Comment", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(blogSiteTitle, "Comments", testDataPath + "Caml_PURestoreCommentNewItem.txt", batchItemsCount + 1);
                Assert.IsTrue(restoreSuccess, "Batch restore a comment as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteWeb(blogSiteTitle);
            }
        }

        [Test, Description("Test batch restore a document set item as new item successfully!")]
        [Category("BVT")]
        public void BatchRestoreDocumentSetNewItem()
        {
            string doclibName = "BVT_PURestoreDocumentSetNewItem";
            try
            {
                spoLib.CreateDocumentLib(siteTitle, doclibName);
                string documentSetTtile = "TestSetforBVT";
                int itemID = spoLib.AddDocumentSetItem(siteTitle, doclibName, documentSetTtile, DateTime.Now.ToString(), DateTime.Now.ToString());
            
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreDocumentSetNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);
                spoLib.DeleteItem(siteTitle, listTitle, itemID);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document Set", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, doclibName, testDataPath + "Caml_PURestoreDocumentSetNewItem.txt", 1);
                Assert.IsTrue(restoreSuccess, "Batch Restore one document set as new item failed!");
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

        [Test, Description("Test batch restore a document item as new version!")]
        [Category("BVT")]
        public void BatchRestoreDocumentNewVersion()
        {
            string doclibName = "BVT_PURestoreDocumentNewVersion";
            string title = "BVTDocument";
            try
            {
                spoLib.CreateDocumentLib(siteTitle, doclibName);
                spoLib.EnableListVersion(siteTitle, doclibName);
                int itemID = spoLib.UploadFile(siteTitle, doclibName, title, testDataPath, "RestoreDocumentItem.jpg");
                Hashtable fieldValues = new Hashtable();
                fieldValues["Title"] = title + "_V1";
                spoLib.UpdateItem(siteTitle, doclibName, itemID, fieldValues);
            
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreDocumentNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable itemIDVersion = new Hashtable();
                itemIDVersion[itemID] = "6.0";
                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, doclibName, testDataPath + "Caml_PURestoreDocumentNewVersion.txt", itemIDVersion);
                Assert.IsTrue(restoreSuccess, "Batch Restore document item as new version failed!");
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

        [Test, Description("Test batch restore a discussion board item as new item!")]
        [Category("BVT")]
        public void BatchRestoreDiscussionboardItem()
        {
            string discussionListTitle = "BVT_PURestoreDisboardNewItem";
            string discussionTitle = "TestDiscussion";
            string discussionBody = "TestDiscussionBody";
            try
            {
                spoLib.CreateDiscussionBoardLib(siteTitle, discussionListTitle);
                int itemID = spoLib.AddDiscussionBoardItem(siteTitle, discussionListTitle, discussionTitle, discussionBody);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreDisboardNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Discussion", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, discussionListTitle, testDataPath + "Caml_PURestoreDisboardNewItem.txt", batchItemsCount + 1);

                Assert.IsTrue(restoreSuccess, "Batch Restore discussion board as new item failed!");
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


        [Test, Description("Test batch restore a task item as new version!")]
        [Category("BVT")]
        public void BatchRestoreTaskNewVersion()
        {
            string taskLibName = "BVT_PURestoreTaskNewVersion";
            string title = "BVTTask";

            spoLib.CreateTaskLib(siteTitle, taskLibName);
            spoLib.EnableListVersion(siteTitle, taskLibName);

            int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);

            Hashtable fieldValues = new Hashtable();
            fieldValues["Title"] = title + "_V2";
            spoLib.UpdateItem(siteTitle, taskLibName, itemID, fieldValues);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreTaskNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Task", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "4.0";
                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, taskLibName, testDataPath + "Caml_PURestoreTaskNewVersion.txt", resultItems);

                Assert.IsTrue(restoreSuccess, "Batch Restore task item as new version failed!");
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

        [Test, Description("Test batch restore a link item as new version!")]
        [Category("BVT")]
        public void BatchRestoreLinkNewVersion()
        {
            string linkLibName = "BVT_PURestoreLinkNewVersion";
            string url = "http://www.bvt.com";
            try
            {
                spoLib.CreateLinkLib(siteTitle, linkLibName);
                spoLib.EnableListVersion(siteTitle, linkLibName);

                int itemID = spoLib.AddLinkItem(siteTitle, linkLibName, url);

                Hashtable fieldValues = new Hashtable();
                fieldValues["URL"] = url + ".cn";
                spoLib.UpdateItem(siteTitle, linkLibName, itemID, fieldValues);


                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_PURestoreLinkNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Link", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "4.0";
                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, linkLibName, testDataPath + "Caml_PURestoreLinkNewVersion.txt", resultItems);

                Assert.IsTrue(restoreSuccess, "Batch Restore link item as new version failed!");
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

        [Test, Description("Test run two batch restore job to restore same calendar item as new item!")]
        [Category("BVT")]
        public void MultiBatchRestoreEventNewItem()
        {
            string eventLibName = "BVT_MultiPURestoreEventNewItem";
            string title = "BVTEvent";

            try
            {
                spoLib.CreateEventLib(siteTitle, eventLibName);
                int itemID = spoLib.AddEventItem(siteTitle, eventLibName, title);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_MultiPURestoreEventNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Event", 1);
                int count = search.resultCount;
                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                SPPowerUserRestore restore1 = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                int batchItemsCount1 = restore.Execute(ConflictResolution.CreateNewVersion, items);
                int batchItemsCount2 = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();
                System.Threading.Thread.Sleep(60000);
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, eventLibName, testDataPath + "Caml_MultiPURestoreEventNewItem.txt", batchItemsCount1 + batchItemsCount2 + 1);
                Assert.IsTrue(restoreSuccess, "Test run two batch restore job to restore same calendar item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, eventLibName);
            }

        }
    }
}
