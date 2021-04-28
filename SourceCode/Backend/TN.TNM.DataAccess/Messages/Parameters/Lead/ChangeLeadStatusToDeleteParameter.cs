using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ChangeLeadStatusToDeleteParameter : BaseParameter
    {
        public Guid LeadId { get; set; }
    }
}
