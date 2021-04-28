using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CreateOrderServiceParameter : BaseParameter
    {
        public CustomerOrder CustomerOrder { get; set; }
        public List<CustomerOrderDetail> ListCustomerOrderDetail { get; set; }
        public List<Guid> ListLocalPointId { get; set; }
    }
}
