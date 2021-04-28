using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class GetAllSystemFeatureRequest : BaseRequest<GetAllSystemFeatureParameter>
    {
        public override GetAllSystemFeatureParameter ToParameter()
        {
            return new GetAllSystemFeatureParameter() {
                UserId = UserId
            };
        }
    }
}
