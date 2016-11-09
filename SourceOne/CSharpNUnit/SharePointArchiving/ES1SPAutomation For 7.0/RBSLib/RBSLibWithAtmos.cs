using System;
using System.IO;
using System.Xml.Serialization;

namespace RBSLib
{
    public class RBSLibWithAtmos : RBSBasicLib
    {
        public RBSLibWithAtmos(string farmServerName, string contentDBName)
            : base(farmServerName, contentDBName)
        {
        }

        /// <summary>
        /// Set the cache expiration days
        /// </summary>
        public virtual void SetStoreCacheExpireDay(string storeName, int days)
        {
            RemoteExecution.Execute(_serverName, AgentPath, string.Format("-operation SetCacheExpireDays -db {0} -storename {1} -ced {2}", _contentDBName, storeName, days));
        }

        /// <summary>
        /// Create a store with ATMOs configured
        /// </summary>
        public virtual void CreateAtmosStore(string storeName, string storageLocation, int poolCapacity, EncryptionType encryptionType, bool isCompressed, string userName, string password,
            string atmosServerUrl, string atmosSubTenant, int atmosPort, string atmosSharedSecret, string atmosUid, int atmosCacheExpireDay)
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
            store.IsAtmosStore = true;

            store.AtmosServerUrl = atmosServerUrl;
            store.AtmosSubTenant = atmosSubTenant;
            store.AtmosPort = atmosPort;
            store.AtmosSharedSecret = atmosSharedSecret;
            store.AtmosUid = atmosUid;
            store.AtmosCacheExpireDay = atmosCacheExpireDay;

            XmlSerializer serializer = new XmlSerializer(typeof(Store));

            MemoryStream ms = new MemoryStream();
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, store);
            string XML = writer.ToString();
            CreateStore(XML);
        }

    }
}
