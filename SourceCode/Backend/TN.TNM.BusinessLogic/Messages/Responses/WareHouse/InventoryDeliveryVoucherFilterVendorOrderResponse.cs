using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class InventoryDeliveryVoucherFilterVendorOrderResponse:BaseResponse
    {
        public List<VendorOrderModel> listVendorOrder { get; set; }
    }
}
