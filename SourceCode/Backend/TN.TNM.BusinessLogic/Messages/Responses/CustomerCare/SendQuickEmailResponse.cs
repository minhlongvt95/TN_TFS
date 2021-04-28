using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class SendQuickEmailResponse : BaseResponse
    {
        public Guid QueueId { get; set; }
    }
}
