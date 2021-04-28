using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Notification
{
    public class GetNotificationResult : BaseResult
    {
        public List<Notifications> NotificationList { get; set; }
        public List<Notifications> ShortNotificationList { get; set; }
        public int NumberOfUncheckedNoti { get; set; }
    }
}
