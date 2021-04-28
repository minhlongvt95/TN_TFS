using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetAllCustomerOrderResult : BaseResult
    {
        public List<CustomerOrderEntityModel> OrderList { get; set; }
        public List<CustomerOrderEntityModel> OrderTop5List { get; set; }
    }
}
