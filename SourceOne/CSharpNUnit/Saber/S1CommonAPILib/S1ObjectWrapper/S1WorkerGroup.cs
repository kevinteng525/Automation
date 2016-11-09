using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Saber.Common;

namespace Saber.S1CommonAPILib
{
    public class S1WorkerGroup : IS1Object
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public List<String> Workers { get; set; }

        public S1WorkerGroup()
        {
            this.Description = "Create by Saber at " + DateTime.Now.ToString();
            this.Workers = new List<string>();
        }

        public bool DeserializeFromXMLFile(String filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("The file " + filePath + " can not be found!");
            }
            XDocument document = XDocument.Load(filePath);
            XElement root = document.Root;
            return DeserializeFromXElement(root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String name = XMLHelper.GetElementValue(element,"name");
            String description = XMLHelper.GetElementValue(element, "description");
            List<String> workers = new List<string>();
            XElement workerElements = element.Element("workers");
            foreach (XElement worker in workerElements.Elements())
            {
                workers.Add(worker.Value);
            }
            if (!String.IsNullOrEmpty( name))
            {
                this.Name = name;
            }
            if (!String.IsNullOrEmpty(description))
            {
                this.Description = description;
            }
            if (workers.Count > 0)
            {
                this.Workers = workers;
            }
            return true;
        }

        public static List<S1WorkerGroup> GetAllWorkerGroup()
        { 
            //TODO
            throw new NotImplementedException();
        }
        public static S1WorkerGroup GetWorkerGroupByID(int groupID)
        {
            //TODO
            throw new NotImplementedException();
        }
        public static S1WorkerGroup GetWorkerGroupByName(string groupName)
        {
            //TODO
            throw new NotImplementedException();
        }
        public static bool UpdateWorkerGroup(S1WorkerGroup group)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
