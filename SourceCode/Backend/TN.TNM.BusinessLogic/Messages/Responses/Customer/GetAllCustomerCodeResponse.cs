using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllCustomerCodeResponse : BaseResponse
    {
        public List<string> CustomerCodeList { get; set; }
    }
}
