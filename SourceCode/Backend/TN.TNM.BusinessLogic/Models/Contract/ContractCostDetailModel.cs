using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Models.Contract
{
    public class ContractCostDetailModel : BaseModel<ContractCostDetail>
    {
        public Guid ContractCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid ContractId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ContractCostDetailModel() { }

        public ContractCostDetailModel(DataAccess.Databases.Entities.ContractCostDetail entity) : base(entity)
        {

        }

        public ContractCostDetailModel(ContractCostDetailEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ContractCostDetail ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ContractCostDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
