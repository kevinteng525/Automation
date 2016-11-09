using System.Xml.Linq;

namespace VerifyLib.VerifyItems
{
    public class InstallationItem : VerifyItem
    {
        public InstallationItem(XElement node, VerifyGroup group)
            : base(node, group)
        {

        }

        protected override void Verify()
        {
            // Check if log is exsist

            // Run CMD Script to Install

            // Check the install finish
        }
    }
}
