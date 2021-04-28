using TN.TNM.DataAccess.Messages.Parameters.Notification;
using TN.TNM.DataAccess.Messages.Results.Notification;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface INotificationDataAccess
    {
        GetNotificationResult GetNotification(GetNotificationParameter parameter);
        RemoveNotificationResult RemoveNotification(RemoveNotificationParameter parameter);
    }
}
