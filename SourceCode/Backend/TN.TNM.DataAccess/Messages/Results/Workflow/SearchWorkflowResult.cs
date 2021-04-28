using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Workflow;

namespace TN.TNM.DataAccess.Messages.Results.Workflow
{
    public class SearchWorkflowResult : BaseResult
    {
        public List<WorkflowEntityModel> WorkflowList { get; set; }
    }
}
