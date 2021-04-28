using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetDataSearchVendorQuoteParameter : BaseParameter
    {
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public List<Guid> VendorGroupIdList { get; set; }
    }
}
