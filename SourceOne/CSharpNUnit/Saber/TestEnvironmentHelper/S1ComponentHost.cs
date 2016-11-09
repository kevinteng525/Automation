using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Saber.TestEnvironment
{
    public enum S1HostType
    {
        Undefined = 0,
        Master = 1,
        NativeArchive = 2,
        Worker = 4,
        WebService = 8,
        Console = 16,
        Search = 32,
        DiscoveryManagerServer = 64,
        DiscoveryManagerClient = 128,
        SupervisorServer = 256,
        SupervisorClient = 512,
        DomainController = 1024,
        ExchangeServer = 2048,
        FileServer = 4096,
        SharePoint = 8192,
        SQLServer = 16384,
        Mobile = 32768,
        DiscoveryManagerWeb = 65536,
    }

    public abstract class S1ComponentConfig
    {
        public S1HostType HostType = S1HostType.Undefined;      

    }

    public class S1MasterConfig : S1ComponentConfig
    {
        
        public S1MasterConfig(XElement config)
        {
            HostType = S1HostType.Master;
            //TODO, add the specific config for the master
        }
        
    }

    public class S1ExchangeServerConfig : S1ComponentConfig
    {
        
        public String ExchangeVersion { get; set; }
        //Other exchange config
        public S1ExchangeServerConfig(XElement config)
        {
            HostType = S1HostType.ExchangeServer; 
            String[] validExchangeVersions = { "Exchange2007_SP1", "Exchange2010", "Exchange2010_SP1","Exchange2010_SP2","Exchange2013" };
            String version = config.Element("version").Value;
            if (String.IsNullOrEmpty(version))
            {
                throw new Exception("Exchange server version can not be gotten!");
            }
            else if (!validExchangeVersions.Contains(version))
            {
                throw new Exception("Exchange server version is not valid!");
            }
            else
            {
                this.ExchangeVersion = version;
            }

            //TODO
        }
    }

    public class S1DomainControllerConfig : S1ComponentConfig
    {
        public String DomainName { get; set; }
        public String Administrator { get; set; }
        public String Password { get; set; }
        //Other domain controller config
        public S1DomainControllerConfig(XElement config)
        {
            HostType = S1HostType.DomainController;
            //TODO
        }
    }
    
    public class S1SQLServerConfig : S1ComponentConfig
    {

        //Other SQL server config
        public S1SQLServerConfig(XElement config)
        {
            HostType = S1HostType.SQLServer;
            
        }
    }

    public class S1ComponentHost
    {
        public String Name { get; internal set; }
        public String FullName {
            get 
            {
                return Name + "." + TestEnvironmentHelper.DomainName;
            }            
        }
        public String IP { get; internal set; }
        public String Description { get; internal set; }
        public int HostType { get; internal set; }
        public List<S1ComponentConfig> Configs { get; internal set; }

        public S1ComponentHost()
        {
            this.Configs = new List<S1ComponentConfig>();
        }

        public S1ComponentConfig GetConfigOfType(S1HostType type)
        {
            foreach (S1ComponentConfig config in Configs)
            {
                if (config.HostType == type)
                {
                    return config;
                }
            }
            return null;
        }

    }

}
