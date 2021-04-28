using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetCustomerDeliveryResponse:BaseResponse
    {
        public List<CustomerSearchModel> Customer { get; set; }

    }
}
