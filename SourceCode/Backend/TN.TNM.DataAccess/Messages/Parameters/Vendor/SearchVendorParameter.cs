using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class SearchVendorParameter : BaseParameter
    {
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public List<Guid> VendorGroupIdList { get; set; }
    }
}
