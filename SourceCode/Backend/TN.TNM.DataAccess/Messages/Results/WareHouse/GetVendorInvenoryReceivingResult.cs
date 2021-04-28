using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetVendorInvenoryReceivingResult:BaseResult
    {
        public List<VendorEntityModel> VendorList { get; set; }

    }
}
