using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class ImportListCustomerParameter: BaseParameter
    {
        public List<Databases.Entities.Customer> ListCustomer { get; set; }
        public List<Databases.Entities.Contact> ListContact { get; set; }
        public List<Databases.Entities.Contact> ListContactAdditional { get; set; }
        public bool IsPotentialCustomer { get; set; }
    }
}
