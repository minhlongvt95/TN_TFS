using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetCustomerOrderDetailByCustomerOrderIdResponse:BaseResponse
    {
        public List<GetVendorOrderDetailByVenderOrderIdModel> ListOrderProduct { get; set; }

    }
}
