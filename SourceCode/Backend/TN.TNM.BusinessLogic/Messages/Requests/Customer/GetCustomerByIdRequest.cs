using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetCustomerByIdRequest : BaseRequest<GetCustomerByIdParameter>
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
        public override GetCustomerByIdParameter ToParameter()
        {
            return new GetCustomerByIdParameter()
            {
                ContactId = ContactId,
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
