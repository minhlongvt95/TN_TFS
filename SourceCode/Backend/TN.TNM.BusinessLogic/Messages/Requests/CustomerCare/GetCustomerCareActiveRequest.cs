using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCustomerCareActiveRequest : BaseRequest<GetCustomerCareActiveParameter>
    {

        public override GetCustomerCareActiveParameter ToParameter()
        {
            return new GetCustomerCareActiveParameter
            {
            };
        }
    }
}
