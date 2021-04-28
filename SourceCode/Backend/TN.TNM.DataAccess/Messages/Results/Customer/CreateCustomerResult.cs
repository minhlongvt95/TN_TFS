using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateCustomerResult : BaseResult
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
        public bool DuplicateContact { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
    }
}
