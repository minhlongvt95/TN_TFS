using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCustomerCareByIdResponse:BaseResponse
    {
        public CustomerCareModel CustomerCare { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CustomerCareFeedBackModel> CustomerCareFeedBack { get; set; }
        public string QueryFilter { get; set; }
        public int TypeCustomer { get; set; }
    }
}
