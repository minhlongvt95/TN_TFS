using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SaleBiddingDetail
    {
        public Guid SaleBiddingDetailId { get; set; }
        public Guid SaleBiddingId { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
