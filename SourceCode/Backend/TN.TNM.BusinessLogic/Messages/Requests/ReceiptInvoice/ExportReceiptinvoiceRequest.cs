using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class ExportReceiptinvoiceRequest : BaseRequest<ExportReceiptInvoiceParameter>
    {
        public Guid ReceiptInvoiceId { get; set; }
        public override ExportReceiptInvoiceParameter ToParameter()
        {
            return new ExportReceiptInvoiceParameter
            {
                ReceiptInvoiceId = ReceiptInvoiceId
            };
        }
    }
}
