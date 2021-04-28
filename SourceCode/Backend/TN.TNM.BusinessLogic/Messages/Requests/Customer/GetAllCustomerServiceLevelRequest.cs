using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllCustomerServiceLevelRequest : BaseRequest<GetAllCustomerServiceLevelParameter>
    {
        public override GetAllCustomerServiceLevelParameter ToParameter()
        {
            return new GetAllCustomerServiceLevelParameter()
            {
                UserId = UserId
            };
        }
    }
}
