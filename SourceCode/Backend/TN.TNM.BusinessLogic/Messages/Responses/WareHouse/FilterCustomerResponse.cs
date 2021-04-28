using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class FilterCustomerResponse:BaseResponse
    {
        public List<CustomerModel> Customer { get; set; }

    }
}
