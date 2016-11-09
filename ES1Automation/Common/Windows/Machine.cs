using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Common.Windows
{
    public class Machine
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public string ExternalIP { get; set; }
        public List<KeyValuePair<string,bool>> Roles { get; set; }
        public List<string> Categories { get; set; }
        public string Description { get; set; }
        public string Config { get; set; }
        public string Domain { get; set; }
        public string Administrator { get; set; }
        public string Password { get; set; }

        public Machine(XElement node)
        {
            this.Name = node.Element("name").Value;
            this.IP = node.Element("ip").Value;
            if (node.Element("externalip") != null)
            {
                this.ExternalIP = node.Element("externalip").Value;
            }
            else
            {
                this.ExternalIP = string.Empty;
            }
            this.Roles = new List<KeyValuePair<string, bool>>();
            foreach (string role in node.Element("roles").Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string r = role.IndexOf('(') > 0 ? role.Substring(0, role.IndexOf('(')) : role;
                KeyValuePair<string, bool> pair = new KeyValuePair<string, bool>(r.Trim(), role.ToLower().Contains("(installed)"));
                this.Roles.Add(pair);
            }
            this.Categories = new List<string>();
            foreach (string category in node.Element("categories").Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.Categories.Add(category);
            }
            this.Description = node.Element("description").Value;
            this.Config = node.Element("config").ToString();
            if (node.Element("domain") != null)
            {
                this.Domain = node.Element("domain").Value;
            }
            if (node.Element("administrator") != null)
            {
                this.Administrator = node.Element("administrator").Value;
            }
            if (node.Element("password") != null)
            {
                this.Password = node.Element("password").Value;
            }
        }

        public string ToXML()
        {
            XElement machine = new XElement("machine");
            XElement name = new XElement("name");
            name.SetValue(this.Name);
            XElement ip = new XElement("ip");
            ip.SetValue(this.IP);
            XElement externalip = new XElement("externalip");
            externalip.SetValue(this.ExternalIP);
            XElement description = new XElement("description");
            description.SetValue(this.Description);
            XElement config = XElement.Parse(this.Config);
            XElement roles = new XElement("roles");
            string r = string.Empty;
            foreach (KeyValuePair<string, bool> role in this.Roles)
            {
                string temp = role.Key + (role.Value ? "(Installed)" : "");//The toXML is only used when we add the xml into DB, no need to add the info whether it's new installed or installed in template
                if (r == string.Empty)
                {
                    r = temp;
                }
                else
                {
                    r += "," + temp;
                }
            }
            roles.SetValue(r);
            XElement categories = new XElement("categories");
            string c = string.Empty;
            foreach (string category in this.Categories)
            {
                if (c == string.Empty)
                {
                    c = category;
                }
                else
                {
                    c += "," + category;
                }
            }
            categories.SetValue(c);
            XElement domain = new XElement("domain");
            if (this.Domain != null)
            {
                domain.SetValue(this.Domain);
            }
            XElement administrator = new XElement("administrator");
            if (this.Administrator != null)
            {
                administrator.SetValue(this.Administrator);
            }
            XElement password = new XElement("password");
            if (this.Password != null)
            {
                password.SetValue(this.Password);
            }            
            machine.Add(name);
            machine.Add(ip);
            machine.Add(externalip);
            machine.Add(description);
            machine.Add(config);
            machine.Add(roles);
            machine.Add(domain);
            machine.Add(administrator);
            machine.Add(password);
            machine.Add(categories);
            
            return machine.ToString();
        }

    }
}
