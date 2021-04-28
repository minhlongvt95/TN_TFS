using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetListCustomeSaleToprForDashboardResponse: BaseResponse
    {
        public List<CustomerModel> ListCusSaleTop { get; set; }
    }
}
