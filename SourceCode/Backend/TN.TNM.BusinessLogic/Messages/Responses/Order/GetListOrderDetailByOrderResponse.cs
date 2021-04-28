using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetListOrderDetailByOrderResponse : BaseResponse
    {
        public List<CustomerOrderDetailEntityModel> ListOrderDetail { get; set; }
    }
}
