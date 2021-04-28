using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class SendQuickSMSResponse : BaseResponse
    {
        public Guid QueueId { get; set; }
    }
}
