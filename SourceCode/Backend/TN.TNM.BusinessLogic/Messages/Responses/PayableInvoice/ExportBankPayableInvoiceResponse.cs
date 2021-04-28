namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
   public  class ExportBankPayableInvoiceResponse : BaseResponse
    {
        public byte[] BankPayableInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
