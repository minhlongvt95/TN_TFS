using System;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class GetWorkflowByIdRequest : BaseRequest<GetWorkflowByIdParameter>
    {
        public Guid WorkflowId { get; set; }
        public override GetWorkflowByIdParameter ToParameter()
        {
            return new GetWorkflowByIdParameter() {
                UserId = UserId,
                WorkflowId = WorkflowId
            };
        }
    }
}
