using System;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class CreateVendorResult : BaseResult
    {
        public Guid VendorId { get; set; }
        public Guid ContactId { get; set; }
    }
}
