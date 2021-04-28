using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailAfterCreatedLeadParameter : BaseParameter
    {
        public string CurrentUsername { get; set; }
        public string CurrentUserEmail { get; set; }
        public string CurrentUrl { get; set; }
        public Guid LeadId { get; set; }
    }
}
