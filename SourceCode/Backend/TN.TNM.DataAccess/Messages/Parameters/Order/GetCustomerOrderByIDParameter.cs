using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetCustomerOrderByIDParameter : BaseParameter
    {
        public Guid CustomerOrderId { get; set; }
    }
}
