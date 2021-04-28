using TN.TNM.BusinessLogic.Models.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Responses.Workflow
{
    public class GetWorkflowByIdResponse : BaseResponse
    {
        public WorkflowModel Workflow { get; set; }
    }
}
