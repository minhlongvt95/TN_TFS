using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetAllCustomerContactParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
