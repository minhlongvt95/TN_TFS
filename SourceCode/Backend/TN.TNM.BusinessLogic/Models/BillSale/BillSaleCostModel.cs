using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.BusinessLogic.Models.BillSale
{
    public class BillSaleCostModel: BaseModel<DataAccess.Databases.Entities.BillOfSaleCost>
    {
        public Guid? BillOfSaleCostId { get; set; }
        public Guid? BillOfSaleId { get; set; }
        public Guid? OrderCostId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? CostId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CostCode { get; set; }

        public BillSaleCostModel() { }

        public BillSaleCostModel(BillSaleCostModel entity)
        {
            Mapper(entity, this);
        }

        public BillSaleCostModel(BillSaleCostEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.BillOfSaleCost ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.BillOfSaleCost();
            Mapper(this, entity);
            return entity;
        }

        public BillSaleCostEntityModel ToEntityModel()
        {
            var entity = new BillSaleCostEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
