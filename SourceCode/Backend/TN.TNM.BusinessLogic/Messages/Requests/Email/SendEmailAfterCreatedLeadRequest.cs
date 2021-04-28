using System;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailAfterCreatedLeadRequest : BaseRequest<SendEmailAfterCreatedLeadParameter>
    {
        public string CurrentUsername { get; set; }
        public string CurrentUserEmail { get; set; }
        public string CurrentUrl { get; set; }
        public Guid LeadId { get; set; }
        public override SendEmailAfterCreatedLeadParameter ToParameter()
        {
            return new SendEmailAfterCreatedLeadParameter() {
                CurrentUserEmail = CurrentUserEmail,
                CurrentUsername = CurrentUsername,
                CurrentUrl = CurrentUrl,
                LeadId = LeadId,
                UserId = UserId
            };
        }
    }
}
