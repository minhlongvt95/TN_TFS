using System;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class InventoryReceivingVoucherModel : BaseModel<Entities.InventoryReceivingVoucher>
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
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }

        public InventoryReceivingVoucherModel() { }

        public InventoryReceivingVoucherModel(Entities.InventoryReceivingVoucher entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override Entities.InventoryReceivingVoucher ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Entities.InventoryReceivingVoucher();
            Mapper(this, entity);
            return entity;
        }
    }
}
