using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Notification;
using TN.TNM.BusinessLogic.Messages.Requests.Notification;
using TN.TNM.BusinessLogic.Messages.Responses.Notification;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Notification
{
    public class NotificationFactory : BaseFactory, INotification
    {
        private INotificationDataAccess iNotificationDataAccess;

        public NotificationFactory(INotificationDataAccess _iNotificationDataAccess, ILogger<NotificationFactory> _logger)
        {
            this.iNotificationDataAccess = _iNotificationDataAccess;
            this.logger = _logger;
        }
        public GetNotificationResponse GetNotification(GetNotificationRequest request)
        {
            try
            {
                logger.LogInformation("Get Notification");
                var parameter = request.ToParameter();
                var result = iNotificationDataAccess.GetNotification(parameter);
                var response = new GetNotificationResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    NotificationList = new List<Models.Notification.NotificationModel>(),
                    ShortNotificationList = new List<Models.Notification.NotificationModel>(),
                    NumberOfUncheckedNoti = result.NumberOfUncheckedNoti
                };

                result.NotificationList.ForEach(item => {
                    response.NotificationList.Add(new Models.Notification.NotificationModel(item));
                });
                result.ShortNotificationList.ForEach(item => {
                    response.ShortNotificationList.Add(new Models.Notification.NotificationModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetNotificationResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        public RemoveNotificationResponse RemoveNotification(RemoveNotificationRequest request)
        {
            try
            {
                logger.LogInformation("Remove Notification");
                var parameter = request.ToParameter();
                var result = iNotificationDataAccess.RemoveNotification(parameter);
                var response = new RemoveNotificationResponse
                {
                    StatusCode = HttpStatusCode.OK
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new RemoveNotificationResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
