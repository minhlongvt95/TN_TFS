using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Vendor
{
    public class GetDataSearchReceivableVendorResponse : BaseResponse
    {
        public List<VendorModel> ListVendor { get; set; }
    }
}
