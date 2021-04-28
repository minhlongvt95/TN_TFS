using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class QuickCreateVendorResponse : BaseResponse
    {
        public Guid VendorId { get; set; }
        public List<VendorCreateOrderEntityModel> ListVendor { get; set; }
    }
}
