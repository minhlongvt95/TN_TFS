using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class GetAllWorkflowCodeRequest : BaseRequest<GetAllWorkflowCodeParameter>
    {
        public override GetAllWorkflowCodeParameter ToParameter()
        {
            return new GetAllWorkflowCodeParameter()
            {
                UserId = UserId
            };
        }
    }
}
