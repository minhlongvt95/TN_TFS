using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerCodeResult : BaseResult
    {
        public List<string> CustomerCodeList { get; set; }

    }
}
