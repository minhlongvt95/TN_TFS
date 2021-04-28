using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetUserProfileRequest : BaseRequest<GetUserProfileParameter>
    {
        public override GetUserProfileParameter ToParameter()
        {
            return new GetUserProfileParameter()
            {
                UserId = UserId
            };
        }
    }
}
