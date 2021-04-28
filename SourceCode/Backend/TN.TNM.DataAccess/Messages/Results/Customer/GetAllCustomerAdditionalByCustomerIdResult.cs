using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerAdditionalByCustomerIdResult : BaseResult
    {
        public List<dynamic> CustomerAdditionalInformationList { get; set; }
    }
}
