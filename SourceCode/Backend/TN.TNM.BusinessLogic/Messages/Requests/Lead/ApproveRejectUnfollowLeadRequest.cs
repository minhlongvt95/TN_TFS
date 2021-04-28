using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ApproveRejectUnfollowLeadRequest : BaseRequest<ApproveRejectUnfollowLeadParameter>
    {
        public List<Guid> LeadIdList { get; set; }
        public bool IsApprove { get; set; }
        public override ApproveRejectUnfollowLeadParameter ToParameter()
        {
            return new ApproveRejectUnfollowLeadParameter() {
                UserId = UserId,
                LeadIdList = LeadIdList,
                IsApprove = IsApprove
            };
        }
    }
}
