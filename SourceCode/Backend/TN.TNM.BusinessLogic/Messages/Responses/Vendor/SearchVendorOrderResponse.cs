using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class SearchVendorOrderResponse : BaseResponse
    {
        public List<VendorOrderModel> VendorOrderList { get; set; }
    }
}
