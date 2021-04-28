using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetVendorOrderByIdResponse : BaseResponse
    {
        public VendorOrderModel VendorOrder { get; set; }
        public List<VendorOrderDetailModel> VendorOrderDetailList { get; set; }
        public Guid ContactId { get; set; }
    }
}
