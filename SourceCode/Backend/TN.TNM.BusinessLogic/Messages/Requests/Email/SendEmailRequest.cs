using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailRequest : BaseRequest<SendEmailParameter>
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }

        public override SendEmailParameter ToParameter()
        {
            return new SendEmailParameter()
            {
                EmailAddress = EmailAddress,
                FullName = FullName,
                UserName = UserName
            };
        }
    }
}
