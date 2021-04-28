using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllCustomerServiceLevelResponse : BaseResponse
    {
        public List<CustomerServiceLevelModel> CustomerServiceLevelList { get; set; }
    }
}
