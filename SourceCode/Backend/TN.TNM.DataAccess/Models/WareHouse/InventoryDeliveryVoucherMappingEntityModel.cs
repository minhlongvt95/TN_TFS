using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InventoryDeliveryVoucherMappingEntityModel
    {
        public Guid InventoryDeliveryVoucherMappingId { get; set; }
        public Guid InventoryDeliveryVoucherId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public decimal? QuantityRequire { get; set; }
        public decimal? QuantityInventory { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public Guid WarehouseId { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? UnitId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public List<Serial> ListSerial { get; set; }
        public int TotalSerial { get; set; }


        public string ProductName { get; set; }
        public string WareHouseName { get; set; }
        public string UnitName { get; set; }
        public string NameMoneyUnit { get; set; }
        public decimal SumAmount { get; set; }

    }
}
