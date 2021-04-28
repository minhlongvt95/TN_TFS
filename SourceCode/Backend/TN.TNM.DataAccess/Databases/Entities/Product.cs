using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Product
    {
        public Product()
        {
            InventoryDetail = new HashSet<InventoryDetail>();
            OrderProductDetailProductAttributeValue = new HashSet<OrderProductDetailProductAttributeValue>();
            ProcurementRequestItem = new HashSet<ProcurementRequestItem>();
            ProductAttribute = new HashSet<ProductAttribute>();
            ProductVendorMapping = new HashSet<ProductVendorMapping>();
            QuoteProductDetailProductAttributeValue = new HashSet<QuoteProductDetailProductAttributeValue>();
            VendorOrderDetail = new HashSet<VendorOrderDetail>();
            VendorOrderProductDetailProductAttributeValue = new HashSet<VendorOrderProductDetailProductAttributeValue>();
        }

        public Guid ProductId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price1 { get; set; }
        public decimal? Price2 { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public decimal? Quantity { get; set; }
        public Guid? ProductUnitId { get; set; }
        public string ProductDescription { get; set; }
        public decimal? Vat { get; set; }
        public decimal? MinimumInventoryQuantity { get; set; }
        public Guid? ProductMoneyUnitId { get; set; }
        public int? Guarantee { get; set; }
        public DateTime? GuaranteeDatetime { get; set; }
        public int? GuaranteeTime { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? ExWarehousePrice { get; set; }
        public Guid? CalculateInventoryPricesId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? WarehouseAccountId { get; set; }
        public Guid? RevenueAccountId { get; set; }
        public Guid? PayableAccountId { get; set; }
        public decimal? ImportTax { get; set; }
        public Guid? CostPriceAccountId { get; set; }
        public Guid? AccountReturnsId { get; set; }
        public bool? FolowInventory { get; set; }
        public bool? ManagerSerialNumber { get; set; }

        public ProductCategory ProductCategory { get; set; }
        public Category ProductMoneyUnit { get; set; }
        public Product ProductNavigation { get; set; }
        public Product InverseProductNavigation { get; set; }
        public ICollection<InventoryDetail> InventoryDetail { get; set; }
        public ICollection<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValue { get; set; }
        public ICollection<ProcurementRequestItem> ProcurementRequestItem { get; set; }
        public ICollection<ProductAttribute> ProductAttribute { get; set; }
        public ICollection<ProductVendorMapping> ProductVendorMapping { get; set; }
        public ICollection<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValue { get; set; }
        public ICollection<VendorOrderDetail> VendorOrderDetail { get; set; }
        public ICollection<VendorOrderProductDetailProductAttributeValue> VendorOrderProductDetailProductAttributeValue { get; set; }
    }
}
