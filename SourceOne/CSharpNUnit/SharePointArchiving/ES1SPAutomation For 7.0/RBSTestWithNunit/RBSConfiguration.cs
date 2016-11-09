using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBSLib;
using System.IO;
using System.Xml;

namespace RBSTestWithNunit
{
    public class RBSConfiguration
    {
        #region Get Configuration

        public static RBSConfiguration GetConfiguration(string configXmlFilePath)
        {
            RBSConfiguration config = new RBSConfiguration(configXmlFilePath);
            return config;
        }

        #endregion

        #region Export

        public string ServerName { get; protected set; }
        public string ContentDatabaseName { get; protected set; }

        public string ServerUserName { get; protected set; }
        public string ServerPassword { get; protected set; }

        public string SiteTitle { get; protected set; }
        public string ListTitle { get; protected set; }

        public bool RestartSharePointService { get; protected set; }

        public bool HaveNotYetRestarted { get; set; }

        public Store GetStore(string storeNameInXML)
        {
            Store store = null;
            if (_storeMap.ContainsKey(storeNameInXML))
            {
                store = _stores[_storeMap[storeNameInXML]];
            }
            return store;
        }

        public bool DeleteSPList { get; protected set; }
        public bool DeleteSPItems { get; protected set; }
        public bool DeleteStores { get; protected set; }
        public bool DeleteStoreStoragePaths { get; protected set; }

        public string GetConnectionString(string name)
        {
            string value = null;
            if (_connectionStrings.ContainsKey(name))
            {
                value = _connectionStrings[name];
            }
            return value;
        }

        #endregion

        protected Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();

        protected Dictionary<string, Store> _stores = new Dictionary<string, Store>();

        protected Dictionary<string, string> _storeMap = new Dictionary<string, string>();

        protected RBSLib.RBSBasicLib _baseLib = null;

        protected string _storeUserName = null;
        protected string _storePassword = null;

        private RBSConfiguration(string configXmlFilePath)
        {
            // Build Seeds
            string DateSeed = DateTime.Now.ToString("yyyy_MM_dd");
            string DateTimeSeed = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string RandomSeed = new Random(DateTime.Now.Millisecond).Next(10000).ToString();
            // End Seeds

            if (File.Exists(configXmlFilePath) == false)
                throw new ApplicationException(string.Format("File '{0}' not exists.", configXmlFilePath));

            XmlDocument doc = new XmlDocument();
            doc.Load(configXmlFilePath);

            XmlNode rbs = doc.SelectSingleNode("/ES1TestData/RBS");
            if (rbs == null)
                throw new ApplicationException("Not a valid config xml, '/ES1TestData/RBS' node not found.");

            ServerName = rbs.Attributes["ServerName"].Value;
            ContentDatabaseName = rbs.Attributes["ContentDatabaseName"].Value;
            ServerUserName = rbs.Attributes["UserName"].Value;
            ServerPassword = rbs.Attributes["Password"].Value;

            // obtain Seed information
            bool useSeed = false;
            string Seed = string.Empty;
            XmlNode seedNode = rbs.SelectSingleNode("Seed");
            if (seedNode != null)
            {
                useSeed = bool.Parse(seedNode.Attributes["UseSeed"].Value);
                if (useSeed)
                {
                    // add this SeedFormat to List Title, Store Name, StoragePath
                    string seedFormat = seedNode.Attributes["SeedFormat"].Value;
                    Seed = seedFormat.Replace("{datetime}", DateTimeSeed).Replace("{date}", DateSeed).Replace("{randomnumber}", RandomSeed);
                }
            }

            // site and list
            XmlNode siteNode = rbs.SelectSingleNode("Site");
            XmlNode listNode = rbs.SelectSingleNode("List");

            if (siteNode == null)
                throw new ApplicationException("Not a valid config xml, '/ES1TestData/RBS/Site' node not found.");
            if (siteNode == null)
                throw new ApplicationException("Not a valid config xml, '/ES1TestData/RBS/List' node not found.");

            SiteTitle = siteNode.Attributes["Title"].Value;
            ListTitle = listNode.Attributes["Title"].Value + Seed;

            // restart SharePoint Service as need
            HaveNotYetRestarted = true;
            RestartSharePointService = false;
            XmlNode restartSPNode = rbs.SelectSingleNode("RestartSharePointService");
            if (restartSPNode != null)
                RestartSharePointService = bool.Parse(restartSPNode.Attributes["Restart"].Value);

            // get all connection strings
            XmlNode connStringsNode = rbs.SelectSingleNode("ConnectionStrings");
            if (connStringsNode != null)
            {
                XmlNodeList connStringNodes = connStringsNode.SelectNodes("//ConnectionString");
                foreach (XmlNode node in connStringNodes)
                {
                    string name = node.Attributes["Name"].Value;
                    string value = node.Attributes["Value"].Value;
                    _connectionStrings[name] = value;
                }
            }
            
            // all stores

            XmlNode storesNode = rbs.SelectSingleNode("Stores");
            if (storesNode == null)
                throw new ApplicationException("Not a valid config xml, '/ES1TestData/RBS/Stores' node not found.");

            _storeUserName = storesNode.Attributes["UserName"].Value;
            _storePassword = storesNode.Attributes["Password"].Value;

            // get all Atmos servers
            Dictionary<string, AtmosServer> atmosServers = new Dictionary<string, AtmosServer>();
            XmlNodeList atmosServerNodes = doc.SelectNodes("//AtmosServer");
            foreach (XmlNode node in atmosServerNodes)
            {
                string name = node.Attributes["Name"].Value;
                atmosServers[name] = new AtmosServer(name,
                   node.Attributes["Server"].Value,
                   int.Parse(node.Attributes["Port"].Value),
                   node.Attributes["SubTenantID"].Value,
                   node.Attributes["UID"].Value,
                   node.Attributes["SharedSecret"].Value);
            }

            // CIFS stores
            XmlNodeList storeNodes = doc.SelectNodes("//Store");
            foreach (XmlNode node in storeNodes)
            {
                string storeNameInXML = node.Attributes["Name"].Value;

                string storeName = storeNameInXML + Seed;
                string storageLocation = node.Attributes["StorageLocation"].Value + Seed;
                int poolCapacity = int.Parse(node.Attributes["PoolCapacity"].Value);
                bool isCompressed = bool.Parse(node.Attributes["IsCompressed"].Value);
                EncryptionType encryptionType = (EncryptionType)Enum.Parse(typeof(EncryptionType), node.Attributes["EncryptionType"].Value);

                Store store = new Store();
                store.StoreName = storeName;
                store.StorageLocation = storageLocation;
                store.PoolCapacity = poolCapacity;
                store.EncryptionType = encryptionType;
                store.IsCompressed = isCompressed;
                store.UserName = _storeUserName;
                store.Password = _storePassword;
                store.Passcode = "P@ssw0rd";
                store.IsAtmosStore = false;

                _stores[storeName] = store;
                _storeMap[storeNameInXML] = storeName;
            }

            // Atmos stores
            XmlNodeList atmosStoreNodes = doc.SelectNodes("//AtmosStore");
            foreach (XmlNode node in atmosStoreNodes)
            {
                string storeNameInXML = node.Attributes["Name"].Value;
                string storeName = storeNameInXML + Seed;
                string storageLocation = node.Attributes["StorageLocation"].Value + Seed;
                int poolCapacity = int.Parse(node.Attributes["PoolCapacity"].Value);
                bool isCompressed = bool.Parse(node.Attributes["IsCompressed"].Value);
                EncryptionType encryptionType = (EncryptionType)Enum.Parse(typeof(EncryptionType), node.Attributes["EncryptionType"].Value);

                int cacheExpireDays = int.Parse(node.Attributes["CacheExpireDays"].Value);
                string atmosServerName = node.Attributes["AtmosServer"].Value;

                if (atmosServers.ContainsKey(atmosServerName) == false)
                    throw new ApplicationException(string.Format("Atmos server '{0}' not found for Atmos store '{1}'", atmosServerName, node.Attributes["Name"].Value));
                AtmosServer server = atmosServers[atmosServerName];

                Store store = new Store();
                store.StoreName = storeName;
                store.StorageLocation = storageLocation;
                store.PoolCapacity = poolCapacity;
                store.EncryptionType = encryptionType;
                store.IsCompressed = isCompressed;
                store.UserName = _storeUserName;
                store.Password = _storePassword;
                store.Passcode = "P@ssw0rd";
                store.IsAtmosStore = true;
                store.AtmosCacheExpireDay = cacheExpireDays;

                store.AtmosServerUrl = server.Server;
                store.AtmosPort = server.Port;
                store.AtmosSubTenant = server.SubTenantID;
                store.AtmosUid = server.UID;
                store.AtmosSharedSecret = server.SharedSecret;

                _stores[storeName] = store;
                _storeMap[storeNameInXML] = storeName;
            }

            // Cleanup
            DeleteSPList = true; DeleteSPItems = true; DeleteStores = true; DeleteStoreStoragePaths = true;
            XmlNode cleanupNode = rbs.SelectSingleNode("Cleanup");
            if (cleanupNode != null)
            {
                DeleteSPList = bool.Parse(cleanupNode.Attributes["DeleteSPList"].Value);
                DeleteSPItems = bool.Parse(cleanupNode.Attributes["DeleteSPItems"].Value);
                DeleteStores = bool.Parse(cleanupNode.Attributes["DeleteStores"].Value);
                DeleteStoreStoragePaths = bool.Parse(cleanupNode.Attributes["DeleteStoreStoragePaths"].Value);
            }
        }

        
    }

    public class AtmosServer
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string SubTenantID { get; set; }
        public string UID { get; set; }
        public string SharedSecret { get; set; }

        public AtmosServer(string name, string server, int port, string subTenantID, string uID, string sharedSecret)
        {
            Name = name;
            Server = server;
            Port = port;
            SubTenantID = subTenantID;
            UID = uID;
            SharedSecret = sharedSecret;
        }
    }
}
