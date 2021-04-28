using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class GetBankReceiptInvoiceByIdParameter : BaseParameter
    {
        public Guid BankReceiptInvoiceId { get; set; }
    }
}
