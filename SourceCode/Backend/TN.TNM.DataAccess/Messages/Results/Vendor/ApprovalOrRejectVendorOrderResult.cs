using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class ApprovalOrRejectVendorOrderResult : BaseResult
    {
        public List<ItemInvalidModel> ListItemInvalidModel { get; set; }
    }
}
