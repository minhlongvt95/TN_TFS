using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class SearchVendorProductPriceParameter : BaseParameter
    {
        public string ProductName { get; set; }
        public string VendorName { get; set; }
    }
}
