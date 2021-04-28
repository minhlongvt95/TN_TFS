using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetCustomerOrderBySellerResult : BaseResult
    {
        public List<CustomerOrderEntityModel> OrderList { get; set; }
        public List<dynamic> lstResult { get; set; }
        public decimal totalProduct { get; set; }
        public int? levelMaxProductCategory { get; set; }
    }
}
