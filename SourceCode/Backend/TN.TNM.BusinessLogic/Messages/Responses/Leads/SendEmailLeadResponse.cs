using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class SendEmailLeadResponse : BaseResponse
    {
        //public Guid QueueId { get; set; }
        public List<Models.Contact.ContactModel> ListCustomerEmailIgnored { get; set; }
    }
}
