using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class SearchPotentialCustomerResult: BaseResult
    {
        public List<DataAccess.Models.Customer.CustomerEntityModel> ListPotentialCustomer { get; set; }
    }
}
