using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetHistoryLeadCareRequest : BaseRequest<GetHistoryLeadCareParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid LeadId { get; set; }

        public override GetHistoryLeadCareParameter ToParameter()
        {
            return new GetHistoryLeadCareParameter()
            {
                UserId = UserId,
                LeadId = LeadId,
                Month = Month,
                Year = Year
            };
        }
    }
}
