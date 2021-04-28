using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetListOrderByLocalPointResult : BaseResult
    {
        public List<CustomerOrderEntityModel> ListOrder { get; set; }
    }
}
