using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CreateCustomerOrderParameter : BaseParameter
    {
        public CustomerOrder CustomerOrder { get; set; }
        public List<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public List<OrderCostDetail> OrderCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public Guid? QuoteId { get; set; }
    }
}
