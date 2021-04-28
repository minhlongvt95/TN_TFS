using System;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class SendQuickSMSResult : BaseResult
    {
        public Guid QueueId { get; set; }
    }
}
