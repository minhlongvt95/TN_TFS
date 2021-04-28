using System;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ChangeLeadStatusToDeleteRequest : BaseRequest<ChangeLeadStatusToDeleteParameter>
    {
        public Guid LeadId { get; set; }
        public override ChangeLeadStatusToDeleteParameter ToParameter()
        {
            return new ChangeLeadStatusToDeleteParameter()
            {
                UserId = UserId,
                LeadId = LeadId
            };
        }
    }
}
