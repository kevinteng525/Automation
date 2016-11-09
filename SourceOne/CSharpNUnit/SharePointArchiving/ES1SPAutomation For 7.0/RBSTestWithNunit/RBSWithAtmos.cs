using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharepointOnline;
using EMC.SourceOne.RBSProvider.ProviderConfig.RemoteStorage;
using System.IO;
using RBSLib;

namespace RBSTestWithNunit
{
    [TestFixture]
    public class RBSWithAtmos : Basic
    {
        //private string siteTitle = "QA Site";
        //private SPOLib spoLib = new SPOLib(@"SharePoint\SP2010sim.xml");
        //private string searchServiceURL = "http://es1all/SearchWs/ExSearchWebService.asmx";
        //private string userName = "es1service";
        //private string password = "emcsiax@QA";
        //private string webAppURL = "http://sp2010sim/";

        //private string atmosServer = "10.37.13.180";
        //private int atmosPort = 80;
        //private string uid = "yuk";
        //private string sharedSecret = "6NEXHFBExnNiLlq1I6/UYAT+0nE=";

        //private string sharePointServer = "kes1dev3";
        //private string sharePointDBName = "WSS_Content";

        //private string cacheLocation = @"\\kes1dev3\RBSData\EMCS1RBS\TestAtmos";
        //private string cacheUsername = @"kes1dev3\administrator";
        //private string cachePassword = "P@ssw0rd";
        //private string storeName = "TestStore";
        //private string siteTitle = "RBSTest";
        //private string libName = "RBSTest";
        private string connectionString
        {
            get
            {
                if (_rbsConfig != null)
                    return _rbsConfig.GetConnectionString(ContentDBName);
                else
                    return null;
            }
        }

        private RemoteStorageConnector connector;

        private string testDataPath = @"TestData\ItemSizeFilter\";
        protected string TestAtmosStoreName = @"AtmosTestStore";
        protected string ContentDBName = "ContentDB";
        protected new string _spConfigXML = @"SharePoint\SP2010sim.xml";
        private Store AtmosTestStore;
        private string siteTitle;
        private string libName;

        [TestFixtureSetUp]
        public void RBSWithAtmosSetup()
        {
            base.Setup(testDataPath, _spConfigXML);

            // create all stores we need in tests
            AtmosTestStore = _rbsConfig.GetStore(TestAtmosStoreName);
            Store[] list = new Store[] { AtmosTestStore };

            _baseLib.CreateStores(list);
            // ES1SPAgent create the directory by default.
    
            connector = new AtmosConnectorProvider(AtmosTestStore.AtmosServerUrl, AtmosTestStore.AtmosPort, AtmosTestStore.AtmosUid, AtmosTestStore.AtmosSharedSecret, AtmosTestStore.AtmosSubTenant);

            //connector.Initialize();
            //prepare a site

            //spo = new SPOLib(_spConfigXML);
            //spo.CreateDocumentLib(_rbsConfig.SiteTitle, _rbsConfig.ListTitle);
     
            siteTitle = _rbsConfig.SiteTitle;
            libName = _rbsConfig.ListTitle;
            Execute("cmd.exe", @"/c iisreset");
        }

        [SetUp]
        public void CaseSetup()
        {

        }

        [TestFixtureTearDown]
        public void RBSWithAtmosCleanUp()
        {
            //spo.DeleteLib(siteTitle, libName);
            base.TearDown();

            // delete all stores as need
            if (_rbsConfig.DeleteStores)
            {
                string[] list = new string[] { AtmosTestStore.StoreName };

                _baseLib.SelectStore("None");
                //Arguments too long... sometimes
                //_baseLib.DeleteStores(list.ToArray());
                foreach (string storeName in list)
                {
                    _baseLib.DeleteStore(storeName);
                }
            }

            // delete all stores' storage path as need
            if (_rbsConfig.DeleteStoreStoragePaths)
            {
                DeleteDirectory(AtmosTestStore.StorageLocation);
            }
        }

        private string UploadData(string filename)
        {
            string filepath = string.Format("{0}{1}", testDataPath, filename);
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            return connector.CreateObjectFromStream(fs, fs.Length);
        }

        private byte[] DownloadData(string id)
        {
            return connector.ReadObject(id);
        }

        private void CompareFileWithCloud(string filename, string id)
        {
            string filepath = string.Format("{0}{1}", testDataPath, filename);
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            Assert.AreEqual(fs.Length, connector.GetObjectSize(id));

            int buffSize = 1024;
            byte[] buff1 = new byte[buffSize];
            byte[] buff2 = new byte[buffSize];
            
            long total = fs.Length;
            int pos = 0;
            while (pos < total)
            {
                int bytesToRead = buffSize;
                if(total - pos < buffSize)
                    bytesToRead = (int)(total - pos);
                int count1 = fs.Read(buff1, 0, bytesToRead);
                buff2 = connector.ReadObject(id, pos, bytesToRead);
                pos += count1;

                Assert.IsTrue(CompareByteArray(buff1, buff2));
            }
            fs.Close();
        }

        private bool CompareByteArray(byte[] buff1, byte[] buff2)
        {
            int length1 = buff1.Length;
            int length2 = buff2.Length;
            int length = Math.Min(length1, length2);
            for (int i = 0; i < length; i++)
            {
                if(!byte.Equals(buff1[i], buff2[i]))
                    return false;
            }
            return true;
        }

        private void UploadCompareDelete(string filename)
        {
            string id = UploadData(filename);
            CompareFileWithCloud(filename, id);
            connector.DeleteObject(id);
            try
            {
                connector.GetObjectSize(id);
                Assert.Fail();
            }
            catch
            {
            }
        }

        private void UploadDelete(string filename)
        {
            string id = UploadData(filename);
            connector.GetObjectSize(id);
            connector.DeleteObject(id);
            try
            {
                connector.GetObjectSize(id);
                Assert.Fail();
            }
            catch
            {
            }
        }

        #region Extension
        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveExtDOC()
        {
            UploadCompareDelete("240kb.doc");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveExtPDF()
        {
            UploadCompareDelete("exported-pdf-p0002.pdf");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveExtJPG()
        {
            UploadCompareDelete("cruella.jpg");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveExtTIF()
        {
            UploadCompareDelete("Lines.tif");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveExtDOCX()
        {
            UploadCompareDelete("20.2kb.docx");
        }
        #endregion

        #region Size
        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveSize10k()
        {
            UploadCompareDelete("18.2kb.txt");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveSize1m()
        {
            UploadCompareDelete("highpointpoolrules.pdf");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyRetrieveSize10m()
        {
            UploadCompareDelete("tiff-p0003.tif");
        }

        //[Test, Description("Upload an item and download it.")]
        //[Category("Atmos")]
        //public void VerifyRetrieveSize100m()
        //{
        //    UploadCompareDelete("20.2kb.docx");
        //}

        //[Test, Description("Upload an item and download it.")]
        //[Category("Atmos")]
        //public void VerifyRetrieveSize1g()
        //{
        //    UploadCompareDelete("20.2kb.docx");
        //}
        #endregion

        #region Extension
        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteExtDOC()
        {
            UploadDelete("240kb.doc");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteExtPDF()
        {
            UploadDelete("exported-pdf-p0002.pdf");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteExtJPG()
        {
            UploadDelete("cruella.jpg");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteExtTIF()
        {
            UploadDelete("Lines.tif");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteExtDOCX()
        {
            UploadDelete("20.2kb.docx");
        }
        #endregion

        #region Size
        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteSize10k()
        {
            UploadDelete("18.2kb.txt");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteSize1m()
        {
            UploadDelete("highpointpoolrules.pdf");
        }

        [Test, Description("Upload an item and download it.")]
        [Category("Atmos")]
        public void VerifyDeleteSize10m()
        {
            UploadDelete("tiff-p0003.tif");
        }

        //[Test, Description("Upload an item and download it.")]
        //[Category("Atmos")]
        //public void VerifyDeleteSize100m()
        //{
        //    UploadDelete("20.2kb.docx");
        //}

        //[Test, Description("Upload an item and download it.")]
        //[Category("Atmos")]
        //public void VerifyDeleteSize1g()
        //{
        //    UploadDelete("20.2kb.docx");
        //}
        #endregion

        #region cache expiration
        [Test, Description("Check if an item exists in cache")]
        [Category("Cache Expiration")]
        public void VerifyItemInCacheBeforeExpiration()
        {
            SelectStore(AtmosTestStore.StoreName);
            SetThreshold(1);

            int id = _spoLib.UploadFile(siteTitle, libName, "VerifyItemInCacheBeforeExpiration", testDataPath, "20.2kb.docx");
            Assert.IsTrue(_spoLib.CheckItemExists(siteTitle, libName, id));

            _baseLib.RunSyncJob();
            Guid guid = new Guid(_spoLib.GetGUID(siteTitle, libName, id));

            string remote = string.Empty;
            string filePath = string.Empty;
            while (string.IsNullOrEmpty(remote))
            {
                System.Threading.Thread.Sleep(5000);
                System.Data.DataSet ds = _baseLib.GetBlobDetail(connectionString, guid);
                if (ds.Tables[0].Rows.Count == 0)
                    remote = string.Empty;
                else
                    remote = ds.Tables[0].Rows[0]["remote_blob_path"].ToString();
                filePath = ds.Tables[0].Rows[0]["blob_path"].ToString();
            }
            
            Assert.IsFalse(filePath.StartsWith("_REMOVED_"));
            Assert.IsTrue(File.Exists(filePath));
        }


        #endregion

        #region Retrieve
        [Test, Description("Delete cache and retrieve again")]
        [Category("Retrieve")]
        public void DeleteCacheAndRetrieveAgain()
        {

            SelectStore(AtmosTestStore.StoreName);
            SetThreshold(1);

            int id = _spoLib.UploadFile(siteTitle, libName, "DeleteCacheAndRetrieveAgain", testDataPath, "20.2kb.docx");
            Assert.IsTrue(_spoLib.CheckItemExists(siteTitle, libName, id));

            _baseLib.RunJob("EMC SourceOne Atmos Externalization Job");

            Guid guid = new Guid(_spoLib.GetGUID(siteTitle, libName, id));

            string remote = string.Empty;
            string filePath = string.Empty;
            while (string.IsNullOrEmpty(remote))
            {
                System.Threading.Thread.Sleep(1000 * 30);
                System.Data.DataSet ds = _baseLib.GetBlobDetail(connectionString, guid);
                if (ds.Tables[0].Rows.Count == 0)
                    remote = string.Empty;
                else
                    remote = ds.Tables[0].Rows[0]["remote_blob_path"].ToString();
                filePath = ds.Tables[0].Rows[0]["blob_path"].ToString();
            }

            if (filePath.StartsWith("_REMOVED_") == false)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }

            Stream stream = _spoLib.DownloadFile(siteTitle, libName, id);
            stream.Close();

            _spoLib.DeleteItem(siteTitle, libName, id);
            Assert.IsFalse(_spoLib.CheckItemExists(siteTitle, libName, id));
 
        }
        #endregion

        #region Delete
        [Test, Description("Check if an item deletes successfully")]
        [Category("Delete")]
        public void VerifyDeleteItemFromSharePoint()
        {

            SelectStore(AtmosTestStore.StoreName);
            SetThreshold(1);

            int id = _spoLib.UploadFile(siteTitle, libName, "VerifyDeleteItemFromSharePoint", testDataPath, @"20.2kb.docx");

            Assert.IsTrue(_spoLib.CheckItemExists(siteTitle, libName, id));

            _spoLib.DeleteItem(siteTitle, libName, id);
            Assert.IsFalse(_spoLib.CheckItemExists(siteTitle, libName, id));
        }

        [Test, Description("Delete cache first and then delete item from SharePoint")]
        [Category("Delete")]
        public void DeleteCacheThenDeleteFromSharePoint()
        {

            SelectStore(AtmosTestStore.StoreName);
            SetThreshold(1);

            int id = _spoLib.UploadFile(siteTitle, libName, "DeleteCacheThenDeleteFromSharePoint", testDataPath, @"20.2kb.docx");
            Assert.IsTrue(_spoLib.CheckItemExists(siteTitle, libName, id));

            Guid guid = new Guid(_spoLib.GetGUID(siteTitle, libName, id));
            System.Data.DataSet ds = _baseLib.GetBlobDetail(connectionString, guid);

            string filePath = ds.Tables[0].Rows[0]["blob_path"].ToString();

            try
            {
                File.Delete(filePath);
            }
            catch { }

            _spoLib.DeleteItem(siteTitle, libName, id);
            Assert.IsFalse(_spoLib.CheckItemExists(siteTitle, libName, id));
        }

        [Test, Description("Delete from atmos server and then delete from SharePoint")]
        [Category("Delete")]
        public void DeleteAtmosThenDeleteFromSharePoint()
        {

            SelectStore(AtmosTestStore.StoreName);
            SetThreshold(1);

            _baseLib.SetMinimumFileSize(1);
            int id = _spoLib.UploadFile(siteTitle, libName, "DeleteAtmosThenDeleteFromSharePoint", testDataPath, "495kb.doc");
            Assert.IsTrue(_spoLib.CheckItemExists(siteTitle, libName, id));

            Guid guid = new Guid(_spoLib.GetGUID(siteTitle, libName, id));

            // Run Atmos externalize job
            _baseLib.RunJob("EMC SourceOne Atmos Externalization Job");

            string remote = string.Empty;
            while (string.IsNullOrEmpty(remote))
            {
                System.Threading.Thread.Sleep(1000*60);
                System.Data.DataSet ds = _baseLib.GetBlobDetail(connectionString, guid);
                if (ds.Tables[0].Rows.Count == 0 )
                    remote = string.Empty;
                else
                    remote = ds.Tables[0].Rows[0]["remote_blob_path"].ToString();
            }
            this.connector.DeleteObject(remote);

            _spoLib.DeleteItem(siteTitle, libName, id);
            Assert.IsFalse(_spoLib.CheckItemExists(siteTitle, libName, id));
        }
        #endregion
    }
}

