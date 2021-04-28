using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.CustomerServiceLevel
{
    public class GetConfigCustomerServiceLevelResponse : BaseResponse
    {
        public List<CustomerServiceLevelModel> CustomerServiceLevel { get; set; }
    }
}
