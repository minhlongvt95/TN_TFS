using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetCheckResetCodeUserRequest : BaseRequest<GetCheckResetCodeUserParameter>
    {
        public string Code { get; set; }
        public override GetCheckResetCodeUserParameter ToParameter()
        {
            return new GetCheckResetCodeUserParameter()
            {
                Code = Code
            };
        }
    }
}
