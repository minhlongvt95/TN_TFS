using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetHistoryCustomerMeetingRequest : BaseRequest<GetHistoryCustomerMeetingParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
        public override GetHistoryCustomerMeetingParameter ToParameter()
        {
            return new GetHistoryCustomerMeetingParameter()
            {
                Month = Month,
                Year = Year,
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
