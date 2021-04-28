using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class WorkFlowSteps
    {
        public Guid WorkflowStepId { get; set; }
        public int StepNumber { get; set; }
        public int? NextStepNumber { get; set; }
        public bool ApprovebyPosition { get; set; }
        public Guid? ApproverPositionId { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid WorkflowId { get; set; }
        public int? BackStepNumber { get; set; }
        public string ApprovedText { get; set; }
        public string NotApprovedText { get; set; }
        public string RecordStatus { get; set; }
        public Guid? TenantId { get; set; }

        public WorkFlows Workflow { get; set; }
    }
}
