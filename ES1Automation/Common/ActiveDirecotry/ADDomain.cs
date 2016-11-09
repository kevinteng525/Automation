using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace Common.ActiveDirecotry
{
    public static class ADDomain
    {
        public static string GetCurrentDomain()
        {
            try
            {
                return Domain.GetCurrentDomain().Name;
            }
            catch
            {
                return null;
            }
        }

        public static string FriendlyDomainToLdapDomain(string friendlyDomainName)
        {
            DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName);
            Domain domain = Domain.GetDomain(objContext);

            return domain.Name;
        }

        public static IList<string> GetDomainControllers()
        {
            IList<string> dcList = new List<string>();

            foreach (DomainController dc in Domain.GetCurrentDomain().DomainControllers)
            {
                dcList.Add(dc.Name);
            }

            return dcList;
        }
    }
}
