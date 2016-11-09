using System.Xml.Linq;
using Common.Windows;
using Microsoft.Win32;
using System.Collections.Generic;

namespace VerifyLib.VerifyItems
{
    /// <summary>
    /// Items in Control Panel => Uninstall
    /// </summary>
    public class UnintallItem : VerifyItem
    {
        private const string ProductsRootKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public string ProductName;

        public UnintallItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            ProductName = GetAttributeValue("productName");

            CheckValue = node.Value;
        }

        protected override void Verify()
        {
            var findVersions = new List<string>();

            RegistryKey products32 = RegistryHelper.GetHKLMKey32(ProductsRootKey);

            if (products32 != null)
            {
                foreach (string name in products32.GetSubKeyNames())
                {
                    string displayName = RegistryHelper.ReadHKLM32(string.Format(@"{0}\{1}", ProductsRootKey, name), "DISPLAYNAME");

                    if (displayName == ProductName)
                    {
                        string findVersion = RegistryHelper.ReadHKLM32(string.Format(@"{0}\{1}", ProductsRootKey, name), "DISPLAYVERSION").Trim();

                        if (!findVersions.Contains(findVersion))
                        {
                            findVersions.Add(findVersion);
                        }
                    }
                }
            }

            RegistryKey products64 = RegistryHelper.GetHKLMKey64(ProductsRootKey);

            if (products64 != null)
            {
                foreach (string name in products64.GetSubKeyNames())
                {
                    string displayName = RegistryHelper.ReadHKLM64(string.Format(@"{0}\{1}", ProductsRootKey, name), "DISPLAYNAME");

                    if (displayName == ProductName)
                    {
                        string findVersion = RegistryHelper.ReadHKLM64(string.Format(@"{0}\{1}", ProductsRootKey, name), "DISPLAYVERSION").Trim();

                        if (!findVersions.Contains(findVersion))
                        {
                            findVersions.Add(findVersion);
                        }
                    }
                }
            }

            if (findVersions.Count == 0)
            {
                Information = "Install Info Not Found";
            }
            else if (findVersions.Count == 1)
            {
                ActualValue = findVersions[0];
            }
            else
            {
                ActualValue = string.Join(";", findVersions);

                Information = "More than one version found";
            }

            VerifyResult = ActualValue == CheckValue ? VerifyResult.Pass : VerifyResult.Failed;
        }
    }
}
