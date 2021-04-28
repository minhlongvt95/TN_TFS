using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Notification;
using TN.TNM.BusinessLogic.Messages.Requests.Notification;
using TN.TNM.BusinessLogic.Messages.Responses.Notification;

namespace TN.TNM.Api.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotification _inotification;
        public NotificationController(INotification inotification)
        {
            this._inotification = inotification;
        }

        /// <summary>
        /// GetNotification
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/notification/getNotification")]
        [Authorize(Policy = "Member")]
        public GetNotificationResponse GetNotification([FromBody]GetNotificationRequest request)
        {
            return _inotification.GetNotification(request);
        }

        /// <summary>
        /// RemoveNotification
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/notification/removeNotification")]
        [Authorize(Policy = "Member")]
        public RemoveNotificationResponse RemoveNotification([FromBody]RemoveNotificationRequest request)
        {
            return _inotification.RemoveNotification(request);
        }
    }
}