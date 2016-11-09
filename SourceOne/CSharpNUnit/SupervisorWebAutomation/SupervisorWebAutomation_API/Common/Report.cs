using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace SupervisorWebAutomation_API.Common
{
    public class ReportDateRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class ReportCriteria
    {

        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }

        //[DeserializeAs(Name="groups")]
        public List<ReviewGroup> Groups { get; set; }

        //[DeserializeAs(Name="members")]
        public List<Member> Members { get; set; }

        //[DeserializeAs(Name="columnOrder")]
        public int ColumnOrder { get; set; }
    }

    public class ManagerReportCriteria : ReportCriteria
    {
        //[DeserializeAs(Name="codes")]
        public List<SupervisoryCode> Codes { get; set; }

        //[DeserializeAs(Name="detailLevel")]
        public int DetailLevel { get; set; }

        //[DeserializeAs(Name="reviewers")]
        public List<Member> Reviewers { get; set; }

              
        public List<BusinessPolicy> Bps { get; set; }
    }

    public class ReportResponse
    {
        public string Content { get; set; }
        public string DataId { get; set; }
    }

    public enum ReportType
    {
        GroupActivity = 2,
        ReviewerActivity = 4,
        MemberActivity = 6
    }

    public enum ManagerReportType
    {
        Configuration = 1,
        GroupActivity = 2,
        MessageDetail = 3,
        ReviewerActivity = 4,
        BusinessPolicyActivity = 5,
        MemberActivity = 6,
        QueryActivity = 7,
        LicenseAvailability = 8
    }

    public enum DetailLevel
    {
        AllDetails = 1,
        ExcludeAddresses4ReviewersAndMembers = 2,
    }

    public enum ColumnOrder
    {
        ReviewerFirst = 1,
        BusinessPolicyFirst = 2,
    }
}
