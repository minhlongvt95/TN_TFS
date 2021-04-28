using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetCustomerFromOrderCreateResult : BaseResult
    {
        public List<CustomerEntityModel> Customer { get; set; }
    }
}
