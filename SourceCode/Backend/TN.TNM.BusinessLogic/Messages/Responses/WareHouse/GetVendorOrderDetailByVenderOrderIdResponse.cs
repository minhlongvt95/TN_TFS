using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetVendorOrderDetailByVenderOrderIdResponse : BaseResponse
    {
        public List<GetVendorOrderDetailByVenderOrderIdModel> ListOrderProduct { get; set; }
    }
}
