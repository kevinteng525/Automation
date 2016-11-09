using System;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharepointOnline;
using RBSLib;
using System.Net;

namespace RBSTestWithNunit
{
    [TestFixture]
    public class RBSBasic : Basic
    {
        protected const string SPConfigXML = @"SharePoint\SP2010sim.xml";
        protected const string TestDataPath = @"TestData\ItemSizeFilter\";
        
        // !!! DO NOT change this name, they are defined in XML !!!
        protected string StoreNameDefaultInXML = @"CIFSStore_Default";
        protected string StoreNameSNInXML = @"CIFSStore_ServerName";
        protected string StoreNameIPInXML = @"CIFSStore_IPAddress";
        protected string StoreNameLDInXML = @"CIFSStore_LocalDriver";

        protected Store StoreDefault = null;
        protected Store StoreSN = null;
        protected Store StoreIP = null;
        protected Store StoreLD = null;

        [TestFixtureSetUp]
        public void RBSBasicSetup()
        {
            // !!! ======= !!!
            //     the user running NUnit must have:
            //        administrator right to SharePoint Server [remote connect]
            //        Farm Admin to SharePoint 
            //        Read/Write/Delete/Create rights in all storage paths
            // !!! ======== !!!
            base.Setup(TestDataPath, SPConfigXML);

            // create all stores we need in tests
            StoreDefault = _rbsConfig.GetStore(StoreNameDefaultInXML);
            StoreSN = _rbsConfig.GetStore(StoreNameSNInXML);
            StoreIP = _rbsConfig.GetStore(StoreNameIPInXML);
            StoreLD = _rbsConfig.GetStore(StoreNameLDInXML);

            // {LocalIPAddress} => 192.168.1.2
            {
                string localIP = string.Empty;
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                StoreIP.StorageLocation = StoreIP.StorageLocation.Replace("{LocalIPAddress}", localIP);
            }

            List<Store> list = new List<Store>();
            list.Add(StoreDefault);
            list.Add(StoreSN);
            list.Add(StoreIP);
            list.Add(StoreLD);

            _baseLib.CreateStores(list.ToArray());
            Execute("cmd.exe", @"/c iisreset");
            // ES1SPAgent create the directory by default.
        }

        [TestFixtureTearDown]
        public void RBSBasicTearDown()
        {
            base.TearDown();

            // delete all stores as need
            if (_rbsConfig.DeleteStores)
            {
                List<string> list = new List<string>();
                list.Add(StoreDefault.StoreName);
                list.Add(StoreSN.StoreName);
                list.Add(StoreIP.StoreName);
                list.Add(StoreLD.StoreName);

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
                DeleteDirectory(StoreDefault.StorageLocation);
                DeleteDirectory(StoreSN.StorageLocation);
                DeleteDirectory(StoreIP.StorageLocation);
                RemoteDeleteDirectory(StoreLD.StorageLocation);
            }
        }

        [SetUp]
        public void CaseSetup()
        {
        }

        [Test, Description("Upload file size greater than 20K")]
        [Category("RBS")]
        public void RBSThreshold1UploadFileGreaterThan20K()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(1);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"20K_jpg", @"Tribu.jpg", StoreDefault.StorageLocation);
            
            Assert.IsTrue(itemExists);

        }

        [Test, Description("Upload file size equals 1K (1.? K)")]
        [Category("RBS")]
        public void RBSThreshold1UploadFileEuqals1K()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(1);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"1K_txt", @"1.1K.txt", StoreDefault.StorageLocation);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload file size greater than 10K")]
        [Category("RBS")]
        public void RBSThreshold1UploadFileGreaterThan10k()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(1);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"18K_txt", @"18.2kb.txt", StoreDefault.StorageLocation);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload file size greater than 50K")]
        [Category("RBS")]
        public void RBSThreshold50UploadFileGreaterThan50k()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(50);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"131_jpg", @"faultless.jpg", StoreDefault.StorageLocation);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload file size less than 50K")]
        [Category("RBS")]
        public void RBSThreshold50UploadFileLessThan50K()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(50);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"30K_PDF", @"exported-pdf-p0002.pdf", StoreDefault.StorageLocation);
            Assert.IsFalse(itemExists);
        }

        [Test, Description("Upload file size equals 50K")]
        [Category("RBS")]
        public void RBSThreshold50UploadFileEquals50k()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(50);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"50K_TIF", @"Shib.TIF", StoreDefault.StorageLocation);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload file size greater than 1M")]
        [Category("RBS")]
        public void RBSThreshold79Upload1MFile()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(79);
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(@"1M_PDF", @"highpointpoolrules.pdf", StoreDefault.StorageLocation);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload file size greater than 5M")]
        [Category("RBS")]
        public void RBSThreshold79Upload5MFile()
        {
            // Upload failed: bad request
            SelectStore(StoreDefault.StoreName);
            SetThreshold(79);
            int itemId = 0;
            bool itemExists = UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(@"5M_pic", _testDataPath, @"5Mpic.TIF", StoreDefault.StorageLocation, _rbsConfig.DeleteSPItems, ref itemId);
            Assert.IsTrue(itemExists);
        }

        [Test, Description("Upload and Download file to UNC storage path with server name")]
        [Category("RBS")]
        public void RBSThreshold8ServerNameUploadDownload()
        {
            SelectStore(StoreSN.StoreName);
            SetThreshold(8);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(10 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreSN.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Upload and Download file to UNC storage path with IP address")]
        [Category("RBS")]
        public void RBSThreshold8IPAddressUploadDownload()
        {
            SelectStore(StoreIP.StoreName);
            SetThreshold(8);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(10 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreIP.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Upload and Download file to local mapping storage path")]
        [Category("RBS")]
        public void RBSThreshold8LocalDriverUploadDownload()
        {
            SelectStore(StoreLD.StoreName);
            SetThreshold(8);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(10 * 1024, ref fileDir, ref fileName);

            // parse remote Local driver to share folder
            string remotePathLD = string.Format(@"\\{0}\{1}", _rbsConfig.ServerName, StoreLD.StorageLocation.Replace(':', '$'));

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

        [Test, Description("Upload and Download file which size 1M")]
        [Category("RBS")]
        public void RBSThreshold90UploadDownload1M()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(90);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(1024 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreDefault.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Upload and Download file which size 5M")]
        [Category("RBS")]
        public void RBSThreshold90UploadDownload5M()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(90);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(5 * 1024 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreDefault.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Upload and Download file which size 10M")]
        [Category("RBS")]
        public void RBSThreshold90UploadDownload50M()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(90);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(50 * 1024 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreDefault.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }

        [Test, Description("Upload and Download file which size 10M")]
        [Category("RBS")]
        public void RBSThreshold90UploadDownload100M()
        {
            SelectStore(StoreDefault.StoreName);
            SetThreshold(90);

            string fileDir = null, fileName = null;
            string filePath = GetTempFile(100 * 1024 * 1024, ref fileDir, ref fileName);

            int itemId = -1;
            bool itemExists = UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(fileName, fileDir, fileName, StoreDefault.StorageLocation, false, ref itemId);
            Assert.IsTrue(itemExists);

            // download it and compare
            bool fileEquals = DownloadFileThenCompareThenDeleteAsNeed(filePath, itemId, _rbsConfig.DeleteSPItems);
            Assert.IsTrue(fileEquals);

            // clear up
            try { File.Delete(filePath); }
            catch { }
        }
    }
}

