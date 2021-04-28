using System;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CheckDuplicatePersonalCustomerResult : BaseResult
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomerByEmail { get; set; }
        public bool IsDuplicateCustomerByPhone { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContactId { get; set; }   
    }
}
