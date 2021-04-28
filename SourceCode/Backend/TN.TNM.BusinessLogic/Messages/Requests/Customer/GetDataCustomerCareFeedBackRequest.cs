using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataCustomerCareFeedBackRequest : BaseRequest<GetDataCustomerCareFeedBackParameter>
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }
        public override GetDataCustomerCareFeedBackParameter ToParameter()
        {
            return new GetDataCustomerCareFeedBackParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId,
                CustomerCareId = CustomerCareId
            };
        }
    }
}
