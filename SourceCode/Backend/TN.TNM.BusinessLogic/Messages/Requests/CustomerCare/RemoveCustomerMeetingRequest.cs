using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class RemoveCustomerMeetingRequest : BaseRequest<RemoveCustomerMettingParameter>
    {
        public Guid CustomerMeetingId { get; set; }
        public override RemoveCustomerMettingParameter ToParameter()
        {
            return new RemoveCustomerMettingParameter
            {
                CustomerMeetingId = this.CustomerMeetingId,
                UserId = this.UserId
            };
        }
    }
}
