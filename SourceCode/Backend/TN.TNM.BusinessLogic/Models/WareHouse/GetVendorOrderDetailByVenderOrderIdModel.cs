using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class GetVendorOrderDetailByVenderOrderIdModel : BaseModel<GetVendorOrderDetailByVenderOrderIdEntityModel>
    {
        public Guid? InventoryReceivingVoucherMappingId { get; set; }
        public Guid VendorOrderId { get; set; }
        public Guid VendorOrderDetailId { get; set; }
        public string VendorOrderCode { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Guid? UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal? QuantityRequire { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public Guid WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public string Note { get; set; }
        public List<Serial> ListSerial { get; set; }
        public int TotalSerial { get; set; }

        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string NameMoneyUnit { get; set; }
        public decimal SumAmount { get; set; }


        public GetVendorOrderDetailByVenderOrderIdModel() { }
        
        public GetVendorOrderDetailByVenderOrderIdModel(GetVendorOrderDetailByVenderOrderIdEntityModel entity)
        {
            Mapper(entity, this);
        }
        public override GetVendorOrderDetailByVenderOrderIdEntityModel ToEntity()
        {
            var entity = new GetVendorOrderDetailByVenderOrderIdEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
