using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin
{
    public class LoginRequest : BaseRequest<LoginParameter>
    {
        public UserModel User { get; set; }

        public override LoginParameter ToParameter()
        {
            return new LoginParameter
            {
                UserId = this.UserId,
                User = this.User.ToEntity()
            };
        }
    }
}
