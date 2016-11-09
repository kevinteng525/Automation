using System.Xml.Linq;
using Common.Windows;

namespace VerifyLib.VerifyItems
{
    public class WinServiceItem : VerifyItem
    {
        private const string Install = "Installed";

        private const string NotInstall = "NotInstalled";

        public string ServiceName;

        public WinServiceItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            ServiceName = GetAttributeValue("serviceName");
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            ExpectValue = Install;
        }

        protected override void Verify()
        {
            ActualValue = WindowsServices.IsServiceInstalled(ServiceName) ? Install : NotInstall;

            VerifyResult = ActualValue == ExpectValue ? VerifyResult.Pass : VerifyResult.Failed;

            if (VerifyResult == VerifyResult.Failed)
            {
                Information = string.Format("Service {0}, not installed.", ServiceName);
            }
        }
    }
}
