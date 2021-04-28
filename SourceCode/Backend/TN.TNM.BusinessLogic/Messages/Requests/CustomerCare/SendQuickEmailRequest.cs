using TN.TNM.BusinessLogic.Models.Queue;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class SendQuickEmailRequest : BaseRequest<SendQuickEmailParameter>
    {
        public QueueModel Queue { get; set; }

        public override SendQuickEmailParameter ToParameter()
        {
            return new SendQuickEmailParameter
            {
                Queue = Queue.ToEntity(),
                UserId = UserId
            };
        }
    }
}
