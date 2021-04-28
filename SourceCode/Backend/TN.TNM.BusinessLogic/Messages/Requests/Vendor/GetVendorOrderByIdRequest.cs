using System;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetVendorOrderByIdRequest : BaseRequest<GetVendorOrderByIdParameter>
    {
        public Guid VendorOrderId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public override GetVendorOrderByIdParameter ToParameter()
        {
            return new GetVendorOrderByIdParameter() {
                UserId = UserId,
                VendorOrderId = VendorOrderId,
                CustomerOrderId = CustomerOrderId
            };
        }
    }
}
