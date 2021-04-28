using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCustomerNewCSResponse : BaseResponse
    {
        public List<GetCustomerNewCSModel> ListCustomerNewOrder { get; set; }
    }
}
