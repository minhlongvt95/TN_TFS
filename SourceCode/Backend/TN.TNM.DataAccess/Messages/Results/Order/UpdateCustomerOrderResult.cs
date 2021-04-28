using System;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class UpdateCustomerOrderResult : BaseResult
    {
        public Guid CustomerOrderID { get; set; }
        public Guid? VendorOrderID { get; set; }
        public Guid? ProcurementRequestId { get; set; }
    }
}
