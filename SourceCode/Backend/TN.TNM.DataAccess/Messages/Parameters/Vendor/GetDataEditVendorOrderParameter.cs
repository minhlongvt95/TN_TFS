using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetDataEditVendorOrderParameter: BaseParameter
    {
        public Guid VendorOrderId { get; set; }
        public bool IsAprroval { get; set; }
        public string Description { get; set; }
    }
}
