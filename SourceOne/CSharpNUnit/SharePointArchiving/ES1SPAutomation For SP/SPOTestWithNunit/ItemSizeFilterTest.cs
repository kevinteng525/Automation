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
    public class ItemSizeFilterTest
    {
        private string testDataPath = @"TestData\ItemSizeFilter\";
        private string activtyPath = @"TestActivities\ItemSizeFilter\";
        private string siteTitle = "root";
        private string listTitle = "AddedByItemSizeFilterAuto";
        private SPOLib spoLib = new SPOLib(@"SharePointOnline\SPOnlinesim.xml");

        [TestFixtureSetUp]
        public void Setup()
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
        public void CleanUp()
        {
            spoLib.DeleteLib(siteTitle, listTitle);
        }

        [Test, Description("Test a SPO activity can be run to archve items which are larger or eauql 50 and less or equal 500!")]
        [Category("ItemSizeFilter")]
        public void SizeDown50Up500()
        {
            //50~500 actually is 48.9~488.3
            string[,] itemTitle = new string[,] {
                                                    {"20.2kb", "20.2kb.docx"}       //n
                                                    ,{"49.4kb", "49.4kb.xlsx"}      //y
                                                    ,{"240kb", "240kb.doc"}         //y
                                                    ,{"487kb", "487kb.doc"}         //y
                                                    ,{"495kb", "495kb.doc"}         //n
                                                };
            int[] itemID = new int[10];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "SizeDown50Up500.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items which are larger or eauql 50 and less or equal 500 failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                for (int i = 0; i < itemTitle.GetLength(0); i++)
                {
                    spoLib.DeleteItem(siteTitle, listTitle, itemID[i]);
                }
            }
        }

        [Test, Description("Test a SPO activity can be run to archve items which are larger or eauql 100!")]
        [Category("ItemSizeFilter")]
        public void SizeDown100()
        {
            //100 actually is 97.67
            string[,] itemTitle = new string[,] {
                                                    {"49.4kb", "49.4kb.xlsx"}        //n
                                                    ,{"92.8kb", "92.8kb.xlsx"}       //n
                                                    ,{"98.5kb", "98.5kb.xlsx"}       //y
                                                    ,{"240kb", "240kb.doc"}          //y
                                                };
            int[] itemID = new int[10];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "SizeDown100.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items which are larger or eauql 100 failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                for (int i = 0; i < itemTitle.GetLength(0); i++)
                {
                    spoLib.DeleteItem(siteTitle, listTitle, itemID[i]);
                }
            }
        }


        [Test, Description("Test a SPO activity can be run to archve items which are less or eauql 1000!")]
        [Category("ItemSizeFilter")]
        public void SizeUp20()
        {
            //20 actually is 19.53
            string[,] itemTitle = new string[,] {
                                                    {"6kb", "6kb.txt"}              //y
                                                    ,{"18.2kb", "18.2kb.txt"}       //y
                                                    ,{"20.2kb", "20.2kb.docx"}      //n
                                                    ,{"200kb", "200kb.xls"}         //n
                                                };
            int[] itemID = new int[10];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "SizeUp20.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items which are less or eauql 1000 failed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                for (int i = 0; i < itemTitle.GetLength(0); i++)
                {
                    spoLib.DeleteItem(siteTitle, listTitle, itemID[i]);
                }
            }
        }

    }
}
