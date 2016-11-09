using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace Common.ActiveDirecotry
{
    public static class ADUser
    {
        public static bool Authenticate(string userName, string password, string domain)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain, userName, password))
                {
                    object nativeObject = entry.NativeObject;

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetEmailAddress(string userName, string password, string domain)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain, userName, password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = string.Format("sAMAccountName={0}", userName);
                        searcher.PropertiesToLoad.Add("Mail");
                        SearchResultCollection en = searcher.FindAll();
                        if (en.Count > 0)
                        {
                            return en[0].Properties["Mail"][0].ToString();
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void AddToGroup(string userDn, string groupDn)
        {
            using (DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + groupDn))
            {
                dirEntry.Properties["member"].Add(userDn);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
        }

        public static void RemoveUserFromGroup(string userDn, string groupDn)
        {
            using (DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + groupDn))
            {
                dirEntry.Properties["member"].Remove(userDn);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
        }
    }
}
