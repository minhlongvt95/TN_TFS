using TN.TNM.BusinessLogic.Models.Queue;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class SendQuickSMSRequest : BaseRequest<SendQuickSMSParameter>
    {
        public QueueModel Queue { get; set; }

        public override SendQuickSMSParameter ToParameter()
        {
            return new SendQuickSMSParameter
            {
                Queue = Queue.ToEntity(),
                UserId = UserId
            };
        }
    }
}
