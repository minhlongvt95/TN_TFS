using TN.TNM.BusinessLogic.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetBankReceiptInvoiceByIdResponse:BaseResponse
    {
        public BankReceiptInvoiceModel BankReceiptInvoice { get; set; }
        public string BankReceiptInvoiceReasonText { get; set; }
        public string BankReceiptTypeText { get; set; }
        public string OrganizationText { get; set; }
        public string StatusText { get; set; }
        public string PriceCurrencyText { get; set; }
        public string CreateName { get; set; }
        public string ObjectName { get; set; }
    }
}
