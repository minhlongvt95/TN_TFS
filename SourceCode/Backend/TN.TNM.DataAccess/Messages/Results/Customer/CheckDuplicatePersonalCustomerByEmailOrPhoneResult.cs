using System;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CheckDuplicatePersonalCustomerByEmailOrPhoneResult : BaseResult
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomer { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContactId { get; set; }
    }
}
