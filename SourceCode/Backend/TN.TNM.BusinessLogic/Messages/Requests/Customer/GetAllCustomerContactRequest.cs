using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllCustomerContactRequest : BaseRequest<GetAllCustomerContactParameter>
    {
        public Guid CustomerId { get; set; }
        public override GetAllCustomerContactParameter ToParameter()
        {
            return new GetAllCustomerContactParameter() {
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
