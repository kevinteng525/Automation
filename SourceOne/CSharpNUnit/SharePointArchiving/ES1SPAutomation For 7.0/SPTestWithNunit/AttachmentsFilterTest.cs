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
    public class AttachmentFilterTest
    {
        private string testDataPath = @"TestData\AttachmentsFilter\";
        private string activtyPath = @"TestActivities\AttachmentsFilter\";
        private string siteTitle = "QA Site";
        private string listTitle = "AddedByAttachmentsFilterAuto";
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

        [Test, Description("Test a SP activity can be run to exclude doc type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDoc()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDoc.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(5, activity1.archivedItemCount, "Archive items exclude doc type failed!");
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

        [Test, Description("Test a SP activity can be run to exclude doc and docx type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDocDocx()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDocDocx.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items exclude doc and docx type failed!");
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

        [Test, Description("Test a SP activity can be run to exclude doc, docx and jpg type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDocDocxJpg()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDocDocxJpg.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items exclude doc, docx and jpg type failed!");
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

        [Test, Description("Test a SP activity can be run to exclude doc, docx, jpg and txt type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDocDocxJpgTxt()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDocDocxJpgTxt.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items exclude doc, docx, jpg and txt type failed!");
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

        [Test, Description("Test a SP activity can be run to exclude doc, docx, jpg, txt and xls type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDocDocxJpgTxtXls()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDocDocxJpgTxtXls.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items exclude doc, docx, jpg, txt and xls type failed!");
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

        [Test, Description("Test a SP activity can be run to exclude docx, jpg, txt and xls type!")]
        [Category("AttachmentsFilter")]
        public void ExcludeDocxJpgTxtXls()
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
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1]);
            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "ExcludeDocxJpgTxtXls.xml", ActivityTypes.Sharepoint);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items exclude docx, jpg, txt and xls type failed!");
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
