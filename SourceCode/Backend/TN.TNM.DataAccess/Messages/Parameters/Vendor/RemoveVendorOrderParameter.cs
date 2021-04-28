using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class RemoveVendorOrderParameter : BaseParameter
    {
        public Guid VendorOrderId { get; set; }
    }
}
