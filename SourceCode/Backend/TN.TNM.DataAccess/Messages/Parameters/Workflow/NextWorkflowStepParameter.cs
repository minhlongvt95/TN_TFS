using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Workflow
{
    public class NextWorkflowStepParameter : BaseParameter
    {
        public string FeatureCode { get; set; }
        public Guid? FeatureId { get; set; }
        public string RecordName { get; set; }
        public bool IsReject { get; set; }
        public string RejectComment { get; set; }
        public bool IsApprove { get; set; }
        public bool IsSendingApprove { get; set; }
    }
}
