using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel
{
    public class GetConfigCustomerServiceLevelResult : BaseResult
    {
        public List<Databases.Entities.CustomerServiceLevel> CustomerServiceLevel { get; set; }
    }
}
