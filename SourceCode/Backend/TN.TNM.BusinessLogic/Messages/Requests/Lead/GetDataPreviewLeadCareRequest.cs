using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataPreviewLeadCareRequest : BaseRequest<GetDataPreviewLeadCareParameter>
    {
        public string Mode { get; set; }
        public Guid LeadId { get; set; }
        public Guid LeadCareId { get; set; }

        public override GetDataPreviewLeadCareParameter ToParameter()
        {
            return new GetDataPreviewLeadCareParameter
            {
                UserId = UserId,
                Mode = Mode,
                LeadId = LeadId,
                LeadCareId = LeadCareId
            };
        }
    }
}
