using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Core.Model
{
    /// <summary>
    /// All the supported categories here to help to filter the test environments
    /// </summary>
    public static class EnvironmentCategory
    {
        public static readonly string IE10 = "IE10";
        public static readonly string IE11 = "IE11";
        public static readonly string Firefox = "Firefox";
        public static readonly string Chrome = "Chrome";
        public static readonly string WindowsServer2008 = "WindowsServer2008";
        public static readonly string WindowsServer2010 = "WindowsServer2010";
        public static readonly string Java = "Java";
        public static readonly string Ruby = "Ruby";
        public static readonly string VisualStudio2010 = "VisualStudio2010";
    }

    
    public enum EnvironmentType
    {
        Undefined = 0,
        SUTAlone = 1,//System Under Test
        TestAgentAlone = 2,//The machine to run the test case
        Residence_Together = 4,//The test agent is within the SUT
        Residence_Seperate = 8, //The test agent and the SUT are in defferent template
    }

    public enum EnvironmentDeploymentType
    {
        Undefined = 0,
        ToBeCreated = 1,//the environment need to be deployed by environment manager
        Existing = 2,//the environment already exists in outer system, no need to deploy new one
    }

    public class EnvironmentCreateType
    {
        public static string Static = "Permanent";//Select the existing environment from the pool
        public static string Dynamic = "Dynamic";//Create the environment from template dynamically

    }
    /// <summary>
    /// Stands for the domain configurations
    /// </summary>
    public class DomainConfig
    {
        public string Name { get; set; }
        public string Adminstrator { get; set; }
        public string Password { get; set; }
        public DomainConfig(string stringConfig)
        {
            initialize(XElement.Parse(stringConfig));
        }
        public DomainConfig(XElement xmlConfig)
        {
            initialize(xmlConfig);
        }

        private void initialize(XElement xmlConfig)
        {
            this.Name = xmlConfig.Element("name").Value;
            this.Adminstrator = xmlConfig.Element("administrator").Value;
            this.Password = xmlConfig.Element("password").Value;
        }

        public string ToXML()
        {
            XElement domain = new XElement("domain");
            XElement domainName = new XElement("name");
            domainName.SetValue(this.Name);
            domain.Add(domainName);
            XElement domainAdmin = new XElement("administrator");
            domainAdmin.SetValue(this.Adminstrator);
            domain.Add(domainAdmin);
            XElement adminPassword = new XElement("password");
            adminPassword.SetValue(this.Password);
            domain.Add(adminPassword);
            return domain.ToString();
        }
    }

    /// <summary>
    /// The system under test configuration
    /// </summary>
    public class SUTConfig
    {
        //the name of the vCloud template typically
        public string Name { get; set; }
        public EnvironmentDeploymentType DeploymentType { get; set; }
        public DomainConfig SUTDomainConfig { get; set; }
        public List<Machine> Machines { get; set; }
        

        public SUTConfig(string xmlString)
        {
            InitializeFromXElement(XElement.Parse(xmlString));
        }

        public SUTConfig(XElement config)
        {
            InitializeFromXElement(config);
        }

        private void InitializeFromXElement(XElement config)
        {
            this.Name = config.Element("name").Value;
            
            switch (config.Element("type").Value.ToLower())
            {
                case "tobecreated":
                    this.DeploymentType = EnvironmentDeploymentType.ToBeCreated;
                    break;
                case "existing":
                    this.DeploymentType = EnvironmentDeploymentType.Existing;
                    break;
                default:
                    this.DeploymentType = EnvironmentDeploymentType.Undefined;
                    break;
            }

            this.SUTDomainConfig = new DomainConfig(config.Element("domain"));

            this.Machines = new List<Machine>();
            foreach (XElement machine in config.Element("machines").Elements("machine"))
            {
                this.Machines.Add(new Machine(machine));
            }
        }

        public string ToXML()
        {
            XElement sutconfig = new XElement("sutconfig");

            XElement name = new XElement("name");
            name.SetValue(this.Name);

            XElement type = new XElement("type");
            switch (this.DeploymentType)
            {
                case EnvironmentDeploymentType.Existing:
                    type.SetValue("existing");
                    break;
                case EnvironmentDeploymentType.ToBeCreated:
                    type.SetValue("tobecreated");
                    break;
                default:
                    type.SetValue("undefined");
                    break;
            }

            XElement domain = XElement.Parse(this.SUTDomainConfig.ToXML());

            XElement machines = new XElement("machines");
            foreach (Machine m in this.Machines)
            {
                XElement xM = XElement.Parse(m.ToXML());
                machines.Add(xM);
            }
            sutconfig.Add(name);
            sutconfig.Add(type);
            sutconfig.Add(domain);
            sutconfig.Add(machines);
            return sutconfig.ToString();     
        }
    }

    /// <summary>
    /// The test agent configuration
    /// </summary>
    public class TestAgentConfig
    {
        //the name of the vCloud template typically
        public string Name { get; set; }

        public EnvironmentDeploymentType DeploymentType { get; set; }

        public DomainConfig TestAgentDomainConfig { get; set; }

        public List<string> Categories { get; set; }

        public List<Machine> Machines { get; set; }

        public TestAgentConfig(XElement config)
        {
            InitializeTestAgentConfigFromXElement(config);
        }

        public TestAgentConfig(string xmlString)
        {
            InitializeTestAgentConfigFromXElement(XElement.Parse(xmlString));
        }

        private void InitializeTestAgentConfigFromXElement(XElement config)
        {
            this.Name = config.Element("name").Value;

            switch (config.Element("type").Value.ToLower())
            {
                case "tobecreated":
                    this.DeploymentType = EnvironmentDeploymentType.ToBeCreated;
                    break;
                case "existing":
                    this.DeploymentType = EnvironmentDeploymentType.Existing;
                    break;
                default:
                    this.DeploymentType = EnvironmentDeploymentType.Undefined;
                    break;
            }

            this.TestAgentDomainConfig = new DomainConfig(config.Element("domain"));

            this.Categories = new List<string>();
            foreach (string c in config.Element("categories").Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.Categories.Add(c.Trim());
            }

            this.Machines = new List<Machine>();
            foreach (XElement machine in config.Element("machines").Elements("machine"))
            {
                this.Machines.Add(new Machine(machine));
            }
        }

        public string ToXML()
        {
            XElement sutconfig = new XElement("testagentconfig");
            XElement name = new XElement("name");
            name.SetValue(this.Name);
            sutconfig.Add(name);

            XElement type = new XElement("type");
            switch (this.DeploymentType)
            {
                case EnvironmentDeploymentType.Existing:
                    type.SetValue("existing");
                    break;
                case EnvironmentDeploymentType.ToBeCreated:
                    type.SetValue("tobecreated");
                    break;
                default:
                    type.SetValue("undefined");
                    break;
            }
            sutconfig.Add(type);

            XElement domain = XElement.Parse(this.TestAgentDomainConfig.ToXML());
            sutconfig.Add(domain);

            XElement categories = new XElement("categories");
            string sC = string.Empty;
            foreach (string c in this.Categories)
            {
                if (sC == string.Empty)
                {
                    sC = c;
                }
                else
                {
                    sC = sC + "," + c;
                }
            }
            categories.SetValue(sC);
            sutconfig.Add(categories);

            XElement machines = new XElement("machines");
            foreach (Machine m in this.Machines)
            {
                XElement xM = XElement.Parse(m.ToXML());
                machines.Add(xM);
            }            
            sutconfig.Add(machines);
            return sutconfig.ToString();
        }
    }


    /// <summary>
    /// SUTConfiguration is the machines' general information, including the machine name/ip/role
    /// TestAgentConfiguration is used to filter the test agent for the SUT:
    /// 1. If the type is SUT, then the TestAgentConfiguration is the Test Agent Requirement, we'll use the categories to filter the SupportedTestAgent(vCloud template)
    /// 2. If the type is TestAgent, then the categories is the properties of the supported test agent.
    /// </summary>
    public class TestEnvironmentConfigHelper
    {
        public EnvironmentType Type { get; set; }
        public DomainConfig DomainConfiguration { get; set; }
        public SUTConfig SUTConfiguration { get; set; }
        public TestAgentConfig TestAgentConfiguration { get; set; }
        private List<XElement> OtherCustmizedConfigurations { get; set; }

        public TestEnvironmentConfigHelper(string xmlConfig)
        {
            InitializeEnvironmentConfig(XElement.Parse(xmlConfig));
        }

        public TestEnvironmentConfigHelper(XElement config)
        {
            InitializeEnvironmentConfig(config);
        }

        private void InitializeEnvironmentConfig(XElement config)
        {
           
            this.OtherCustmizedConfigurations = new List<XElement>();
            foreach (XElement element  in config.Elements())
            {
                if (element.Name.ToString().ToLower() == "domain")
                {
                    this.DomainConfiguration = new DomainConfig(element);
                }
                else if (element.Name.ToString().ToLower() == "sutconfig")
                {
                    this.SUTConfiguration = new SUTConfig(element);
                }
                else if (element.Name.ToString().ToLower() == "testagentconfig")
                {
                    this.TestAgentConfiguration = new TestAgentConfig(element);
                }
                else if (element.Name.ToString().ToLower() == "type")
                {
                    string environmentType = element.Value;
                    switch (environmentType.ToLower())
                    {
                        case "sut":
                            this.Type = EnvironmentType.SUTAlone;
                            break;
                        case "testagent":
                            this.Type = EnvironmentType.TestAgentAlone;
                            break;
                        case "together":
                            this.Type = EnvironmentType.Residence_Together;
                            break;
                        case "seperate":
                            this.Type = EnvironmentType.Residence_Seperate;
                            break;
                    }
                }
                else
                {
                    this.OtherCustmizedConfigurations.Add(element);
                }
            }
        }

        public string ToXML()
        {
            XElement config = new XElement("config");
            XElement type = new XElement("type");
            string t = string.Empty;
            switch (this.Type)
            {
                case EnvironmentType.Residence_Seperate:
                    t = "seperate";
                    break;
                case EnvironmentType.Residence_Together:
                    t = "together";
                    break;
                case EnvironmentType.SUTAlone:
                    t = "sut";
                    break;
                case EnvironmentType.TestAgentAlone:
                    t = "testagent";
                    break;
            }
            type.SetValue(t);
            XElement domain = XElement.Parse(this.DomainConfiguration.ToXML());
            XElement sutConfig = XElement.Parse(this.SUTConfiguration.ToXML());
            XElement testAgentConfig = XElement.Parse(this.TestAgentConfiguration.ToXML());
            config.Add(type);
            config.Add(domain);
            config.Add(sutConfig);
            config.Add(testAgentConfig);
            foreach (XElement element in this.OtherCustmizedConfigurations)
            {
                config.Add(element);
            }
            return config.ToString();
        }
    }
    
    /// <summary>
    /// Helper class to handle the operation for Environment Configurations
    /// </summary>
    public class EnvironmentConfigHelper
    {
        //TODO
        public static string SetResidenceType(string xmlConfig, EnvironmentType type)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            config.Type = type;
            return config.ToXML();
        }

        public static string SetMachineIP(string xmlConfig, EnvironmentType type, string machineName, string ip)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            //No matter SUT and TestAgent, update the SUT
            if (config.SUTConfiguration != null)
            {
                foreach (Machine m in config.SUTConfiguration.Machines)
                {
                    if (m.Name.ToLower() == machineName.ToLower())
                    {
                        m.IP = ip;
                    }
                }
            }
            if (config.TestAgentConfiguration != null)
            {
                foreach (Machine m in config.TestAgentConfiguration.Machines)
                {
                    if (m.Name.ToLower() == machineName.ToLower())
                    {
                        m.IP = ip;
                    }
                }
            }

            return config.ToXML();
        }

        public static string SetMachineExternalIP(string xmlConfig, EnvironmentType type, string machineName, string ip)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            //No matter SUT and TestAgent, update the SUT
            if (config.SUTConfiguration != null)
            {
                foreach (Machine m in config.SUTConfiguration.Machines)
                {
                    if (m.Name.ToLower() == machineName.ToLower())
                    {
                        m.ExternalIP = ip;
                    }
                }
            }
            if (config.TestAgentConfiguration != null)
            {
                foreach (Machine m in config.TestAgentConfiguration.Machines)
                {
                    if (m.Name.ToLower() == machineName.ToLower())
                    {
                        m.ExternalIP = ip;
                    }
                }
            }

            return config.ToXML();
        }


        public static EnvironmentType GetResidenceType(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            return config.Type;
        }

        public static List<Machine> GetTestAgents(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            if (config.TestAgentConfiguration != null)
            {
                return config.TestAgentConfiguration.Machines;
            }
            else
            {
                return null;
            }
        }

        public static List<Machine> GetSUTMachines(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            if (config.SUTConfiguration != null)
            {
                return config.SUTConfiguration.Machines;
            }
            else
            {
                return null;
            }
        }

        public static string GetDomainName(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            return config.DomainConfiguration.Name;
        }

        public static string GetDomainAdministrator(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            return config.DomainConfiguration.Adminstrator;
        }

        public static string GetDomainPassword(string xmlConfig)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            return config.DomainConfiguration.Password;
        }

        public static string GetTestAgentIp(string xmlConfig, string agentName)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            foreach (Machine m in config.TestAgentConfiguration.Machines)
            {
                if (m.Name.ToLower() == agentName.ToLower())
                {
                    return m.IP;
                }
            }
            return null;
        }

        public static string GetSUTIp(string xmlConfig, string machineName)
        {
            TestEnvironmentConfigHelper config = new TestEnvironmentConfigHelper(xmlConfig);
            foreach (Machine m in config.SUTConfiguration.Machines)
            {
                if (m.Name.ToLower() == machineName.ToLower())
                {
                    return m.IP;
                }
            }
            return null;
        }

    }
}
