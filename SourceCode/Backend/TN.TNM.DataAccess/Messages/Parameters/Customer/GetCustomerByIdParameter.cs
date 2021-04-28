using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetCustomerByIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
    }
}
