using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Saber.S1CommonAPILib
{
    public interface IS1Object
    {
        bool DeserializeFromXMLFile(String filePath);
        bool DeserializeFromXElement(XElement element);
    }
}
