using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class CreateVendorResponse : BaseResponse
    {
        public Guid VendorId { get; set; }
        public Guid ContactId { get; set; }
    }
}
