using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllCustomerCodeRequest : BaseRequest<GetAllCustomerCodeParameter>
    {
        public string Mode { get; set; }
        public string Code { get; set; }
        public override GetAllCustomerCodeParameter ToParameter()
        {
            return new GetAllCustomerCodeParameter() {
                UserId = UserId,
                Code = Code,
                Mode = Mode
            };
        }
    }
}
