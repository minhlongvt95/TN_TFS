using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllHistoryProductByCustomerIdResponse : BaseResponse
    {
        public List<dynamic> listProduct { get; set; }
    }
}
