using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.BillSale
{
    public class BillSaleDetailEntityModel
    {
        public Guid? BillOfSaleDetailId { get; set; }
        public Guid? BillOfSaleId { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public Guid? WarehouseId { get; set; }
        public string WarehouseCode { get; set; } // Mã kho
        public decimal? MoneyForGoods { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? AccountDiscountId { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public bool Active { get; set; }
        public Guid? UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal? BusinessInventory { get; set; }
        public string ProductName { get; set; }
        public decimal? ActualInventory { get; set; }
        public string IncurredUnit { get; set; }
        public short? CostsQuoteType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid? OrderId { get; set; }
        public int? OrderNumber { get; set; }
        public List<BillSaleDetailProductAttributeEntityModel> ListBillSaleDetailProductAttribute { get; set; }
    }
}
