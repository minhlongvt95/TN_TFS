using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Order
{
    public class CustomerOrderDetailEntityModel
    {
        public Guid OrderDetailId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public Guid? UnitId { get; set; }
        public string IncurredUnit { get; set; }
        public int? Guarantee { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? GuaranteeTime { get; set; }
        public DateTime? GuaranteeDatetime { get; set; }
        public string NameVendor { get; set; }
        public string NameMoneyUnit { get; set; }
        public string NameGene { get; set; }
        public string NameProductUnit { get; set; }
        public string NameProduct { get; set; } 
        public decimal SumAmount { get; set; }
        public Guid? WarehouseId { get; set; }
        public decimal? PriceInitial { get; set; }
        public bool? IsPriceInitial { get; set; }
        public int? WarrantyPeriod { get; set; }
        public int? ActualInventory { get; set; }
        public int? BusinessInventory { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int? OrderNumber { get; set; }
        public List<OrderProductDetailProductAttributeValueEntityModel> OrderProductDetailProductAttributeValue { get; set; }
    }
}
