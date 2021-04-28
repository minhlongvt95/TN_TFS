using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class UpdateVendorOrderByIdResult : BaseResult
    {
        public List<ItemInvalidModel> ListItemInvalidModel { get; set; }
    }
}
