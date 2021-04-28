using TN.TNM.DataAccess.Messages.Parameters.Workflow;
using TN.TNM.DataAccess.Messages.Results.Workflow;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IWorkflowDataAccess
    {
        CreateWorkflowResult CreateWorkflow(CreateWorkflowParameter parameter);
        SearchWorkflowResult SearchWorkflow(SearchWorkflowParameter parameter);
        GetWorkflowByIdResult GetWorkflowById(GetWorkflowByIdParameter parameter);
        UpdateWorkflowByIdResult UpdateWorkflowById(UpdateWorkflowByIdParameter parameter);
        GetAllWorkflowCodeResult GetAllWorkflowCode(GetAllWorkflowCodeParameter parameter);
        GetAllSystemFeatureResult GetAllSystemFeature(GetAllSystemFeatureParameter parameter);
        NextWorkflowStepResult NextWorkflowStep(NextWorkflowStepParameter parameter);
        GetMasterDataCreateWorkflowResult GetMasterDataCreateWorkflow(GetMasterDataCreateWorkflowParameter parameter);
    }
}
