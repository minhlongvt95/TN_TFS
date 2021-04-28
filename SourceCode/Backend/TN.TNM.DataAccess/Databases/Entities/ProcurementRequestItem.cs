using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProcurementRequestItem
    {
        public Guid ProcurementRequestItemId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? VendorId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public Guid? ProcurementPlanId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? QuantityApproval { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public string IncurredUnit { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public int? OrderNumber { get; set; }

        public ProcurementPlan ProcurementPlan { get; set; }
        public ProcurementRequest ProcurementRequest { get; set; }
        public Product Product { get; set; }
        public Vendor Vendor { get; set; }
    }
}
