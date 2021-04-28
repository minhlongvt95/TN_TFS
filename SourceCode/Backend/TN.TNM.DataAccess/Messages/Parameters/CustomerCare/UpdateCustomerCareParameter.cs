using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateCustomerCareParameter:BaseParameter
    {
        public Databases.Entities.CustomerCare CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public string QueryFilter { get; set; }
    }
}
