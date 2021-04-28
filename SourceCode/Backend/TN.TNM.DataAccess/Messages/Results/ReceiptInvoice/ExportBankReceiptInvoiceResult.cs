namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class ExportBankReceiptInvoiceResult : BaseResult
    {
        public byte[] BankReceiptInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
