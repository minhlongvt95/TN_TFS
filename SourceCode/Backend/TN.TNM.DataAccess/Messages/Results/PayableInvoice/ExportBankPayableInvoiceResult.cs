namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class ExportBankPayableInvoiceResult : BaseResult
    {
        public byte[] BankPayableInvoicePdf { get; set; }
        public string Code { get; set; }
    }
}
