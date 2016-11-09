using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saber.S1CommonAPILib
{
    public class S1Worker : IS1Object
    {
        public String Name { get; set; }
        public int Id { get; set; }

        public bool DeserializeFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool DeserializeFromXElement(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
