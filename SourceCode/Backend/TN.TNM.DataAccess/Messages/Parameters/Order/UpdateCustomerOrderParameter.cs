using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateCustomerOrderParameter : BaseParameter
    {
        public CustomerOrder CustomerOrder { get; set; }
        public List<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public List<OrderCostDetail> OrderCostDetail { get; set; }
        public int TypeAccount { get; set; }
    }
}
