using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class QuickCreateVendorMasterdataResponse: BaseResponse
    {
        public List<DataAccess.Databases.Entities.Category> ListVendorCategory { get; set; }
        public List<string> ListVendorCode { get; set; }
    }
}
