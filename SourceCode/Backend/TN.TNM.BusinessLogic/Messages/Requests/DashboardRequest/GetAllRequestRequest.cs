using TN.TNM.DataAccess.Messages.Parameters.DashboardRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.DashboardRequest
{
    public class GetAllRequestRequest : BaseRequest<GetAllRequestParameter>
    {
        public override GetAllRequestParameter ToParameter()
        {
            return new GetAllRequestParameter()
            {
                UserId = UserId
            };
        }
    }
}
