//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Build
    {
        public Build()
        {
            this.AutomationTasks = new HashSet<AutomationTask>();
        }
    
        public int BuildId { get; set; }
        public int ProviderId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Number { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> ReleaseId { get; set; }
    
        public virtual ICollection<AutomationTask> AutomationTasks { get; set; }
        public virtual Product Product { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Branch Branch_1 { get; set; }
        public virtual Release Release { get; set; }
    }
}
