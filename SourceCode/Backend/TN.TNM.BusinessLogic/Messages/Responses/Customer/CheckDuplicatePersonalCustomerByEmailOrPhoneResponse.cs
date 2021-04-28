using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class CheckDuplicatePersonalCustomerByEmailOrPhoneResponse: BaseResponse
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomer { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContactId { get; set; }
    }
}
