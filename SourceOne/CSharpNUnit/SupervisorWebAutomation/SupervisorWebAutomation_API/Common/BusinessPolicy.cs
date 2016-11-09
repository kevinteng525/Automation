using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;

namespace SupervisorWebAutomation_API.Common
{
    public class BusinessPolicy
    {       
        //[DeserializeAs(Name="id")]
        public string Id { get; set; }

        //[DeserializeAs(Name="description")]
        public string Description { get; set; }
        
        //[DeserializeAs(Name="name")]
        public string Name { get; set; }

        //this is for the post request to generate report
        public bool IsDeleted { get; set; }

        public bool Selected { get; set; }
    }

    public class ReportBusinessPolicies
    {
        [DeserializeAs(Name = "bps")]
        public List<BusinessPolicy> BusinessPolicies { get; set; }

        [DeserializeAs(Name = "deletedBPs")]
        public List<BusinessPolicy> DeletedBusinessPolicies { get; set; }
    }

}
