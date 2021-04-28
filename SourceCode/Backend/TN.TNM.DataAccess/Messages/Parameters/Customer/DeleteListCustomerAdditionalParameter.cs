using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class DeleteListCustomerAdditionalParameter : BaseParameter
    {
        public List<Guid> ListCusAddInfId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
