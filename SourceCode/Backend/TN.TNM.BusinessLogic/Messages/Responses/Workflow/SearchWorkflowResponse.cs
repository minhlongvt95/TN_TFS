using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Responses.Workflow
{
    public class SearchWorkflowResponse : BaseResponse
    {
        public List<WorkflowModel> WorkflowList { get; set; }
    }
}
