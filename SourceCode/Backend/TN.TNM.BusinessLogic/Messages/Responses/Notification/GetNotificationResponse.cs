using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Notification;

namespace TN.TNM.BusinessLogic.Messages.Responses.Notification
{
    public class GetNotificationResponse : BaseResponse
    {
        public List<NotificationModel> NotificationList { get; set; }
        public List<NotificationModel> ShortNotificationList { get; set; }
        public int NumberOfUncheckedNoti { get; set; }
    }
}
