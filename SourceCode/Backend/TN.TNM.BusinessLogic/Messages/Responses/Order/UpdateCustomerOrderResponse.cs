using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class UpdateCustomerOrderResponse : BaseResponse
    {
        public Guid CustomerOrderID { get; set; }
        public Guid? VendorOrderID { get; set; }
        public Guid? ProcurementRequestId { get; set; }
    }
}
