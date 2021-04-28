using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class FilterVendorResult:BaseResult
    {
        public List<VendorEntityModel> VendorList { get; set; }
    }
}
