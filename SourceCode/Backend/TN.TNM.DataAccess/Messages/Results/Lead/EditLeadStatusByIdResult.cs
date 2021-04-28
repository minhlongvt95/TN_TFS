using System;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class EditLeadStatusByIdResult : BaseResult
    {
        public Guid LeadId { get; set; }
    }
}
