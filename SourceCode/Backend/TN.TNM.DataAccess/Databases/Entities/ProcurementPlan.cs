using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProcurementPlan
    {
        public ProcurementPlan()
        {
            ProcurementRequestItem = new HashSet<ProcurementRequestItem>();
        }

        public Guid ProcurementPlanId { get; set; }
        public string ProcurementPlanCode { get; set; }
        public string ProcurementContent { get; set; }
        public decimal? ProcurementAmount { get; set; }
        public int? ProcurementMonth { get; set; }
        public int? ProcurementYear { get; set; }
        public Guid ProductCategoryId { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ProductCategory ProductCategory { get; set; }
        public ICollection<ProcurementRequestItem> ProcurementRequestItem { get; set; }
    }
}
