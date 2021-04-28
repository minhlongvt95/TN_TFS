using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailForgotPassRequest : BaseRequest<SendEmailForgotPassParameter>
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }

        public override SendEmailForgotPassParameter ToParameter()
        {
            return new SendEmailForgotPassParameter()
            {
                UserId = UserId,
                EmailAddress = EmailAddress,
                FullName = FullName,
                UserName = UserName
            };
        }
    }
}
