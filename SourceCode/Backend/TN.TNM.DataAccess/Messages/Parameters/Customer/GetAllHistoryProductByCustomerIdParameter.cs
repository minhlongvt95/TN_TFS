using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetAllHistoryProductByCustomerIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
