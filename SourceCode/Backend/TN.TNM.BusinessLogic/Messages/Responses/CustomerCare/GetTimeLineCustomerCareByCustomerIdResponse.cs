using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetTimeLineCustomerCareByCustomerIdResponse : BaseResponse
    {
        public List<dynamic> ListCustomerCare { get; set; }
    } 
}
