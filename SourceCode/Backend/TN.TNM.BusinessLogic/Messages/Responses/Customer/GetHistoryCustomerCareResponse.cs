using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetHistoryCustomerCareResponse : BaseResponse
    {
        public List<CustomerCareInforBusinessModel> ListCustomerCareInfor { get; set; }
    }
}
