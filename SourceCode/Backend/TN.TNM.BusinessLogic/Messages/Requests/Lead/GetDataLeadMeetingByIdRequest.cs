using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataLeadMeetingByIdRequest : BaseRequest<GetDataLeadMeetingByIdParameter>
    {
        public Guid? LeadMeetingId { get; set; }
        public override GetDataLeadMeetingByIdParameter ToParameter()
        {
            return new GetDataLeadMeetingByIdParameter()
            {
                LeadMeetingId = LeadMeetingId,
                UserId = UserId
            };
        }
    }
}
