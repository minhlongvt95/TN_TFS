using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetQuantityApprovalParameter : BaseParameter
    {
        public Guid? VendorOrderDetailId { get; set; }
        public Guid? ProcurementRequestItemId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
