using System;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class CreateCustomerCareResponse: BaseResponse
    {
        public Guid CustomerCareId { get; set; }
        public List<Guid> CustomerCareCustomer { get; set; }
    }
}
