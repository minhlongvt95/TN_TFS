using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Models.Order
{
    public class OrderProductDetailProductAttributeValueModel : BaseModel<OrderProductDetailProductAttributeValue>
    {
        public Guid OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }


        public OrderProductDetailProductAttributeValueModel() { }

        public OrderProductDetailProductAttributeValueModel(OrderProductDetailProductAttributeValue entity): base(entity) {
            Mapper(entity, this);
        }
        public OrderProductDetailProductAttributeValueModel(OrderProductDetailProductAttributeValueModel model)
        {
            Mapper(this, model);
        }
        public OrderProductDetailProductAttributeValueModel(OrderProductDetailProductAttributeValueEntityModel model)
        {
            Mapper(model,this);
        }
        public override OrderProductDetailProductAttributeValue ToEntity()
        {
            var entity = new OrderProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
