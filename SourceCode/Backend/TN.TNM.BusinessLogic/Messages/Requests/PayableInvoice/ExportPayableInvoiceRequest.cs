using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class ExportPayableInvoiceRequest : BaseRequest<ExportPayableInvoiceParameter>
    {
        public Guid PayableInvoiceId { get; set; }
        public override ExportPayableInvoiceParameter ToParameter()
        {
            return new ExportPayableInvoiceParameter() {
                PayableInvoiceId = PayableInvoiceId,
                UserId = UserId
            };
        }
    }
}
