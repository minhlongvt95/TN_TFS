using System;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class UpdateActiveVendorRequest : BaseRequest<UpdateActiveVendorParameter>
    {
        public Guid VendorId { get; set; }
        public override UpdateActiveVendorParameter ToParameter()
        {
            return new UpdateActiveVendorParameter() {
                UserId = UserId,
                VendorId = VendorId
            };
        }
    }
}
