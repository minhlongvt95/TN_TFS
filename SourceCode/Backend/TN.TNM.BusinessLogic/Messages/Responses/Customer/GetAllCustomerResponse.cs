using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllCustomerResponse : BaseResponse
    {
        public List<CustomerModel> CustomerList { get; set; }
    }
}
