using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetVendorOrderByIdResult : BaseResult
    {
        public VendorOrderEntityModel VendorOrder { get; set; }
        public List<VendorOrderDetailEntityModel> VendorOrderDetailList { get; set; }
        public Guid ContactId { get; set; }
    }
}
