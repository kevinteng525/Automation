using System;
using System.Xml.Linq;

namespace ATFExchangeCentre.Requests
{
    public class CMDRequest : Request
    {
        public string FileName { get; set; }

        public string CMDScript { get; set; }

        public string Domain { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string CMDResult { get; protected set; }

        public CMDRequest()
        {
            Type = Request.CMD;
        }

        public CMDRequest(XElement root)
            : base(root)
        {
            FileName = RequestFactory.GetAttributeValue(RequestRoot, "filename");
            Domain = RequestFactory.GetAttributeValueOptional(RequestRoot, "domain");
            Username = RequestFactory.GetAttributeValueOptional(RequestRoot, "username");
            Password = RequestFactory.GetAttributeValueOptional(RequestRoot, "password");
            CMDScript = RequestRoot.Value;
            CMDResult = ResponseRoot.Value;
        }

        protected override void CheckParams()
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                throw new Exception("Filename should not be null");
            }
        }

        protected override void HandleRequest()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                CMDResult = Machine.RunCMDSript(FileName, CMDScript);
            }

            CMDResult = Machine.RunCMDSript(FileName, CMDScript, Domain, Username, Password);
        }

        public override XElement ToXML()
        {
            base.ToXML();

            // request
            RequestRoot.Add(new XAttribute("filename", FileName));
            RequestRoot.Add(new XAttribute("domain", Domain ?? string.Empty));
            RequestRoot.Add(new XAttribute("username", Username ?? string.Empty));
            RequestRoot.Add(new XAttribute("password", Password ?? string.Empty));
            RequestRoot.Value = CMDScript ?? string.Empty;

            // response
            ResponseRoot.Value = CMDResult ?? string.Empty;

            return Root;
        }
    }
}
