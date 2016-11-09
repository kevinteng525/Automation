using System;
using System.Text;
using System.Xml.Linq;
using Common.Windows;

namespace ATFExchangeCentre.Requests
{
    public class SystemRequest : Request
    {
        public string SystemInfo { get; protected set; }

        public SystemRequest()
        {
            Type = Request.SYSTEM;
        }

        public SystemRequest(XElement root)
            : base(root)
        {
            SystemInfo = ResponseRoot.Value;
        }

        protected override void HandleRequest()
        {
            StringBuilder status = new StringBuilder();

            // domain name
            status.AppendFormat("DomainName = {0};", Environment.UserDomainName);
            // server name
            status.AppendFormat("ServerName = {0};", Environment.MachineName);
            // operator system name
            status.AppendFormat("OSName = {0};", ServerStatus.GetOSFriendlyName());
            status.AppendFormat("OSVersion = {0};", Environment.OSVersion.ToString());
            status.AppendFormat("Is64BitOS = {0};", Environment.Is64BitOperatingSystem);

            SystemInfo = status.ToString();
        }

        public override XElement ToXML()
        {
            base.ToXML();

            ResponseRoot.Value = SystemInfo ?? string.Empty;

            return Root;
        }
    }
}
