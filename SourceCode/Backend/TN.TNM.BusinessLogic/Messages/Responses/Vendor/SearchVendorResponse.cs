using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class SearchVendorResponse : BaseResponse
    {
        public List<VendorModel> VendorList { get; set; }
    }
}
