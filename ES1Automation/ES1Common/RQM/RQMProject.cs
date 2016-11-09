using System.Xml.Linq;

namespace ES1Common.RQM
{
    public class RQMProject
    {
        public string Title { get; set; }

        public string Alias { get; set; }

        public RQMProject()
        { }

        public RQMProject(XElement xmlString)
        {
            Title = xmlString.Element(RQMServer.XN3 + "title").Value;

            Alias = xmlString.Element(RQMServer.XN2 + "alias").Value;
        }
    }
}
