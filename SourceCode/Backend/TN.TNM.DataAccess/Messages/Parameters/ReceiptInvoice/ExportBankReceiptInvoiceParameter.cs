using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class ExportBankReceiptInvoiceParameter : BaseParameter
    {
        public Guid BankReceiptInvoiceId { get; set; }
    }
}
