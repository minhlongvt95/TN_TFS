using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class CheckEmailLeadRequest : BaseRequest<CheckEmailLeadParameter>
    {
        public string Email { get; set; }

        public override CheckEmailLeadParameter ToParameter() => new CheckEmailLeadParameter
        {
            Email = Email,
            UserId = UserId
        };
    }
}
