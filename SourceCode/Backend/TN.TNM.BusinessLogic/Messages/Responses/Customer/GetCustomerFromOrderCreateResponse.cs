using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetCustomerFromOrderCreateResponse : BaseResponse
    {
        public List<CustomerModel> Customer { get; set; }
    }
}
