using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace Common.ActiveDirecotry
{
    public static class ADGroup
    {
        public static void Create(string ouPath, string name)
        {
            if (!DirectoryEntry.Exists("LDAP://CN=" + name + "," + ouPath))
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + ouPath);
                DirectoryEntry group = entry.Children.Add("CN=" + name, "group");
                group.Properties["sAmAccountName"].Value = name;
                group.CommitChanges();
            }
        }

        public static void Delete(string ouPath, string groupPath)
        {
            if (DirectoryEntry.Exists("LDAP://" + groupPath))
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + ouPath);
                DirectoryEntry group = new DirectoryEntry("LDAP://" + groupPath);
                entry.Children.Remove(group);
                group.CommitChanges();
            }
        }
    }
}
