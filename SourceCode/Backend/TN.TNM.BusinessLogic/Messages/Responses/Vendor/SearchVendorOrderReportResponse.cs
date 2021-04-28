using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class SearchVendorOrderReportResponse : BaseResponse
    {
        public List<VendorOrderReportEntityModel> ListVendorOrderReport { get; set; }
    }
}
