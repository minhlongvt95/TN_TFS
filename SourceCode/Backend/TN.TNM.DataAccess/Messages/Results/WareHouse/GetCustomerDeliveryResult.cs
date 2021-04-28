using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetCustomerDeliveryResult:BaseResult
    {
        public List<CustomerEntityModel> LstCustomer { get; set; }

    }
}
