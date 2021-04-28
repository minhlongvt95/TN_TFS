using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LeadCareFilter
    {
        public Guid LeadCareFilterId { get; set; }
        public Guid? LeadCareId { get; set; }
        public string QueryContent { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
