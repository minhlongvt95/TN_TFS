using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductVendorMapping
    {
        public Guid ProductVendorMappingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid VendorId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }
        public string VendorProductName { get; set; }
        public decimal? MiniumQuantity { get; set; }
        public decimal? Price { get; set; }
        public Guid? UnitPriceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string VendorProductCode { get; set; }
        public int? OrderNumber { get; set; }

        public Product Product { get; set; }
        public Vendor Vendor { get; set; }
    }
}
