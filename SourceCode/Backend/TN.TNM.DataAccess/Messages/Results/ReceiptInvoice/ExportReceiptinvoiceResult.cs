namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class ExportReceiptinvoiceResult : BaseResult
    {
        public byte[] ReceiptInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
