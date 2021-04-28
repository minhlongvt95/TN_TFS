using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Workflow;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class CreateWorkflowRequest : BaseRequest<CreateWorkflowParameter>
    {
        public WorkflowModel Workflow { get; set; }
        public List<WorkflowStepModel> WorkflowStepList { get; set; }
        public override CreateWorkflowParameter ToParameter()
        {
            var lst = new List<WorkFlowSteps>();
            WorkflowStepList.ForEach(step => {
                lst.Add(step.ToEntity());
            });

            return new CreateWorkflowParameter() {
                UserId = UserId,
                Workflow = Workflow.ToEntity(),
                WorkflowStepList = lst
            };
        }
    }
}
