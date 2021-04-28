using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class VendorOrderDetail
    {
        public VendorOrderDetail()
        {
            VendorOrderProductDetailProductAttributeValue = new HashSet<VendorOrderProductDetailProductAttributeValue>();
        }

        public Guid VendorOrderDetailId { get; set; }
        public Guid VendorId { get; set; }
        public Guid VendorOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid? UnitId { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public string IncurredUnit { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public decimal? Cost { get; set; }
        public decimal? PriceWarehouse { get; set; }
        public decimal? PriceValueWarehouse { get; set; }
        public bool? IsEditCost { get; set; }
        public Guid? ProcurementRequestItemId { get; set; }
        public int? OrderNumber { get; set; }

        public Product Product { get; set; }
        public Category Unit { get; set; }
        public Vendor Vendor { get; set; }
        public ICollection<VendorOrderProductDetailProductAttributeValue> VendorOrderProductDetailProductAttributeValue { get; set; }
    }
}
