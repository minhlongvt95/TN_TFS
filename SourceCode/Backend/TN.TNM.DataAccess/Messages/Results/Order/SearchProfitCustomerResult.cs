using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class SearchProfitCustomerResult: BaseResult
    {
        public List<DataAccess.Models.CustomerOrder.SearchProfitCustomerModel> ListSearchProfitCustomer { get; set; }
    }
}
