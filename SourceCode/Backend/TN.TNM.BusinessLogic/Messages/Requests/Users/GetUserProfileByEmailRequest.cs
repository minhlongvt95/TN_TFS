using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetUserProfileByEmailRequest : BaseRequest<GetUserProfileByEmailParameter>
    {
        public string UserEmail { get; set; }
        public override GetUserProfileByEmailParameter ToParameter()
        {
            return new GetUserProfileByEmailParameter()
            {                
                UserEmail = UserEmail,
                UserId = UserId
            };
        }
    }
}
