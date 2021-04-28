using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class ApprovalOrRejectVendorOrderResponse : BaseResponse
    {
        public List<ItemInvalidModel> ListItemInvalidModel { get; set; }
    }
}
