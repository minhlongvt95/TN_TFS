using System;
using TN.TNM.DataAccess.Messages.Parameters.Notification;

namespace TN.TNM.BusinessLogic.Messages.Requests.Notification
{
    public class RemoveNotificationRequest : BaseRequest<RemoveNotificationParameter>
    {
        public Guid NotificationId { get; set; }
        public override RemoveNotificationParameter ToParameter()
        {
            return new RemoveNotificationParameter()
            {
                NotificationId = NotificationId,
                UserId = UserId
            };
        }
    }
}
