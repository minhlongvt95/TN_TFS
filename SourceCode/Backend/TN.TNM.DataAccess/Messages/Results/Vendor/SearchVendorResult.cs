using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class SearchVendorResult : BaseResult
    {
        public List<VendorEntityModel> VendorList { get; set; }
    }
}
