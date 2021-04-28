using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class GetOrderByCustomerIdParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
