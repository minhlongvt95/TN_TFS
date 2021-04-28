using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class FeatureWorkFlowProgress
    {
        public Guid FeatureWorkflowProgressId { get; set; }
        public Guid ApprovalObjectId { get; set; }
        public int CurrentStep { get; set; }
        public string Comment { get; set; }
        public Guid? ApproverPersonId { get; set; }
        public Guid? ApproverPositionId { get; set; }
        public string SystemFeatureCode { get; set; }
        public string ProgressStatus { get; set; }
        public int? NextStepNumber { get; set; }
        public int? BackStepNumber { get; set; }
        public string RecordStatus { get; set; }
        public string Name { get; set; }
        public Guid? ActorId { get; set; }
        public Guid? FeatureCreatedBy { get; set; }
        public DateTime? ChangeStepDate { get; set; }
        public bool? IsFinalApproved { get; set; }
        public Guid? TenantId { get; set; }
    }
}
