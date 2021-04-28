using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class FilterVendorResponse:BaseResponse
    {
        public List<VendorModel> VendorList { get; set; }

    }
}
