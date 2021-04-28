using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Models.Order
{
    public class OrderCostDetailModel : BaseModel<OrderCostDetail>
    {
        public Guid OrderCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid OrderId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public string CostCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public OrderCostDetailModel() { }

        public OrderCostDetailModel(OrderCostDetail entity): base(entity) {
            Mapper(entity, this);
        }
        public OrderCostDetailModel(OrderCostDetailModel model)
        {
            Mapper(this, model);
        }
        public OrderCostDetailModel(OrderCostDetailEntityModel model)
        {
            Mapper(model, this);
        }
        public override OrderCostDetail ToEntity()
        {
            var entity = new OrderCostDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
