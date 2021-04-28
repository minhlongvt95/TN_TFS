using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class UpdateActiveVendorParameter : BaseParameter
    {
        public Guid VendorId { get; set; }
    }
}
