using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class ApprovalOrRejectVendorOrderRequest : BaseRequest<ApprovalOrRejectVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }
        public bool IsAprroval { get; set; }
        public string Description { get; set; }

        public override ApprovalOrRejectVendorOrderParameter ToParameter()
        {
            return new ApprovalOrRejectVendorOrderParameter()
            {
                UserId = UserId,
                VendorOrderId = VendorOrderId,
                IsAprroval = IsAprroval,
                Description = Description
            };
        }
    }
}
