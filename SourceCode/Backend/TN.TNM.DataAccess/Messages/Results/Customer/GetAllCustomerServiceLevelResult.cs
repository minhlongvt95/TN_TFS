using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerServiceLevelResult : BaseResult
    {
        public List<CustomerServiceLevel> CustomerServiceLevelList { get; set; }
    }
}
