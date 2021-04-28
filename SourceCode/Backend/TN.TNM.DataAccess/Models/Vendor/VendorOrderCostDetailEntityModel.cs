using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorOrderCostDetailEntityModel
    {
        public Guid VendorOrderCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid VendorOrderId { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string CostCode { get; set; }
    }
}
