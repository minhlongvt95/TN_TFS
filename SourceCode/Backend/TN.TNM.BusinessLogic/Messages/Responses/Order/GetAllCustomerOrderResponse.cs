using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetAllCustomerOrderResponse: BaseResponse
    {
        public List<CustomerOrderModel> OrderList { get; set; }
        public List<CustomerOrderModel> OrderTop5List { get; set; }
    }
}
