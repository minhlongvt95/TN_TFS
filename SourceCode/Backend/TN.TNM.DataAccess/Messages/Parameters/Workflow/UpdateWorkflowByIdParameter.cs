using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Workflow
{
    public class UpdateWorkflowByIdParameter : BaseParameter
    {
        public WorkFlows Workflow { get; set; }
        public List<WorkFlowSteps> WorkflowStepList { get; set; }
    }
}
