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

namespace SPOTestWithNunit
{
    [TestFixture] 
    public class BVTTest
    {
        private string testDataPath = @"TestData\BVT\";
        private string bvtActivtyPath = @"TestActivities\BVT\";
        private string siteTitle = "root";
        private string listTitle = "AddedByBVTAuto";
        private SPOLib spoLib = new SPOLib(@"SharePointOnline\SPOnlinesim.xml");

        [TestFixtureSetUp]
        public void BVTSetup()
        {
            Configuration.FillInValues("Configuration.xml");
            Helper.ConfigES1();
            spoLib.CreateDocumentLib(siteTitle, listTitle);
             
        }

        [SetUp]
        public void CaseSetup()
        {

        }

        [TestFixtureTearDown]
        public void BVTCleanUp()
        {
            spoLib.DeleteLib(siteTitle, listTitle);
        }

        [Test, Description("Test a daily recursive activity can be run and archive successfully!")]
        [Category("BVT")]
        public void ArchiveScheduledDaily()
        {
            string itemTitle = "ScheduledDaily";
            int itemID = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ScheduledDaily.txt");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath+"BVT_ArchiveScheduleDaily.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                Task task = activity1.GetScheduledTask(10);
                String trigger = task.Triggers[0].ToString();
                Assert.AreEqual("At 12:56 AM every day, starting 11/16/2011 and ending 1/1/2038", trigger, "Trigger of the schedule job is not correct!");
                task.Run();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Scheduled Daily failed");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, listTitle, itemID);
            }
        }

        [Test, Description("Test a SPO activity can be run and archive successfully!")]
        [Category("BVT")]
        public void ArchiveItemASAP()
        {
            string itemTitle = "ItemASAP";
            int itemID = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ItemASAP.jpg");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveItemASAP.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items ASAP failed");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, listTitle, itemID);
            }
        }

        [Test, Description("Test a recursive activity can be run and archive modified item successfully!")]
        [Category("BVT")]
        public void ArchiveModifiedItem()
        {
            string itemTitle = "ModifiedItem";
            int itemID = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ModifiedItem.log");
            try
            {

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveModifiedItem.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                Task task = activity1.GetScheduledTask(10);
                task.Run();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Original item failed!");

                spoLib.ModifyItem(siteTitle, listTitle, itemID);
                task.Run();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Modified item failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, listTitle, itemID);
            }
        }

        [Test, Description("Test a SPO activity with MOVE action can be run and archive successfully!")]
        [Category("BVT")]
        public void ArchiveMoveItem()
        {
            string itemTitle = "MoveItem";
            int itemID = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "MoveItem.xlsx");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveMoveItem.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);
                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Move item failed!");
                Assert.AreEqual(0, spoLib.GetItemCount(siteTitle, listTitle), "Item on Sharepoint side didn't removed successfully!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, listTitle, itemID);
            }
        }

        [Test, Description("Test a SPO activity can archive items on multiple sites!")]
        [Category("BVT")]
        public void ArchiveMultiSites()
        {
            string itemTitle = "ItemMultiSites";
            int itemID1 = spoLib.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ItemMultiSites.doc");
            SPOLib spoLib888 = new SPOLib(@"SharePointOnline\SPOnlinesim888.xml");
            spoLib888.CreateDocumentLib(siteTitle, listTitle);
            int itemID2 = spoLib888.UploadFile(siteTitle, listTitle, itemTitle, testDataPath, "ItemMultiSites.doc");
            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveMultiSites.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive Items from Multi Sites failed");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteItem(siteTitle, listTitle, itemID1);
                spoLib888.DeleteItem(siteTitle, listTitle, itemID2);
                spoLib888.DeleteLib(siteTitle, listTitle);
            }
        }

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveAnnouncement!")]
        [Category("BVT")]
        public void ArchiveAnnouncement()
        {
            string annoucementLibName = "AnnouncementForBVT";
            string title = "BVT";
            string body = "BVT test";
            
            Microsoft.SharePoint.Client.List annoucement = spoLib.CreateAnnouncementLib(siteTitle, annoucementLibName);
            try
            {
                int itemID = spoLib.AddAnnouncementItem(siteTitle, annoucementLibName, title, body);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveAnnouncement.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Announcement item failed");
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

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveTask!")]
        [Category("BVT")]
        public void ArchiveTask()
        {
            string taskLibName = "TaskForBVT";
            string title = "BVT";

            Microsoft.SharePoint.Client.List task = spoLib.CreateTaskLib(siteTitle, taskLibName);
            try
            {
                int itemID = spoLib.AddTaskItem(siteTitle, taskLibName, title);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveTask.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Task item failed");
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

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveLink!")]
        [Category("BVT")]
        public void ArchiveLink()
        {
            string linkLibName = "LinkForBVT";
            string url = "http://bvt.com";

            Microsoft.SharePoint.Client.List link = spoLib.CreateLinkLib(siteTitle, linkLibName);
            try
            {
                int itemID = spoLib.AddLinkItem(siteTitle, linkLibName, url);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveLink.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Link item failed");
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

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveDiscussionBoard!")]
        [Category("BVT")]
        public void ArchiveDiscussionBoard()
        {
            string discussionBoardLibName = "DiscussionBoardForBVT";
            string subject = "BVT";
            string body = "BVT test";

            Microsoft.SharePoint.Client.List disscussionBoard = spoLib.CreateDiscussionBoardLib(siteTitle, discussionBoardLibName);
            try
            {
                int itemID = spoLib.AddDiscussionBoardItem(siteTitle, discussionBoardLibName, subject, body);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveDiscussionBoard.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Disscussion Board item failed");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                spoLib.DeleteLib(siteTitle, discussionBoardLibName);
            }
        }

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveContact!")]
        [Category("BVT")]
        public void ArchiveContact()
        {
            string contactLibName = "ContactForBVT";
            string lastname = "BVT";

            Microsoft.SharePoint.Client.List contact = spoLib.CreateContactLib(siteTitle, contactLibName);
            try
            {
                int itemID = spoLib.AddContactItem(siteTitle, contactLibName, lastname);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveContact.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Contact item failed");
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

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchiveEvent!")]
        [Category("BVT")]
        public void ArchiveEvent()
        {
            string eventLibName = "EventCalenderForBVT";
            string title = "BVT";

            Microsoft.SharePoint.Client.List eventCalender = spoLib.CreateEventLib(siteTitle, eventLibName);
            try
            {
                int itemID = spoLib.AddEventItem(siteTitle, eventLibName, title);

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchiveEvent.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Event item failed");
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

        [Test, Description("Test a SPO activity can archive other type list, e.g. ArchivePictureLibrary!")]
        [Category("BVT")]
        public void ArchivePictureLibrary()
        {
            string pictureLibName = "PictureLibraryForBVT";
            string title = "BVT";

            Microsoft.SharePoint.Client.List pictureLibrary = spoLib.CreatePictureLib(siteTitle, pictureLibName);
            try
            {
                int itemID = spoLib.UploadFile(siteTitle, pictureLibName, title, testDataPath, "PictureLib.jpg");

                ES1Activity activity1 = ActivityFactory.CreateActivity(bvtActivtyPath + "BVT_ArchivePicture.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive Picture Library item failed");
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

    }
}
