using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetListOrderByLocalPointResponse : BaseResponse
    {
        public List<CustomerOrderEntityModel> ListOrder { get; set; }
    }
}
