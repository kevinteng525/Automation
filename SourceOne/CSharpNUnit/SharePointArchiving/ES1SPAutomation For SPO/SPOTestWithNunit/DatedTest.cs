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
    public class DatedTest
    {
        private string testDataPath = @"TestData\Dated\";
        private string activtyPath = @"TestActivities\Dated\";
        private string siteTitle = "root";
        private string listTitle = "AddedByDatedAuto";
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

        [Test, Description("Test a SPO activity can be run to archive items created date after 12/16/2009")]
        [Category("DatedCreatedDateAfter")]
        public void DCDAfter12162009()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/16/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/15/2010"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "12/17/2009"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "12/16/2009"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "12/15/2009"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "12/16/2008"}
                                                };
            
            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDAfter12162009.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date after 12/16/2009 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date after 02/28/1983")]
        [Category("DatedCreatedDateAfter")]
        public void DCDAfter02281983()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "02/28/1983"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "02/28/1983"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "03/01/1983"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "02/28/1990"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDAfter02281983.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items the created date after 02/28/1983 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date before 12/19/1999")]
        [Category("DatedCreatedDateBefore")]
        public void DCDBefore12191999()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/28/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/20/1999"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "12/19/1999"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "12/18/1999"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDBefore12191999.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date before 12/19/1999 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date before 07/09/2008")]
        [Category("DatedCreatedDateBefore")]
        public void DCDBefore07092008()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/28/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "07/10/2008"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/09/2008"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/08/2008"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDBefore07092008.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the created date before 07/09/2008 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date between 02/27/2010 and 06/07/2100")]
        [Category("DatedCreatedDateBetween")]
        public void DCDBetween02272010And06072100()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "02/27/2010"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "02/27/2010"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "02/27/2010"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "02/28/2010"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "06/07/2100"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "12/07/2100"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDBetween02272010And06072100.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 02/27/2010 and 06/07/2100 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date between 07/03/2009 and 07/03/2009")]
        [Category("DatedCreatedDateBetween")]
        public void DCDBetween07032009And07032009()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "07/03/2009"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "07/03/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/03/2009"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/03/2009"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "07/03/2009"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDBetween07032009And07032009.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the created date between 07/03/2009 and 07/03/2009 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date exactly on 12/28/2009")]
        [Category("DatedCreatedDateExactlyOn")]
        public void DCDExactlyOn12282009()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "06/09/2100"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/28/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/09/2008"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/08/2008"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDExactlyOn12282009.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items the created date exactly on 12/28/2009 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date exactly on 06/09/2100")]
        [Category("DatedCreatedDateExactlyOn")]
        public void DCDExactlyOn06092100()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "06/09/2100"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/28/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/09/2008"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/08/2008"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDExactlyOn06092100.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(1, activity1.archivedItemCount, "Archive items the created date exactly on 06/09/2100 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items created date include all")]
        [Category("DatedIncludeAll")]
        public void DCDIncludeAll()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "06/09/2100"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/30/2011"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/09/2008"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/08/2008"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DCDIncludeAll.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(5, activity1.archivedItemCount, "Archive items the created date include all failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date after 01/02/1991")]
        [Category("DatedModifiedDateAfter")]
        public void DMDAfter01021991()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/16/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/15/2010"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "12/17/2009"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "01/02/1991"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "01/01/1991"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "01/16/1930"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDAfter01021991.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date after 01/02/1991 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date after 07/14/1983")]
        [Category("DatedModifiedDateAfter")]
        public void DMDAfter07141983()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "07/14/1983"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "07/14/1983"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/15/1983"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "10/14/1990"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "07/13/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "01/16/1930"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDAfter07141983.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items the modified date after 07/14/1983 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date before 12/21/2009")]
        [Category("DatedModifiedDateBefore")]
        public void DMDBefore12212009()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/28/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/21/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "12/20/2009"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "12/18/1999"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDBefore12212009.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date before 12/21/2009 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date before 03/05/1983")]
        [Category("DatedModifiedDateBefore")]
        public void DMDBefore03051983()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "12/28/2011"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "12/21/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "03/05/1983"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "03/04/1983"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDBefore03051983.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date before 03/05/1983 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date between 05/16/1983 and 12/24/1999")]
        [Category("DatedModifiedDateBetween")]
        public void DMDBetween05161983And12241999()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "05/16/1983"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "05/16/1983"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "03/05/1990"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "05/17/1990"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "12/24/1999"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/2000"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDBetween05161983And12241999.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(4, activity1.archivedItemCount, "Archive items the modified date between 05/16/1983 and 12/24/1999 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date between 07/08/2009 and 07/08/2009")]
        [Category("DatedModifiedDateBetween")]
        public void DMDBetween07082009And07082009()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "07/08/2009"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "07/08/2009"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "07/08/2009"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "07/09/2009"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "12/24/2010"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/2100"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDBetween07082009And07082009.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(2, activity1.archivedItemCount, "Archive items the modified date between 07/08/2009 and 07/08/2009 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date exactly on 01/02/2010")]
        [Category("DatedModifiedDateExactlyOn")]
        public void DMDExactlyOn01022010()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "01/02/2010"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "01/02/2010"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "01/02/2010"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "03/04/1983"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/1980"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDExactlyOn01022010.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date exactly on 01/02/2010 failed!");
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

        [Test, Description("Test a SPO activity can be run to archive items modified date exactly on 06/14/2100")]
        [Category("DatedModifiedDateExactlyOn")]
        public void DMDExactlyOn06142100()
        {
            string[,] itemTitle = new string[,] {
                                                    {"DocAttachment", "DocAttach.doc", "06/14/2100"} 
                                                    ,{"DocxAttachment", "DocxAttach.docx", "06/14/2100"}
                                                    ,{"JpgAttachment", "JpgAttach.jpg", "06/14/2100"} 
                                                    ,{"TxtAttachment", "TxtAttach.txt", "03/04/1983"}
                                                    ,{"XlsAttachment", "XlsAttach.xls", "02/27/1983"}
                                                    ,{"PptxAttachment", "PptxAttach.pptx", "02/28/2101"}
                                                };

            int[] itemID = new int[6];

            for (int i = 0; i < itemTitle.GetLength(0); i++)
            {
                itemID[i] = spoLib.UploadFile(siteTitle, listTitle, itemTitle[i, 0], testDataPath, itemTitle[i, 1], DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString(), DateTime.ParseExact(itemTitle[i, 2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddHours(i).ToString());

            }

            try
            {
                ES1Activity activity1 = ActivityFactory.CreateActivity(activtyPath + "DMDExactlyOn06142100.xml", ActivityTypes.SharepointOnline);
                activity1.Create();
                activity1.Run(600);

                Assert.AreEqual(3, activity1.archivedItemCount, "Archive items the modified date exactly on 06/14/2100 failed!");
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
