namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class ExportReceiptinvoiceResponse : BaseResponse
    {
        public byte[] ReceiptInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
