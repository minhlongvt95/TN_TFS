using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CancelVendorOrderRequest : BaseRequest<CancelVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }

        public override CancelVendorOrderParameter ToParameter()
        {
            return new CancelVendorOrderParameter()
            {
                UserId = UserId,
                VendorOrderId = VendorOrderId
            };
        }
    }
}
