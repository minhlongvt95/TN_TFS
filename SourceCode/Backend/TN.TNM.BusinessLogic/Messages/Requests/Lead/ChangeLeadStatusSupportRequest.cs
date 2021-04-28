using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ChangeLeadStatusSupportRequest : BaseRequest<ChangeLeadStatusSupportParameter>
    {
        public Guid LeadId { get; set; }
        public Guid StatusSupportId { get; set; }

        public override ChangeLeadStatusSupportParameter ToParameter()
        {
            return new ChangeLeadStatusSupportParameter()
            {
                UserId = UserId,
                LeadId = LeadId,
                StatusSupportId = StatusSupportId
            };
        }
    }
}
