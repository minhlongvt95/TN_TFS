using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class FilterCustomerResult:BaseResult
    {
        public List<CustomerEntityModel> Customer { get; set; }

    }
}
