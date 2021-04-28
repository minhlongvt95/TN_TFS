using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class UpdateVendorOrderByIdResponse : BaseResponse
    {
        public List<ItemInvalidModel> ListItemInvalidModel { get; set; }
    }
}
