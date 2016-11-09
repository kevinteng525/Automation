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

namespace SPTestWithNunit
{
    [TestFixture]
    public class AgedTest
    {
        private string testDataPath = @"TestData\Aged\";
        private string activtyPath = @"TestActivities\Aged\";
        private string siteTitle = "QA Site";
        private string listTitle = "AddedByAgedAuto";
        private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");

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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 day and 0 day!")]
        [Category("DateFilter")]
        public void ACDBetween0dAnd0d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i).ToString(), DateTime.Now.AddDays(-i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0dAnd0d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(0, activity1.archivedItemCount, "Archive items the created date between 0 day and 0 day failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 day and 1 day!")]
        [Category("DateFilter")]
        public void ACDBetween0dAnd1d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i).ToString(), DateTime.Now.AddDays(-i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0dAnd1d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items the created date between 0 day and 1 day failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 day and 100 days!")]
        [Category("DateFilter")]
        public void ACDBetween0dAnd100d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i*30).ToString(), DateTime.Now.AddDays(-i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0dAnd100d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 0 day and 100 days failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 day and 1000 days!")]
        [Category("DateFilter")]
        public void ACDBetween0dAnd1000d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 300).ToString(), DateTime.Now.AddDays(-i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0dAnd1000d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 0 day and 1000 days failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 day and 4 weeks!")]
        [Category("DateFilter")]
        public void ACDBetween0dAnd4w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 6).ToString(), DateTime.Now.AddDays(-i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0dAnd4w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(5, activity1.archivedItemCount, "Archive items the created date between 0 day and 4 weeks failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 month and 0 month!")]
        [Category("DateFilter")]
        public void ACDBetween0mAnd0m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i*30).ToString(), DateTime.Now.ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0mAnd0m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(0, activity1.archivedItemCount, "Archive items the created date between 0 month and 0 month failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 month and 100 months!")]
        [Category("DateFilter")]
        public void ACDBetween0mAnd100m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 1100).ToString(), DateTime.Now.AddDays(-i * 1100).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0mAnd100m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date between 0 month and 100 months failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 week and 1 week!")]
        [Category("DateFilter")]
        public void ACDBetween0wAnd1w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 2).ToString(), DateTime.Now.AddDays(-i * 2).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0wAnd1w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 0 week and 1 week failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 0 month and 12 months!")]
        [Category("DateFilter")]
        public void ACDBetween0mAnd12m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 100).ToString(), DateTime.Now.AddDays(-i * 100).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween0mAnd12m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 0 month and 12 months failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date between 1 year and 10 years!")]
        [Category("DateFilter")]
        public void ACDBetween1yAnd10y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 365*3).ToString(), DateTime.Now.AddDays(-i * 365*3).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDBetween1yAnd10y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date between 1 year and 10 years failed!");
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

        [Test, Description("Test a SP activity can be run to archive items created date newer than 0 day!")]
        [Category("DateFilter")]
        public void ACDNewerThan0d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 10).ToString(), DateTime.Now.AddDays(-i * 10).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDNewerThan0d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(0, activity1.archivedItemCount, "Archive items the created date newer than 0 day!");
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

        [Test, Description("Test a SP activity can be run to archive items created date newer than 10 weeks!")]
        [Category("AgedCreatedDateNewerThan")]
        public void ACDNewerThan10w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 20).ToString(), DateTime.Now.AddDays(-i * 20).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDNewerThan10w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date newer than 10 weeks!");
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

        [Test, Description("Test a SP activity can be run to archive items created date newer than 12 months!")]
        [Category("AgedCreatedDateNewerThan")]
        public void ACDNewerThan12m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 150).ToString(), DateTime.Now.AddDays(-i * 150).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDNewerThan12m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date newer than 12 months!");
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

        [Test, Description("Test a SP activity can be run to archive items created date newer than 2 years!")]
        [Category("AgedCreatedDateNewerThan")]
        public void ACDNewerThan2y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 360).ToString(), DateTime.Now.AddDays(-i * 360).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDNewerThan2y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date newer than 2 years!");
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

        [Test, Description("Test a SP activity can be run to archive items created date older than 0 week!")]
        [Category("AgedCreatedDateOlderThan")]
        public void ACDOlderThan0w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 100).ToString(), DateTime.Now.AddDays(-i * 100).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDOlderThan0w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(6, activity1.archivedItemCount, "Archive items the created date older than 0 week!");
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

        [Test, Description("Test a SP activity can be run to archive items created date older than 100 days!")]
        [Category("AgedCreatedDateOlderThan")]
        public void ACDOlderThan100d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 70).ToString(), DateTime.Now.AddDays(-i * 70).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDOlderThan100d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date older than 100 days!");
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

        [Test, Description("Test a SP activity can be run to archive items created date older than 4 months!")]
        [Category("AgedCreatedDateOlderThan")]
        public void ACDOlderThan4m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 50).ToString(), DateTime.Now.AddDays(-i * 50).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDOlderThan4m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date older than 4 months!");
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

        [Test, Description("Test a SP activity can be run to archive items created date older than 1 year!")]
        [Category("AgedCreatedDateOlderThan")]
        public void ACDOlderThan1y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 300).ToString(), DateTime.Now.AddDays(-i * 300).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ACDOlderThan1y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date older than 1 year!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date between 1 day and 100 days!")]
        [Category("AgedModifiedDateBetween")]
        public void AMDBetween1dAnd100d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30).ToString(), DateTime.Now.AddDays(-i * 30).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDBetween1dAnd100d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date between 1 day and 100 days!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date between 1 week and 52 weeks!")]
        [Category("AgedModifiedDateBetween")]
        public void AMDBetween1wAnd52w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 7 * 20).ToString(), DateTime.Now.AddDays(-i * 7 * 20).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDBetween1wAnd52w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items the modified date between 1 week and 52 weeks!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date between 12 month and 100 months!")]
        [Category("AgedModifiedDateBetween")]
        public void AMDBetween12mAnd100m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30 * 30).ToString(), DateTime.Now.AddDays(-i * 30 * 30).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDBetween12mAnd100m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date between 12 month and 100 months!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date between 2 years and 100 months!")]
        [Category("AgedModifiedDateBetween")]
        public void AMDBetween2yAnd100m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30 * 30).ToString(), DateTime.Now.AddDays(-i * 30 * 30).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDBetween2yAnd100m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date between 2 years and 100 months!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date between 1 year and 10 years!")]
        [Category("AgedModifiedDateBetween")]
        public void AMDBetween1yAnd10y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 365 * 3).ToString(), DateTime.Now.AddDays(-i * 365 * 3).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDBetween1yAnd10y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date between 1 year and 10 years!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date newer than 0 week!")]
        [Category("AgedModifiedDateNewer")]
        public void AMDNewerThan0w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 7 * 3).ToString(), DateTime.Now.AddDays(-i * 7 * 3).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDNewerThan0w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(0, activity1.archivedItemCount, "Archive items the modified date newer than 0 week!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date newer than 50 days!")]
        [Category("AgedModifiedDateNewer")]
        public void AMDNewerThan50d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 15).ToString(), DateTime.Now.AddDays(-i * 15).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDNewerThan50d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date newer than 50 days!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date newer than 15 months!")]
        [Category("AgedModifiedDateNewer")]
        public void AMDNewerThan15m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30 * 4).ToString(), DateTime.Now.AddDays(-i * 30 * 4).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDNewerThan15m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date newer than 15 months!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date newer than 9 years!")]
        [Category("AgedModifiedDateNewer")]
        public void AMDNewerThan9y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 365 * 4).ToString(), DateTime.Now.AddDays(-i * 365 * 4).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDNewerThan9y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date newer than 9 years!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date older than 0 years!")]
        [Category("AgedModifiedDateOlder")]
        public void AMDOlderThan0y()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 365 * 4).ToString(), DateTime.Now.AddDays(-i * 365 * 4).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDOlderThan0y.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(6, activity1.archivedItemCount, "Archive items the modified date older than 0 years!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date older than 20 days!")]
        [Category("AgedModifiedDateOlder")]
        public void AMDOlderThan20d()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30).ToString(), DateTime.Now.AddDays(-i * 30).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDOlderThan20d.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(5, activity1.archivedItemCount, "Archive items the modified date older than 20 days!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date older than 48 weeks!")]
        [Category("AgedModifiedDateOlder")]
        public void AMDOlderThan48w()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 7 * 40).ToString(), DateTime.Now.AddDays(-i * 7 * 40).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDOlderThan48w.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date older than 48 weeks!");
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

        [Test, Description("Test a SP activity can be run to archive items modified date older than 30 months!")]
        [Category("AgedModifiedDateOlder")]
        public void AMDOlderThan30m()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt"}
                                                    ,{"XlsAttachment", "XlsAttach.xls"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx"}
                                                };
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.Now.AddDays(-i * 30 * 20).ToString(), DateTime.Now.AddDays(-i * 30 * 20).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "AMDOlderThan30m.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date older than 30 months!");
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
