using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetCheckUserNameRequest : BaseRequest<GetCheckUserNameParameter>
    {
        public string UserName { get; set; }
        public override GetCheckUserNameParameter ToParameter()
        {
            return new GetCheckUserNameParameter()
            {
                UserName = UserName
            };
        }
    }
}
