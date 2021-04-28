using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class CheckPhoneLeadRequest : BaseRequest<CheckPhoneLeadParameter>
    {
        public string PhoneNumber { get; set; }

        public override CheckPhoneLeadParameter ToParameter() => new CheckPhoneLeadParameter
        {
            PhoneNumber = PhoneNumber,
            UserId = UserId
        };
    }
}
