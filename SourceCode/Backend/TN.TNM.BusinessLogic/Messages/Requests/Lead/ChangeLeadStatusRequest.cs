using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ChangeLeadStatusRequest : BaseRequest<ChangeLeadStatusParameter>
    {
        public Guid LeadId { get; set; }
        public Guid StatusId { get; set; }

        public override ChangeLeadStatusParameter ToParameter()
        {
            return new ChangeLeadStatusParameter()
            {
                LeadId = LeadId,
                StatusId = StatusId,
                UserId = UserId
            };
        }
    }
}
