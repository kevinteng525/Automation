using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBSLib;
using SharepointOnline;
using System.IO;
using System.Diagnostics;

namespace RBSTestWithNunit
{
    public class Basic
    {
        protected string _testDataPath = null;
        protected string _spConfigXML = null;
        protected SPOLib _spoLib = null;
        protected RBSBasicLib _baseLib = null;
        protected RBSConfiguration _rbsConfig = null;

        protected void Setup(string testDataPath, string spConfigXML)
        {
            _testDataPath = testDataPath;
            _spConfigXML = spConfigXML;

            // !!! ======= !!!
            //     the user running NUnit must have:
            //        administrator right to SharePoint Server [remote connect]
            //        Farm Admin to SharePoint 
            //        Read/Write/Delete/Create rights in all storage paths
            // !!! ======== !!!
            _rbsConfig = RBSConfiguration.GetConfiguration(_spConfigXML);

            // passing the password as well?
            _baseLib = new RBSLib.RBSBasicLib(_rbsConfig.ServerName, _rbsConfig.ContentDatabaseName);

            // restart SharePoint Service as needed
            if (_rbsConfig.RestartSharePointService && _rbsConfig.HaveNotYetRestarted)
            {
                _baseLib.RestartSharePointServer();
                _rbsConfig.HaveNotYetRestarted = false;
            }

            // activate the RBS in current content database
            _baseLib.ActivateRBS();

            // create SP lib oject to operate with SharePoint using Client Object Model
            _spoLib = new SPOLib(_spConfigXML);
            _spoLib.CreateDocumentLib(_rbsConfig.SiteTitle, _rbsConfig.ListTitle); // it will return the lib if it exists.
        }

        protected void TearDown()
        {
            // delete the doc library
            if (_rbsConfig.DeleteSPList)
            {
                try { _spoLib.DeleteLib(_rbsConfig.SiteTitle, _rbsConfig.ListTitle); }
                catch { }
            }
        }

        #region Helper

        protected string _currentStore = null;
        protected void SelectStore(string storeName)
        {
            if (_baseLib != null && storeName != _currentStore)
            {
                _baseLib.SelectStore(storeName);
                _currentStore = storeName;
            }
        }

        protected int _currentThreshold = -1;
        protected void SetThreshold(int thresholdSize)
        {
            if (_baseLib != null && _currentThreshold != thresholdSize)
            {
                _baseLib.SetMinimumFileSize(thresholdSize);
                _currentThreshold = thresholdSize;
            }
        }

        protected void SelectStoreAndSetMinimumFileSize(string storeName, int thresholdSize)
        {
            if (_baseLib != null)
            {
                if (_currentStore != storeName)
                {
                    _baseLib.SelectStore(storeName);
                    _currentStore = storeName;
                }
                if (_currentThreshold != thresholdSize)
                {
                    _baseLib.SetMinimumFileSize(thresholdSize);
                    _currentThreshold = thresholdSize;
                }
            }
        }

        protected void DeleteDirectory(string dir)
        {
            try { Directory.Delete(dir, true); }
            catch { }
        }

        protected void RemoteDeleteDirectory(string dir)
        {
            RemoteExecution.Execute(_rbsConfig.ServerName, "cmd.exe", string.Format(@"/C RMDIR /S /Q ""{0}""", dir), false);
        }

        /// <summary>
        /// MS make something in docx/xlsx, so do not upload docx&xlsx, upload txt, pdf, tif, jpeg
        /// </summary>
        protected bool UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(string itemName, string fileDir, string fileName, string storageLocation, bool deleteAfterCheck, ref int itemId)
        {
            // it is UploadFile() to SharePoint
            itemId = _spoLib.UploadFile(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemName, fileDir, fileName);

            // sleep a while to wait SharePoint write blob to RBS
            System.Threading.Thread.Sleep(20 * 1000);

            // need it exists both in SharePoint and RBS Store
            // check exists: fileDir must end with \, e.g. c:\temp\
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId);
            bool rawItemExistsInRBSStore = _baseLib.CheckItemExistsInRBS(fileDir + fileName, storageLocation);

            // delete it as needed
            if (deleteAfterCheck)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId); }
                catch { }
            }

            return rawItemExistsInRBSStore && itemExistsInSP;
        }

        protected bool UploadFileThenCheckExistsThenCompareThenDeleteAsInConfigXML(string itemName, string fileName, string storageLocation)
        {
            int itemId = -1;
            return UploadFileThenCheckExistsThenCompareThenDeleteAsNeed(itemName, _testDataPath, fileName, storageLocation, _rbsConfig.DeleteSPItems, ref itemId);
        }

        protected bool UploadBigFileThenCheckExistsThenCompareThenDeleteAsNeed(string itemName, string fileDir, string fileName, string storageLocation, bool deleteAfterCheck, ref int itemId)
        {
            // it is UploadBigFile()
            itemId = _spoLib.UploadBigFile(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemName, fileDir, fileName);

            // sleep a while to wait SharePoint write blob to RBS
            System.Threading.Thread.Sleep(10 * 1000);

            // check exists: fileDir must end with \, e.g. c:\temp\
            // need it exists both in RBS Store and SharePoint
            bool itemExistsInRBS = _baseLib.CheckItemExistsInRBS(fileDir + fileName, storageLocation);
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId);

            // delete it as needed
            if (deleteAfterCheck)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId); }
                catch { }
            }

            return itemExistsInRBS && itemExistsInSP;
        }

        protected bool DownloadFileThenCompareThenDeleteAsNeed(string filePath, int itemId, bool deleteAfterCompare)
        {
            // download it and compare it
            string downloadFilePath = Path.GetTempFileName();
            Stream downloadStream = _spoLib.DownloadFile(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId);

            using (BinaryReader reader = new BinaryReader(downloadStream))
            {
                FileStream downloadFileStream = File.OpenWrite(downloadFilePath);
                using (BinaryWriter writer = new BinaryWriter(downloadFileStream))
                {
                    const int BufferSize = 4 * 1024;
                    byte[] buffer = new byte[BufferSize];
                    int read = -1;
                    while ((read = reader.Read(buffer, 0, BufferSize)) > 0)
                    {
                        writer.Write(buffer, 0, read);
                    }
                    writer.Flush();
                }
            }

            bool fileTheSame = RBSLib.FileHelper.BinaryCompare(filePath, downloadFilePath);

            try { File.Delete(downloadFilePath); }
            catch { }

            if (deleteAfterCompare)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId); }
                catch { }
            }

            return fileTheSame;
        }

        protected string GetTempFile(int size, ref string fileDir, ref string fileName)
        {
            string filePath = RBSLib.FileHelper.GetTempBlob(size);
            fileDir = Path.GetDirectoryName(filePath);
            fileDir = fileDir + (fileDir.EndsWith(@"\") == false ? @"\" : "");
            fileName = Path.GetFileName(filePath);
            return filePath;
        }

        protected bool UploadFile_CheckExistsInSP_CompareProccessItemInRBSStore_DeleteAsNeed(
            string itemName, string fileDir, string fileName, string storageLocation, bool deleteAfterCheck, ref int itemId,
            bool isCompressed, EncryptionType encryptionType, bool checkInRBSStore)
        {
            // it is UploadFile() to SharePoint
            itemId = _spoLib.UploadFile(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemName, fileDir, fileName);

            // sleep a while to wait SharePoint write blob to RBS
            // sleep more till the blob is encrypted / compressed into RBS Store
            System.Threading.Thread.Sleep(60 * 1000);

            // test to see if it exists in SharePoint
            bool itemExistsInSP = _spoLib.CheckItemExists(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId);
            bool processedItemExistsInRBSStore = false;

            //
            // For files e.g. .doc; .docx; .ppt; .pptx; office files,
            //   Microsoft SharePoint add some metadata, the binary are not the same, but content are the same
            //   We do not check them.
            //
            if (checkInRBSStore == true)
            {
                // check exists in RBS Store, first we compress/encrypt this raw file, 
                //   and then compass this processed file with the one in the RBS Store StorageLocation
                // check exists: fileDir must end with \, e.g. c:\temp\
                string filePath = fileDir + fileName;
                string fileAfterProcessPath = Path.GetTempFileName();
                CompressAndEncryptFile(filePath, isCompressed, encryptionType, ref fileAfterProcessPath);

                processedItemExistsInRBSStore = _baseLib.CheckItemExistsInRBS(fileAfterProcessPath, storageLocation);

                try { File.Delete(fileAfterProcessPath); }
                catch { }
            }
            else
            {
                processedItemExistsInRBSStore = true;
            }

            // delete it as needed
            if (deleteAfterCheck)
            {
                try { _spoLib.DeleteItem(_rbsConfig.SiteTitle, _rbsConfig.ListTitle, itemId); }
                catch { }
            }

            return itemExistsInSP && processedItemExistsInRBSStore;
        }

        #endregion

        #region Compression and Encryption

        public void CompressAndEncryptFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        {
            FileInfo fiIn = new FileInfo(inFile);
            FileInfo fiOut = new FileInfo(outFile);
            inFile = fiIn.FullName;
            outFile = fiOut.FullName;

            string args = string.Format(@"/c {0} -ce -c {1} -e {2} -src ""{3}"" -dest ""{4}""",
                "RBSAppNET35.exe", isCompressed, encryptType, inFile, outFile);
            Execute("cmd.exe", args);
        }

        public void DecryptAndDecompressFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        {
            FileInfo fiIn = new FileInfo(inFile);
            FileInfo fiOut = new FileInfo(outFile);
            inFile = fiIn.FullName;
            outFile = fiOut.FullName;

            string args = string.Format(@"/c {0} -dd -c {1} -e {2} -src ""{3}"" -dest ""{4}""",
                "RBSAppNET35.exe", isCompressed, encryptType, inFile, outFile);
            Execute("cmd.exe", args);
        }

        public static void Execute(string filename, string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.RedirectStandardError = true;
            process.Start();

            // wait it ends
            process.WaitForExit();

            // log it
            string cmd = string.Format("{0} running: {1} {2}", DateTime.Now.ToString(), process.StartInfo.FileName, process.StartInfo.Arguments);
            File.AppendAllLines("RBSAppNET35.exe.log", new string[] { cmd });

            //string stdoutput = process.StandardOutput.ReadToEnd();
            //string stderr = process.StandardError.ReadToEnd();
            //File.AppendAllLines("RBSAppNET35.exe.log", new string[] { cmd, stdoutput, stderr });
        }

        #endregion

        //
        // ONLINE ATMOs
        //
        //Address: http://api.atmosonline.com
        //Access Token: ddca93e30ed9476c9676f5fc5231eb32/A68999772788f4522dce
        //Secret Key: yirPeG/RMqugHsGjBgzOl2Rf7Es=
        //

        //
        // PRIVATE ATMOs
        //Server:       10.37.13.180
        //Port:         80
        //Subtenant ID:
        //UID:          RBSStore
        //Secret Key:   DOZHxkOBSm5714EmnalL3sJQHBY=
        //
    }
}
