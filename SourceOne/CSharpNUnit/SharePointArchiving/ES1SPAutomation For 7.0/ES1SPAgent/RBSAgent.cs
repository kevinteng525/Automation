using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using EMC.SourceOne.RBSProvider.ProviderUICore.Extensions;
using System.Security.Principal;
using EMC.SourceOne.RBSProvider.ProviderConfig;
using System.Text;
using System.IO;
using System.Threading;
using EMC.SourceOne.RBSProvider.ProviderService.Atmos;

namespace ES1SPAgent
{
    public class RBSAgent
    {
        protected SPContentDatabase _currentContentDB = null;
        protected RBSAgentParameter _parameter = null;

        /// <summary>
        /// Create one RBSAgent instance
        /// </summary>
        public RBSAgent(RBSAgentParameter parameter)
        {
            SPFarm farm = SPFarm.Local;
            if (farm == null)
            {
                string platformType = IntPtr.Size == 8 ? "x64" : "x86";
                throw new ApplicationException(string.Format("SPFarm.Local is null, make sure current user '{0}' is farm admin, or current application platform '{1}' matches with SharePoint application platform.",
                    WindowsIdentity.GetCurrent().Name, platformType));
            }

            // not only run job, need Content Database
            if ((parameter.Operation | (int)RBSAgentOperation.RunJob) != (int)RBSAgentOperation.RunJob)
            {
                SPContentDatabase contentDB = null;
                foreach (SPService service in farm.Services)
                {
                    if (service is SPWebService)
                    {
                        SPWebService spWebService = service as SPWebService;
                        foreach (SPWebApplication webApp in spWebService.WebApplications)
                        {
                            foreach (SPContentDatabase db in webApp.ContentDatabases)
                            {
                                if (db.Name.Equals(parameter.ContentDBName, StringComparison.InvariantCultureIgnoreCase) == true)
                                {
                                    contentDB = db;
                                    break;
                                }
                            }

                            if (contentDB != null)
                                break;
                        }
                    }

                    if (contentDB != null)
                        break;

                }

                _currentContentDB = contentDB;

                if (_currentContentDB == null)
                {
                    throw new ApplicationException(string.Format("The Content Database with name '{0}' not found in current farm.", parameter.ContentDBName));
                }

                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    bool isActivation = _currentContentDB.GetRBSActivationStatus();
                    bool isES1RBS = _currentContentDB.ActiveStoreIsSourceOneRBS();
                });
            }

            _parameter = parameter;
        }

        #region Export Methods

        public void DoOperations()
        {

            RBSAgentOperation[] operations = new RBSAgentOperation[]
                {
                    RBSAgentOperation.ActivateRBS,
                    RBSAgentOperation.CreateStore,
                    RBSAgentOperation.DeactivateRBS,
                    RBSAgentOperation.DeleteStore,
                    RBSAgentOperation.RunJob,
                    RBSAgentOperation.SelectStore,
                    RBSAgentOperation.SetCacheExpireDays,
                    RBSAgentOperation.SetMinFileSize,
                    RBSAgentOperation.CreateStores,
                    RBSAgentOperation.DeleteStores
                };

            int theOpIndex = 0;
            while (_parameter.Operation != 0 && theOpIndex < operations.Length)
            {
                RBSAgentOperation theOperation = operations[theOpIndex++];
                if ((_parameter.Operation & (int)theOperation) == (int)theOperation)
                {
                    try
                    {
                        string methodName = theOperation.ToString();
                        Console.WriteLine(string.Format("[{0}]  ...  ", methodName));
                        this.DoOperation(theOperation);
                        Console.WriteLine("[Done]");
                        Thread.Sleep(3 * 1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("[Failed: {0}]", ex.Message));
                        Thread.Sleep(3 * 1000);
                    }
                    finally
                    {
                        _parameter.Operation -= (int)theOperation;
                    }
                }
            }
        }

        public void DoOperation(RBSAgentOperation operation)
        {
            switch (operation)
            {
                case RBSAgentOperation.ActivateRBS: ActivateRBS(); break;
                case RBSAgentOperation.CreateStore: CreateStore(); break;
                case RBSAgentOperation.DeactivateRBS: DeactivateRBS(); break;
                case RBSAgentOperation.SelectStore: SelectStore(); break;
                case RBSAgentOperation.DeleteStore: DeleteStore(); break;
                case RBSAgentOperation.SetMinFileSize: SetMinFileSize(); break;
                case RBSAgentOperation.SetCacheExpireDays: SetCacheExpireDays(); break;
                case RBSAgentOperation.RunJob: RunJob(); break;
                case RBSAgentOperation.CreateStores: CreateStores(); break;
                case RBSAgentOperation.DeleteStores: DeleteStores(); break;
                default: break;
            }
        }

        public void CreateStore()
        {
            this.CreateStore(_parameter.StoreFile);
        }
        public void SelectStore()
        {
            this.SelectStore(_parameter.StoreName);
        }
        public void DeleteStore()
        {
            this.DeleteStore(_parameter.StoreName);
        }
        public void SetMinFileSize()
        {
            this.SetMinimumFileSize(_parameter.MinFileSize);
        }
        public void SetCacheExpireDays()
        {
            this.SetStoreCacheExpireDay(_parameter.StoreName, _parameter.CacheExpireDays);
        }
        public void RunJob()
        {
            this.RunJob(_parameter.JobName);
        }
        public void CreateStores()
        {
            this.CreateStores(_parameter.StoreFiles);
        }
        public void DeleteStores()
        {
            this.DeleteStores(_parameter.StoreNames);
        }

        #endregion

        #region RBS

        /// <summary>
        /// Test if the RBS is activated.
        /// </summary>
        protected bool IsRBSActivated()
        {
            bool activated = false;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                SPContentDatabase db = _currentContentDB;
                SPRemoteBlobStorageSettings settings = db.RemoteBlobStorageSettings;
                DatabaseConfiguration dbConfig = null;
                if (db.GetRBSActivationStatus() == true)
                    dbConfig = db.GetDatabaseConfiguration();

                if (settings.Installed() && settings.Enabled && dbConfig != null && db.GetRBSActivationStatus())
                {
                    activated = true;
                }
            });

            return activated;
        }

        /// <summary>
        /// Activate RBS
        /// </summary>
        public virtual void ActivateRBS()
        {
            // Create "EMC SourceOne Atmos Externalization Job" job
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                Console.WriteLine("    Create Job: EMC SourceOne Atmos Externalization Job ... ");
                ActivateCacheUploadJob(_currentContentDB.WebApplication);
                Console.WriteLine("    Done");
            });

            if (IsRBSActivated() == true)
            {
                Console.WriteLine("    Already activated.");
            }
            else
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    Console.WriteLine("    Activating ...");

                    SPContentDatabase db = _currentContentDB;
                    db.InstallRBS();
                    db.InstallSourceOneRBS();

                    if (db.RemoteBlobStorageSettings.Enabled == false)
                        db.RemoteBlobStorageSettings.Enable();

                    db.ActivateRBS();

                    if (db.RemoteBlobStorageSettings.MinimumBlobStorageSize == 0)
                    {
                        db.RemoteBlobStorageSettings.MinimumBlobStorageSize = 80 * 1024;
                        db.Update();
                    }

                    // trust certificate
                    if (db.GetRBSActivationStatus() == true)
                    {
                        DatabaseConfiguration dbConfig = db.GetDatabaseConfiguration();
                        dbConfig.TrustServerCertificate = true;
                        dbConfig.Save();
                    }

                    // Create "EMC SourceOne Atmos Externalization Job" job
                    Console.WriteLine("    Create Job: EMC SourceOne Atmos Externalization Job ...");
                    ActivateCacheUploadJob(_currentContentDB.WebApplication);
                });
            }
        }

        private void ActivateCacheUploadJob(SPWebApplication webApp)
        {
            // SPWebApplication webApp = SPWebApplication.Lookup(new Uri(_webAppUrl));
            CacheSynchronizer cacheSyncJob = new CacheSynchronizer(webApp, null);
            foreach (SPJobDefinition job in webApp.JobDefinitions)
            {
                if (job.Name.Equals(cacheSyncJob.Name))
                {
                    job.Delete();
                    break;
                }
            }

            SPHourlySchedule hourlySchedule = new SPHourlySchedule();
            hourlySchedule.BeginMinute = 0;
            hourlySchedule.EndMinute = 10;
            cacheSyncJob.Schedule = hourlySchedule;
            cacheSyncJob.Update();
        }

        /// <summary>
        /// Deactivate RBS
        /// </summary>
        public virtual void DeactivateRBS()
        {
            if (IsRBSActivated() == true)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    SPContentDatabase db = _currentContentDB;

                    // List only have 3rd party stores, no our SouceOne Stores, we should allow deactivate
                    db.UnistallSourceOneRBS(true);
                    db.DeactivateRBS();

                    //If no other 3rd party stores left
                    if (db.RemoteBlobStorageSettings.GetProviderNames().Count == 0)
                    {
                        db.RemoteBlobStorageSettings.Disable();
                        db.UninstallRBS(true);
                    }
                });
            }
        }

        /// <summary>
        /// Files with a size greater than or equal to this value are saved to the RBS store 
        ///   [equal to] at byte level, it will not be saved to the RBS store.
        /// </summary>
        /// <param name="kilobytes"></param>
        public virtual void SetMinimumFileSize(int kilobytes)
        {
            Console.WriteLine(string.Format("    Set minimum file size to '{0}' KB", kilobytes));

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                _currentContentDB.RemoteBlobStorageSettings.MinimumBlobStorageSize = kilobytes * 1024;
                _currentContentDB.Update();
            });
        }

        /// <summary>
        /// Set retry count, default is 3
        /// </summary>
        public virtual void SetOperationRetryAttempts(int retryAttempts)
        {
            if (retryAttempts < 0)
                throw new ArgumentException("retryAttempts must be equal or greater then 0.");

            SPContentDatabase db = _currentContentDB;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                DatabaseConfiguration dbConfig = null;
                if (db.GetRBSActivationStatus() == true)
                    dbConfig = db.GetDatabaseConfiguration();
                if (dbConfig != null)
                {
                    if (retryAttempts != dbConfig.OperationRetryAttempts)
                    {
                        dbConfig.OperationRetryAttempts = retryAttempts;
                        dbConfig.Save();
                    }
                }
            });
        }

        #endregion

        #region Store Operation

        /// <summary>
        /// Set the store as current activate store
        /// </summary>
        public virtual void SelectStore(string storeName)
        {
            Console.WriteLine(string.Format("    Select store '{0}'", storeName));

            // select 'None' as store, it will be saved content db
            if (string.Compare(storeName, "None", true) == 0 || string.Compare(storeName, "null", true) == 0)
                storeName = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                _currentContentDB.RemoteBlobStorageSettings.SetActiveProviderName(storeName);
                _currentContentDB.Update(); // no update is needed seems
            });
        }

        /// <summary>
        /// Delete one store
        /// </summary>
        public virtual void DeleteStore(string storeName)
        {
            Console.WriteLine(string.Format("    Delete store '{0}'", storeName));

            bool deleted = false;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                deleted = _currentContentDB.DeleteBlobStore(storeName);
            });

            if (deleted == false)
                throw new ApplicationException(string.Format("Store '{0}' in use, can not delete.", storeName));
        }

        public virtual void CreateStores(string[] storefiles)
        {
            foreach (string storefile in storefiles)
            {
                try
                {
                    CreateStore(storefile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("    [Failed: {0}]", ex.Message));
                }
                System.Threading.Thread.Sleep(1 * 1000);
            }
        }

        public virtual void DeleteStores(string[] storenames)
        {
            foreach (string storename in storenames)
            {
                try
                {
                    DeleteStore(storename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("    [Failed: {0}]", ex.Message));
                }
                System.Threading.Thread.Sleep(1 * 1000);
            }
        }

        /// <summary>
        /// Create a store
        /// </summary>
        public virtual void CreateStore(string storeFile)
        {
            Store store = null;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Store));
                using (TextReader textReader = new StreamReader(storeFile))
                {
                    store = (Store)deserializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("not a valid store xml: {0}", ex.Message));
            }

            if (store != null)
            {
                if (store.IsAtmosStore == false)
                {
                    Console.WriteLine(string.Format("    Create store '{0}'", store.StoreName));
                    CreateCIFSStore(store.StoreName, store.StorageLocation, store.PoolCapacity, store.EncryptionType, store.IsCompressed, store.UserName, store.Password);
                }
                else
                {
                    Console.WriteLine(string.Format("    Create ATMOs store '{0}'", store.StoreName));
                    CreateAtmosStore(store.StoreName, store.StorageLocation, store.PoolCapacity, store.EncryptionType, store.IsCompressed, store.UserName, store.Password,
                        store.AtmosServerUrl, store.AtmosSubTenant, store.AtmosPort, store.AtmosSharedSecret, store.AtmosUid, store.AtmosCacheExpireDay);
                }

                Console.WriteLine(string.Format("    Create storage location '{0}'", store.StorageLocation));
                if (!Directory.Exists(store.StorageLocation))
                {
                    try { Directory.CreateDirectory(store.StorageLocation); }
                    catch { } // ignore it.
                }
            }
        }

        /// <summary>
        /// Create a CIFS store
        /// </summary>
        public virtual void CreateCIFSStore(string storeName, string storageLocation, int poolCapacity, EncryptionType encryptionType, bool isCompressed, string userName, string password)
        {
            BlobStoreConfiguration config = BlobStoreConfiguration.GetDefault();
            config.BlobStoreName = storeName;
            config.BlobStoreLocation = storageLocation;
            config.PoolCapacity = poolCapacity;
            config.EncryptionAlgorithm = (short)encryptionType;
            config.IsCompressed = isCompressed;
            config.EncryptionPasscode = "P@ssw0rd";
            // credential
            BlobStoreCredentials credential = new BlobStoreCredentials(userName, Encoding.Unicode.GetBytes(password));
            config.SetCredentials(credential);

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                _currentContentDB.SaveBlobStore(config);
            });
        }

        /// <summary>
        /// Create an ATMOs store
        /// </summary>
        public virtual void CreateAtmosStore(string storeName, string storageLocation, int poolCapacity, EncryptionType encryptionType, bool isCompressed, string userName, string password,
            string atmosServerUrl, string atmosSubTenant, int atmosPort, string atmosSharedSecret, string atmosUid, int atmosCacheExpireDay)
        {
            BlobStoreConfiguration config = BlobStoreConfiguration.GetDefault();
            config.BlobStoreName = storeName;
            config.BlobStoreLocation = storageLocation;
            config.PoolCapacity = poolCapacity;
            config.EncryptionAlgorithm = (short)encryptionType;
            config.IsCompressed = isCompressed;
            config.EncryptionPasscode = "P@ssw0rd";
            // ATMOS relative
            config.BlobStoreType = "SourceOneAtmos";
            config.AtmosServerUrl = atmosServerUrl;
            config.AtmosSubTenant = atmosSubTenant;
            config.AtmosPort = atmosPort;
            config.AtmosSharedSecret = atmosSharedSecret;
            config.AtmosUid = atmosUid;
            config.AtmosCacheExpireDay = atmosCacheExpireDay;

            // credential
            BlobStoreCredentials credential = new BlobStoreCredentials(userName, Encoding.Unicode.GetBytes(password));
            config.SetCredentials(credential);

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                _currentContentDB.SaveBlobStore(config);
            });
        }

        /// <summary>
        /// Set Cache Expire Days
        /// </summary>
        public virtual void SetStoreCacheExpireDay(string storeName, int atmosCacheExpireDays)
        {
            Console.WriteLine(string.Format("    Set ATMOs store '{0}' cache expire day to '{1}' days", storeName, atmosCacheExpireDays));

            if (atmosCacheExpireDays < 0)
                throw new ArgumentException("the AtmosCacheExpireDay should be equal or greater than 0.");

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                BlobStoreConfiguration config = BlobStoreConfiguration.GetDefault();
                config = _currentContentDB.GetStoreConfiguration(storeName);
                config.AtmosCacheExpireDay = atmosCacheExpireDays;
                _currentContentDB.SaveBlobStore(config);
            });
        }

        #endregion

        #region Job

        public virtual void RunJob(string jobName)
        {
            Console.WriteLine(string.Format("    Run job: {0}", jobName));
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                SPJobDefinition theJob = null;
                SPFarm farm = SPFarm.Local;
                foreach (SPService service in farm.Services)
                {
                    if (service is SPWebService)
                    {
                        SPWebService spWebService = service as SPWebService;
                        foreach (SPWebApplication webApp in spWebService.WebApplications)
                        {
                            foreach (SPJobDefinition job in webApp.JobDefinitions)
                            {
                                if (job.Name.Equals(jobName, StringComparison.InvariantCultureIgnoreCase) == true)
                                {
                                    theJob = job;
                                    break;
                                }
                            }

                            if (theJob != null)
                                break;
                        }
                    }

                    if (theJob != null)
                        break;
                }

                if (theJob != null)
                {
                    theJob.RunNow();
                }
                else
                {
                    throw new ApplicationException(string.Format("Job with name '{0}' not found", jobName));
                }
            });
        }

        #endregion
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
}

