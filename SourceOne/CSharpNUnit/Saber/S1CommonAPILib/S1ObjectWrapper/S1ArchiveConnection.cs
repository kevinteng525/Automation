using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Saber.Common;

namespace Saber.S1CommonAPILib
{
    public class S1ArchiveConnection : IS1Object
    {
        /// <summary>
        /// Archive connection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Archive connection description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Archive connection type
        /// Three types of connection supported: 
        ///     EMail Xtender 4.X
        ///     Native Archive
        ///     In Place Migrated Native Archive
        /// The native archive connection is created by default
        /// </summary>
        public S1ArchiveConnectionType ArchiveConnectionType { get; set; }

        /// <summary>
        /// Database Server of the archive
        /// </summary>
        public string DatabaseServer { get; set; }

        /// <summary>
        /// Database name of the archive
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Default construction
        /// </summary>
        public S1ArchiveConnection()
        {
            this.ArchiveConnectionType = S1ArchiveConnectionType.NativeArchive;
        }

        /// <summary>
        /// Create archive connection
        /// </summary>
        /// <param name="name">the connection name</param>
        /// <param name="dbServer">the database server</param>
        /// <param name="dbName">the database name</param>
        public S1ArchiveConnection(string name, string dbServer, string dbName)
        {
            this.ArchiveConnectionType = S1ArchiveConnectionType.NativeArchive;
            this.Name = name;
            this.DatabaseServer = dbServer;
            this.DatabaseName = dbName;

        }

        public bool DeserializeFromXMLFile(String filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Can not find the file: " + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String name = XMLHelper.GetElementValue(element, "name");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            else
            {
                throw new Exception("The name must be specified.");
            }
            String description = XMLHelper.GetElementValue(element, "description");
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }

            String connectiontype = XMLHelper.GetElementValue(element, "connectiontype");
            if (!String.IsNullOrEmpty(connectiontype))
            {
                S1ArchiveConnectionType type = S1ArchiveConnectionType.Unknown;
                switch (connectiontype.ToLower())
                {
                    case "emailxtender4x":
                        type = S1ArchiveConnectionType.EMailXtender4X;
                        break;
                    case "nativearchive":
                        type = S1ArchiveConnectionType.NativeArchive;
                        break;
                    case "inplacemigratednativearchive":
                        type = S1ArchiveConnectionType.InPlaceMigratedNativeArchive;
                        break;
                    default:
                        throw new Exception("The connection type: " + connectiontype + " is not valid.");
                }
                this.ArchiveConnectionType  = type;
            }
            else
            {
                throw new Exception("The connection type must be specified.");
            }
            String databaseserver = XMLHelper.GetElementValue(element, "databaseserver");
            if (!String.IsNullOrEmpty(databaseserver))
            {
                this.DatabaseServer = databaseserver;
            }
            else
            {
                throw new Exception("The archive server must be specified.");
            }
            String databasename = XMLHelper.GetElementValue(element, "databasename");
            if (!String.IsNullOrEmpty(databasename))
            {
                this.DatabaseName = databasename;
            }
            else
            {
                throw new Exception("The archive database must be specified.");
            }
            return true;
        }
    }
}
