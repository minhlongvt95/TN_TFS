using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetBankReceiptInvoiceByIdResult : BaseResult
    {
        public BankReceiptInvoice BankReceiptInvoice { get; set; }
        public string BankReceiptInvoiceReasonText { get; set; }
        public string BankReceiptTypeText { get; set; }
        public string OrganizationText { get; set; }
        public string StatusText { get; set; }
        public string PriceCurrencyText { get; set; }
        public string CreateName { get; set; }
        public string ObjectName { get; set; }
    }
}
