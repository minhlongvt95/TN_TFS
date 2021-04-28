using TN.TNM.BusinessLogic.Messages.Requests.Notification;
using TN.TNM.BusinessLogic.Messages.Responses.Notification;

namespace TN.TNM.BusinessLogic.Interfaces.Notification
{
    public interface INotification
    {
        GetNotificationResponse GetNotification(GetNotificationRequest request);
        RemoveNotificationResponse RemoveNotification(RemoveNotificationRequest request);
    }
}
