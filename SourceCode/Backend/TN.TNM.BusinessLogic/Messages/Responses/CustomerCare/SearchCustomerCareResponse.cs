using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class SearchCustomerCareResponse:BaseResponse
    {
        public List<CustomerCareModel> LstCustomerCare { get; set; }
    }
}
