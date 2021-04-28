using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class QuickCreateVendorResult : BaseResult
    {
        public Guid VendorId { get; set; }
        public List<VendorCreateOrderEntityModel> ListVendor { get; set; }
    }
}
