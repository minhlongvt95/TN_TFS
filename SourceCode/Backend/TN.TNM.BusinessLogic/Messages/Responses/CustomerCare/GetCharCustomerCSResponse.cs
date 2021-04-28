using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCharCustomerCSResponse : BaseResponse
    {
        public List<GetCharCustomerCSModel> ListChar { get; set; }
    }
}
