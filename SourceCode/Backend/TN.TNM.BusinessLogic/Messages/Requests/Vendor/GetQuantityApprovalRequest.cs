using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetQuantityApprovalRequest : BaseRequest<GetQuantityApprovalParameter>
    {
        public Guid VendorOrderDetailId { get; set; }
        public Guid? ProcurementRequestItemId { get; set; }
        public Guid? ProductId { get; set; }
        public override GetQuantityApprovalParameter ToParameter()
        {
            return new GetQuantityApprovalParameter
            {
                UserId = this.UserId,
                VendorOrderDetailId = this.VendorOrderDetailId,
                ProcurementRequestItemId = this.ProcurementRequestItemId,
                ProductId = this.ProductId
            };
        }
    }
}
