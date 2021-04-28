using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetCustomerByEmployeeIdResult:BaseResult
    {
        public List<CustomerEntityModel> ListCustomer { get; set; }
    }
}
