using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Diagnostics;

namespace Common.ADCommon
{
    public static class ADHelper
    {
        public static IList<string> GetMailBox(string domain, string username, string password, int size)
        {
            DirectoryEntry adRoot = new DirectoryEntry("LDAP://" + domain, username, password, AuthenticationTypes.Secure);

            DirectorySearcher searcher = new DirectorySearcher(adRoot);
            searcher.SearchScope = SearchScope.Subtree;
            searcher.ReferralChasing = ReferralChasingOption.All;
            searcher.SizeLimit = size;

            string[] properties = new string[] { "mail" };
            searcher.PropertiesToLoad.AddRange(properties);

            string filter = string.Format("(&(ObjectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))");
            //string filter = string.Format("（objectCategory=group）");
            searcher.Filter = filter;

            IList<string> mailboxs = new List<string>();

            foreach (SearchResult result in searcher.FindAll())
            {
                DirectoryEntry directoryEntry = result.GetDirectoryEntry();

                if (directoryEntry.Properties.Contains("Mail"))
                {
                    mailboxs.Add(directoryEntry.Properties["Mail"].Value.ToString());
                }
            }

            searcher.Dispose();

            return mailboxs;
        }

        public static bool ValidateDomainUser(string UserName, string Password, string Domain)
        {
            bool bValid = false;

            using (var context = new PrincipalContext(ContextType.Domain, Domain))
            {
                bValid = context.ValidateCredentials(UserName, Password);
            }

            return bValid;
        }
    }
}
