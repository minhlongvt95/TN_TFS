using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetVendorByIdParameter : BaseParameter
    {
        public Guid VendorId { get; set; }
        public Guid ContactId { get; set; }
    }
}
