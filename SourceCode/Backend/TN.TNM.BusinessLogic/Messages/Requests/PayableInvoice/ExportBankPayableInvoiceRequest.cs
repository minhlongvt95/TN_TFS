using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class ExportBankPayableInvoiceRequest : BaseRequest<ExportBankPayableInvoiceParameter>
    {
        public Guid BankPayableInvId { get; set; }
        public override ExportBankPayableInvoiceParameter ToParameter()
        {
            return new ExportBankPayableInvoiceParameter()
            {
                BankPayableInvId = BankPayableInvId
            };
        }
    }
}
