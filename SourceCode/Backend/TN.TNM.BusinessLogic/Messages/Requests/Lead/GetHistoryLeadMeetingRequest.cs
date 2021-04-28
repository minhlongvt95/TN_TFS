using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetHistoryLeadMeetingRequest : BaseRequest<GetHistoryLeadMeetingParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid LeadId { get; set; }
        public override GetHistoryLeadMeetingParameter ToParameter()
        {
            return new GetHistoryLeadMeetingParameter()
            {
                Month = Month,
                Year = Year,
                LeadId = LeadId,
                UserId = UserId
            };
        }
    }
}
