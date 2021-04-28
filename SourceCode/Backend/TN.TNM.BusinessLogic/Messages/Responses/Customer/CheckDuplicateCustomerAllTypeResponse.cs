using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Results.Customer;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Contact;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class CheckDuplicateCustomerAllTypeResponse: BaseResponse
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomer { get; set; }
        public LeadModel DuplicateLeadModel { get; set; }
        public ContactModel DuplicateLeadContactModel { get; set; }
        public ContactModel DuplicateCustomerContactModel { get; set; }
    }
}
