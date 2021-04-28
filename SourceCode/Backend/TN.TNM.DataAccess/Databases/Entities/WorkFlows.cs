using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class WorkFlows
    {
        public WorkFlows()
        {
            WorkFlowSteps = new HashSet<WorkFlowSteps>();
        }

        public Guid WorkFlowId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SystemFeatureId { get; set; }
        public string WorkflowCode { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<WorkFlowSteps> WorkFlowSteps { get; set; }
    }
}
