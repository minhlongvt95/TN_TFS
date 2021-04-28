using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetAllVendorCodeResult : BaseResult
    {
        public List<string> VendorCodeList { get; set; }
    }
}
