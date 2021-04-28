using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class QuickCreateVendorMasterdataResult : BaseResult
    {
        public List<Databases.Entities.Category> ListVendorCategory { get; set; }
        public List<string> ListVendorCode { get; set; }
    }
}
