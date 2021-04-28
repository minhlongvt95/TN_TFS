using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Workflow;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class UpdateWorkflowByIdRequest : BaseRequest<UpdateWorkflowByIdParameter>
    {
        public WorkflowModel Workflow { get; set; }
        public List<WorkflowStepModel> WorkflowStepList { get; set; }
        public override UpdateWorkflowByIdParameter ToParameter()
        {
            var lst = new List<WorkFlowSteps>();
            WorkflowStepList.ForEach(step => {
                lst.Add(step.ToEntity());
            });

            return new UpdateWorkflowByIdParameter()
            {
                UserId = UserId,
                Workflow = Workflow.ToEntity(),
                WorkflowStepList = lst
            };
        }
    }
}
