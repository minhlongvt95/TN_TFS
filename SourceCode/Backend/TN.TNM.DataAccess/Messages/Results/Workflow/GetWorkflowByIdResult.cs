using TN.TNM.DataAccess.Models.Workflow;

namespace TN.TNM.DataAccess.Messages.Results.Workflow
{
    public class GetWorkflowByIdResult : BaseResult
    {
        public WorkflowEntityModel Workflow { get; set; }
    }
}
