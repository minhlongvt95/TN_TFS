using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class InventoryDeliveryVoucherModel : BaseModel<Entities.InventoryDeliveryVoucher>
    {
        public Guid InventoryDeliveryVoucherId { get; set; }
        public string InventoryDeliveryVoucherCode { get; set; }
        public Guid StatusId { get; set; }
        public int InventoryDeliveryVoucherType { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid ObjectId { get; set; }
        public string Receiver { get; set; }
        public string Reason { get; set; }
        public DateTime? InventoryDeliveryVoucherDate { get; set; }
        public TimeSpan? InventoryDeliveryVoucherTime { get; set; }
        public int LicenseNumber { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }

        public string NameObject { get; set; }
        public string NameCreate { get; set; }
        public string NameOutOfStock { get; set; }
        public string NameStatus { get; set; }
        public Guid? VendorId { get; set; }
        public string VedorName { get; set; }


        public InventoryDeliveryVoucherModel() { }

        public InventoryDeliveryVoucherModel(Entities.InventoryDeliveryVoucher entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }
        
        public InventoryDeliveryVoucherModel(InventoryDeliveryVoucherEntityModel dataaccessModel)
        {
            Mapper(dataaccessModel, this);
        }
        public override InventoryDeliveryVoucher ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Entities.InventoryDeliveryVoucher();
            Mapper(this, entity);
            return entity;
        }
    }
}
