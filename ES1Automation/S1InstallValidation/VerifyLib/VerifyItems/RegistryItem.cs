using System.Xml.Linq;
using Common.Windows;

namespace VerifyLib.VerifyItems
{
    public class RegistryItem : VerifyItem
    {
        public string SubKey;

        public string Key;

        public RegistryItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            SubKey = GetAttributeValue("subkey");

            Key = GetAttributeValue("key");
        }

        protected override void Verify()
        {
            ActualValue = RegistryHelper.ReadHKLM32(SubKey, Key);

            if (ActualValue == null)
            {
                ActualValue = RegistryHelper.ReadHKLM64(SubKey, Key);
            }

            VerifyResult = ActualValue == ExpectValue ? VerifyResult.Pass : VerifyResult.Failed;

            if (ActualValue == null)
            {
                ActualValue = NotExist;

                Information = "register key not found";
            }
            else if(VerifyResult == VerifyResult.Failed)
            {
                Information = "register value not match";
            }
        }
    }
}
