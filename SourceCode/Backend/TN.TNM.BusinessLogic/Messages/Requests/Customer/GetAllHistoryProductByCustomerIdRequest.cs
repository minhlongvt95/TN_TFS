using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetAllHistoryProductByCustomerIdRequest : BaseRequest<GetAllHistoryProductByCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }

        public override GetAllHistoryProductByCustomerIdParameter ToParameter()
        {
            return new GetAllHistoryProductByCustomerIdParameter()
            {
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
