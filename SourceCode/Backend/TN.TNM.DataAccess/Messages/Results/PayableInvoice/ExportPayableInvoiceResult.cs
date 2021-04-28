namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class ExportPayableInvoiceResult : BaseResult
    {
        public byte[] PayableInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
