using System;
using System.Xml.Linq;
using Common.RegistryCommon;
using Microsoft.Win32;

namespace VerifyLib.VerifyItems
{
    public class COMItem : VerifyItem
    {
        private const string Register = "Register";

        private const string NotRegister = "Not Register";

        public string ProgID;

        public Guid CLSID;

        public COMItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            ProgID = GetAttributeValue("progID");

            CLSID = new Guid(GetAttributeValue("clsID"));
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            ExpectValue = Register;

            DisplayName = string.Format("{0} - ProgID:{1}", Name, ProgID);
        }

        protected override void Verify()
        {
            RegistryKey progID = RegistryEdit.GetClassRootKey(ProgID);

            string clsID = RegistryEdit.ReadHKCR(string.Format(@"{0}\{1}", ProgID, "CLSID"), string.Empty);
            clsID = clsID == null ? string.Empty : clsID.TrimStart('{').TrimEnd('}').ToUpper();

            if (progID != null && clsID == CLSID.ToString().ToUpper())
            {
                ActualValue = Register;
            }
            else
            {
                ActualValue = NotRegister;

                Information = CLSID.ToString();
            }

            VerifyResult = ExpectValue == ActualValue ? VerifyResult.Pass : VerifyResult.Failed;
        }
    }
}
