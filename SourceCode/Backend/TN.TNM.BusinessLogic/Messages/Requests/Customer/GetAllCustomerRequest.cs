using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllCustomerRequest : BaseRequest<GetAllCustomerParameter>
    {
        public override GetAllCustomerParameter ToParameter()
        {
            return new GetAllCustomerParameter
            {
                UserId = UserId
            };
        }
    }
}
