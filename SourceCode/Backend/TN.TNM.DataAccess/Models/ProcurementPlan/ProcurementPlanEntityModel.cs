using System;

namespace TN.TNM.DataAccess.Models.ProcurementPlan
{
    public class ProcurementPlanEntityModel
    {
        public Guid ProcurementPlanId { get; set; }
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

        public ProcurementPlanEntityModel()
        {

        }
        public ProcurementPlanEntityModel(Databases.Entities.ProcurementPlan entity)
        {
            ProcurementPlanId = entity.ProcurementPlanId;
            ProcurementPlanCode = entity.ProcurementPlanCode;
            ProcurementContent = entity.ProcurementContent;
            ProcurementAmount = entity.ProcurementAmount;
            ProcurementMonth = entity.ProcurementMonth;
            ProcurementMonth = entity.ProcurementMonth;
            ProcurementYear = entity.ProcurementYear;
            ProcurementPlanCode = entity.ProcurementPlanCode;
            ProductCategoryId = entity.ProductCategoryId;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
        }
    }
}
