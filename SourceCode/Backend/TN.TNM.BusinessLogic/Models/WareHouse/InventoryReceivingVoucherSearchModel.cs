

using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class InventoryReceivingVoucherSearchModel : BaseModel<DataAccess.Models.WareHouse.InventoryReceivingVoucherModel>
    {
        public Guid InventoryReceivingVoucherId { get; set; }
        public string InventoryReceivingVoucherCode { get; set; }
        public Guid StatusId { get; set; }
        public int InventoryReceivingVoucherType { get; set; }
        public Guid WarehouseId { get; set; }
        public string ShiperName { get; set; }
        public Guid? Storekeeper { get; set; }
        public DateTime InventoryReceivingVoucherDate { get; set; }
        public TimeSpan InventoryReceivingVoucherTime { get; set; }
        public int LicenseNumber { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }

        public Guid VendorId { get; set; }
        public Guid CustomerId { get; set; }
        public string NameStorekeeper { get; set; }
        public string NameVendor { get; set; }
        public string CustomerName { get; set; }
        public string NameCreate { get; set; }
        public string NameStatus { get; set; }
        public List<VendorOrderModel> ListVendorOrder { get; set; }
        public List<CustomerOrderModel> ListCustomerOrder { get; set; }

        public InventoryReceivingVoucherSearchModel() { }
        public InventoryReceivingVoucherSearchModel(DataAccess.Models.WareHouse.InventoryReceivingVoucherModel entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Models.WareHouse.InventoryReceivingVoucherModel ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Models.WareHouse.InventoryReceivingVoucherModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
