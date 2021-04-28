using System;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class SendQuickEmailResult : BaseResult
    {
        public Guid QueueId { get; set; }
    }
}
