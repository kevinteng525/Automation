using System.Xml.Linq;

namespace RequestLib.Requests
{
    public class RunCMDRequest : Request
    {
        public string Filename { get; set; }

        public string CMDScript { get; set; }

        public string Domain { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public RunCMDRequest(string server, int port)
            : base(server, port)
        {
            Type = "CMD";
        }

        protected override void SetParams()
        {
            base.SetParams();

            RequestXML.Add(new XAttribute("filename", Filename));
            RequestXML.Add(new XAttribute("domain", Domain ?? string.Empty));
            RequestXML.Add(new XAttribute("username", Username ?? string.Empty));
            RequestXML.Add(new XAttribute("password", Password ?? string.Empty));

            RequestXML.Value = CMDScript;
        }
    }
}
