using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllCustomerAdditionalByCustomerIdRequest : BaseRequest<GetAllCustomerAdditionalByCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }

        public override GetAllCustomerAdditionalByCustomerIdParameter ToParameter()
        {
            return new GetAllCustomerAdditionalByCustomerIdParameter()
            {
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
