using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataCustomerMeetingByIdRequest : BaseRequest<GetDataCustomerMeetingByIdParameter>
    {
        public Guid? CustomerMeetingId { get; set; }
        public override GetDataCustomerMeetingByIdParameter ToParameter()
        {
            return new GetDataCustomerMeetingByIdParameter()
            {
                CustomerMeetingId = CustomerMeetingId,
                UserId = UserId
            };
        }
    }
}
