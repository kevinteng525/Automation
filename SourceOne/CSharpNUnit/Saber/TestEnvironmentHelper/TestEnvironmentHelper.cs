using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Saber.TestEnvironment
{
    public static class TestEnvironmentHelper
    {
        #region private member
        //Galaxy will generate Environment.xml under C:\SaberAgent\Config.
        private static String environmentConfigFile = "C:\\SaberAgent\\Config\\Environment.xml";
        private static List<S1ComponentHost> hosts = null;
        private static String domainName = "";
        private static String domainAdministrator = "";
        private static String domainAdminPassword = "";
        private static String defaultAccount = "";
        private static String dbServer = String.Empty;
        private static String archiveDB = String.Empty;
        private static String activityDB = String.Empty;
        private static String searchDB = String.Empty;
        #endregion

        #region public properties
        public static String DomainName 
        {
            get
            {
                return domainName;
            }
            private set
            {
                domainName = value;
            } 
        }
        public static String DomainAdministrator
        {
            get
            {
                return domainAdministrator;
            }
            private set
            {
                domainAdministrator = value;
            }
        }
        public static String DomainAdminPassword
        {
            get
            {
                return domainAdminPassword;
            }
            private set
            {
                domainAdminPassword = value;
            }
        }
        public static String DefaultAccount
        {
            get
            {
                return defaultAccount;
            }
            private set
            {
                defaultAccount = value;
            }
        }
        public static String DBServer
        {
            get
            {
                return dbServer;

            }
            private set
            {
                dbServer = value;
            }
        }
        public static String ArchiveDB
        {
            get
            {
                return archiveDB;

            }
            private set
            {
                archiveDB = value;
            }
        }
        public static String ActivityDB
        {
            get
            {
                return activityDB;
            }
            set
            {
                activityDB = value;
            }
        }
        public static String SearchDB
        {
            get
            {
                return searchDB;
            }
            set
            {
                searchDB = value;
            }
        }
        #endregion


        static TestEnvironmentHelper()
        {   

            XDocument document = XDocument.Load(environmentConfigFile);

            if (document == null)
            {
                Console.WriteLine("Null");
            }

            XElement root = document.Root;//environment
            XElement machines = root.Element("sutconfig").Element("machines");//machines
            InitializeHosts(machines);
            XElement domain = root.Element("domain");
            InitializeDomain(domain);
            //TODO, initialze the s1 configurations
            XElement s1configs = root.Element("s1configs");
            InitializeS1Config(s1configs);
        }

        public static List<S1ComponentHost> GetHostsWithS1ComponentType(S1HostType hostType)
        {
            List<S1ComponentHost> hostsList = new List<S1ComponentHost>();
            foreach (S1ComponentHost host in hosts)
            {
                if ((host.HostType & (int)hostType) > 0)
                {
                    hostsList.Add(host);
                }
            }
            return hostsList;
        }


        #region private members
        private static void InitializeS1Config(XElement s1configs)
        {
            defaultAccount = s1configs.Element("defaultaccount").Value;
            dbServer = s1configs.Element("dbserver").Value;
            archiveDB = s1configs.Element("archivedb").Value;
            activityDB = s1configs.Element("activitydb").Value;
            searchDB = s1configs.Element("searchdb").Value;
        }

        private static void InitializeDomain(XElement domain)
        {
            DomainName = domain.Element("name").Value;
            DomainAdministrator = domain.Element("administrator").Value;
            DomainAdminPassword = domain.Element("password").Value;
        }

        private static void InitializeHosts(XElement machines)
        {
            hosts = new List<S1ComponentHost>();
            foreach (XElement machine in machines.Elements("machine"))
            {
                String name = machine.Element("name").Value;
                String ip = machine.Element("ip").Value;
                String description = machine.Element("description").Value;
                String types = machine.Element("roles").Value;
                XElement config = machine.Element("config");
                S1ComponentHost host = new S1ComponentHost();
                host.Name = name;
                host.IP = ip;
                host.Description = description;
                foreach (string tempType in types.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string type = tempType.Replace("(Installed)", "").Trim();//remove the postfix which indicates whether this component is installed in template instead of during the execution of task
                    switch (type)
                    {
                        case "Master":
                            host.HostType |= (int)S1HostType.Master;
                            host.Configs.Add(new S1MasterConfig(config));
                            break;
                        case "ExchangeServer":
                            host.HostType |= (int)S1HostType.ExchangeServer;
                            host.Configs.Add(new S1ExchangeServerConfig(config));
                            break;
                        case "DomainController":
                            host.HostType |= (int)S1HostType.DomainController;
                            host.Configs.Add(new S1DomainControllerConfig(config));
                            break;
                        case "NativeArchive":
                            host.HostType |= (int)S1HostType.NativeArchive;
                            break;
                        case "Worker":
                            host.HostType |= (int)S1HostType.Worker;
                            break;
                        case "WebService":
                            host.HostType |= (int)S1HostType.WebService;
                            break;
                        case "DMServer":
                            break;
                        case "DMClient":
                            break;
                        case "SQLServer":
                            host.HostType |= (int)S1HostType.SQLServer;
                            host.Configs.Add(new S1SQLServerConfig(config));
                            break;
                        case "FileServer":
                            break;
                        case "SCOMServer":
                            break;
                        case "SharePointServer":
                            break;
                        case "Search":
                            host.HostType |= (int)S1HostType.Search;
                            break;
                        case "Mobile":
                            host.HostType |= (int)S1HostType.Mobile;
                            break;
                        case "Console":
                            host.HostType |= (int)S1HostType.Console;
                            break;
                        case "DMWeb":
                            host.HostType |= (int)S1HostType.DiscoveryManagerWeb;
                            break;
                        default:
                            break;
                    }
                }
                AddHost(host);
            }
        }

        private static void AddHost(S1ComponentHost host)
        {
            foreach (S1ComponentHost h in hosts)
            {
                if (host.Name == h.Name)
                {
                    h.HostType |= host.HostType;
                    if (host.Configs.Count > 0)
                    {
                        h.Configs.Add(host.Configs[0]);
                    }
                    return;
                }
            }
            hosts.Add(host);
        }

        #endregion
        
    }
}
