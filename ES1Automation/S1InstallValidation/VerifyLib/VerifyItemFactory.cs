using System;
using System.Xml.Linq;
using VerifyLib.VerifyItems;

namespace VerifyLib
{
    public class VerifyItemFactory
    {
        public static VerifyItem CreateVerifyItem(VerifyType type, XElement node, VerifyGroup group)
        {
            try
            {
                switch (type)
                {
                    case VerifyType.Registry:
                        return new RegistryItem(node, group);

                    case VerifyType.File:
                        return new FileItem(node, group);

                    case VerifyType.Version:
                        return new VersionItem(node, group);

                    case VerifyType.Uninstall:
                        return new UnintallItem(node, group);

                    case VerifyType.Database:
                        return new DatabaseItem(node, group);

                    case VerifyType.COM:
                        return new COMItem(node, group);

                    case VerifyType.WinService:
                        return new WinServiceItem(node, group);

                    case VerifyType.EventLog:
                        return new EventLogItem(node, group);

                    case VerifyType.GACAssembly:
                        return new GACAssemblyItem(node, group);

                    default:
                        node.Add(new XAttribute("information", string.Format("{0} not defined to verify", type)));
                        return new ErrorItem(node, group);
                }

            }
            catch(Exception ex)
            {
                var errorNode = new XElement("item");
                errorNode.Add(new XAttribute("name", "Create item error"));
                errorNode.Add(new XAttribute("information", ex.Message));

                return new ErrorItem(errorNode, group);
            }
        }
    }
}
