using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class CheckDuplicatePersonalCustomerResponse : BaseResponse
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomerByEmail { get; set; }
        public bool IsDuplicateCustomerByPhone { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContactId { get; set; }
    }
}
