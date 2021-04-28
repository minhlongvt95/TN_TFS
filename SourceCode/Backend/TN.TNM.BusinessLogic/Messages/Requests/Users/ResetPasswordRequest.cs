using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class ResetPasswordRequest : BaseRequest<ResetPasswordParameter>
    {
        public string Password { get; set; }
        public override ResetPasswordParameter ToParameter()
        {
            return new ResetPasswordParameter()
            {
                UserId = UserId,
                Password = Password
            };
        }
    }
}
