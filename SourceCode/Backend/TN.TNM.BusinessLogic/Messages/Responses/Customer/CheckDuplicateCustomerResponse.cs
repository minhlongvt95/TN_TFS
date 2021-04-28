using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class CheckDuplicateCustomerResponse:BaseResponse
    {
        public bool IsDuplicate { get; set; }
        public bool IsDuplicateByEmailLead { get; set; }
        public bool IsDuplicateByPhoneLead { get; set; }
        public bool IsDuplicateByEmailCustomer { get; set; }
        public bool IsDuplicateByPhoneCustomer { get; set; }
        public Guid ContactId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid LeadId { get; set; }
    }
}
