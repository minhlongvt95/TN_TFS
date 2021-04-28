using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class UnfollowListLeadParamerter: BaseParameter
    {
        public List<Guid> ListLeadId { get; set; }
    }
}
