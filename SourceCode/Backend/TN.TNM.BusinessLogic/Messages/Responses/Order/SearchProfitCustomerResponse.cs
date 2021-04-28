using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class SearchProfitCustomerResponse: BaseResponse
    {
        public List<DataAccess.Models.CustomerOrder.SearchProfitCustomerModel> ListSearchProfitCustomer { get; set; }
    }
}
