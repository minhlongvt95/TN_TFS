using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetHistoryCustomerCareRequest : BaseRequest<GetHistoryCustomerCareParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }

        public override GetHistoryCustomerCareParameter ToParameter()
        {
            return new GetHistoryCustomerCareParameter()
            {
                UserId = UserId,
                Month = Month,
                Year = Year,
                CustomerId = CustomerId
            };
        }
    }
}
