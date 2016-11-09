using System.Xml.Linq;
using Common.Windows;

namespace VerifyLib.VerifyItems
{
    public class GACAssemblyItem : VerifyItem
    {
        public string AssemblyName;

        public string AssemblyVersion;

        public GACAssemblyItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            AssemblyName = GetAttributeValue("assemblyName");

            AssemblyVersion = GetAttributeValue("assemblyVersion");
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            ExpectValue = AssemblyVersion;

            // Load all GAC assembly
            if(VerifyGroup.VerifyEnvironment.GACAssemblyVersions == null)
            {
                VerifyGroup.VerifyEnvironment.GACAssemblyVersions = GACAssembly.GetGACAssemblyVersions();
            }
        }

        protected override void Verify()
        {
            if (VerifyGroup.VerifyEnvironment.GACAssemblyVersions.ContainsKey(AssemblyName))
            {
                if (VerifyGroup.VerifyEnvironment.GACAssemblyVersions[AssemblyName].Contains(AssemblyVersion))
                {
                    ActualValue = AssemblyVersion;
                }
                else
                {
                    ActualValue = string.Join(";", VerifyGroup.VerifyEnvironment.GACAssemblyVersions[AssemblyName]);
                }

                if (VerifyGroup.VerifyEnvironment.GACAssemblyVersions[AssemblyName].Count > 1)
                {
                    Information = string.Join(";", VerifyGroup.VerifyEnvironment.GACAssemblyVersions[AssemblyName]);
                }
            }
            else
            {
                Information = string.Format("{0} {1} {2}", AssemblyName, AssemblyVersion, NotExist);
            }

            VerifyResult = ActualValue == ExpectValue ? VerifyResult.Pass : VerifyResult.Failed;
        }
    }
}
