using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataLeadCareFeedBackRequest : BaseRequest<GetDataLeadCareFeedBackParameter>
    {
        public Guid LeadId { get; set; }
        public Guid LeadCareId { get; set; }

        public override GetDataLeadCareFeedBackParameter ToParameter()
        {
            return new GetDataLeadCareFeedBackParameter
            {
                UserId = UserId,
                LeadId = LeadId,
                LeadCareId = LeadCareId
            };
        }
    }
}
