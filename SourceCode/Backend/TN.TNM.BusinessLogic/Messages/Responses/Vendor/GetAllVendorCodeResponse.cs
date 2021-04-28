using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetAllVendorCodeResponse : BaseResponse
    {
        public List<string> VendorCodeList { get; set; }
        public List<VendorModel> VendorList { get; set; }
    }
}
