using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateCustomerServiceParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
        public List<CustomerOrderDetail> ListCustomerOrderDetail { get; set; }
    }
}
