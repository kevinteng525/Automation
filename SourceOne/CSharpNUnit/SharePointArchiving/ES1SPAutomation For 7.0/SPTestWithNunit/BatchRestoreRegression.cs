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
    class BatchRestoreRegression
    {
        private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");
        private string siteTitle = "QA Site";
        private string regressionActivtyPath = @"TestActivities\BatchRestoreRegression\";


        private string searchServiceURL = "http://es1all/SearchWs/ExSearchWebService.asmx";
        private string userName = "es1service";
        private string password = "emcsiax@QA";
        private string webAppURL = "http://sp2010sim/";

        private string testDataPath = @"TestData\BatchRestoreRegression\";

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

        [Test, Description("As a Power User, I want to handle the conflict by creating new version with original item exists if the type is Calendar")]
        [Category("Regression")]
        public void BatchRestoreCalendarNewVersion()
        {
            string calendarLibName = "Reg_RestoreCalendarNewVersion";
            string title = "RegCalendar";

            spoLib.CreateEventLib(siteTitle, calendarLibName);
            spoLib.EnableListVersion(siteTitle, calendarLibName);
            int itemID = spoLib.AddEventItem(siteTitle, calendarLibName, title);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreCalendarNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Event", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, calendarLibName, testDataPath + "Caml_RestoreCalendarNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "Batch Restore calendar item as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, calendarLibName);
            }
        }

        [Test, Description("As a Power User, I want to handle the conflict by creating new version with original item exists if the type is Discussion Board")]
        [Category("Regression")]
        public void BatchRestoreDiscussionBoardNewVersion()
        {
            string dicBoardLibName = "Reg_RestoreDiscussionBoardNewVersion";
            string discussionTitle = "RegDiscussion";
            string discussionBody = "TestDiscussionBody";

            spoLib.CreateDiscussionBoardLib(siteTitle, dicBoardLibName);
            spoLib.EnableListVersion(siteTitle, dicBoardLibName);
            int itemID = spoLib.AddDiscussionBoardItem(siteTitle, dicBoardLibName, discussionTitle, discussionBody);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreDisboardNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Discussion", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, dicBoardLibName, testDataPath + "Caml_RestoreDisboardNewVersion.txt", resultItems);

                Assert.IsTrue(restoreSuccess, "Batch Restore discussion board as new version failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, dicBoardLibName);
            }
        }

        [Test, Description("As a Power User, I want to handle the conflict by creating new version with original item exists if the type is Contact")]
        [Category("Regression")]
        public void BatchRestoreContactNewVersion()
        {
            string contactLibName = "Reg_RestoreContactNewVersion";
            string lastname = "RegContact";
                        
            spoLib.CreateContactLib(siteTitle, contactLibName);
            spoLib.EnableListVersion(siteTitle, contactLibName);
            int itemID = spoLib.AddContactItem(siteTitle, contactLibName, lastname);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreContactNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, contactLibName, testDataPath + "Caml_RestoreContactNewVersion.txt", resultItems);
                Assert.IsTrue(restoreSuccess, "Restore contact item as new version list failed!");
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new version with original item exists if the type is Announcements")]
        [Category("Regression")]
        public void BatchRestoreAnnouncementNewVersion()
        {
            string annoucementLibName = "Reg_RestoreAnnounceNewVersion";
            string title = "Reg_Announcement";
            string body = "regression test";

            spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            spoLib.EnableListVersion(siteTitle, annoucementLibName);
            int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreAnnounceNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "2.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, annoucementLibName, testDataPath + "Caml_RestoreAnnounceNewVersion.txt", resultItems);
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new version with original item exists if the type is Picture Library")]
        [Category("Regression")]
        public void BatchRestorePictureNewVersion()
        {
            string pictureLibName = "Reg_RestorePictureNewVersion";
            string title = "RegPicture";

            spoLib.CreatePictureLib(siteTitle, pictureLibName);
            spoLib.EnableListVersion(siteTitle, pictureLibName);
            int itemID = spoLib.UploadFile(siteTitle, pictureLibName, title, testDataPath, "Reg_Picture.jpg");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestorePictureNewVersion.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Picture", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewVersion, items);
                restore.WaitForRestore();

                Hashtable resultItems = new Hashtable();
                resultItems[itemID] = "4.0";

                bool restoreSuccess = spoLib.ValidateRestoreNewVersion(siteTitle, pictureLibName, testDataPath + "Caml_RestorePictureNewVersion.txt", resultItems);
      
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Tasks")]
        [Category("Regression")]
        public void BatchRestoreTaskNewItem()
        {
            string taskLibName = "Reg_RestoreTaskNewItem";
            string title = "Reg_Task";

            spoLib.CreateTaskLib(siteTitle, taskLibName);
            int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreTaskNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Task", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, taskLibName, testDataPath + "Caml_RestoreTaskNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore task item as new item failed!");
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Calendar")]
        [Category("Regression")]
        public void BatchRestoreCalendarNewItem()
        {
            string calendarLibName = "Reg_RestoreCalendarNewItem";
            string title = "RegCalendar";

            spoLib.CreateEventLib(siteTitle, calendarLibName);
            int itemID = spoLib.AddEventItem(siteTitle, calendarLibName, title);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreCalendarNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Event", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, calendarLibName, testDataPath + "Caml_RestoreCalendarNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Batch Restore calendar item as new item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, calendarLibName);
            }
        }

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Links")]
        [Category("Regression")]
        public void BatchRestoreLinkNewItem()
        {
            string linkLibName = "Reg_RestoreLinkNewItem";
            string url = "http://www.regression.com";
            try
            {
                spoLib.CreateLinkLib(siteTitle, linkLibName);
               
                int itemID = spoLib.AddLinkItem(siteTitle, linkLibName, url);

                Hashtable fieldValues = new Hashtable();
                fieldValues["URL"] = url + ".cn";
                spoLib.UpdateItem(siteTitle, linkLibName, itemID, fieldValues);


                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreLinkNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Link", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, linkLibName, testDataPath + "Caml_RestoreLinkNewItem.txt", items.Count + 1);

                Assert.IsTrue(restoreSuccess, "Batch Restore link item as new item failed!");
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Contact")]
        [Category("Regression")]
        public void BatchRestoreContactNewItem()
        {
            string contactLibName = "Reg_RestoreContactNewItem";
            string lastname = "RegContact";

            spoLib.CreateContactLib(siteTitle, contactLibName);
           
            int itemID = spoLib.AddContactItem(siteTitle, contactLibName, lastname);

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreContactNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);


                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Contact", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, contactLibName, testDataPath + "Caml_RestoreContactNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Restore contact item as new item list failed!");
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Announcements")]
        [Category("Regression")]
        public void BatchRestoreAnnouncementNewItem()
        {
            string annoucementLibName = "Reg_RestoreAnnounceNewItem";
            string title = "Reg_Announcement";
            string body = "regression test";

            spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreAnnounceNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Announcement", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

                
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, annoucementLibName, testDataPath + "Caml_RestoreAnnounceNewItem.txt", items.Count + 1);
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one if the type is Folder")]
        [Category("Regression")]
        public void BatchRestoreDocumentNewItemUnderFolder()
        {
            string documentLibName = "Reg_RestoreDocumentNewItemUnderFolder";

            spoLib.CreateDocumentLib(siteTitle, documentLibName);
            spoLib.CreateFolderLevelByLevel(siteTitle, documentLibName, "Folder1");
            string itemTitle = "RestoreDocumentItemUnderFolder";
            int itemID = spoLib.AddFileForFolder(siteTitle, documentLibName, itemTitle, "Folder1", testDataPath, "RestoreDocumentItem.jpg", "", "");

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreDocumentNewItemUnderFolder.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

               bool restoreSuccess = spoLib.ValidateRestore(siteTitle, documentLibName, testDataPath + "Caml_RestoreDocumentNewItemUnderFolder.txt", items.Count + 1);
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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item exists if the type is Picture Library")]
        [Category("Regression")]
        public void BatchRestorePictureNewItem()
        {
            string pictureLibName = "Reg_RestorePictureNewItem";
            string title = "RegPicture";

            spoLib.CreatePictureLib(siteTitle, pictureLibName);
           
            int itemID = spoLib.UploadFile(siteTitle, pictureLibName, title, testDataPath, "Reg_Picture.jpg");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestorePictureNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);
                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Picture", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);

                restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

               
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, pictureLibName, testDataPath + "Caml_RestorePictureNewItem.txt", items.Count + 1);

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

        [Test, Description("As a Power User, I want to handle the conflict by creating new one with original item existed if the type is Document Lib")]
        [Category("Regression")]
        public void BatchRestoreDocumentNewItem()
        {
            string doclibName = "Reg_RestoreDocumentNewItem";
            string title = "RegDocument";
            try
            {
                spoLib.CreateDocumentLib(siteTitle, doclibName);
                
                int itemID = spoLib.UploadFile(siteTitle, doclibName, title, testDataPath, "RestoreDocumentItem.jpg");

                ES1Activity activity1 = ActivityFactory.CreateActivity(regressionActivtyPath + "Reg_RestoreDocumentNewItem.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items failed");
                Helper.WaitForIndex(Configuration.MCLocation);

                SearchUtil search = new SearchUtil(searchServiceURL, userName, password);
                List<RestoreItem> items = search.GetRestoreItems(SearchUtil.SearchField.SourceType, "Document", 1);
                int count = search.resultCount;

                SPPowerUserRestore restore = new SPPowerUserRestore(webAppURL, searchServiceURL, userName, password);
                int batchItemsCount = restore.Execute(ConflictResolution.CreateNewItem, items);
                restore.WaitForRestore();

               
                bool restoreSuccess = spoLib.ValidateRestore(siteTitle, doclibName, testDataPath + "Caml_RestoreDocumentNewItem.txt", items.Count + 1);
                Assert.IsTrue(restoreSuccess, "Batch Restore document item as new item failed!");
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
