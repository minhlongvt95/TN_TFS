
using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class InventoryDeliveryVoucherMappingModel : BaseModel<InventoryDeliveryVoucherMappingEntityModel>
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

        public InventoryDeliveryVoucherMappingModel() { }
        public InventoryDeliveryVoucherMappingModel(InventoryDeliveryVoucherMappingEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override InventoryDeliveryVoucherMappingEntityModel ToEntity()
        {
            var entity = new InventoryDeliveryVoucherMappingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
