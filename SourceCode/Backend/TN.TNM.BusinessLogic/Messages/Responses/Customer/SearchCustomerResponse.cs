using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class SearchCustomerResponse : BaseResponse
    {
        public List<CustomerSearchModel> Customer { get; set; }
    }
}
