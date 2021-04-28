using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetBankPayableInvoiceByIdRequest : BaseRequest<GetBankPayableInvoiceByIdParameter>
    {
        public Guid BankPayableInvoiceId { get; set; }
        public override GetBankPayableInvoiceByIdParameter ToParameter()
        {
            return new GetBankPayableInvoiceByIdParameter()
            {
                UserId = UserId,
                BankPayableInvoiceId = BankPayableInvoiceId
            };
        }
    }
}
