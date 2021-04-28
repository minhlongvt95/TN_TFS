using TN.TNM.BusinessLogic.Messages.Requests.Workflow;
using TN.TNM.BusinessLogic.Messages.Responses.Workflow;

namespace TN.TNM.BusinessLogic.Interfaces.Workflow
{
    public interface IWorkflow
    {
        CreateWorkflowResponse CreateWorkflow(CreateWorkflowRequest request);
        GetAllWorkflowCodeResponse GetAllWorkflowCode(GetAllWorkflowCodeRequest request);
        GetWorkflowByIdResponse GetWorkflowById(GetWorkflowByIdRequest request);
        UpdateWorkflowByIdResponse UpdateWorkflowById(UpdateWorkflowByIdRequest request);
        SearchWorkflowResponse SearchWorkflow(SearchWorkflowRequest request);
        GetAllSystemFeatureResponse GetAllSystemFeature(GetAllSystemFeatureRequest request);
        NextWorkflowStepResponse NextWorkflowStep(NextWorkflowStepRequest request);
        GetMasterDataCreateWorkflowResponse GetMasterDataCreateWorkflow(GetMasterDataCreateWorkflowRequest request);
    }
}
