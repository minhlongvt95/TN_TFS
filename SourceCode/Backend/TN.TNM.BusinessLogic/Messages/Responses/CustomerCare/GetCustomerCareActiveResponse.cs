using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCustomerCareActiveResponse : BaseResponse
    {
        public List<GetCustomerCareActiveModel> ListCategoryCare { get; set; }
    }
}
