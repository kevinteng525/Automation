using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;

namespace SupervisorWebAutomation_API.Common
{
    public class MessageListFilter
    {
        public int DBID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long NumDaysUnreviewed { get; set; } // Values: -1 = none, 0 = all, n = number of days

        public long NumDaysRecentlyReviewed { get; set; }   // Values: -1 = none, 0 = all, n = number of days

        public bool UseArchiveDateRange { get; set; }

        //[JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ArchiveDateStart { get; set; }

        //[JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ArchiveDateEnd { get; set; }

        public bool IncludeEscalated { get; set; }  // include escalated messages

        public bool IncludeAllRevGroups { get; set; }   // True indicates that the ReviewGroups list should be ignored
        // and this filter should include ALL review groups currently
        // available in the system

        public List<long> ReviewGroups { get; set; }    // List of selected review groups - used only if IncludeAllRevGroups is false

        public bool RemoveWhenReviewComplete { get; set; }

        public bool AddFlaggedASAP { get; set; }

        public long MsgListMaxLen { get; set; }

        public DateTime? LastQueryTime { get; set; }

        public bool IsDefaultFilter { get; set; }

        public bool CanBeChanged { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]

        public string SortColumn { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        //public SortOrder SortOrder { get; set; }  // Sort order
        public string SortOrder { get; set; }

        public int BatchFlagMode { get; set; }

        public int ReportEnabledState { get; set; }

        public List<FilterSelectorOption> NumDaysUnreviewedOptions { get; set; }

        public List<FilterSelectorOption> NumDaysRecentlyReviewedOptions { get; set; }

        public List<FilterSelectorOption> ReviewGroupsOptions { get; set; }
    }

    public enum SortOrder
    {
        Ascending = 0,
        Descending = 1
    }

    public enum SortColumn
    {
        MessageId = 0,
        Subject = 1,
        MemberName = 2,
        Status = 3,
        HasAttachments = 4,
        HasComments = 5,
        ReceivedDate = 6,
        ReviewDueDate = 7,
        To = 8
    };

    public class FilterSelectorOption
    {
        [DeserializeAs(Name = "label")]
        public string Label { get; set; }
        [DeserializeAs(Name = "value")]
        public string Value { get; set; }
    }
}
