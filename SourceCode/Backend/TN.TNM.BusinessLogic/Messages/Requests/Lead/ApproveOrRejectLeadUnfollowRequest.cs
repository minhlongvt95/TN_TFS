using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;


namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ApproveOrRejectLeadUnfollowRequest: BaseRequest<ApproveOrRejectLeadUnfollowParameter>
    {
        public List<Guid> LeadIdList { get; set; }
        public bool IsApprove { get; set; }

        public override ApproveOrRejectLeadUnfollowParameter ToParameter()
        {
            return new ApproveOrRejectLeadUnfollowParameter()
            {
                UserId = UserId,
                LeadIdList = LeadIdList,
                IsApprove = IsApprove
            };
        }
    }
}
