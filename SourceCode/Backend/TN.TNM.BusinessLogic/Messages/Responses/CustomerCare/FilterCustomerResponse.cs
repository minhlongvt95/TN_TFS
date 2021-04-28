using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class FilterCustomerResponse:BaseResponse
    {
        public List<FilterCustomerModel> ListCustomer { get; set; }
    }
}
