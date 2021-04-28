using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel
{
    public class AddLevelCustomerParameter : BaseParameter
    {
        public List<Databases.Entities.CustomerServiceLevel> CustomerServiceLevel { get; set; }
    }
}
