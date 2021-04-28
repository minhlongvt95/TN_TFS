using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCustomerNewCSRequest : BaseRequest<GetCustomerNewCSParameter>
    {

        public override GetCustomerNewCSParameter ToParameter()
        {
            return new GetCustomerNewCSParameter
            {
            };
        }
    }
}
