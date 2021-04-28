using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorOrderProcurementRequestMappingEntityModel
    {
        public Guid VendorOrderProcurementRequestMappingId { get; set; }
        public Guid? VendorOrderId { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
