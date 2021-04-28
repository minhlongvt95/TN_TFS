using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllHistoryProductByCustomerIdResult : BaseResult
    {
        public List<dynamic> listProduct { get; set; }
    }
}
