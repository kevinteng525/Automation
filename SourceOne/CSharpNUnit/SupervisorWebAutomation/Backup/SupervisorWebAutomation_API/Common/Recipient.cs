using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupervisorWebAutomation_API.Common
{
    public class Recipient
    {

        public int Attributes { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public RecipientType Type { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public RecipientDataSource DataSource { get; set; }
    }

    public enum RecipientType
    {
        Unknown = 0,
        MailUser = 1,
        DistributionList = 2,
        LDAP = 4
    }

    public enum RecipientDataSource
    {
        Unknown = 0,
        Exchange = 1,
        Notes = 2,
        LDAP = 4
    }
}
