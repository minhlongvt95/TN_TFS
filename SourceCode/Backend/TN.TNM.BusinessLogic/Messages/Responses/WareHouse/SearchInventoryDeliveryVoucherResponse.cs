using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class SearchInventoryDeliveryVoucherResponse:BaseResponse
    {
        public List<InventoryDeliveryVoucherModel> lstResult { get; set; }

    }
}
