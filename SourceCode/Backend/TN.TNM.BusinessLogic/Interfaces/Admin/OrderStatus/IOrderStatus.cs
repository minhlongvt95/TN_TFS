using TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.OrderStatus
{
    public interface IOrderStatus
    {
        /// <summary>
        /// Get All Order Status
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        GetAllOrderStatusResponse GetAllOrderStatus(GetAllOrderStatusRequest request);
        /// <summary>
        /// Get Order Status By ID
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        GetOrderStatusByIDResponse GetOrderStatusByID(GetOrderStatusByIDRequest request);

    }
}
