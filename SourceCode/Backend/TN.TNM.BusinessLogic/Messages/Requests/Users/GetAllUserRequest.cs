using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetAllUserRequest : BaseRequest<GetAllUserParameter>
    {
        public override GetAllUserParameter ToParameter()
        {
            return new GetAllUserParameter()
            {
                UserId = UserId
            };
        }
    }
}
