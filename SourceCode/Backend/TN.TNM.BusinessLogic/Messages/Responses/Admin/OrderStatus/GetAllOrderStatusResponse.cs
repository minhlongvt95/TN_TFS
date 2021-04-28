using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.OrderStatus;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus
{
    public class GetAllOrderStatusResponse: BaseResponse
    {
        public List<OrderStatusModel> listOrderStatus { get; set; }
    }
}
