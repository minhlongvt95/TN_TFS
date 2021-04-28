using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetTimeLineCustomerCareByCustomerIdResult : BaseResult
    {
        public List<dynamic> ListCustomerCare { get; set; }
    } 
}
