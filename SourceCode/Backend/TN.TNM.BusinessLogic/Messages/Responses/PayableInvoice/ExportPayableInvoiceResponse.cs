namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class ExportPayableInvoiceResponse : BaseResponse
    {
        public byte[] PayableInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
