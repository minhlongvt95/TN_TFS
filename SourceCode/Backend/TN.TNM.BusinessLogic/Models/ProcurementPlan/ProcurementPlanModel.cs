using System;
using TN.TNM.DataAccess.Models.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Models.ProcurementPlan
{
    public class ProcurementPlanModel : BaseModel<DataAccess.Databases.Entities.ProcurementPlan>
    {
        public Guid? ProcurementPlanId { get; set; }
        public string ProcurementPlanCode { get; set; }
        public string ProcurementContent { get; set; }
        public decimal? ProcurementAmount { get; set; }
        public int? ProcurementMonth { get; set; }
        public int? ProcurementYear { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ProcurementPlanModel() { }

        public ProcurementPlanModel(ProcurementPlanModel entity)
        {
            Mapper(entity, this);
        }
        public ProcurementPlanModel(ProcurementPlanEntityModel entity)
        {
            Mapper(entity, this);
        }

        public ProcurementPlanModel(DataAccess.Databases.Entities.ProcurementPlan entity) : base(entity)
        {
        }

        //thực thi phương thức chuyển từ model ở BusinessLogic sang model trên DB
        public override DataAccess.Databases.Entities.ProcurementPlan ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProcurementPlan();
            Mapper(this, entity);
            return entity;
        }
    }
}
