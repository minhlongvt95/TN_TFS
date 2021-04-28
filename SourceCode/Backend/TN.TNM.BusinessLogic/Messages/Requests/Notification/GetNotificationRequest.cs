using System;
using TN.TNM.DataAccess.Messages.Parameters.Notification;

namespace TN.TNM.BusinessLogic.Messages.Requests.Notification
{
    public class GetNotificationRequest : BaseRequest<GetNotificationParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetNotificationParameter ToParameter()
        {
            return new GetNotificationParameter()
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
