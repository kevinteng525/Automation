using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Saber.Common;

namespace Saber.S1CommonAPILib
{

    public class S1OrganizationalPolicy : IS1Object
    {
        /// <summary>
        /// the policy name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the policy description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// create a policy object without specify any values
        /// </summary>
        public S1OrganizationalPolicy()
        {
 
        }

        /// <summary>
        /// Create policy offering the name
        /// if the policy with the same name exists already, return it; else create a new one
        /// </summary>
        /// <param name="name"></param>
        public S1OrganizationalPolicy(string name)
        {
            this.Name = name;
            this.Description = name;
        }

        /// <summary>
        /// Create policy offering the name and description
        /// if the policy with the same name exists already, return it; else create a new one
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public S1OrganizationalPolicy(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }


        public bool DeserializeFromXMLFile(string filePath)
        {
            if(!System.IO.File.Exists(filePath))
            {
                throw new Exception("File not found: " + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            XElement root = document.Root;
            return DeserializeFromXElement(root);
            
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String name = XMLHelper.GetElementValue(element,"name");
            String description = XMLHelper.GetElementValue(element, "description");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }
            return true;
        }
    }
}
