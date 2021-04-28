using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class VendorOrderProcurementRequestMapping
    {
        public Guid VendorOrderProcurementRequestMappingId { get; set; }
        public Guid? VendorOrderId { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
