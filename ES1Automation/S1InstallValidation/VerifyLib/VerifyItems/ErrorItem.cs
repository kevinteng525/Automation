using System.Xml.Linq;

namespace VerifyLib.VerifyItems
{
    public class ErrorItem : VerifyItem
    {
        public ErrorItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            XAttribute informationAttr = node.Attribute("information");

            if (informationAttr != null)
            {
                Information = informationAttr.Value;
            }
        }

        protected override void Verify()
        {
            VerifyResult = VerifyResult.Failed;
        }
    }
}
