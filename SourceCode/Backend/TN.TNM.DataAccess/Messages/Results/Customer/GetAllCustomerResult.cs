using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerResult : BaseResult
    {
        public List<Databases.Entities.Customer> CustomerList { get; set; }
    }
}
