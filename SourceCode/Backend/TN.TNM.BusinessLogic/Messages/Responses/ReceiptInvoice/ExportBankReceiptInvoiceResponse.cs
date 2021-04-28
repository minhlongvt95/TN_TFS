namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class ExportBankReceiptInvoiceResponse : BaseResponse
    {
        public byte[] BankReceiptInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
