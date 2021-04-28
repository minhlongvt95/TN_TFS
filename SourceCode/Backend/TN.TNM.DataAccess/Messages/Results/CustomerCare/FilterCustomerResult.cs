using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class FilterCustomerResult:BaseResult
    {
        public List<CustomerEntityModel> ListCustomer { get; set; }
    }
}
