using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Cost
{
    public class CostEntityModel
    {
        public Guid CostId { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public string CostCodeName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string StatusName { get; set; }
        public string OrganizationName { get; set; }
    }
}
