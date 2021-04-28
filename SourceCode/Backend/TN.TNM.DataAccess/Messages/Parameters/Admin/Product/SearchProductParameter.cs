using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class SearchProductParameter:BaseParameter
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public List<Guid> ListProductCategory { get; set; }
        public List<Guid> ListVendor { get; set; }
    }
}
