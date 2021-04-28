using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class SendEmailSupportLeadResponse : BaseResponse
    {
        public Guid QueueId { get; set; }
    }
}
