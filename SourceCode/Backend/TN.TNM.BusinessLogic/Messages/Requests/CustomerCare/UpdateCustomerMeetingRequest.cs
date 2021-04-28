using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateCustomerMeetingRequest : BaseRequest<UpdateCustomerMettingParameter>
    {
        public Guid CustomerMeetingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override UpdateCustomerMettingParameter ToParameter()
        {
            return new UpdateCustomerMettingParameter
            {
                CustomerMeetingId = this.CustomerMeetingId,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                UserId = this.UserId
            };
        }
    }
}
