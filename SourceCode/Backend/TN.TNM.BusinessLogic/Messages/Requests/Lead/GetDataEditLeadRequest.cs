using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataEditLeadRequest: BaseRequest<GetDataEditLeadParameter>
    {
        public Guid LeadId { get; set; }
        public override GetDataEditLeadParameter ToParameter()
        {
            return new GetDataEditLeadParameter
            {
                LeadId = LeadId,
                UserId = UserId
            };
        }
    }
}
