using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class ExportBankReceiptInvoiceRequest : BaseRequest<ExportBankReceiptInvoiceParameter>
    {
        public Guid BankReceiptInvoiceId { get; set; }
        public override ExportBankReceiptInvoiceParameter ToParameter()
        {
            return new ExportBankReceiptInvoiceParameter() {
                UserId = UserId,
                BankReceiptInvoiceId = BankReceiptInvoiceId
            };
        }
    }
}
