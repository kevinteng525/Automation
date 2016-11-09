using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace RBSLib
{
    public class RBSBasicLib
    {
        public const string AgentExeName = "ES1SPAgent.exe";
        public string AgentPath { get; protected set; }

        protected string _serverName = null;
        protected string _contentDBName = null;

        public RBSBasicLib(string serverName, string contentDBName)
        {
            _serverName = serverName;
            _contentDBName = contentDBName;

            // because nunit: use relative path
            //string workingDir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(RBSBasicLib)).Location);
            //AgentPath = Path.Combine(workingDir, AgentExeName);
            AgentPath = AgentExeName;
        }

        #region Item/File Operation

        /// <summary>
        /// Upload file to SharePoint
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void UploadFile(string fileName, string listUri)
        {
            // It is in SPOLib
        }

        /// <summary>
        /// Retrieve file, from RBS or Atmos Provider (operate in SharePoint)
        /// </summary>
        public void RetrieveFile()
        {
        }


        /// <summary>
        /// Delete files, from RBS or Atmos Provider (operate in SharePoint)
        /// </summary>
        public void DeleteFile()
        {
        }

        #endregion

        #region Check Item

        /// <summary>
        /// Check the item exists or NOT in SharePoint
        /// </summary>
        public bool CheckItemExistsInSharePoint()
        {
            bool exists = false;

            return exists;
        }

        /// <summary>
        /// Check the the document is written to RBS store
        /// </summary>
        public bool CheckItemExistsInRBS(string filePath, string RbsRootFolder)
        {
            bool exists = false;

            FileInfo fi = new FileInfo(filePath);
            int theLength = (int) fi.Length; // long => int

            if (Directory.Exists(RbsRootFolder))
            {
                string[] files = Directory.GetFiles(RbsRootFolder, "*.rbsblob", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fi2Compress = new FileInfo(file);
                    if (fi2Compress.Length == theLength)
                    {
                        exists = FileHelper.BinaryCompare(filePath, file);
                        break;
                    }
                }
            }

            return exists;
        }

        /// <summary>
        /// Check the the document is written to Content DB
        /// </summary>
        public bool CheckItemExistInContentDB()
        {
            bool exists = false;
            return exists;
        }

        #endregion

        //#region Encryption & Compression

        ///// <summary>
        ///// ILMerge to ES1SPAgent, we move this method to 'RBSTestWithNunit' project
        ///// </summary>
        //public virtual void CompressAndEncryptFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        //{
        //    using (EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobWriterStream writerStream = 
        //        new EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobWriterStream(outFile, null,
        //                isCompressed ? EMC.SourceOne.RBSProvider.ProviderConfig.CompressionType.DeflateStream : EMC.SourceOne.RBSProvider.ProviderConfig.CompressionType.None, 
        //                (EMC.SourceOne.RBSProvider.ProviderConfig.EncryptionType)((short)encryptType), 0x8000, 0x8000))
        //    {
        //        using (FileStream inputStream = new FileStream(inFile, FileMode.Open, FileAccess.Read))
        //        {
        //            inputStream.CopyTo(writerStream, 0x8000);
        //            writerStream.Commit();

        //            inputStream.Close();
        //            writerStream.Close();
        //        }
        //    }


        //}

        ///// <summary>
        ///// ILMerge to ES1SPAgent, we move this method to 'RBSTestWithNunit' project
        ///// </summary>
        //public virtual void DecryptAndDecompressFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        //{
        //    using (EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobReaderStream readerStream = new EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobReaderStream(inFile, 0x8000))
        //    {
        //        using (FileStream outputStream = File.Create(outFile))
        //        {
        //            readerStream.CopyTo(outputStream, 0x8000);

        //            outputStream.Close();
        //            readerStream.Close();
        //        }
        //    }
        //}

        //#endregion

        #region RBS & Store Operation

        public virtual void ActivateRBS()
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation ActivateRBS -db {0}", _contentDBName));
        }

        public virtual void DeactivateRBS()
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation DeactivateRBS -db {0}", _contentDBName));
        }

        public virtual void SetMinimumFileSize(int kBytes)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation SetMinFileSize -db {0} -mfs {1}", _contentDBName, kBytes));
        }

        public virtual void SelectStore(string storeName)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation SelectStore -db {0} -sn {1}", _contentDBName, storeName));
        }

        public virtual void SelectStoreAndSetMinimumFileSize(string storeName, int kBytes)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation SelectStore,SetMinFileSize -db {0} -sn {1} -mfs {2}", _contentDBName, storeName, kBytes));
        }

        public virtual void DeleteStore(string storeName)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation DeleteStore -db {0} -sn {1}", _contentDBName, storeName));
        }

        public virtual void CreateCIFSStore(string storeName, string storageLocation, int poolCapacity, EncryptionType encryptionType, bool isCompressed, string userName, string password)
        {
            Store store = new Store();
            store.StoreName = storeName;
            store.StorageLocation = storageLocation;
            store.PoolCapacity = poolCapacity;
            store.EncryptionType = encryptionType;
            store.IsCompressed = isCompressed;
            store.UserName = userName;
            store.Password = password;
            store.Passcode = "P@ssw0rd";
            store.IsAtmosStore = false;

            CreateStore(store);
        }

        public virtual void CreateStore(Store store)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Store));
            serializer.Serialize(writer, store);
            string XML = writer.ToString();
            CreateStore(XML);
        }

        public virtual void CreateStore(string storeConfigXML)
        {
            string storeFile = @"C:\tempQi.xml";
            string remoteStoreFile = string.Format(@"\\{0}\C$\tempQi.xml", _serverName);
            System.IO.File.WriteAllText(remoteStoreFile, storeConfigXML);

            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation CreateStore -db {0} -storefile {1}", _contentDBName, storeFile));

            try { File.Delete(remoteStoreFile); }
            catch { } // ignore it.
        }

        public virtual void CreateStores(Store[] stores)
        {
            if (stores.Length > 0)
            {
                string[] storeFiles = new string[stores.Length];
                string[] remoteStoreFiles = new string[stores.Length];
                int i = -1;

                Random rand = new Random(DateTime.Now.Millisecond);
                foreach (Store store in stores)
                {
                    StringWriter writer = new StringWriter();
                    XmlSerializer serializer = new XmlSerializer(typeof(Store));
                    serializer.Serialize(writer, store);
                    string XML = writer.ToString();

                    string storeFileName = string.Format(@"temp_{0}_{1}.xml", rand.Next(10000), DateTime.Now.Millisecond);
                    string storeFile = string.Format(@"C:\{0}", storeFileName);
                    string remoteStoreFile = string.Format(@"\\{0}\C$\{1}", _serverName, storeFileName);
                    System.IO.File.WriteAllText(remoteStoreFile, XML);

                    storeFiles[++i] = storeFile;
                    remoteStoreFiles[i] = remoteStoreFile;
                }

                string args = string.Join("|", storeFiles);
                RemoteExecution.Execute(_serverName, AgentPath, string.Format(@"-operation CreateStores -db {0} -storefiles ""{1}""", _contentDBName, args));

                // remove temp files
                foreach (string file in remoteStoreFiles)
                {
                    try { File.Delete(file); }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Arguments too long... sometimes
        /// </summary>
        public virtual void DeleteStores(string[] storenames)
        {
            if (storenames.Length > 0)
            {
                string args = string.Join(",", storenames);
                RemoteExecution.Execute(_serverName, AgentPath, string.Format(@"-operation DeleteStores -db {0} -storenames ""{1}""", _contentDBName, args));
            }
        }

        #endregion

        #region Job operation

        public virtual void RunJob(string jobName)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format(@"-operation RunJob -jn ""{0}""", jobName));
        }

        #endregion

        #region SharePoint Server

        public virtual void RestartSPAdmin()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c net stop spadminv4 & net start spadminv4", false);
        }

        public virtual void RestartSPTimer()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c net stop sptimerv4 & net start sptimerv4", false);
        }

        public virtual void IisReset()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c iisreset", false);
        }

        public virtual void RestartMSSQL()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c net stop mssqlserver & net start mssqlserver", false);
        }

        public virtual void RestartSharePointServer()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c net stop spadminv4 & net stop sptimerv4 & net stop mssqlserver & net start mssqlserver & net start spadminv4 & net start sptimerv4 & iisreset", false);
        }

        public virtual void RestartServer()
        {
            RemoteExecution.Execute(_serverName, "cmd.exe", "/c shutdown -r -t 1 -f", false);
        }

        #endregion

        #region get database
        private DataSet RunSQLQuery(string connectionString, SqlCommand cmd)
        {
            SqlConnection cnn = new SqlConnection(connectionString);
            cmd.Connection = cnn;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }

        public DataSet GetBlobDetail(string connectionString, Guid id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[AllDocStreams] WHERE Id = @Id");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Id", SqlDbType.VarBinary, 64);
            cmd.Parameters["@Id"].Value = id.ToByteArray();
            DataSet ds = RunSQLQuery(connectionString, cmd);
            byte[] bytes = ds.Tables[0].Rows[0]["RbsId"] as byte[];
            DataTable table = ds.Tables[0];           

            cmd = new SqlCommand("SELECT * FROM dbo.fn_RbsBlobDetails_RbsId(@RbsId)");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@RbsId", SqlDbType.VarBinary, 64);
            cmd.Parameters["@RbsId"].Value = bytes;
            ds = RunSQLQuery(connectionString, cmd);
            //int blobnumber = int.Parse(ds.Tables[0].Rows[0]["StoreBlobId"].ToString());
            byte[] BlobId = ds.Tables[0].Rows[0]["StoreBlobId"] as byte[];

            cmd = new SqlCommand("SELECT * FROM [mssqlrbs_emces1].[rbs_emces1_blobs] WHERE store_blob_id=@store_blob_id");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@store_blob_id", SqlDbType.VarBinary, 255);
            cmd.Parameters["@store_blob_id"].Value = BlobId;
            ds = RunSQLQuery(connectionString, cmd);
            return ds;
        }
        #endregion


        public virtual void RunSyncJob()
        {
            RemoteExecution.Execute(_serverName, AgentPath, "-operation RunJob -jn \"EMC SourceOne Atmos Externalization Job\"");
        }
    }


    public class Store
    {
        public string StoreName;
        public string StorageLocation;
        public int PoolCapacity;
        public EncryptionType EncryptionType;
        public bool IsCompressed;
        public string UserName;
        public string Password;
        public string Passcode;

        public bool IsAtmosStore;

        public string AtmosServerUrl;
        public string AtmosSubTenant;
        public int AtmosPort;
        public string AtmosSharedSecret;
        public string AtmosUid;
        public int AtmosCacheExpireDay;
    }

    public enum EncryptionType : short
    {
        None = 0,
        AES128 = 1,
        AES256 = 2,
        DES64 = 3,
        RC2_128 = 4,
        TripleDES128 = 5,
        TripleDES192 = 6,
    }
}
