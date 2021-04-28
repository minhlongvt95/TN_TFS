using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Workflow
{
    public class GetWorkflowByIdParameter : BaseParameter
    {
        public Guid WorkflowId { get; set; }
    }
}
