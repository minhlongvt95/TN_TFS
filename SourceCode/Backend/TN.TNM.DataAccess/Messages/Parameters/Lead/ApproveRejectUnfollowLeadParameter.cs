using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ApproveRejectUnfollowLeadParameter : BaseParameter
    {
        public List<Guid> LeadIdList { get; set; }
        public bool IsApprove { get; set; }
    }
}
