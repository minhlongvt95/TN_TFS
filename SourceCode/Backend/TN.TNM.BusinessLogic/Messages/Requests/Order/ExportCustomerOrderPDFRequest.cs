using System;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class ExportCustomerOrderPDFRequest : BaseRequest<ExportCustomerOrderPDFParameter>
    {
        public Guid CustomerOrderId { get; set; }
        public override ExportCustomerOrderPDFParameter ToParameter() => new ExportCustomerOrderPDFParameter()
        {
          CustomerOrderId = CustomerOrderId,
          UserId = UserId
        };
        
    }
}
