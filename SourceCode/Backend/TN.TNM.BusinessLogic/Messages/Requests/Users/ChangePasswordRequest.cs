using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class ChangePasswordRequest : BaseRequest<ChangePasswordParameter>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public override ChangePasswordParameter ToParameter()
        {
            return new ChangePasswordParameter() {
                UserId = UserId,
                OldPassword = OldPassword,
                NewPassword = NewPassword
            };
        }
    }
}
