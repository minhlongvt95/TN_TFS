using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.HubConfig;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Notification;
using TN.TNM.DataAccess.Messages.Results.Notification;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class NotificationDAO : BaseDAO, INotificationDataAccess
    {
        private IHubContext<Notification> hub;

        public NotificationDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace,
            IHubContext<Notification> _hub)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hub = _hub;
        }
        public GetNotificationResult GetNotification(GetNotificationParameter parameter)
        {
            List<Notifications> lst = new List<Notifications>();
            List<Notifications> shortlst = new List<Notifications>();
            lst = context.Notifications.Where(n => n.ReceiverId == parameter.EmployeeId).OrderByDescending(n => n.CreatedDate).ToList();
            shortlst = lst.Take(10).ToList();
            var numberOfNoti = lst.Count(n => !n.Viewed);

            //hub.Clients.All.SendAsync("ReceiveNotifications", lst);

            return new GetNotificationResult() {
                NotificationList = lst,
                ShortNotificationList = shortlst,
                Status = true,
                NumberOfUncheckedNoti = numberOfNoti
            };
        }
        public RemoveNotificationResult RemoveNotification(RemoveNotificationParameter parameter)
        {
            var notification = context.Notifications.FirstOrDefault(n => n.NotificationId == parameter.NotificationId);
            notification.Viewed = true;
            context.SaveChanges();
            return new RemoveNotificationResult() {
                Status = true
            };
        }
    }
}
