using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetDataCustomerCareFeedBackParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }
    }
}
