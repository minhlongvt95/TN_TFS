using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CheckDuplicateCustomerAllTypeResult: BaseResult
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomer { get; set; }
        public Databases.Entities.Lead DuplicateLeadModel { get; set; }
        public Databases.Entities.Contact DuplicateLeadContactModel { get; set; }
        public Databases.Entities.Contact DuplicateCustomerContactModel { get; set; }
    }
}
