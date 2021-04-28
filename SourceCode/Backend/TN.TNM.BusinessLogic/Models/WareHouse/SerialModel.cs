using System;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.WareHouse
{
    public class SerialModel : BaseModel<Entities.Serial>
    {
        public Guid SerialId { get; set; }
        public string SerialCode { get; set; }
        public Guid ProductId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? WarehouseId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }

        public SerialModel() { }

        public SerialModel(Entities.Serial entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override Entities.Serial ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Entities.Serial();
            Mapper(this, entity);
            return entity;
        }
    }
}
