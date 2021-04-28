using System;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class NextWorkflowStepRequest : BaseRequest<NextWorkflowStepParameter>
    {
        public string FeatureCode { get; set; }
        public Guid? FeatureId { get; set; }
        public string RecordName { get; set; }
        public bool IsReject { get; set; }
        public string RejectComment { get; set; }
        public bool IsApprove { get; set; }
        public bool IsSendingApprove { get; set; }
        public override NextWorkflowStepParameter ToParameter()
        {
            return new NextWorkflowStepParameter() {
                FeatureCode = FeatureCode,
                FeatureId = FeatureId,
                RecordName = RecordName,
                IsReject = IsReject,
                RejectComment = RejectComment,
                IsApprove = IsApprove,
                IsSendingApprove = IsSendingApprove,
                UserId = UserId
            };
        }
    }
}
