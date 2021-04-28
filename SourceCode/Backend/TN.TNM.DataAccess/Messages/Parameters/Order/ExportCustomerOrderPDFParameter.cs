using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class ExportCustomerOrderPDFParameter : BaseParameter
    {
        public Guid CustomerOrderId { get; set; }

    }
}
