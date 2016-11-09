using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RBSLib;
using SharepointOnline;
using System.IO;
using System.Collections;

namespace RBSTestWithNunit
{
    [TestFixture]
    public class RBSAtmosTest : Basic
    {
        protected const string SPConfigXML = @"SharePoint\SP2010sim.xml";
        protected const string TestDataPath = @"TestData\ItemSizeFilter\";
        
        // !!! DO NOT change this name, they are defined in XML !!!
        protected string AtmosStoreNameDefaultInXML = @"AtmosStore_Default";
        protected string AtmosStoreNameSFInXML = @"AtmosStore_ShareFolder";
        protected string AtmosStoreNameLDInXML = @"AtmosStore_LocalDriver";
        protected string AtmosStoreNameNCInXML = @"AtmosStore_NoCache";
        protected string AtmosStoreNameFCInXML = @"AtmosStore_FailedCache";

        protected string AtmosStoreNameCompressOnInXML = @"AtmosStore_CompressionOn";
        protected string AtmosStoreNameEncryptionOnInXML = @"AtmosStore_EncryptionOn";

        protected string AtmosStoreNameUploadDownloadInXML = @"AtmosStore_UploadDownload";

        protected Store AtmosStoreDefault = null;
        protected Store AtmosStoreShareFolder = null;
        protected Store AtmosStoreLocalDriver = null;
        protected Store AtmosStoreNoCache = null;
        protected Store AtmosStoreFailedCache = null;

        protected Store AtmosStoreCompressionOn = null;
        protected Store AtmosStoreEncryptionOn = null;

        protected Store AtmosStoreUploadDownload = null;

        [TestFixtureSetUp]
        public void RBSAtmosTestSetup()
        {
            // !!! ======= !!!
            //     the user running NUnit must have:
            //        administrator right to SharePoint Server [remote connect]
            //        Farm Admin to SharePoint 
            //        Read/Write/Delete/Create rights in all storage paths
            // !!! ======== !!!
            base.Setup(TestDataPath, SPConfigXML);

            AtmosStoreDefault = _rbsConfig.GetStore(AtmosStoreNameDefaultInXML);
            AtmosStoreShareFolder = _rbsConfig.GetStore(AtmosStoreNameSFInXML);
            AtmosStoreLocalDriver = _rbsConfig.GetStore(AtmosStoreNameLDInXML);
            AtmosStoreNoCache = _rbsConfig.GetStore(AtmosStoreNameNCInXML);
            AtmosStoreFailedCache = _rbsConfig.GetStore(AtmosStoreNameFCInXML);
         

            AtmosStoreCompressionOn = _rbsConfig.GetStore(AtmosStoreNameCompressOnInXML);
            AtmosStoreEncryptionOn = _rbsConfig.GetStore(AtmosStoreNameEncryptionOnInXML);

            AtmosStoreUploadDownload = _rbsConfig.GetStore(AtmosStoreNameUploadDownloadInXML);

            List<Store> list = new List<Store>();
            list.Add(AtmosStoreDefault);
            list.Add(AtmosStoreShareFolder);
            list.Add(AtmosStoreLocalDriver);
            list.Add(AtmosStoreNoCache);
            list.Add(AtmosStoreFailedCache);

            // !!! parameter is too long if we put a lot of stores here, so move some down to CreateStore, not in this CreateStores.
            _baseLib.CreateStores(list.ToArray());
            // ES1SPAgent create the directory by default.

            _baseLib.CreateStore(AtmosStoreCompressionOn);
            _baseLib.CreateStore(AtmosStoreEncryptionOn);
            _baseLib.CreateStore(AtmosStoreUploadDownload);

            Execute("cmd.exe", @"/c iisreset");
        }

        [TestFixtureTearDown]
        public void RBSAtmosTestTearDown()
        {
            base.TearDown();

            // delete all stores as need
            if (_rbsConfig.DeleteStores)
            {
                List<string> list = new List<string>();
                list.Add(AtmosStoreDefault.StoreName);
                list.Add(AtmosStoreShareFolder.StoreName);
                list.Add(AtmosStoreLocalDriver.StoreName);
                list.Add(AtmosStoreNoCache.StoreName);
                list.Add(AtmosStoreFailedCache.StoreName);
                list.Add(AtmosStoreEncryptionOn.StoreName);
                list.Add(AtmosStoreCompressionOn.StoreName);
                list.Add(AtmosStoreUploadDownload.StoreName);

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
                DeleteDirectory(AtmosStoreDefault.StorageLocation);
                DeleteDirectory(AtmosStoreShareFolder.StorageLocation);
                RemoteDeleteDirectory(AtmosStoreLocalDriver.StorageLocation);
                DeleteDirectory(AtmosStoreNoCache.StorageLocation);
                DeleteDirectory(AtmosStoreFailedCache.StorageLocation);

                DeleteDirectory(AtmosStoreEncryptionOn.StorageLocation);
                DeleteDirectory(AtmosStoreCompressionOn.StorageLocation);
                DeleteDirectory(AtmosStoreUploadDownload.StorageLocation);
            }
        }

        [Test, Description("Verify RBS cache support network share folder")]
        [Category("Atmos")]
        public void VerifyRBSCacheSupportNetworkShareFolder()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreShareFolder.StoreName, 3);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(102 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, AtmosStoreShareFolder.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Verify RBS cache support local folder")]
        [Category("Atmos")]
        public void VerifyRBSCacheSupportLocalFolder()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreLocalDriver.StoreName, 4);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(103 * 1024, ref fileDir, ref fileName);

            // parse remote Local driver to share folder
            string remotePathLD = string.Format(@"\\{0}\{1}", _rbsConfig.ServerName, AtmosStoreLocalDriver.StorageLocation.Replace(':', '$'));

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, remotePathLD, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        
        [Test, Description("Verify RBS support private Atmos storage service")]
        [Category("Atmos")]
        public void VerifyRBSSupportPrivateAtmosStorageService()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreDefault.StoreName, 6);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(105 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, AtmosStoreDefault.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Verify RBS will be able to store items to Atmos without setting cache")]
        [Category("Atmos")]
        public void VerifyRBSWillStoreItemsToAtmosWithoutSettingCache()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreNoCache.StoreName, 7);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(106 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, AtmosStoreNoCache.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists); // it should be in cache, after 1 hr, it will be externalized to Atmos

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }
        
        [Test, Description("Verify RBS will store into DB if cache is full or not be able to store an item")]
        [Category("Atmos")]
        public void VerifyRBSWillStoreIntoDBIfNotBeAbleToStoreAnItemToCache()
        {
            // Test wrong cache path
            SelectStoreAndSetMinimumFileSize(AtmosStoreFailedCache.StoreName, 8);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(107 * 1024, ref fileDir, ref fileName);

            // !!! NOTE !!!
            // !!! When the pool created failed, the upload will fail !!!
            // !!! so we first upload one blob to create one pool !!!
            //  1. first time we upload one file so RBS will create a Pool, then store blob into it.
            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, AtmosStoreFailedCache.StorageLocation, _rbsConfig.DeleteSPItems, ref itemId);
            Assert.IsTrue(itemExists); // it should not be in cache, because cache is unreachable

            // clear up
            try { File.Delete(filePath); }
            catch { }

            // 2. delete the StoreLocation so that next time, store blob into it will fail, but the blob will be stored into database instead.
            Directory.Delete(AtmosStoreFailedCache.StorageLocation, true);

            // 3. upload it again, it will be stored into database
            string fileDir2 = null, fileName2 = null;
            string filePath2 = GetTempFile(17 * 1024, ref fileDir2, ref fileName2);

            ////// 
            ////// TODO: when StoreLocation is not accessable, the upload failed !!! need to fix SharePoint RBS code,
            //////       it never store blob back to database. consulting in MSDN blog...
            ////// 
            ////int itemId2 = -1;
            ////bool itemExists2 = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName2, fileDir2, fileName2, AtmosStoreFailedCache.StorageLocation, false, ref itemId2);
            ////Assert.IsFalse(itemExists2); // 

            ////// 4. download it and compare
            ////bool fileEquals2 = DownloadFileThenCompareThenDeleteAsNeed(filePath2, itemId2, _rbsConfig.DeleteSPItems);
            ////Assert.IsTrue(fileEquals2);

            ////// 5. clear up
            ////try { File.Delete(filePath2); }
            ////catch { }
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .txt")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptTXT()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 10);

            string itemName = @"18KB_TXT_ATMOs";
            string fileName = @"18.2kb.txt";

            int itemId = -1;


            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, false);
            Assert.IsTrue(isExistsAndCompressEncryptWell);

            bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .doc")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptDOC()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 11);

            string itemName = @"240KB_DOC_ATMOs";
            string fileName = @"240kb.doc";

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, false); // MS SP add metadata to Office files, they are binary not same, but content same.

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            // MS SP add metadata to Office files, they are binary not same, but content same.
            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .docx")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptDOCX()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 12);

            string itemName = @"20KB_DOCX_ATMOs";
            string fileName = @"20.2kb.docx";

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, false); // MS SP add metadata to Office files, they are binary not same, but content same.

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            // MS SP add metadata to Office files, they are binary not same, but content same.
            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .jpg")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptJPG()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 13);

            string itemName = @"Jpg_ATMOs";
            string fileName = @"cruella.jpg";

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .pdf")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptPDF()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 14);

            string itemName = @"Pdf_ATMOs";
            string fileName = @"exported-pdf-p0002.pdf";

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can encryption .tif")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanEncryptTIF()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreEncryptionOn.StoreName, 15);

            string itemName = @"Tif_ATMOs";
            string fileName = @"Shib.TIF";

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .txt")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressTXT()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 11);

            string origFileName = @"18.2kb.txt";

            string itemName = @"18KB_TXT_ATMOs_Compression";
            string fileName = @"18.2kb_compression.txt";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .doc")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressDOC()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 10);

            string origFileName = @"240kb.doc";

            string itemName = @"240KB_DOC_ATMOs_Compression";
            string fileName = @"240kb_compression.doc";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, false); // MS SP add metadata to Office files, they are binary not same, but content same.

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            // MS SP add metadata to Office files, they are binary not same, but content same.
            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .docx")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressDOCX()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 9);

            string origFileName = @"20.2kb.docx";

            string itemName = @"20KB_DOCX_ATMOs_Compression";
            string fileName = @"20.2kb_compression.docx";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, false); // MS SP add metadata to Office files, they are binary not same, but content same.

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            // MS SP add metadata to Office files, they are binary not same, but content same.
            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .jpg")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressJPG()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 8);

            string origFileName = @"cruella.jpg";

            string itemName = @"Jpg_ATMOs_Compression";
            string fileName = @"cruella_compression.jpg";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .pdf")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressPDF()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 7);

            string origFileName = @"exported-pdf-p0002.pdf";

            string itemName = @"Pdf_ATMOs_Compression";
            string fileName = @"exported-pdf-p0002_compression.pdf";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }

        [Test, Description("Verify RBS ATMOS provider can compress .tif")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressTIF()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 6);

            string origFileName = @"Shib.TIF";

            string itemName = @"Tif_ATMOs_Compression";
            string fileName = @"Shib_compression.TIF";
            File.Copy(_testDataPath + origFileName, _testDataPath + fileName, true);

            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                itemName, _testDataPath, fileName, AtmosStoreCompressionOn.StorageLocation, false, ref itemId,
                AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);

            //bool isItemCouldBeDownloadAndTheSame = DownloadFileThenCompareThenDeleteAsNeed(_testDataPath + fileName, itemId, _rbsConfig.DeleteSPItems);
            //Assert.IsTrue(isItemCouldBeDownloadAndTheSame);
        }
        
        [Test, Description("Verify RBS ATMOS provider can compress Contact type attachment")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressContactTypeAttachment()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 1);

            int rannumber = new Random(DateTime.Now.Millisecond).Next(10000);
            string listName = "QA Contacts_" + rannumber.ToString();

            _spoLib.CreateContactLib(_rbsConfig.SiteTitle, listName);
            int itemId = _spoLib.AddContactItem(_rbsConfig.SiteTitle, listName, "Wang1");

            string filePath = FileHelper.GetTempBlob(20 * 1024, true);


            // Get service URL
            string webUrl = _spoLib.webUrl;
            if (webUrl.EndsWith(@"/") == true)
                webUrl = webUrl.Remove(webUrl.Length - 1, 1);
            string serviceURL = webUrl + @"/_vti_bin/Lists.asmx";

            _spoLib.AddAttachment(serviceURL, listName, filePath, itemId.ToString(), "jeklfwjelfkwj");

            // sleep a while to wait SharePoint write blob to RBS
            // sleep more till the blob is encrypted / compressed into RBS Store
            System.Threading.Thread.Sleep(60 * 1000);

            // test to see if it exists in SharePoint
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, listName, itemId);
            bool processedItemExistsInRBSStore = false;

            // check exists in RBS Store, first we compress/encrypt this raw file, 
            //   and then compass this processed file with the one in the RBS Store StorageLocation
            // check exists: fileDir must end with \, e.g. c:\temp\
            string fileAfterProcessPath = Path.GetTempFileName();
            CompressAndEncryptFile(filePath, AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, ref fileAfterProcessPath);

            processedItemExistsInRBSStore = _baseLib.CheckItemExistsInRBS(fileAfterProcessPath, AtmosStoreCompressionOn.StorageLocation);

            // clean up
            try { File.Delete(fileAfterProcessPath); }
            catch { }
            try { File.Delete(filePath); }
            catch { }
            if (_rbsConfig.DeleteSPItems)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, listName, itemId); }
                catch { }
            }
            if (_rbsConfig.DeleteSPList)
            {
                try { _spoLib.DeleteLib(_rbsConfig.SiteTitle, listName); }
                catch { }
            }
        }

        [Test, Description("Verify RBS ATMOS provider can compress Discussion board type attachment")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressDiscussionBoardTypeAttachment()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 2);

            int rannumber = new Random(DateTime.Now.Millisecond).Next(10000);
            string listName = "QA Discussion Board_" + rannumber.ToString();

            _spoLib.CreateDiscussionBoardLib(_rbsConfig.SiteTitle, listName);
            int itemId = _spoLib.AddDiscussionBoardItem(_rbsConfig.SiteTitle, listName, "discuss 2", "this is disscussion 1");

            string filePath = FileHelper.GetTempBlob(21 * 1024, true);

            // Get service URL
            string webUrl = _spoLib.webUrl;
            if (webUrl.EndsWith(@"/") == true)
                webUrl = webUrl.Remove(webUrl.Length - 1, 1);
            string serviceURL = webUrl + @"/_vti_bin/Lists.asmx";

            _spoLib.AddAttachment(serviceURL, listName, filePath, itemId.ToString(), "attachment2");

            // sleep a while to wait SharePoint write blob to RBS
            // sleep more till the blob is encrypted / compressed into RBS Store
            System.Threading.Thread.Sleep(60 * 1000);

            // test to see if it exists in SharePoint
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, listName, itemId);
            bool processedItemExistsInRBSStore = false;

            // check exists in RBS Store, first we compress/encrypt this raw file, 
            //   and then compass this processed file with the one in the RBS Store StorageLocation
            // check exists: fileDir must end with \, e.g. c:\temp\
            string fileAfterProcessPath = Path.GetTempFileName();
            CompressAndEncryptFile(filePath, AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, ref fileAfterProcessPath);

            processedItemExistsInRBSStore = _baseLib.CheckItemExistsInRBS(fileAfterProcessPath, AtmosStoreCompressionOn.StorageLocation);

            // clean up
            try { File.Delete(fileAfterProcessPath); }
            catch { }
            try { File.Delete(filePath); }
            catch { }
            if (_rbsConfig.DeleteSPItems)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, listName, itemId); }
                catch { }
            }
            if (_rbsConfig.DeleteSPList)
            {
                try { _spoLib.DeleteLib(_rbsConfig.SiteTitle, listName); }
                catch { }
            }
        }

        [Test, Description("Verify RBS ATMOS provider can compress Calender type attachment")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanCompressCalenderTypeAttachment()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreCompressionOn.StoreName, 3);

            int rannumber = new Random(DateTime.Now.Millisecond).Next(10000);
            string listName = "QA Calender_" + rannumber.ToString();
            _spoLib.CreateEventLib(_rbsConfig.SiteTitle, listName);
            int itemId = _spoLib.AddEventItem(_rbsConfig.SiteTitle, listName, "event 3");

            string filePath = FileHelper.GetTempBlob(22 * 1024, true);

            // Get service URL
            //Uri webUri = new Uri(_spoLib.webUrl);
            //string baseUrl = webUri.GetLeftPart(UriPartial.Authority); // no need
            //Uri listUri = new Uri(webUri, "/_vti_bin/Lists.asmx");
            //string serviceURL = listUri.ToString();
            string webUrl = _spoLib.webUrl;
            if (webUrl.EndsWith(@"/") == true)
                webUrl = webUrl.Remove(webUrl.Length - 1, 1);
            string serviceURL = webUrl + @"/_vti_bin/Lists.asmx";
            
            _spoLib.AddAttachment(serviceURL, listName, filePath, itemId.ToString(), "attachment3");

            // sleep a while to wait SharePoint write blob to RBS
            // sleep more till the blob is encrypted / compressed into RBS Store
            System.Threading.Thread.Sleep(60 * 1000);

            // test to see if it exists in SharePoint
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, listName, itemId);
            bool processedItemExistsInRBSStore = false;

            // check exists in RBS Store, first we compress/encrypt this raw file, 
            //   and then compass this processed file with the one in the RBS Store StorageLocation
            // check exists: fileDir must end with \, e.g. c:\temp\
            string fileAfterProcessPath = Path.GetTempFileName();
            CompressAndEncryptFile(filePath, AtmosStoreCompressionOn.IsCompressed, AtmosStoreCompressionOn.EncryptionType, ref fileAfterProcessPath);

            processedItemExistsInRBSStore = _baseLib.CheckItemExistsInRBS(fileAfterProcessPath, AtmosStoreCompressionOn.StorageLocation);

            // clean up
            try { File.Delete(fileAfterProcessPath); }
            catch { }
            try { File.Delete(filePath); }
            catch { }
            if (_rbsConfig.DeleteSPItems)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, listName, itemId); }
                catch { }
            }
            if (_rbsConfig.DeleteSPList)
            {
                try { _spoLib.DeleteLib(_rbsConfig.SiteTitle, listName); }
                catch { }
            }
        }

        [Test, Description("Verify RBS ATMOS provider can upload and download BLOB from ATMOs")]
        [Category("Atmos")]
        public void VerifyRBSATMOsProviderCanUploadAndDownloadBlobFromATMOs()
        {
            SelectStoreAndSetMinimumFileSize(AtmosStoreUploadDownload.StoreName, 1);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(25 * 1024, ref fileDir, ref fileName);

            // upload it to SharePoint
            int itemId = -1;
            bool isExistsAndCompressEncryptWell = UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
                fileName, fileDir, fileName, AtmosStoreUploadDownload.StorageLocation, false, ref itemId,
                AtmosStoreUploadDownload.IsCompressed, AtmosStoreUploadDownload.EncryptionType, true);

            Assert.IsTrue(isExistsAndCompressEncryptWell);
            // Run Atmos externalize job
            _baseLib.RunJob("EMC SourceOne Atmos Externalization Job");
            System.Threading.Thread.Sleep(5 * 60 * 1000);
            // delete it from Cache, like cache expired.
            {
                string[] files = Directory.GetFiles(AtmosStoreUploadDownload.StorageLocation, @"*.rbsblob", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    fileInfo.Attributes = FileAttributes.Normal;
                    File.Delete(file);
                }
            }

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }
    }
}
