using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class FilterCustomerInInventoryDeliveryVoucherResponse:BaseResponse
    {
        public List<CustomerModel> LstCustomer { get; set; }
    }
}
