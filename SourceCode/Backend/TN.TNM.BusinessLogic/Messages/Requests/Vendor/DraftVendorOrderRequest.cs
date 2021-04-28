using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class DraftVendorOrderRequest : BaseRequest<DraftVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }
        public bool IsCancelApproval { get; set; }

        public override DraftVendorOrderParameter ToParameter()
        {
            return new DraftVendorOrderParameter()
            {
                UserId = UserId,
                VendorOrderId = VendorOrderId,
                IsCancelApproval = IsCancelApproval
            };
        }
    }
}
