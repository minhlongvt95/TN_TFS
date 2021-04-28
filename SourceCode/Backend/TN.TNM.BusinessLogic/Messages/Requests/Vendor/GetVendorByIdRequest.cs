using System;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetVendorByIdRequest : BaseRequest<GetVendorByIdParameter>
    {
        public Guid VendorId { get; set; }
        public Guid ContactId { get; set; }
        public override GetVendorByIdParameter ToParameter()
        {
            return new GetVendorByIdParameter() {
                VendorId = VendorId,
                ContactId = ContactId,
                UserId = UserId
            };
        }
    }
}
