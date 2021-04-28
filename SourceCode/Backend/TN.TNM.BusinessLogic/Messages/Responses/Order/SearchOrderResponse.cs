using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class SearchOrderResponse : BaseResponse
    {
        public List<CustomerOrderModel> ListOrder { get; set; }
    }
}
