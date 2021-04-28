using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetAllCustomerAdditionalByCustomerIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
