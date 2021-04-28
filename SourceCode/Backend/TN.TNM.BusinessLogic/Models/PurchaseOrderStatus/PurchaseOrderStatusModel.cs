using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.PurchaseOrderStatus;

namespace TN.TNM.BusinessLogic.Models.PurchaseOrderStatus
{
    public class PurchaseOrderStatusModel : BaseModel<DataAccess.Databases.Entities.PurchaseOrderStatus>
    {
        public Guid PurchaseOrderStatusId { get; set; }
        public string PurchaseOrderStatusCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public PurchaseOrderStatusModel(PurchaseOrderStatusModel entity)
        {
            Mapper(entity, this);
        }

        public PurchaseOrderStatusModel(DataAccess.Databases.Entities.PurchaseOrderStatus entity) : base(entity)
        {

        }

        public override DataAccess.Databases.Entities.PurchaseOrderStatus ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.PurchaseOrderStatus();
            Mapper(this, entity);
            return entity;
        }
    }
}
