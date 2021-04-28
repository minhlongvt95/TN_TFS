using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class CreateCustomerCareParameter : BaseParameter
    {
        public Databases.Entities.CustomerCare CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public List<string> ListTypeCustomer { get; set; }
        public string QueryFilter { get; set; }
    }
}
