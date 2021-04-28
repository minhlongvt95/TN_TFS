using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetVendorOrderByIdParameter : BaseParameter
    {
        public Guid VendorOrderId { get; set; }
        public Guid CustomerOrderId { get; set; }
    }
}
