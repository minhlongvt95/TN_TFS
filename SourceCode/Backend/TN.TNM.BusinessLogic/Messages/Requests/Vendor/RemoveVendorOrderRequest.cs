using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class RemoveVendorOrderRequest : BaseRequest<RemoveVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }

        public override RemoveVendorOrderParameter ToParameter()
        {
            return new RemoveVendorOrderParameter()
            {
                UserId = UserId,
                VendorOrderId = VendorOrderId
            };
        }
    }
}
