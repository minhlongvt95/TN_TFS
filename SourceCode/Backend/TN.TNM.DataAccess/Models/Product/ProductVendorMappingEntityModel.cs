using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductVendorMappingEntityModel
    {
        public Guid VendorId { get; set; }
        public Guid? MoneyUnitId { get; set; }
        public Guid ProductVendorMappingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }
        public string VendorProductName { get; set; }
        public string VendorProductCode { get; set; }
        public decimal? MiniumQuantity { get; set; }
        public decimal? Price { get; set; }
        public Guid? UnitPriceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string MoneyUnitName { get; set; }
        public string ProductUnitName { get; set; }
        public string ProductCode { get; set; }
        public int? OrderNumber { get; set; }
        public List<Guid?> ListSuggestedSupplierQuoteId { get; set; }
    }
}
