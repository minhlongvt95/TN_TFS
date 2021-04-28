using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetBankReceiptInvoiceByIdRequest:BaseRequest<GetBankReceiptInvoiceByIdParameter>
    {
        public Guid BankReceiptInvoiceId { get; set; }
        public override GetBankReceiptInvoiceByIdParameter ToParameter()
        {
            return new GetBankReceiptInvoiceByIdParameter()
            {
                BankReceiptInvoiceId = BankReceiptInvoiceId,
                UserId = UserId
            };
        }
     
    }
}
