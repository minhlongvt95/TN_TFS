using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class GetReceiptInvoiceByIdParameter : BaseParameter
    {
        public Guid ReceiptInvoiceId { get; set; }
    }
}
