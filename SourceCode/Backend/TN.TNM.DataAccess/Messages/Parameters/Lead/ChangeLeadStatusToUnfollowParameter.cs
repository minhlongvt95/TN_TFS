using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ChangeLeadStatusToUnfollowParameter : BaseParameter
    {
        public List<Guid> LeadIdList { get; set; }
    }
}
