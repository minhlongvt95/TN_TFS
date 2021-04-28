using System;

namespace TN.TNM.BusinessLogic.Models.ProcurementRequest
{
    public class ProcurementPlanModel : BaseModel<DataAccess.Databases.Entities.ProcurementPlan>
    {
        public Guid ProcurementPlanId { get; set; }
        public string ProcurementPlanCode { get; set; }
        public string ProcurementContent { get; set; }
        public decimal? ProcurementAmount { get; set; }
        public int? ProcurementMonth { get; set; }
        public int? ProcurementYear { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ProcurementPlanModel() { }
        public ProcurementPlanModel(DataAccess.Databases.Entities.ProcurementPlan entity) : base(entity) { }
        public override DataAccess.Databases.Entities.ProcurementPlan ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProcurementPlan();
            Mapper(this, entity);
            return entity;
        }
    }
}
