using System;

namespace TN.TNM.BusinessLogic.Models.OrderStatus
{
    public class OrderStatusModel : BaseModel<DataAccess.Databases.Entities.OrderStatus>
    {
        public Guid OrderStatusId { get; set; }
        public string OrderStatusCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public OrderStatusModel(OrderStatusModel entity)
        {
            Mapper(entity, this);
        }

        public OrderStatusModel(DataAccess.Databases.Entities.OrderStatus entity) : base(entity)
        {

        }

        public override DataAccess.Databases.Entities.OrderStatus ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.OrderStatus();
            Mapper(this, entity);
            return entity;
        }

    }
}
