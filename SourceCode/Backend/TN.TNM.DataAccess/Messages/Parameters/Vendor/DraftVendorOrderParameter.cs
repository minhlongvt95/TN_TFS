using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class DraftVendorOrderParameter : BaseParameter
    {
        public Guid VendorOrderId { get; set; }
        public bool IsCancelApproval { get; set; }
    }
}
