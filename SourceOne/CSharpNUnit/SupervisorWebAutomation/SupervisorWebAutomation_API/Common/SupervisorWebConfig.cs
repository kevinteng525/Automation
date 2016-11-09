using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

using SupervisorWebAutomation_API.Common;


namespace SupervisorWebAutomation_API.Config
{
    public enum AccountRole
    {
        Supervisor = 1,
        Reviewer = 2,
        Member = 4,
    }

    public class SupervisorWebConfig
    {
        private static string _url = string.Empty;
        private static string _user = string.Empty;
        private static string _password = string.Empty;
        private static string configPath = @"C:\SaberAgent\Config\Environment.xml";
        private static XDocument _config;

        public static XDocument Config
        {
            get
            {
                if (_config==null)
                {
                    Initialize();
                    return _config;
                }
                else
                {
                    return _config;
                }
            }
        }

        public static string URL
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    Initialize();
                    return _url;
                }
                else
                {
                    return _url;
                }
            }
        }

        public static string User
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    Initialize();
                    return _user;
                }
                else
                {
                    return _user;
                }
            }
        }

        public static string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    Initialize();
                    return _password;
                }
                else
                {
                    return _password;
                }
            }
        }

        private static void Initialize()
        {
            //string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            //UriBuilder uri = new UriBuilder(codeBase);
            //string path = Uri.UnescapeDataString(uri.Path);
            //string configPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), @"config\SupervisorWebConfig.xml");

            if (System.IO.File.Exists(configPath))
            {
                _config = XDocument.Load(configPath);

                _user = _config.Root.Element("sutconfig").Element("domain").Element("administrator").Value;

                _password = _config.Root.Element("sutconfig").Element("domain").Element("password").Value;

                foreach (XElement machine in _config.Root.Element("sutconfig").Element("machines").Elements())
                {
                    if (machine.Element("roles").Value.Contains("Supervisor"))
                    {
                        _url = string.Format("https://{0}/SupervisorWeb", machine.Element("externalip").Value);

                        break;
                    }                    
                }
            }
            else
            {
                throw new Exception(string.Format("The config file {0} does not exist.", configPath));
            }
        }

        public static Account GetAccountByRole(AccountRole role)
        {
            if (string.IsNullOrEmpty(_url))
            {
                Initialize();
            }
            foreach (XElement account in _config.Root.Element("testdata").Elements("account"))
            {
                if (account.Attributes("role").Count() > 0 && account.Attribute("role").Value.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(role.ToString().ToLower()))
                {
                    return new Account() { UserName = account.Attribute("username").Value, Password = account.Attribute("password").Value, RememberMe = true };
                }
            }
            return null;
        }

        public static List<XElement> GetAllMessages()
        {
            if (string.IsNullOrEmpty(_url))
            {
                Initialize();
            }

            List<XElement> messages = new List<XElement>();

            foreach (XElement message in _config.Root.Element("testdata").Element("messages").Elements("message"))
            {
                messages.Add(message);
            }

            return messages;
 
        }
    }
}
