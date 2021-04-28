using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.BusinessLogic.Models.BillSale
{
    public class OrderBillModel:BaseModel<DataAccess.Databases.Entities.CustomerOrder>
    {
        public Guid? OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal? TotalOrder { get; set; }
        public decimal? TotalQuantity { get; set; }
        public Guid? CustomerId { get; set; }

        public OrderBillModel()
        {

        }

        public OrderBillModel(OrderBillModel entity)
        {
            Mapper(entity, this);
        }

        public OrderBillModel(OrderBillEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.CustomerOrder ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.CustomerOrder();
            Mapper(this, entity);
            return entity;
        }

        public OrderBillEntityModel ToEntityModel()
        {
            var entity = new OrderBillEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
