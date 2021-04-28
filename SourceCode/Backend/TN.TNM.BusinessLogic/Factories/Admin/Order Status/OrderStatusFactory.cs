using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Order_Status
{
    public class OrderStatusFactory : BaseFactory,IOrderStatus
    {
        private IOrderStatusDataAccess iOrderStatusDataAccess;

        public OrderStatusFactory(IOrderStatusDataAccess _iOrderStatusDataAccess, ILogger<OrderStatusFactory> _logger)
        {
            this.iOrderStatusDataAccess = _iOrderStatusDataAccess;
            this.logger = _logger;
        }

        public GetAllOrderStatusResponse GetAllOrderStatus(GetAllOrderStatusRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Order Status");
                var parameter = request.ToParameter();
                var result = iOrderStatusDataAccess.GetAllOrderStatus(parameter);
                var response = new GetAllOrderStatusResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    listOrderStatus = new List<OrderStatusModel>()
                };
                result.listOrderStatus.ForEach(orderStatusEntity =>
                {
                    response.listOrderStatus.Add(new OrderStatusModel(orderStatusEntity));
                });
                return response;

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetAllOrderStatusResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetOrderStatusByIDResponse GetOrderStatusByID(GetOrderStatusByIDRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Order Status");
                var parameter = request.ToParameter();
                var result = iOrderStatusDataAccess.GetOrderStatusByID(parameter);
                var response = new GetOrderStatusByIDResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    orderStatus = new OrderStatusModel(result.orderStatus)
                };
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetOrderStatusByIDResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
