using TN.TNM.BusinessLogic.Models.OrderStatus;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus
{
    public class GetOrderStatusByIDResponse: BaseResponse
    {
        public OrderStatusModel orderStatus { get; set; }
    }
}
