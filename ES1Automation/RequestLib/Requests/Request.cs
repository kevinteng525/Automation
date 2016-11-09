using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RequestLib.Requests
{
    public abstract class Request
    {
        public string Server {get;set;}

        public int Port {get;set;}

        public Request(string server, int port)
        {
            this.Server = server;
            this.Port = port;
        }

        protected XElement RequestXML = new XElement("request");

        public string ResultString { get; protected set; }

        protected abstract string RequestServer();

        public virtual string RunCMDRequest(string domain, string username, string password, string filename, string script)
        {
            RequestXML.Add(new XAttribute("type", "CMD"));
            RequestXML.Add(new XAttribute("filename", filename));
            RequestXML.Add(new XAttribute("domain", domain ?? string.Empty));
            RequestXML.Add(new XAttribute("username", username ?? string.Empty));
            RequestXML.Add(new XAttribute("password", password ?? string.Empty));

            RequestXML.Value = script;

            return RequestServer();
        }

        public virtual Dictionary<string, string> GetStatus()
        {
            RequestXML.Add(new XAttribute("type", "CMD"));

            string statusResult = RequestServer();

            string[] statusList = statusResult.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            var status = new Dictionary<string, string>();

            foreach (string rawStatus in statusList)
            {
                string[] pairs = rawStatus.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (pairs.Count() == 2)
                {
                    status.Add(pairs[0].Trim(), pairs[1].Trim());
                }
            }

            return status;
        }

        public virtual string RunTestCase(string testCaseId)
        {
            return string.Empty;
        }
    }

    public class ValidationGroup
    {
        public string GroupName { get; set; }

        public List<ValidationResult> ValidationResults { get; set; }

        public ValidationGroup(string name)
        {
            GroupName = name;

            ValidationResults = new List<ValidationResult>();
        }
    }

    public class ValidationResult
    {
        public string Name { get; set; }

        public string ExpectValue { get; set; }

        public string ActualValue { get; set; }

        public string Information { get; set; }

        public VerifyResult VerifyResult { get; set; }
    }
}
