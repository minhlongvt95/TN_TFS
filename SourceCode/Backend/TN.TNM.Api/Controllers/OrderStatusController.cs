using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus;

namespace TN.TNM.Api.Controllers
{
    public class OrderStatusController : Controller
    {
        private readonly IOrderStatus iOrderStatus;
        public OrderStatusController(IOrderStatus _iOrderStatus)
        {
            this.iOrderStatus = _iOrderStatus;
        }

        /// <summary>
        /// get All Order Status
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/orderstatus/getAllOrderStatus")]
        [Authorize(Policy = "Member")]
        public GetAllOrderStatusResponse GetAllOrderStatus([FromBody]GetAllOrderStatusRequest request)
        {
            return this.iOrderStatus.GetAllOrderStatus(request);
        }
        /// <summary>
        /// get All Order Status
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/orderstatus/getOrderStatusByID")]
        [Authorize(Policy = "Member")]
        public GetOrderStatusByIDResponse GetOrderStatusByID([FromBody]GetOrderStatusByIDRequest request)
        {
            return this.iOrderStatus.GetOrderStatusByID(request);
        }


    }
}