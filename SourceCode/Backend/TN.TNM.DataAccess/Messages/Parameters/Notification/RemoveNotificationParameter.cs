using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Notification
{
    public class RemoveNotificationParameter : BaseParameter
    {
        public Guid NotificationId { get; set; }
    }
}
