using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetCustomerOrderBySellerResponse : BaseResponse
    {
        public List<CustomerOrderModel> OrderList { get; set; }
        public List<dynamic> lstResult { get; set; }
        public decimal totalProduct { get; set; }
        public int? levelMaxProductCategory { get; set; }
    }
}
