using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ChangeLeadStatusToUnfollowRequest : BaseRequest<ChangeLeadStatusToUnfollowParameter>
    {
        public List<Guid> LeadIdList { get; set; }
        public override ChangeLeadStatusToUnfollowParameter ToParameter()
        {
            return new ChangeLeadStatusToUnfollowParameter()
            {
                UserId = UserId,
                LeadIdList = LeadIdList
            };
        }
    }
}
